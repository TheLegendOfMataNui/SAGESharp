using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class SubroutineStatement : Statement
    {
        public Token Name;
        public List<Token> Parameters = new List<Token>();
        public BlockStatement Body;

        public SubroutineStatement(Token name, IEnumerable<Token> parameters, BlockStatement body)
        {
            this.Name = name;
            this.Parameters = new List<Token>(parameters); // Copies the elements
            this.Body = body;
        }

        public override T AcceptVisitor<T>(StatementVisitor<T> visitor)
        {
            return visitor.VisitSubroutineStatement(this);
        }
    }
}
