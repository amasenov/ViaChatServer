using Chat.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace Chat.Application.Models.Filters
{
    public record PostFilters
    {
        public Guid? Id { get; init; }
        public string SearchTerm { get; init; }
        public string RoomName { get; init; }

        public static implicit operator List<Expression<Func<Post, bool>>> (PostFilters filters)
        {
            if (filters.IsNull())
            {
                return null;
            }

            List<Expression<Func<Post, bool>>> result = new();
            if (filters.Id.HasValue)
            {
                result.Add(p => p.Id.Equals(filters.Id.Value));
            };

            if (filters.SearchTerm.HasValue())
            {
                string searchTerm = filters.SearchTerm.Trim().ToUpper();
                result.Add(p => p.Message.ToUpper().Contains(searchTerm));
            }

            if (filters.RoomName.HasValue())
            {
                string roomName = filters.RoomName.Trim().ToUpper();
                result.Add(p => p.Message.ToUpper().Equals(roomName));
            }

            return result;
        }
    }
}
