using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public interface ExpressionVisitor<T, C>
    {
        T VisitBinaryExpression(BinaryExpression expr, C context);
        T VisitLiteralExpression(LiteralExpression expr, C context);
        T VisitVariableExpression(VariableExpression expr, C context);
        T VisitGroupingExpression(GroupingExpression expr, C context);
        T VisitUnaryExpression(UnaryExpression expr, C context);
        T VisitArrayAccessExpression(ArrayAccessExpression expr, C context);
        T VisitCallExpression(CallExpression expr, C context);
        T VisitConstructorExpression(ConstructorExpression expr, C context);
        T VisitArrayExpression(ArrayExpression expr, C context);
    }

    public abstract class Expression
    {
        public abstract SourceSpan Span { get; }

        public abstract T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context);

        public abstract Expression Duplicate();
    }
}
