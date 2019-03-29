using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Expressions
{
    public class ConstructorExpression : Expression
    {
        public Token TypeName { get; }
        //public CallExpression Call { get; }
        public IEnumerable<Expression> Arguments { get; }
        
        /*public ConstructorExpression(Token typeName, CallExpression call)
        {
            this.TypeName = typeName;
            this.Call = call;
        }*/

        public ConstructorExpression(Token typeName, IEnumerable<Expression> arguments)
        {
            this.TypeName = typeName;
            this.Arguments = arguments;
        }

        public override T AcceptVisitor<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitConstructorExpression(this, context);
        }

        public override string ToString()
        {
            return "new " + TypeName.Content + "(" + String.Join(", ", Arguments) + ")";
        }
    }
}
