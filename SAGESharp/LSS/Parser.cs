using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAGESharp.LSS.Expressions;
using SAGESharp.LSS.Statements;

namespace SAGESharp.LSS
{
    public class Parser
    {
        public class Result
        {
            public List<ClassStatement> Classes = new List<ClassStatement>();
            public List<SubroutineStatement> Functions = new List<SubroutineStatement>();
            public List<GlobalStatement> Globals = new List<GlobalStatement>();
            public List<SyntaxError> Errors = new List<SyntaxError>();
        }

        private List<Token> Tokens;
        private List<SyntaxError> Errors;
        private int CurrentIndex;
        private int CurrentLine;

        public Result Parse(List<Token> tokens)
        {
            this.Tokens = tokens;
            this.CurrentIndex = 0;
            this.CurrentLine = 0;
            Result result = new Result();
            this.Errors = result.Errors;

            while (!IsAtEnd())
            {
                SkipWhitespace();
                if (ConsumeIfType(out _, TokenType.KeywordClass))
                {
                    SkipWhitespace();
                    // Header
                    Token name = ConsumeType(TokenType.Symbol, "Expected a class name.");
                    SkipWhitespace();
                    Token superclassName = null;
                    if (ConsumeIfType(out _, TokenType.Colon))
                    {
                        SkipWhitespace();
                        superclassName = ConsumeType(TokenType.Symbol, "Expected a superclass name.");
                        SkipWhitespace();
                    }
                    ConsumeType(TokenType.OpenBrace, "Expected a class body.");
                    SkipWhitespace();
                    ClassStatement cls = new ClassStatement();
                    cls.Name = name;
                    cls.SuperclassName = superclassName;

                    // Body
                    while (!IsAtEnd() && Peek().Type != TokenType.CloseBrace)
                    {
                        Token t = Consume();
                        SkipWhitespace();
                        if (t.Type == TokenType.KeywordProperty)
                        {
                            Token propertyName = ConsumeType(TokenType.Symbol, "Expected a property name.");
                            cls.Properties.Add(new PropertyStatement(propertyName));
                            ConsumeType(TokenType.Semicolon, "Expected semicolon after property name.");
                        }
                        else if (t.Type == TokenType.KeywordMethod)
                        {
                            cls.Methods.Add(ParseSubroutine());
                        }
                        else
                        {
                            Errors.Add(new SyntaxError("Expected property or method.", t.SourceLocation.Offset, t.SourceLength, CurrentLine));
                        }
                        SkipWhitespace();
                    }

                    ConsumeType(TokenType.CloseBrace, "Expected a closing brace for class body.");
                    result.Classes.Add(cls);
                }
                else if (ConsumeIfType(out _, TokenType.KeywordFunction))
                {
                    SkipWhitespace();
                    result.Functions.Add(ParseSubroutine());
                }
                else if (ConsumeIfType(out _, TokenType.KeywordGlobal))
                {
                    SkipWhitespace();
                    result.Globals.Add(new GlobalStatement(ConsumeType(TokenType.Symbol, "Expected a name for the global.")));
                    ConsumeType(TokenType.Semicolon, "Expected a semicolon after global name.");
                }
                else
                {
                    Token extra = Consume();
                    if (extra.Type != TokenType.Whitespace && extra.Type != TokenType.Comment && extra.Type != TokenType.MultilineComment)
                    {
                        result.Errors.Add(new SyntaxError("Expected class or function", extra.SourceLocation.Offset, extra.SourceLength, CurrentLine));
                    }
                }
            }

            return result;
        }

        private SubroutineStatement ParseSubroutine()
        {
            Token name = ConsumeType(TokenType.Symbol, "Expected a name.");
            ConsumeType(TokenType.OpenParenthesis, "Expected an open parenthesis after subroutine name.");

            // Parameters
            SkipWhitespace();
            List<Token> parameters = new List<Token>();
            while (!IsAtEnd() && Peek().Type != TokenType.CloseParenthesis)
            {
                if (parameters.Count > 0)
                {
                    ConsumeType(TokenType.Comma, "Expected a comma or closing parenthesis after parameter.");
                    SkipWhitespace();
                }
                Token parameter = ConsumeType(TokenType.Symbol, "Expected a parameter name.");
                parameters.Add(parameter);
                SkipWhitespace();
            }
            if (IsAtEnd())
            {
                Errors.Add(new SyntaxError("Unterminated parameter list by the end of the source.", CurrentIndex, 0, CurrentLine));
                return new SubroutineStatement(name, parameters, new BlockStatement(new List<InstructionStatement>()));
            }
            ConsumeType(TokenType.CloseParenthesis, "Unterminated subroutine parameter list by the end of the source.");
            SkipWhitespace();

            // Body
            BlockStatement body = ParseBlock();
            return new SubroutineStatement(name, parameters, body);
        }

