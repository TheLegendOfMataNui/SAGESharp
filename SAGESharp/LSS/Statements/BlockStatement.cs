using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class BlockStatement : InstructionStatement
    {
        public List<InstructionStatement> Instructions { get; }

        public override SourceSpan Span { get; }

        public BlockStatement(SourceSpan span, IEnumerable<InstructionStatement> instructions)
        {
            this.Span = span;
            this.Instructions = new List<InstructionStatement>(instructions);
        }

        public override T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context)
        {
            return visitor.VisitBlockStatement(this, context);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            foreach (InstructionStatement s in Instructions)
            {
                sb.AppendLine(s.ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
