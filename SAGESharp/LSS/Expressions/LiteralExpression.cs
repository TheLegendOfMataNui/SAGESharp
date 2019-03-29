using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class LiteralExpression : Expression
    {
        public Token Value { get; }

        public LiteralExpression(Token value)
        {
            this.Value = value;
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitLiteralExpression(this, context);
        }

        public override string ToString()
        {
            return Value.Content;
        }
    }
}
