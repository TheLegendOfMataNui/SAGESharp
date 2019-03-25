using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class ArrayAccessExpression : Expression
    {
        public Expression Array { get; }
        public Expression Index { get; }

        public ArrayAccessExpression(Expression array, Expression index)
        {
            this.Array = array;
            this.Index = index;
        }

        public override T AcceptVisitor<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitArrayAccessExpression(this);
        }
    }
}
