using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public interface StatementVisitor<T>
    {
        T VisitBlockStatement(BlockStatement s);
        T VisitClassStatement(ClassStatement s);
        T VisitPropertyStatement(PropertyStatement s);
        T VisitSubroutineStatement(SubroutineStatement s);
        T VisitGlobalStatement(GlobalStatement s);
    }

    public abstract class Statement
    {
        public TextLocation SourceLocation;
        public int SourceLength;

        public abstract T AcceptVisitor<T>(StatementVisitor<T> visitor);
    }
}
