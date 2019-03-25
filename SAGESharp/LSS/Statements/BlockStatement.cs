using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class BlockStatement : Statement
    {
        public List<InstructionStatement> Instructions;

        public BlockStatement(IEnumerable<InstructionStatement> instructions)
        {
            this.Instructions = new List<InstructionStatement>(instructions);
        }

        public override T AcceptVisitor<T>(StatementVisitor<T> visitor)
        {
            return visitor.VisitBlockStatement(this);
        }
    }
}
