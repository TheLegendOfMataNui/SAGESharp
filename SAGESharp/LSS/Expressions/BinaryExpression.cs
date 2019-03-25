using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class BinaryExpression : Expression
    {
        public Expression Left { get; }
        public Token Operation { get; }
        public Expression Right { get; }

        public BinaryExpression(Expression left, Token operation, Expression right)
        {
            this.Left = left;
            this.Operation = operation;
            this.Right = right;
        }

        public override T AcceptVisitor<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }
    }
}
