﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public class SubroutineStatement : Statement
    {
        public override SourceSpan Span { get; }
        public Token Name;
        public List<Token> Parameters = new List<Token>();
        public BlockStatement Body;

        public SubroutineStatement(SourceSpan span, Token name, IEnumerable<Token> parameters, BlockStatement body)
        {
            this.Span = span;
            this.Name = name;
            this.Parameters = new List<Token>(parameters); // Copies the elements
            this.Body = body;
        }

        public override T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context)
        {
            return visitor.VisitSubroutineStatement(this, context);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Name.Content);
            sb.Append("(");
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(Parameters[i].Content);
            }
            sb.AppendLine(")");
            sb.Append(Body.ToString());

            return sb.ToString();
        }
    }
}
