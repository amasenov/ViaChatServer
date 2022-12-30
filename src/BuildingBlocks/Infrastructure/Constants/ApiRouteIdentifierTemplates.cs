namespace ViaChatServer.BuildingBlocks.Infrastructure.Constants
{
    /// <summary>
    /// Api route identifier templates.
    /// Routing constraints are used to restrict how the parameters are matched.
    /// </summary>
    public static class ApiRouteIdentifierTemplates
    {
        /// <summary>
        /// The room identifier
        /// </summary>
        public const string RoomId = "{roomId:guid}";
        /// <summary>
        /// The user identifier
        /// </summary>
        public const string UserId = "{userId:guid}";
        /// <summary>
        /// The post identifier
        /// </summary>
        public const string PostId = "{postId:guid}";
    }
}
