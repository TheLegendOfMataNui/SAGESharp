using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class ClassStatement : Statement
    {
        public Token Name;
        public Token SuperclassName;
        public List<PropertyStatement> Properties = new List<PropertyStatement>();
        public List<SubroutineStatement> Methods = new List<SubroutineStatement>();

        public override T AcceptVisitor<T>(StatementVisitor<T> visitor)
        {
            return visitor.VisitClassStatement(this);
        }
    }
}
