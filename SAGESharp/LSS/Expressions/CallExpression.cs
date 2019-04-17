using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class CallExpression : Expression
    {
        public override SourceSpan Span { get; }
        public Expression Target { get; }
        public List<Expression> Arguments { get; }

        public CallExpression(SourceSpan span, Expression target, IEnumerable<Expression> arguments)
        {
            this.Span = span;
            this.Target = target;
            this.Arguments = new List<Expression>(arguments); // copies the elements of 'arguments'
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitCallExpression(this, context);
        }

        public override string ToString()
        {
            return Target.ToString() + "(" + String.Join(", ", Arguments) + ")";
        }
    }
}
