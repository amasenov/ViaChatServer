using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ViaChatServer.BuildingBlocks.Infrastructure.Resources;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Models.QueryParameters
{
    /// <summary>
    /// The paging specific query parameters.
    /// </summary>
    public record PagingQueryParameters : IValidatableObject
    {
        private const int defaultPageNumber = 1;
        private const int defaultPageSize = 10;
        private const int maxPageSize = 50;

        /// <summary>
        /// The number of the page.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessageResourceName = nameof(ModelValidation.InvalidPageNumber), ErrorMessageResourceType = typeof(ModelValidation))]
        public int PageNumber { get; set; } = defaultPageNumber;

        /// <summary>
        /// The number of items per page.
        /// </summary>
        [Range(1, maxPageSize, ErrorMessageResourceName = nameof(ModelValidation.InvalidPageSize), ErrorMessageResourceType = typeof(ModelValidation))]
        public int PageSize { get; set; } = defaultPageSize;

        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        /// A collection that holds failed-validation information.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PageNumber < 1)
            {
                yield return new ValidationResult("Invalid input!", new[] { nameof(PageNumber) });
            }
            if (PageSize < 0)
            {
                yield return new ValidationResult("Invalid input!", new[] { nameof(PageSize) });
            }
        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(this.PageNumber)}:{this.PageNumber} - {nameof(this.PageSize)}:{this.PageSize}";
        }
    }
}
