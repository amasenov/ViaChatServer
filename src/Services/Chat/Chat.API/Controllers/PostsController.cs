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
    /// The controller that can be used for managing posts.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route(ApiRoutes.BaseVersionRoute + "/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class PostsController : ControllerBase
    {
        private readonly PostService _postService;

        public PostsController(PostService postService)
        {
            _postService = postService;
        }

        /// <summary>Get a list of posts asynchronous.</summary>
        /// <param name="pagingQueryParameters">The paging query parameters.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <param name="orderDescending">if set to <c>true</c> [order descending].</param>
        /// <returns>A paged collection of posts.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResponse<PostDto>>> GetPostsAsync(
            [FromQuery] PagingQueryParameters pagingQueryParameters,
            [FromQuery] string searchTerm = null,
            [FromQuery] PostSortByType sortBy = PostSortByType.Id,
            [FromQuery] bool orderDescending = false)
        {
            PagingOptions pagingOptions = pagingQueryParameters;

            var result = await _postService.GetPostsAsync(pagingOptions, sortBy, orderDescending, searchTerm);

            return Ok(result);
        }

        /// <summary>Gets the post by identifier asynchronous.</summary>
        /// <param name="postId">The post identifier</param>
        /// <returns>The post in case it was found.</returns>
        [HttpGet(ApiRouteIdentifierTemplates.PostId, Name = nameof(GetPostByIdAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> GetPostByIdAsync([FromRoute] Guid postId)
        {
            var result = await _postService.GetPostByIdAsync(postId);

            return Ok(result);
        }

        /// <summary>Creates the post asynchronous.</summary>
        /// <param name="model">The create post model.</param>
        /// <returns>The link and body of the created entry in case of success.</returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PostDto>> CreatePostAsync([FromBody] CreatePost model)
        {
            var result = await _postService.CreatePostAsync(model);

            return Created("", result);
        }

        /// <summary>Updates the post asynchronous.</summary>
        /// <param name="postId">The post identifier.</param>
        /// <param name="patchDoc">The patch document.</param>
        /// <returns>The updated post in case of success.</returns>
        [HttpPatch(ApiRouteIdentifierTemplates.PostId)]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [NewtonsoftJsonFormatter]
        public async Task<ActionResult<PostDto>> UpdatePostAsync([FromRoute] Guid postId, [FromBody] JsonPatchDocument<UpdatePost> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(ModelState);
            }

            var (model, entity) = await _postService.GetPostForPatchingAsync(postId);

            patchDoc.ApplyTo(model, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.ValidateModel();

            var result = await _postService.UpdatePostAsync(postId, model, entity);

            return Ok(result);
        }

        /// <summary>Updates the post asynchronous.</summary>
        /// <param name="postId">The post identifier</param>
        /// <param name="model">The update post model.</param>
        /// <returns>The updated post in case of success.</returns>
        [HttpPut(ApiRouteIdentifierTemplates.PostId)]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> UpdatePostAsync([FromRoute] Guid postId, [FromBody] UpdatePost model)
        {
            var policy = await _postService.UpdatePostAsync(postId, model);

            return Ok(policy);
        }

        /// <summary>Deletes the post asynchronous.</summary>
        /// <param name="postId">The post identifier</param>
        /// <returns>No content in case of success.</returns>
        [HttpDelete(ApiRouteIdentifierTemplates.PostId)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletePostAsync([FromRoute] Guid postId)
        {
            await _postService.DeletePostAsync(postId);

            return NoContent();
        }
    }
}
