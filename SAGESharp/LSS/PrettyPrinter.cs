using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAGESharp.LSS.Statements;
using SAGESharp.LSS.Expressions;

namespace SAGESharp.LSS
{
    public static class PrettyPrinter
    {
        public static string IndentationText { get; set; } = "    ";

        private class PrintContext : StatementVisitor<object, object>, ExpressionVisitor<object, object>
        {
            public StringBuilder StringBuilder { get; } = new StringBuilder();
            public int IndentationLevel { get; private set; }
            public string IndentationContents { get; private set; }

            public void PushIndent()
            {
                IndentationLevel++;
                IndentationContents += IndentationText;
            }

            public void PopIndent()
            {
                IndentationLevel--;
                IndentationContents = IndentationContents.Substring(0, IndentationText.Length * IndentationLevel);
            }

            public object VisitBinaryExpression(BinaryExpression expr, object context)
            {
                expr.Left.AcceptVisitor(this, context);
                if (expr.Operation.Type != TokenType.Period
                    && expr.Operation.Type != TokenType.PeriodDollarSign
                    && expr.Operation.Type != TokenType.ColonColon
                    && expr.Operation.Type != TokenType.ColonColonDollarSign)
                {
                    StringBuilder.Append(" ");
                }
                StringBuilder.Append(expr.Operation.Content);
                if (expr.Operation.Type != TokenType.Period
                    && expr.Operation.Type != TokenType.PeriodDollarSign
                    && expr.Operation.Type != TokenType.ColonColon
                    && expr.Operation.Type != TokenType.ColonColonDollarSign)
                {
                    StringBuilder.Append(" ");
                }
                expr.Right.AcceptVisitor(this, context);
                return null;
            }

            public object VisitLiteralExpression(LiteralExpression expr, object context)
            {
                StringBuilder.Append(expr.Value.Content);
                return null;
            }

            public object VisitVariableExpression(VariableExpression expr, object context)
            {
                StringBuilder.Append(expr.Symbol.Content);
                return null;
            }

            public object VisitGroupingExpression(GroupingExpression expr, object context)
            {
                StringBuilder.Append("(");
                expr.Contents.AcceptVisitor(this, context);
                StringBuilder.Append(")");
                return null;
            }

            public object VisitUnaryExpression(UnaryExpression expr, object context)
            {
                if (expr.IsPrefix)
                    StringBuilder.Append(expr.Operation.Content);
                expr.Contents.AcceptVisitor(this, context);
                if (!expr.IsPrefix)
                    StringBuilder.Append(expr.Operation.Content);
                return null;
            }

            public object VisitArrayAccessExpression(ArrayAccessExpression expr, object context)
            {
                expr.Array.AcceptVisitor(this, context);
                StringBuilder.Append("[");
                expr.Index.AcceptVisitor(this, context);
                StringBuilder.Append("]");
                return null;
            }

            public object VisitCallExpression(CallExpression expr, object context)
            {
                expr.Target.AcceptVisitor(this, context);
                StringBuilder.Append("(");
                for (int i = 0; i < expr.Arguments.Count; i++)
                {
                    if (i > 0)
                        StringBuilder.Append(", ");
                    expr.Arguments[i].AcceptVisitor(this, context);
                }
                StringBuilder.Append(")");
                return null;
            }

            public object VisitConstructorExpression(ConstructorExpression expr, object context)
            {
                StringBuilder.Append("new ");
                StringBuilder.Append(expr.TypeName.Content);
                StringBuilder.Append("(");
                int i = 0;
                foreach (Expression argument in expr.Arguments)
                {
                    if (i > 0)
                        StringBuilder.Append(", ");
                    argument.AcceptVisitor(this, context);
                    i++;
                }
                StringBuilder.Append(")");
                return null;
            }

            public object VisitArrayExpression(ArrayExpression expr, object context)
            {
                StringBuilder.Append("[ ");
                PushIndent();
                int i = 0;
                foreach (Expression element in expr.Elements)
                {
                    if (i > 0)
                    {
                        StringBuilder.AppendLine(", ");
                        StringBuilder.Append(IndentationContents);
                    }
                    element.AcceptVisitor(this, context);
                    i++;
                }
                PopIndent();
                StringBuilder.Append(" ]");
                return null;
            }

            public object VisitBlockStatement(BlockStatement s, object context)
            {
                StringBuilder.AppendLine("{");
                PushIndent();
                if (s.Instructions.Count == 0)
                {
                    StringBuilder.AppendLine(IndentationContents);
                }
                else
                {
                    foreach (Statement statement in s.Instructions)
                    {
                        StringBuilder.Append(IndentationContents);
                        statement.AcceptVisitor(this, context);
                        StringBuilder.AppendLine();
                    }
                }
                PopIndent();
                StringBuilder.Append(IndentationContents);
                StringBuilder.Append("}");
                return null;
            }

            public object VisitClassStatement(ClassStatement s, object context)
            {
                StringBuilder.Append("class ");
                StringBuilder.Append(s.Name.Content);
                if (s.SuperclassName != null)
                {
                    StringBuilder.Append(" : ");
                    StringBuilder.Append(s.SuperclassName.Content);
                }
                StringBuilder.AppendLine(" {");
                PushIndent();
                foreach (PropertyStatement property in s.Properties)
                {
                    StringBuilder.Append(IndentationContents);
                    property.AcceptVisitor(this, context);
                    StringBuilder.AppendLine();
                }
                foreach (SubroutineStatement method in s.Methods)
                {
                    StringBuilder.AppendLine(IndentationContents);
                    StringBuilder.Append(IndentationContents);
                    StringBuilder.Append("method ");
                    method.AcceptVisitor(this, context);
                    StringBuilder.AppendLine();
                }
                PopIndent();
                StringBuilder.Append(IndentationContents);
                StringBuilder.Append("}");
                return null;
            }

