using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class GlobalStatement : Statement
    {
        public override SourceSpan Span { get; }
        public Token Name;

        public GlobalStatement(SourceSpan span, Token name)
        {
            this.Span = span;
            this.Name = name;
        }

        public override T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context)
        {
            return visitor.VisitGlobalStatement(this, context);
        }

        public override string ToString()
        {
            return "global " + Name.Content + ";";
        }
    }
}
