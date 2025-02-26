using System.Linq.Expressions;

namespace RoomMateFinderApplication.Services
{
    public static class ExpressionCombiner
    {
        public static Expression<Func<T, bool>> AndAlso<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(expr1),
                Expression.Invoke(expr2)
            );

            return Expression.Lambda<Func<T, bool>>(body);
        }
    }
}