        private BlockStatement ParseBlock()
        {
            List<InstructionStatement> instructions = new List<InstructionStatement>();
            ConsumeType(TokenType.OpenBrace, "Expected an open brace for subroutine body.");
            SkipWhitespace();

            while (!IsAtEnd() && Peek().Type != TokenType.CloseBrace)
            {
                // TODO: Read ExecutableStatement
                Consume(); // Hack to not get stuck

                SkipWhitespace();
            }
            ConsumeType(TokenType.CloseBrace, "Unterminated subroutine body by the end of the source.");

            BlockStatement result = new BlockStatement(instructions);
            return result;
        }

        private InstructionStatement ParseInstructionStatement()
        {
            throw new NotImplementedException();
        }

        public Expression ParseExpression(List<Token> tokens)
        {
            this.Tokens = tokens;
            this.CurrentIndex = 0;
            this.CurrentLine = 0;
            Result result = new Result();
            this.Errors = result.Errors;

            return ParseExpression();
        }

        // TODO: Revert to private after testing
        private Expression ParseExpression()
        {
            return ParseLogicalOrExpression();
        }

        // true, false, null, integer, float, string, variable, ( )
        private Expression ParseTerminalExpression()
        {
            // TODO: Parse array initializers
            Token t = Consume();
            if (t.Type == TokenType.KeywordTrue || t.Type == TokenType.KeywordFalse
                || t.Type == TokenType.KeywordNull || t.Type == TokenType.IntegerLiteral
                || t.Type == TokenType.FloatLiteral || t.Type == TokenType.StringLiteral)
                return new LiteralExpression(t);
            else if (t.Type == TokenType.Symbol)
                return new VariableExpression(t);
            else if (t.Type == TokenType.OpenParenthesis)
            {
                Expression child = ParseExpression();
                SkipWhitespace();
                Token closeParen = Consume();
                // TODO: Assert that closeParen is, in fact, a close parenthesis
                // once we have a proper panic & sync mechanism
                return new GroupingExpression(child);
            }
            else // TODO: Add proper panic & sync
            {
                Errors.Add(new SyntaxError("Expected an expression.", t.SourceLocation.Offset, t.SourceLength, CurrentLine));
                throw new Exception("Expected an expression.");
            }
        }

