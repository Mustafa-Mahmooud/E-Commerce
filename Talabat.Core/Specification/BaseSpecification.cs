using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Talabat.Core.Entities;

namespace Talabat.Core.Specification
{
    public class BaseSpecification<T> : ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>>? Critirea { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get ; set ; }
        public Expression<Func<T, object>> OrderByDesc { get ; set ; }

    

        public BaseSpecification()
        {
        }
        public BaseSpecification(Expression<Func<T, bool>> criteriaExpression)
        {
            Critirea = criteriaExpression;
        }
        public void AddOrderByAsc(Expression<Func<T, object>> expression)
        {
            OrderBy = expression;
        }
        public void AddOrderByDesc(Expression<Func<T, object>> expression)
        {
            OrderByDesc = expression;
        }

       
        



    }
}