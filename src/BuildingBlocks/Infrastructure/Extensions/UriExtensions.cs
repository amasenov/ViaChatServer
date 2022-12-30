using System;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class UriExtensions
    {
        public static Uri TryExtend(this Uri input, string extendPart)
        {
            if (input.IsNull())
            {
                return null;
            }

            if (extendPart.IsEmpty())
            {
                return input;
            }

            var hasEndForwardSlash = char.Equals(input.AbsoluteUri[^1], '/');
            var hasFrontForwardSlash = char.Equals(extendPart[0], '/');

            var middlePart = (!hasEndForwardSlash && !hasFrontForwardSlash) ? "/" : string.Empty;
            var updatedExtendPart = (hasEndForwardSlash && hasFrontForwardSlash) ? extendPart[1..] : extendPart;

            return new Uri($"{input.AbsoluteUri}{middlePart}{updatedExtendPart}");
        }
    }
}
