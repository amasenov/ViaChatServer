using Chat.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace Chat.Application.Models.Filters
{
    public record RoomFilters
    {
        public Guid? Id { get; init; }
        public string SearchTerm { get; init; }
        public string Name { get; init; }

        public static implicit operator List<Expression<Func<Room, bool>>>(RoomFilters filters)
        {
            if (filters.IsNull())
            {
                return null;
            }

            List<Expression<Func<Room, bool>>> result = new();
            if (filters.Id.HasValue)
            {
                result.Add(r => r.Id.Equals(filters.Id.Value));
            };

            if (filters.SearchTerm.HasValue())
            {
                string searchTerm = filters.SearchTerm.Trim().ToUpper();
                result.Add(r => r.Name.ToUpper().Contains(searchTerm));
            }

            if (filters.Name.HasValue())
            {
                string name = filters.Name.Trim().ToUpper();
                result.Add(r => r.Name.ToUpper().Equals(name));
            }

            return result;
        }
    }
}
