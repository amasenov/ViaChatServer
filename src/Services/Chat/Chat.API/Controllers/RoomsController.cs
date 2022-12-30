using Chat.Application.Models;
using Chat.Application.Models.Enumerations.Sort;
using Chat.Application.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Net.Mime;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Attributes;
using ViaChatServer.BuildingBlocks.Infrastructure.Constants;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;
using ViaChatServer.BuildingBlocks.Infrastructure.Models.QueryParameters;

namespace Chat.API.Controllers
{
    /// <summary>
    /// The controller that can be used for managing rooms.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route(ApiRoutes.BaseVersionRoute + "/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class RoomsController : ControllerBase
    {
        private readonly RoomService _roomService;

        public RoomsController(RoomService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>Get a list of rooms asynchronous.</summary>
        /// <param name="pagingQueryParameters">The paging query parameters.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <param name="orderDescending">if set to <c>true</c> [order descending].</param>
        /// <returns>A paged collection of rooms.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResponse<RoomDto>>> GetRoomsAsync(
            [FromQuery] PagingQueryParameters pagingQueryParameters,
            [FromQuery] string searchTerm = null,
            [FromQuery] RoomSortByType sortBy = RoomSortByType.Id,
            [FromQuery] bool orderDescending = false)
        {
            PagingOptions pagingOptions = pagingQueryParameters;

            var result = await _roomService.GetRoomsAsync(pagingOptions, sortBy, orderDescending, searchTerm);

            return Ok(result);
        }

        /// <summary>Gets the room by identifier asynchronous.</summary>
        /// <param name="roomId">The room identifier</param>
        /// <returns>The room in case it was found.</returns>
        [HttpGet(ApiRouteIdentifierTemplates.RoomId, Name = nameof(GetRoomByIdAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoomDto>> GetRoomByIdAsync([FromRoute] Guid roomId)
        {
            var result = await _roomService.GetRoomByIdAsync(roomId);

            return Ok(result);
        }

        /// <summary>Creates the room asynchronous.</summary>
        /// <param name="model">The create room model.</param>
        /// <returns>The link and body of the created entry in case of success.</returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<RoomDto>> CreateRoomAsync([FromBody] CreateRoom model)
        {
            var result = await _roomService.CreateRoomAsync(model);

            return Created("", result);
        }

        /// <summary>Updates the room asynchronous.</summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="patchDoc">The patch document.</param>
        /// <returns>The updated room in case of success.</returns>
        [HttpPatch(ApiRouteIdentifierTemplates.RoomId)]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [NewtonsoftJsonFormatter]
        public async Task<ActionResult<RoomDto>> UpdateRoomAsync([FromRoute] Guid roomId, [FromBody] JsonPatchDocument<UpdateRoom> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(ModelState);
            }

            var (model, entity) = await _roomService.GetRoomForPatchingAsync(roomId);

            patchDoc.ApplyTo(model, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.ValidateModel();

            var result = await _roomService.UpdateRoomAsync(roomId, model, entity);

            return Ok(result);
        }

        /// <summary>Updates the room asynchronous.</summary>
        /// <param name="roomId">The room identifier</param>
        /// <param name="model">The update room model.</param>
        /// <returns>The updated room in case of success.</returns>
        [HttpPut(ApiRouteIdentifierTemplates.RoomId)]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoomDto>> UpdateRoomAsync([FromRoute] Guid roomId, [FromBody] UpdateRoom model)
        {
            var policy = await _roomService.UpdateRoomAsync(roomId, model);

            return Ok(policy);
        }

        /// <summary>Deletes the room asynchronous.</summary>
        /// <param name="roomId">The room identifier</param>
        /// <returns>No content in case of success.</returns>
        [HttpDelete(ApiRouteIdentifierTemplates.RoomId)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteRoomAsync([FromRoute] Guid roomId)
        {
            await _roomService.DeleteRoomAsync(roomId);

            return NoContent();
        }
    }
}
