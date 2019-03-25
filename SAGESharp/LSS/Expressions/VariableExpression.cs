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

        public override T AcceptVisitor<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitVariableExpression(this);
        }

        public override string ToString()
        {
            return Symbol.Content;
        }
    }
}
