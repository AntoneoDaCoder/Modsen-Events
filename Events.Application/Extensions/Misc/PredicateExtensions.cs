using Events.Core.Models;
using System.Linq.Expressions;

namespace Events.Application.Extensions.Misc
{
    public static class PredicateExtensions
    {
        public static Expression<Func<Event, bool>> CombinePredicates(IEnumerable<Expression<Func<Event, bool>>> predicates)
        {
            Expression<Func<Event, bool>> combined = e => true;
            foreach (var predicate in predicates)
            {
                combined = CombinePredicates(combined, predicate);
            }
            return combined;
        }

        private static Expression<Func<Event, bool>> CombinePredicates(Expression<Func<Event, bool>> first, Expression<Func<Event, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(Event), "e");
            var firstBody = Expression.Invoke(first, parameter);
            var secondBody = Expression.Invoke(second, parameter);
            var combinedBody = Expression.AndAlso(firstBody, secondBody);
            return Expression.Lambda<Func<Event, bool>>(combinedBody, parameter);
        }
    }
}