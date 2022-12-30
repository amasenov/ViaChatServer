namespace ViaChatServer.BuildingBlocks.Infrastructure.Constants
{
    /// <summary>
    /// Api route definitions
    /// </summary>
    public static class ApiRoutes
    {
        /// <summary>
        /// The base API route.
        /// </summary>
        public const string BaseApiRoute = "api";

        /// <summary>
        /// The base route including the api versioning placeholder.
        /// </summary>
        public const string BaseVersionRoute = BaseApiRoute + "/v{version:apiVersion}";
    }
}
