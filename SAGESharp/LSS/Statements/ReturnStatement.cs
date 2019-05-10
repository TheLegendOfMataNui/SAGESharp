using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class ReturnStatement : InstructionStatement
    {
        public override SourceSpan Span => ReturnKeyword.Span + Value.Span;

        public Token ReturnKeyword { get; }
        public Expressions.Expression Value { get; }

        public ReturnStatement(Token returnKeyword, Expressions.Expression value)
        {
            this.ReturnKeyword = returnKeyword;
            this.Value = value;
        }

        public override T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context)
        {
            return visitor.VisitReturnStatement(this, context);
        }

        public override string ToString()
        {
            return "return" + (Value != null ? " " + Value.ToString() : "") + ";";
        }
    }
}
