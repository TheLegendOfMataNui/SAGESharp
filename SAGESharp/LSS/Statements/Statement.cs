using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS.Statements
{
    public interface StatementVisitor<T, C>
    {
        T VisitBlockStatement(BlockStatement s, C context);
        T VisitClassStatement(ClassStatement s, C context);
        T VisitPropertyStatement(PropertyStatement s, C context);
        T VisitSubroutineStatement(SubroutineStatement s, C context);
        T VisitGlobalStatement(GlobalStatement s, C context);
        T VisitExpressionStatement(ExpressionStatement s, C context);
        T VisitReturnStatement(ReturnStatement s, C context);
        T VisitIfStatement(IfStatement s, C context);
        T VisitWhileStatement(WhileStatement s, C context);
        T VisitAssignmentStatement(AssignmentStatement s, C context);
        T VisitVariableDeclarationStatement(VariableDeclarationStatement s, C context);
        T VisitForEachStatement(ForEachStatement s, C context);
    }

    public abstract class Statement
    {
        public abstract SourceSpan Span { get; }

        public abstract T AcceptVisitor<T, C>(StatementVisitor<T, C> visitor, C context);
    }
}
