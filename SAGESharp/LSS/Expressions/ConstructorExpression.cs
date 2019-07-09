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
        public List<Expression> Arguments { get; } // HACK: Null for Arguments means this is just the instantiation without the constructor call.

        public ConstructorExpression(SourceSpan span, Token typeName, IEnumerable<Expression> arguments)
        {
            this.Span = span;
            this.TypeName = typeName;
            if (arguments != null)
                this.Arguments = new List<Expression>(arguments);
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitConstructorExpression(this, context);
        }

        public override Expression Duplicate()
        {
            List<Expression> clonedArguments = null;
            if (Arguments != null)
            {
                clonedArguments = new List<Expression>();
                foreach (Expression argument in Arguments)
                {
                    clonedArguments.Add(argument.Duplicate());
                }
            }
            return new ConstructorExpression(Span, TypeName, clonedArguments); // TypeName is immutable
        }

        public override string ToString()
        {
            return "new " + TypeName.Content + "(" + String.Join(", ", Arguments) + ")";
        }
    }
}
