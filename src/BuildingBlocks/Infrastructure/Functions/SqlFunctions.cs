using System;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Functions
{
    public static class SqlFunctions
    {
        /// <summary>
        /// Just a simple match score calculation function that counts the number
        /// Of times a particular search term occurs in a searched field value
        /// and the number of characters it spans in the search field
        /// </summary>
        public static int SearchScore(string keyword, string value) => throw new NotSupportedException();
    }
}
