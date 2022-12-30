using System;
using ViaChatServer.BuildingBlocks.Infrastructure.Models.QueryParameters;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Models
{
    /// <summary>
    /// The options to be applied when paging.
    /// </summary>
    public record PagingOptions
    {
        public PagingOptions(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }

            if (pageSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Gets the page number.
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the number of items to skip based on the pagenumber and pagesize.
        /// </summary>
        public int Skip => (PageNumber - 1) * PageSize;

        public static implicit operator PagingOptions(PagingQueryParameters param) => new(param.PageNumber, param.PageSize);
    }
}
