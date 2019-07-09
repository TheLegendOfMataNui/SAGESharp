using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class VariableDeclarationStatement : InstructionStatement
    {
        public override SourceSpan Span { get; }
        public Token Name { get; }
        public Expressions.Expression Initializer { get; }

        public VariableDeclarationStatement(SourceSpan span, Token name, Expressions.Expression initializer)
        {
            this.Span = span;
            this.Name = name;
            this.Initializer = initializer;
        }

        public override T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context)
        {
            return visitor.VisitVariableDeclarationStatement(this, context);
        }

        public override string ToString()
        {
            return "var " + Name.Content + (Initializer == null ? "" : " = " + Initializer.ToString()) + ";";
        }
    }
}
