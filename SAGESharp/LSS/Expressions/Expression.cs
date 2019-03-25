using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public interface ExpressionVisitor<T>
    {
        T VisitBinaryExpression(BinaryExpression expr);
        T VisitLiteralExpression(LiteralExpression expr);
        T VisitVariableExpression(VariableExpression expr);
        T VisitGroupingExpression(GroupingExpression expr);
        T VisitUnaryExpression(UnaryExpression expr);
        T VisitArrayAccessExpression(ArrayAccessExpression expr);
    }

    public abstract class Expression
    {

        public abstract T AcceptVisitor<T>(ExpressionVisitor<T> visitor);
    }
}
