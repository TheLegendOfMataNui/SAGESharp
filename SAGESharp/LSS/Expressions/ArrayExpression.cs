using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class ArrayExpression : Expression
    {
        public override SourceSpan Span { get; }
        public List<Expression> Elements { get; }

        public ArrayExpression(SourceSpan span, IEnumerable<Expression> elements)
        {
            this.Span = span;
            this.Elements = new List<Expression>(elements);
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitArrayExpression(this, context);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[ ");
            sb.Append(String.Join(", ", Elements));
            sb.Append(" ]");

            return sb.ToString();
        }
    }
}
