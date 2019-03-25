using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class ReturnStatement : InstructionStatement
    {
        public Token ReturnKeyword { get; }
        public Expressions.Expression Value { get; }

        public ReturnStatement(Token returnKeyword, Expressions.Expression value)
        {
            this.ReturnKeyword = returnKeyword;
            this.Value = value;
        }

        public override T AcceptVisitor<T>(StatementVisitor<T> visitor)
        {
            return visitor.VisitReturnStatement(this);
        }

        public override string ToString()
        {
            return "return" + (Value != null ? " " + Value.ToString() : "") + ";";
        }
    }
}
