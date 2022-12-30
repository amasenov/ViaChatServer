using System.Linq;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Models
{
    public record PagedResponseQuery<T> where T : class
    {
        public PagedResponseQuery(int pageNumber, int pageSize, int total)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Total = total;
        }

        public bool HasMore
        {
            get
            {
                return (PageNumber * PageSize) < Total;
            }
        }

        public int PageNumber { get; }

        public int PageSize { get; }

        public int Total { get; }

        public IQueryable<T> Query { get; set; }
    }
}
