using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class VariableExpression : Expression
    {
        public Token Symbol { get; }

        public VariableExpression(Token symbol)
        {
            this.Symbol = symbol;
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitVariableExpression(this, context);
        }

        public override string ToString()
        {
            return Symbol.Content;
        }
    }
}
