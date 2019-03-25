using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class PropertyStatement : Statement
    {
        public Token Name;

        public PropertyStatement(Token name)
        {
            this.Name = name;
        }

        public override T AcceptVisitor<T>(StatementVisitor<T> visitor)
        {
            return visitor.VisitPropertyStatement(this);
        }
    }
}
