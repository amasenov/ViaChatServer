using System;

namespace Chat.Application.Models.Enumerations.Includes
{
    [Flags]
    public enum UserIncludes
    {
        None = 0,

        Posts = 1 << 0,

        Room = 1 << 1,

        All = Room | Posts
    }
}
