using Chat.Domain.Entities;

using System.ComponentModel.DataAnnotations;

using ViaChatServer.BuildingBlocks.Infrastructure.Resources;

namespace Chat.Application.Models
{
    /// <summary>
    /// The model that is used to transfer the data for creating a post
    /// </summary>
    public record CreatePost : UpdatePost
    {
        /// <summary>
        /// The user of the post
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(ModelValidation.RequiredField), ErrorMessageResourceType = typeof(ModelValidation))]
        [StringLength(64, MinimumLength = 3, ErrorMessageResourceName = nameof(ModelValidation.InvalidInputLength), ErrorMessageResourceType = typeof(ModelValidation))]
        public string User { get; set; }
        /// <summary>
        /// The room of the post
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(ModelValidation.RequiredField), ErrorMessageResourceType = typeof(ModelValidation))]
        [StringLength(63, MinimumLength = 3, ErrorMessageResourceName = nameof(ModelValidation.InvalidInputLength), ErrorMessageResourceType = typeof(ModelValidation))]
        public string Room { get; set; }

        public static implicit operator Post(CreatePost model) => (model != null) ? new()
        {
            Message = model.Msg,
            UserName = model.User
        } : null;
    }
}