        // [ ] ( ) . .$ :: ++ (postfix) -- (postfix) // Note: Right now parentheses are handled by ParseTerminalExpression()
        private Expression ParseScopeExpression()
        {
            Expression result = ParseTerminalExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.OpenSquareBracket, /*TokenType.OpenParenthesis, */TokenType.Period, TokenType.PeriodDollarSign, TokenType.ColonColon, TokenType.PlusPlus, TokenType.DashDash))
            {
                if (token.Type == TokenType.OpenSquareBracket)
                {
                    Expression index = ParseExpression();
                    SkipWhitespace();
                    Token closeSquareBracket = Consume();
                    // TODO: Assert that that was actually a close square bracket
                    // once we have a proper panic & sync system
                    result = new ArrayAccessExpression(result, index);
                }
                /*else if (token.Type == TokenType.OpenParenthesis)
                {
                    Expression content = 
                }*/
                else if (token.Type == TokenType.Period || token.Type == TokenType.PeriodDollarSign || token.Type == TokenType.ColonColon)
                {
                    Expression rhs = ParseTerminalExpression();
                    result = new BinaryExpression(result, token, rhs);
                    SkipWhitespace();
                }
                else if (token.Type == TokenType.PlusPlus || token.Type == TokenType.DashDash)
                {
                    result = new UnaryExpression(result, token);
                    SkipWhitespace();
                }
            }
            return result;
        }

        // - ! ~ ++ (prefix) -- (prefix)
        private Expression ParseUnaryExpression()
        {
            SkipWhitespace();
            if (ConsumeIfType(out Token token, TokenType.Dash, TokenType.Exclamation, TokenType.Tilde, TokenType.PlusPlus, TokenType.DashDash))
            {
                Expression inner = ParseUnaryExpression();
                return new UnaryExpression(inner, token);
            }
            return ParseScopeExpression();
        }

        // ^
        private Expression ParseExponentialExpression()
        {
            Expression result = ParseUnaryExpression();

            SkipWhitespace();
            if (ConsumeIfType(out Token token, TokenType.Caret))
            {
                result = new BinaryExpression(result, token, ParseExponentialExpression());
            }
            return result;
        }

        // * / % 
        private Expression ParseMultiplicitaveExpression()
        {
            Expression result = ParseExponentialExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.Asterisk, TokenType.Slash, TokenType.Percent))
            {
                Expression rhs = ParseExponentialExpression();
                result = new BinaryExpression(result, token, rhs);
                SkipWhitespace();
            }
            return result;
        }

        // + -
        private Expression ParseAdditiveExpression()
        {
            Expression result = ParseMultiplicitaveExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.Plus, TokenType.Dash))
            {
                Expression rhs = ParseMultiplicitaveExpression();
                result = new BinaryExpression(result, token, rhs);
                SkipWhitespace();
            }
            return result;
        }

        // << >>
        private Expression ParseShiftExpression()
        {
            Expression result = ParseAdditiveExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.LessLess, TokenType.GreaterGreater))
            {
                Expression rhs = ParseAdditiveExpression();
                result = new BinaryExpression(result, token, rhs);
                SkipWhitespace();
            }
            return result;
        }

        // < > <= >=
        private Expression ParseRelationalExpression()
        {
            Expression result = ParseShiftExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.Less, TokenType.Greater, TokenType.LessEquals, TokenType.GreaterEquals))
            {
                Expression rhs = ParseShiftExpression();
                result = new BinaryExpression(result, token, rhs);
                SkipWhitespace();
            }
            return result;
        }

        // == !=
        private Expression ParseEqualityExpression()
        {
            Expression result = ParseRelationalExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.EqualsEquals, TokenType.ExclamationEquals))
            {
                Expression rhs = ParseRelationalExpression();
                result = new BinaryExpression(result, token, rhs);
                SkipWhitespace();
            }
            return result;
        }

        // &
        private Expression ParseAndExpression()
        {
            Expression result = ParseEqualityExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.Ampersand))
            {
                Expression rhs = ParseEqualityExpression();
                result = new BinaryExpression(result, token, rhs);
                SkipWhitespace();
            }
            return result;
        }

        // #
        private Expression ParseXorExpression()
        {
            Expression result = ParseAndExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.Octothorpe))
            {
                Expression rhs = ParseAndExpression();
                result = new BinaryExpression(result, token, rhs);
                SkipWhitespace();
            }
            return result;
        }

        // |
        private Expression ParseOrExpression()
        {
            Expression result = ParseXorExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.Pipe))
            {
                Expression rhs = ParseXorExpression();
                result = new BinaryExpression(result, token, rhs);
                SkipWhitespace();
            }
            return result;
        }

        // &&
        private Expression ParseLogicalAndExpression()
        {
            Expression result = ParseOrExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.AmpersandAmpersand))
            {
                Expression rhs = ParseOrExpression();
                result = new BinaryExpression(result, token, rhs);
                SkipWhitespace();
            }
            return result;
        }

        // ||
        private Expression ParseLogicalOrExpression()
        {
            Expression result = ParseLogicalAndExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.PipePipe))
            {
                Expression rhs = ParseLogicalAndExpression();
                result = new BinaryExpression(result, token, rhs);
                SkipWhitespace();
            }
            return result;
        }

        private Token Peek()
        {
            if (CurrentIndex >= Tokens.Count)
            {
                return new Token(TokenType.EndOfStream, "", CurrentIndex, 0);
            }
            return Tokens[CurrentIndex];
        }

        private void SkipWhitespace()
        {
            while (!IsAtEnd() && (Peek().Type == TokenType.Whitespace || Peek().Type == TokenType.Comment && Peek().Type == TokenType.MultilineComment))
            {
                Token whitespace = Consume();
                for (int i = 0; i < whitespace.Content.Length; i++)
                {
                    if (whitespace.Content[i] == '\n')
                        CurrentLine++;
                }
            }
        }

        private bool ConsumeIfType(out Token result, params TokenType[] types)
        {
            TokenType t = Peek().Type;
            foreach (TokenType type in types)
            {
                if (t == type)
                {
                    result = Consume();
                    return true;
                }
            }
            result = null;
            return false;
        }

        private Token ConsumeType(TokenType type, string failureMessage)
        {
            if (CurrentIndex >= Tokens.Count)
            {
                return new Token(TokenType.EndOfStream, "", CurrentIndex, 0);
            }
            Token result = Consume();
            if (result.Type != type)
            {
                this.Errors.Add(new SyntaxError(failureMessage, result.SourceLocation.Offset, result.SourceLength, this.CurrentLine));
            }
            return result;
        }

        private Token Consume()
        {
            return Tokens[CurrentIndex++];
        }

        private bool IsAtEnd()
        {
            return CurrentIndex >= Tokens.Count || Tokens[CurrentIndex].Type == TokenType.EndOfStream;
        }
    }
}
