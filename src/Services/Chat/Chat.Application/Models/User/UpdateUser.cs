using System.ComponentModel.DataAnnotations;

using ViaChatServer.BuildingBlocks.Infrastructure.Resources;

namespace Chat.Application.Models
{
    /// <summary>
    /// The model that is used to transfer the data for updating a user
    /// </summary>
    public record UpdateUser
    {
        /// <summary>
        /// The name of the user
        /// </summary>
        /// <remarks>Size is limited to 64 chars.</remarks>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(ModelValidation.RequiredField), ErrorMessageResourceType = typeof(ModelValidation))]
        [StringLength(64, MinimumLength = 3, ErrorMessageResourceName = nameof(ModelValidation.InvalidInputLength), ErrorMessageResourceType = typeof(ModelValidation))]
        public string Name { get; set; }
    }
}
