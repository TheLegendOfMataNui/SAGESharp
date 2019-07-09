using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class PropertyStatement : Statement
    {
        public override SourceSpan Span { get; }
        public Token Name;

        public PropertyStatement(SourceSpan span, Token name)
        {
            this.Span = span;
            this.Name = name;
        }

        public override T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context)
        {
            return visitor.VisitPropertyStatement(this, context);
        }

        public override string ToString()
        {
            return "property " + Name.Content + ";";
        }
    }
}
