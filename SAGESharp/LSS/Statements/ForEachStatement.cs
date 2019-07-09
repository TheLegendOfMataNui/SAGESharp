using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class ForEachStatement : InstructionStatement
    {
        public override SourceSpan Span { get; }
        public Token Variable { get; }
        public Expressions.Expression Collection { get; }
        public InstructionStatement Body { get; }

        public ForEachStatement(SourceSpan span, Token variable, Expressions.Expression collection, InstructionStatement body)
        {
            this.Span = span;
            this.Variable = variable;
            this.Collection = collection;
            this.Body = body;
        }

        public override T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context)
        {
            return visitor.VisitForEachStatement(this, context);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("foreach (var ");
            sb.Append(Variable.Content);
            sb.Append(" : ");
            sb.Append(Collection.ToString());
            sb.AppendLine(")");
            sb.Append(Body.ToString());
            return sb.ToString();
        }
    }
}
