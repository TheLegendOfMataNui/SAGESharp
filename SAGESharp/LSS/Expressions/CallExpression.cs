using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class CallExpression : Expression
    {
        public Expression Target { get; }
        public List<Expression> Arguments { get; }

        public CallExpression(Expression target, IEnumerable<Expression> arguments)
        {
            this.Target = target;
            this.Arguments = new List<Expression>(arguments); // copies the elements of 'arguments'
        }

        public override T AcceptVisitor<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitCallExpression(this);
        }

        public override string ToString()
        {
            return Target.ToString() + "(" + String.Join(", ", Arguments) + ")";
        }
    }
}
