using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class VariableExpression : Expression
    {
        public override SourceSpan Span => Symbol.Span;
        public Token Symbol { get; }

        public VariableExpression(Token symbol)
        {
            this.Symbol = symbol;
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitVariableExpression(this, context);
        }

        public override Expression Duplicate()
        {
            return new VariableExpression(Symbol); // Symbol is immutable
        }

        public override string ToString()
        {
            return Symbol.Content;
        }
    }
}
