using Chat.Application.Models;
using Chat.Application.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net.Mime;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Constants;

namespace Chat.API.Controllers
{
    /// <summary>
    /// The controller that can be used for managing chats.
    /// </summary>
    [ApiController]
    [Route(ApiRoutes.BaseVersionRoute + "/[controller]")]
    [ApiVersion("1.0")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class ChatsController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly RoomService _roomService;

        public ChatsController(PostService postService, RoomService roomService)
        {
            _postService = postService;
            _roomService = roomService;
        }

        /// <summary>Creates a room asynchronous.</summary>
        /// <returns>The created room in case of success.</returns>
        [HttpGet("/" + ApiRoutes.BaseVersionRoute + "/createRoom")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<RoomDto>> CreateRoomAsync([FromQuery] CreateRoom model)
        {
            var result = await _roomService.CreateRoomAsync(model);

            return Created("", result);
        }

        /// <summary>Creates a post asynchronous.</summary>
        /// <returns>The created post in case of success.</returns>
        [HttpGet("/" + ApiRoutes.BaseVersionRoute + "/post")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreatePostAsync([FromQuery] CreatePost model)
        {
            var result = await _postService.CreatePostAsync(model);

            return Created("", result);
        }

        /// <summary>Get a list of posts asynchronous.</summary>
        /// <returns>A list of posts.</returns>
        [HttpGet("/" + ApiRoutes.BaseVersionRoute + "/get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetPostsAsync([FromQuery] string room, [FromQuery] int limit = 20)
        {
            var results = await _postService.GetPostsAsync(room, limit);

            return Ok(results);
        }
    }
}