            public object VisitPropertyStatement(PropertyStatement s, object context)
            {
                StringBuilder.Append("property ");
                StringBuilder.Append(s.Name.Content);
                StringBuilder.Append(";");
                return null;
            }

            public object VisitSubroutineStatement(SubroutineStatement s, object context)
            {
                StringBuilder.Append(s.Name.Content);
                StringBuilder.Append("(");
                int i = 0;
                foreach (Token param in s.Parameters)
                {
                    if (i > 0)
                        StringBuilder.Append(", ");
                    StringBuilder.Append(param.Content);
                    i++;
                }
                StringBuilder.Append(") ");
                s.Body.AcceptVisitor(this, context);
                return null;
            }

            public object VisitGlobalStatement(GlobalStatement s, object context)
            {
                StringBuilder.Append("global ");
                StringBuilder.Append(s.Name.Content);
                StringBuilder.Append(";");
                return null;
            }

            public object VisitExpressionStatement(ExpressionStatement s, object context)
            {
                s.Expression.AcceptVisitor(this, context);
                StringBuilder.Append(";");
                return null;
            }

            public object VisitReturnStatement(ReturnStatement s, object context)
            {
                if (s.Value == null)
                {
                    StringBuilder.Append("return;");
                }
                else
                {
                    StringBuilder.Append("return ");
                    s.Value.AcceptVisitor(this, context);
                    StringBuilder.Append(";");
                }
                return null;
            }

            public object VisitIfStatement(IfStatement s, object context)
            {
                IfStatement current = s;
                while (current != null)
                {
                    if (current.Condition != null)
                    {
                        StringBuilder.Append("if (");
                        current.Condition.AcceptVisitor(this, context);
                        StringBuilder.Append(") ");
                    }
                    current.Body.AcceptVisitor(this, context);
                    current = current.ElseStatement;
                    if (current != null)
                    {
                        StringBuilder.AppendLine();
                        StringBuilder.Append(IndentationContents);
                        StringBuilder.Append("else ");
                    }
                }
                return null;
            }

            public object VisitWhileStatement(WhileStatement s, object context)
            {
                StringBuilder.Append("while (");
                s.Condition.AcceptVisitor(this, context);
                StringBuilder.Append(") ");
                s.Body.AcceptVisitor(this, context);
                return null;
            }

            public object VisitAssignmentStatement(AssignmentStatement s, object context)
            {
                s.Target.AcceptVisitor(this, context);
                StringBuilder.Append(" = ");
                s.Value.AcceptVisitor(this, context);
                StringBuilder.Append(";");
                return null;
            }

            public object VisitVariableDeclarationStatement(VariableDeclarationStatement s, object context)
            {
                StringBuilder.Append("var ");
                StringBuilder.Append(s.Name.Content);
                if (s.Initializer != null)
                {
                    StringBuilder.Append(" = ");
                    s.Initializer.AcceptVisitor(this, context);
                }
                StringBuilder.Append(";");
                return null;
            }

            public object VisitForEachStatement(ForEachStatement s, object context)
            {
                StringBuilder.Append("foreach (var ");
                StringBuilder.Append(s.Variable.Content);
                StringBuilder.Append(" : ");
                s.Collection.AcceptVisitor(this, context);
                StringBuilder.Append(") ");
                s.Body.AcceptVisitor(this, context);
                return null;
            }

            public object VisitDoWhileStatement(DoWhileStatement s, object context)
            {
                StringBuilder.Append("do ");
                s.Body.AcceptVisitor(this, context);
                StringBuilder.Append(" while (");
                s.Condition.AcceptVisitor(this, context);
                StringBuilder.Append(")");
                return null;
            }
        }

        public static string Print(params Statements.Statement[] statements)
        {
            PrintContext context = new PrintContext();

            foreach (Statement statement in statements)
            {
                statement.AcceptVisitor(context, null);
            }

            return context.StringBuilder.ToString();
        }

        public static string Print(params Expression[] expressions)
        {
            PrintContext context = new PrintContext();

            foreach (Expression expression in expressions)
            {
                expression.AcceptVisitor(context, null);
            }

            return context.StringBuilder.ToString();
        }

        public static string Print(Parser.Result parseResults)
        {
            PrintContext context = new PrintContext();

            foreach (GlobalStatement global in parseResults.Globals)
            {
                global.AcceptVisitor(context, null);
                context.StringBuilder.AppendLine();
            }
            context.StringBuilder.AppendLine();
            foreach (SubroutineStatement function in parseResults.Functions)
            {
                context.StringBuilder.Append("function ");
                function.AcceptVisitor(context, null);
                context.StringBuilder.AppendLine();
                context.StringBuilder.AppendLine();
            }
            foreach (ClassStatement cls in parseResults.Classes)
            {
                cls.AcceptVisitor(context, null);
                context.StringBuilder.AppendLine();
                context.StringBuilder.AppendLine();
            }

            return context.StringBuilder.ToString();
        }
    }
}
