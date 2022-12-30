using System.ComponentModel.DataAnnotations;

using ViaChatServer.BuildingBlocks.Infrastructure.Resources;

namespace Chat.Application.Models
{
    /// <summary>
    /// The model that is used to transfer the data for updating a post
    /// </summary>
    public record UpdatePost
    {
        /// <summary>
        /// The message of the post
        /// </summary>
        /// <remarks>Size is max 140 chars.</remarks>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(ModelValidation.RequiredField), ErrorMessageResourceType = typeof(ModelValidation))]
        [StringLength(140, MinimumLength = 3, ErrorMessageResourceName = nameof(ModelValidation.InvalidInputLength), ErrorMessageResourceType = typeof(ModelValidation))]
        public string Msg { get; set; }
    }
}
