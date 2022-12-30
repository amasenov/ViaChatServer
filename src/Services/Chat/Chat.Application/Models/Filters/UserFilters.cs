using Chat.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace Chat.Application.Models.Filters
{
    public record UserFilters
    {
        public Guid? Id { get; init; }
        public string SearchTerm { get; init; }
        public string Name { get; init; }

        public static implicit operator List<Expression<Func<User, bool>>> (UserFilters filters)
        {
            if (filters.IsNull())
            {
                return null;
            }

            List<Expression<Func<User, bool>>> result = new();
            if (filters.Id.HasValue)
            {
                result.Add(u => u.Id.Equals(filters.Id.Value));
            };

            if (filters.SearchTerm.HasValue())
            {
                string searchTerm = filters.SearchTerm.Trim().ToUpper();
                result.Add(u => u.Name.ToUpper().Contains(searchTerm));
            }

            if (filters.Name.HasValue())
            {
                string name = filters.Name.Trim().ToUpper();
                result.Add(u => u.Name.ToUpper().Equals(name));
            }

            return result;
        }
    }
}
