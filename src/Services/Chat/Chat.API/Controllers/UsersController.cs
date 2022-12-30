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
    /// The controller that can be used for managing users.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route(ApiRoutes.BaseVersionRoute + "/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>Get a list of users asynchronous.</summary>
        /// <param name="pagingQueryParameters">The paging query parameters.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <param name="orderDescending">if set to <c>true</c> [order descending].</param>
        /// <returns>A paged collection of users.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResponse<UserDto>>> GetUsersAsync(
            [FromQuery] PagingQueryParameters pagingQueryParameters,
            [FromQuery] string searchTerm = null,
            [FromQuery] UserSortByType sortBy = UserSortByType.Id,
            [FromQuery] bool orderDescending = false)
        {
            PagingOptions pagingOptions = pagingQueryParameters;

            var result = await _userService.GetUsersAsync(pagingOptions, sortBy, orderDescending, searchTerm);

            return Ok(result);
        }

        /// <summary>Gets the user by identifier asynchronous.</summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>The user in case it was found.</returns>
        [HttpGet(ApiRouteIdentifierTemplates.UserId, Name = nameof(GetUserByIdAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync([FromRoute] Guid userId)
        {
            var result = await _userService.GetUserByIdAsync(userId);

            return Ok(result);
        }

        /// <summary>Creates the user asynchronous.</summary>
        /// <param name="model">The create user model.</param>
        /// <returns>The link and body of the created entry in case of success.</returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserDto>> CreateUserAsync([FromBody] CreateUser model)
        {
            var result = await _userService.CreateUserAsync(model);

            return Created("", result);
        }

        /// <summary>Updates the user asynchronous.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="patchDoc">The patch document.</param>
        /// <returns>The updated user in case of success.</returns>
        [HttpPatch(ApiRouteIdentifierTemplates.UserId)]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [NewtonsoftJsonFormatter]
        public async Task<ActionResult<UserDto>> UpdateUserAsync([FromRoute] Guid userId, [FromBody] JsonPatchDocument<UpdateUser> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(ModelState);
            }

            var (model, entity) = await _userService.GetUserForPatchingAsync(userId);

            patchDoc.ApplyTo(model, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.ValidateModel();

            var result = await _userService.UpdateUserAsync(userId, model, entity);

            return Ok(result);
        }

        /// <summary>Updates the user asynchronous.</summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="model">The update user model.</param>
        /// <returns>The updated user in case of success.</returns>
        [HttpPut(ApiRouteIdentifierTemplates.UserId)]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> UpdateUserAsync([FromRoute] Guid userId, [FromBody] UpdateUser model)
        {
            var policy = await _userService.UpdateUserAsync(userId, model);

            return Ok(policy);
        }

        /// <summary>Deletes the user asynchronous.</summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>No content in case of success.</returns>
        [HttpDelete(ApiRouteIdentifierTemplates.UserId)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUserAsync([FromRoute] Guid userId)
        {
            await _userService.DeleteUserAsync(userId);

            return NoContent();
        }
    }
}
