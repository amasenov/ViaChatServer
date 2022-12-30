using System;

namespace Chat.Application.Models.Enumerations.Includes
{
    [Flags]
    public enum PostIncludes
    {
        None = 0,

        User = 1 << 0,

        Room = 1 << 1,

        All = User | Room
    }
}
