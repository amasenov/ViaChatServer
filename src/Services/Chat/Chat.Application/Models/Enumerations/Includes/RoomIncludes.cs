using System;

namespace Chat.Application.Models.Enumerations.Includes
{
    [Flags]
    public enum RoomIncludes
    {
        None = 0,

        Users = 1 << 0,

        Posts = 1 << 1,

        All = Users | Posts
    }
}
