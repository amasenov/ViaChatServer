using System.ComponentModel.DataAnnotations;

using ViaChatServer.BuildingBlocks.Infrastructure.Resources;

namespace Chat.Application.Models
{
    /// <summary>
    /// The model that is used to transfer the data for updating a room
    /// </summary>
    public record UpdateRoom
    {
        /// <summary>
        /// The name of the room
        /// </summary>
        /// <remarks>The name should be less than 64 chars.</remarks>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(ModelValidation.RequiredField), ErrorMessageResourceType = typeof(ModelValidation))]
        [StringLength(63, MinimumLength = 3, ErrorMessageResourceName = nameof(ModelValidation.InvalidInputLength), ErrorMessageResourceType = typeof(ModelValidation))]
        public string Name { get; set; }
    }
}
