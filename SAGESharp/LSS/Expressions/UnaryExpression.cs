using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class UnaryExpression : Expression
    {
        public Expression Contents { get; }
        public Token Operation { get; }

        public UnaryExpression(Expression contents, Token operation)
        {
            this.Contents = contents;
            this.Operation = operation;
        }

        public override T AcceptVisitor<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpression(this);
        }
    }
}
