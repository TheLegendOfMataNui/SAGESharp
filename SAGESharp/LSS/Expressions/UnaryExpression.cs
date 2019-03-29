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
        public bool IsPrefix { get; }

        public UnaryExpression(Expression contents, Token operation, bool isPrefix)
        {
            this.Contents = contents;
            this.Operation = operation;
            this.IsPrefix = isPrefix;
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitUnaryExpression(this, context);
        }

        public override string ToString()
        {
            if (IsPrefix)
                return Operation.Content + Contents.ToString();
            else
                return Contents.ToString() + Operation.Content;
        }
    }
}
