using System.Linq.Expressions;

namespace Core.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria {get;}       // where criteria
        List<Expression<Func<T, Object>>> Includes {get;}

        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }

        /* pagination
        If we had a product list, let's say we have 10 products inside it.
        We could choose to take the first five. And Skip, none.
        And if we went onto the second page, then we could choose to take another five, but then skip five, 
        so we'd get the second page.
        */
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}