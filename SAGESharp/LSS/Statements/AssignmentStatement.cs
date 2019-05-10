using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class AssignmentStatement : InstructionStatement
    {
        public Expressions.Expression Target { get; }
        public Expressions.Expression Value { get; }

        public override SourceSpan Span => Target.Span + Value.Span;

        public AssignmentStatement(Expressions.Expression target, Expressions.Expression value)
        {
            this.Target = target;
            this.Value = value;
        }

        public override T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context)
        {
            return visitor.VisitAssignmentStatement(this, context);
        }

        public override string ToString()
        {
            return Target.ToString() + " = " + Value.ToString() + ";";
        }
    }
}
