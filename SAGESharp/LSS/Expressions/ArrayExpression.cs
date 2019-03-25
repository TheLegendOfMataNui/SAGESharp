using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class ArrayExpression : Expression
    {
        public List<Expression> Elements { get; }

        public ArrayExpression(IEnumerable<Expression> elements)
        {
            this.Elements = new List<Expression>(elements);
        }

        public override T AcceptVisitor<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitArrayExpression(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{ ");
            sb.Append(String.Join(", ", Elements));
            sb.Append(" }");

            return sb.ToString();
        }
    }
}
