using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Entities;
using ViaChatServer.BuildingBlocks.Infrastructure.Exceptions;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Repositories
{
    public record BaseIdEntityRepository<TEntity> : BaseEntityRepository<TEntity>, IBaseEntityRepository<TEntity>, IBaseIdEntityRepository<TEntity>, IDisposable where TEntity : BaseIdEntity
    {
        public BaseIdEntityRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// A method that can be reused to retrieve the details of an entity that inherits from BaseIdEntity model.
        /// </summary>
        /// <param name="resourceId">The requested entity id.</param>
        /// <param name="filters">The additional filters that should be applied to the request.</param>
        /// <param name="includes">The resources that should be included in the request.</param>
        /// <param name="excludeTracking">If request should exclude tracking or not.</param>
        /// <param name="ignoreQueryFilters">If request should ignore global query filters or not.</param>
        /// <param name="useSingleQuery">If single query should be used to retrieve related resources.</param>
        /// <param name="handleError">If error should be handled when query returns no results.</param>
        /// <param name="projection">Applying custom select to request only certain data from entity.</param>
        /// <returns>The requested entity, if found, otherwise null or error if handleError is true.</returns>
        /// <exception cref="EntityNotFoundException"></exception>
        public async Task<TEntity> GetEntityAsync(Guid resourceId,
                                                  List<Expression<Func<TEntity, bool>>> filters = null,
                                                  List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null,
                                                  bool excludeTracking = false,
                                                  bool ignoreQueryFilters = false,
                                                  bool useSingleQuery = false,
                                                  bool handleError = true,
                                                  Expression<Func<TEntity, TEntity>> projection = null)
        {
            try
            {
                EntityOptions<TEntity> options = new() { ExcludeTracking = excludeTracking, IgnoreQueryFilters = ignoreQueryFilters, UseSingleQuery = useSingleQuery };
                var query = base.CreateBaseQuery(options);

                query = query.Where(x => x.Id == resourceId);

                if (filters.HasValue())
                {
                    foreach (var filter in filters)
                    {
                        query = query.Where(filter);
                    }
                }

                if (includes.HasValue())
                {
                    foreach (var include in includes)
                    {
                        query = include(query);
                    }
                }

                if (projection.HasValue())
                {
                    query = query.Select(projection);
                }

                var entity = await query.FirstOrDefaultAsync();

                if(entity.HasValue() || !handleError)
                {
                    return entity;
                }
            }
            catch(Exception ex)
            {
                // TODO: Add error logging
                _ = ex.Message;
            }

            return handleError ? throw new EntityNotFoundException($"{typeof(TEntity).Name.ToLowerFirstCharacter()}Id") : null;
        }

        /// <summary>
        /// A method that can be reused for different validations on resources.
        /// </summary>
        /// <param name="expression">Expression to filter out the resources by.</param>
        /// <param name="exception">The exception that should be thrown.</param>
        /// <param name="handleError">If an exception should be thrown or the result should be returned.</param>
        /// <param name="validValue">What value should be considered as valid for the validation.</param>
        /// <returns>An exception if handleError is true or a boolean with the validation value.</returns>
        public async Task<bool> HandleValidationAsync(Expression<Func<TEntity, bool>> expression,
                                                      HttpRequestException exception,
                                                      bool handleError = true,
                                                      bool validValue = false,
                                                      bool excludeTracking = true,
                                                      bool ignoreQueryFilters = false)
        {
            EntityOptions<TEntity> options = new() { ExcludeTracking = excludeTracking, IgnoreQueryFilters = ignoreQueryFilters, UseSingleQuery = true };
            var validation = await CreateBaseQuery(options)
                .Where(expression)
                .AnyAsync();

            var isValid = !validValue ^ validation;
            if (handleError && !isValid)
            {
                throw exception;
            }

            return isValid;
        }

        /// <summary>
        /// A method that can be reused for different validations on resources.
        /// </summary>
        /// <param name="expressions">List of expressions to filter out the resource by.</param>
        /// <param name="exception">The exception that should be thrown.</param>
        /// <param name="handleError">If an exception should be thrown or the result should be returned.</param>
        /// <param name="validValue">What value should be considered as valid for the validation.</param>
        /// <returns>An exception if handleError is true or a boolean with the validation value.</returns>
        public async Task<bool> HandleValidationAsync(List<Expression<Func<TEntity, bool>>> expressions,
                                                      HttpRequestException exception,
                                                      bool handleError = true,
                                                      bool validValue = false,
                                                      bool excludeTracking = true,
                                                      bool ignoreQueryFilters = false)
        {
            EntityOptions<TEntity> options = new() { Filters = expressions, ExcludeTracking = excludeTracking, IgnoreQueryFilters = ignoreQueryFilters, UseSingleQuery = true };
            var query = CreateBaseQuery(options);

            var validation = await query.AnyAsync();

            var isValid = !validValue ^ validation;
            if (handleError && !isValid)
            {
                throw exception;
            }

            return isValid;
        }
    }
}
