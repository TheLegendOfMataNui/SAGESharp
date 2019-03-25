using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class IfStatement : InstructionStatement
    {
        public Expressions.Expression Condition { get; } // Could be null for an 'else' statement
        public InstructionStatement Body { get; }
        public IfStatement ElseStatement { get; } // Like a linked list, points to the next else (could be an 'else if' if it has a Condition)

        public IfStatement(Expressions.Expression condition, InstructionStatement body, IfStatement elseStatement)
        {
            this.Condition = condition;
            this.Body = body;
            this.ElseStatement = elseStatement;
        }

        public override T AcceptVisitor<T>(StatementVisitor<T> visitor)
        {
            return visitor.VisitIfStatement(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Condition != null)
            {
                sb.Append("if (");
                sb.Append(Condition.ToString());
                sb.Append(")");
            }
            sb.AppendLine();
            sb.Append(Body.ToString());
            if (ElseStatement != null)
            {
                sb.AppendLine();
                sb.Append("else ");
                sb.Append(ElseStatement.ToString());
            }
            return sb.ToString();
        }
    }
}
