using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class WhileStatement : InstructionStatement
    {
        public Expressions.Expression Condition { get; }
        public InstructionStatement Body { get; }

        public WhileStatement(Expressions.Expression condition, InstructionStatement body)
        {
            this.Condition = condition;
            this.Body = body;
        }

        public override T AcceptVisitor<T>(StatementVisitor<T> visitor)
        {
            return visitor.VisitWhileStatement(this);
        }

        public override string ToString()
        {
            return "while (" + Condition.ToString() + ")\n" + Body.ToString();
        }
    }
}
