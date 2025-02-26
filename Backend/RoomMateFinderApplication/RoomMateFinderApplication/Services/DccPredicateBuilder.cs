using System.Linq.Expressions;

namespace RoomMateFinderApplication.Services
{
    public static class DccPredicateBuilder
    {
        //
        // Summary:
        //     DccExpressionVisitor class.
        private class DccExpressionVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression oldParameter;

            private readonly ParameterExpression newParameter;

            public DccExpressionVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                this.oldParameter = oldParameter;
                this.newParameter = newParameter;
            }

            protected override System.Linq.Expressions.Expression VisitParameter(ParameterExpression node)
            {
                if (node == oldParameter)
                {
                    return newParameter;
                }

                return base.VisitParameter(node);
            }
        }

        //
        // Summary:
        //     Ors the specified expr2.
        //
        // Parameters:
        //   expr1:
        //     The expr1.
        //
        //   expr2:
        //     The expr2.
        //
        // Type parameters:
        //   T:
        //     Generic type.
        //
        // Returns:
        //     Expression.
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            System.Linq.Expressions.Expression right = new DccExpressionVisitor(expr2?.Parameters[0], expr1?.Parameters[0]).Visit(expr2?.Body);
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.OrElse(expr1?.Body, right), expr1?.Parameters);
        }

        //
        // Summary:
        //     Ands the specified expr2.
        //
        // Parameters:
        //   expr1:
        //     The expr1.
        //
        //   expr2:
        //     The expr2.
        //
        // Type parameters:
        //   T:
        //     Generic type.
        //
        // Returns:
        //     Expression.
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            System.Linq.Expressions.Expression right = new DccExpressionVisitor(expr2?.Parameters[0], expr1?.Parameters[0]).Visit(expr2?.Body);
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.AndAlso(expr1?.Body, right), expr1?.Parameters);
        }
    }
}
