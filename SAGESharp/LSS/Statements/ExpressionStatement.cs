using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class ExpressionStatement : InstructionStatement
    {
        public Expressions.Expression Expression { get; }

        public override SourceSpan Span => Expression.Span;

        public ExpressionStatement(Expressions.Expression expression)
        {
            this.Expression = expression;
        }

        public override T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context)
        {
            return visitor.VisitExpressionStatement(this, context);
        }

        public override string ToString()
        {
            return Expression.ToString() + ";";
        }
    }
}
