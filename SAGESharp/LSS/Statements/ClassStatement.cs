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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("class ");
            sb.Append(Name.Content);
            if (SuperclassName != null)
            {
                sb.Append(" : ");
                sb.Append(SuperclassName.Content);
            }
            sb.AppendLine(" {");

            foreach (PropertyStatement p in Properties)
            {
                sb.Append("property ");
                sb.Append(p.Name.Content);
                sb.AppendLine(";");
            }

            foreach (SubroutineStatement s in Methods)
            {
                sb.Append("method ");
                sb.AppendLine(s.ToString());
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}
