using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class GroupingExpression : Expression
    {
        public Expression Contents { get; }

        public GroupingExpression(Expression contents)
        {
            this.Contents = contents;
        }

        public override T AcceptVisitor<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpression(this);
        }

        public override string ToString()
        {
            return "(" + Contents.ToString() + ")";
        }
    }
}
