using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Models
{
    /// <summary>Represents a model of paged response.</summary>
    public record PagedResponse<T> //where T : struct // where T : class
    {
        public PagedResponse()
        {

        }

        public PagedResponse(int pageNumber, int pageSize, int total) : base()
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Total = total;
        }

        /// <summary>Gets the indication if there are more results.</summary>
        [JsonPropertyName("hasMore")]
        public bool HasMore
        {
            get
            {
                return (PageNumber * PageSize) < Total;
            }
        }

        /// <summary>Gets the current page number.</summary>
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        /// <summary>Gets the current page size.</summary>
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        /// <summary>Gets the total results.</summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }

        /// <summary>Gets the results.</summary>
        [JsonPropertyName("results")]
        public IEnumerable<T> Results { get; set; }
    }
}
