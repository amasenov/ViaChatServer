using System.Collections.Generic;
using System.Linq;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns true if the list is contains elements
        /// Returns false if the list is empty or is null
        /// </summary>
        public static bool HasValues<T>(this IEnumerable<T> list) => list?.Any() == true;
        /// <summary>
        /// Returns true if the list is not null and does not contain any elements
        /// Returns false if the list is null or contains elements
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> list) => list?.Any() == false;
    }
}
