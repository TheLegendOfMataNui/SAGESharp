using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (ConsumeIfType(TokenType.KeywordClass))
                {
                    SkipWhitespace();
                    // Header
                    Token name = ConsumeType(TokenType.Symbol, "Expected a class name.");
                    SkipWhitespace();
                    Token superclassName = null;
                    if (ConsumeIfType(TokenType.Colon))
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
                else if (ConsumeIfType(TokenType.KeywordFunction))
                {
                    SkipWhitespace();
                    result.Functions.Add(ParseSubroutine());
                }
                else if (ConsumeIfType(TokenType.KeywordGlobal))
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

            BlockStatement body = new BlockStatement(instructions);
            return new SubroutineStatement(name, parameters, body);
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

        private bool ConsumeIfType(TokenType type)
        {
            if (Peek().Type == type)
            {
                CurrentIndex++;
                return true;
            }
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
