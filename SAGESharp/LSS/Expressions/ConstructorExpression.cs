using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class ConstructorExpression : Expression
    {
        public override SourceSpan Span { get; }
        public Token TypeName { get; }
        public IEnumerable<Expression> Arguments { get; }

        public ConstructorExpression(SourceSpan span, Token typeName, IEnumerable<Expression> arguments)
        {
            this.Span = span;
            this.TypeName = typeName;
            this.Arguments = arguments;
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitConstructorExpression(this, context);
        }

        public override string ToString()
        {
            return "new " + TypeName.Content + "(" + String.Join(", ", Arguments) + ")";
        }
    }
}
