using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class ArrayAccessExpression : Expression
    {
        public override SourceSpan Span { get; }
        public Expression Array { get; }
        public Expression Index { get; }

        public ArrayAccessExpression(SourceSpan span, Expression array, Expression index)
        {
            this.Span = span;
            this.Array = array;
            this.Index = index;
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitArrayAccessExpression(this, context);
        }

        public override Expression Duplicate()
        {
            return new ArrayAccessExpression(Span, Array.Duplicate(), Index.Duplicate());
        }

        public override string ToString()
        {
            return Array.ToString() + "[" + Index.ToString() + "]";
        }
    }
}
