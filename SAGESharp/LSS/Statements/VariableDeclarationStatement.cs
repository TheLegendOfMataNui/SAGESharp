using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class VariableDeclarationStatement : InstructionStatement
    {
        public Token Name { get; }
        public Expressions.Expression Initializer { get; }

        public VariableDeclarationStatement(Token name, Expressions.Expression initializer)
        {
            this.Name = name;
            this.Initializer = initializer;
        }

        public override T AcceptVisitor<T>(StatementVisitor<T> visitor)
        {
            return visitor.VisitVariableDeclarationStatement(this);
        }

        public override string ToString()
        {
            return "var " + Name.Content + (Initializer == null ? "" : " = " + Initializer.ToString()) + ";";
        }
    }
}
