using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class GroupingExpression : Expression
    {
        public override SourceSpan Span { get; }
        public Expression Contents { get; }

        public GroupingExpression(SourceSpan span, Expression contents)
        {
            this.Span = span;
            this.Contents = contents;
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitGroupingExpression(this, context);
        }

        public override Expression Duplicate()
        {
            return new GroupingExpression(Span, Contents);
        }

        public override string ToString()
        {
            return "(" + Contents.ToString() + ")";
        }
    }
}
