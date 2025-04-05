using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities;
using Talabat.Core.Specification;

internal static class Specification_Evaluator<TEntity> where TEntity : BaseEntity
{
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inner, ISpecification<TEntity> spec)
    {
        if (inner == null)
            throw new ArgumentNullException(nameof(inner));

        if (spec == null)
            throw new ArgumentNullException(nameof(spec));

        var query = inner;

        if (spec.Critirea != null)
        {
            query = query.Where(spec.Critirea);
        }

        

        if (spec.OrderBy != null) 
        {
            query = query.OrderBy(spec.OrderBy);
        }

        if (spec.OrderByDesc != null) 
        {
            query = query.OrderByDescending(spec.OrderByDesc);
        }

        if (spec.Includes != null)
        {
            query = spec.Includes.Aggregate(query, (currentQuery, include) => currentQuery.Include(include));
        }
        
        Console.WriteLine($"Generated Query: {query.ToQueryString()}");

        return query;
    }
}