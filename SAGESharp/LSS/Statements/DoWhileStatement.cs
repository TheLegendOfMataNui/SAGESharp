using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class DoWhileStatement : InstructionStatement
    {
        public override SourceSpan Span { get; }
        public InstructionStatement Body { get; }
        public Expressions.Expression Condition { get; }

        public DoWhileStatement(SourceSpan span, InstructionStatement body, Expressions.Expression condition)
        {
            this.Span = span;
            this.Condition = condition;
            this.Body = body;
        }

        public override T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context)
        {
            return visitor.VisitDoWhileStatement(this, context);
        }

        public override string ToString()
        {
            return "do " + Body.ToString() + " while (" + Condition.ToString() + ")";
        }
    }
}
