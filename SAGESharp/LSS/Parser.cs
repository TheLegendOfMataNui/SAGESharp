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
        private uint CurrentLine;
        private string CurrentFilename;

        public Result Parse(List<Token> tokens, Compiler.Settings settings = null)
        {
            if (settings == null)
                settings = new Compiler.Settings();

            this.Tokens = tokens;
            this.CurrentIndex = 0;
            this.CurrentLine = 0;
            Result result = new Result();
            this.Errors = result.Errors;

            while (!IsAtEnd())
            {
                SkipWhitespace();
                if (ConsumeIfType(out Token classToken, TokenType.KeywordClass))
                {
                    SkipWhitespace();
                    
                    result.Classes.Add(ParseClassStatement(classToken));
                }
                else if (ConsumeIfType(out Token functionKeyword, TokenType.KeywordFunction))
                {
                    SkipWhitespace();
                    result.Functions.Add(ParseSubroutineStatement(functionKeyword));
                }
                else if (ConsumeIfType(out Token globalKeyword, TokenType.KeywordGlobal))
                {
                    SkipWhitespace();
                    Token globalName = ConsumeType(TokenType.Symbol, "Expected a name for the global.");
                    result.Globals.Add(new GlobalStatement(globalKeyword.Span + globalName.Span, globalName));
                    ConsumeType(TokenType.Semicolon, "Expected a semicolon after global name.");
                }
                else
                {
                    Token extra = Consume();
                    if (extra.Type != TokenType.Whitespace && extra.Type != TokenType.Comment && extra.Type != TokenType.MultilineComment && extra.Type != TokenType.EndOfStream)
                    {
                        result.Errors.Add(new SyntaxError("Expected global, class, or function", extra.Span));
                    }
                }
            }

            return result;
        }

        private ClassStatement ParseClassStatement(Token classKeyword)
        {
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
            List<PropertyStatement> properties = new List<PropertyStatement>();
            List<SubroutineStatement> methods = new List<SubroutineStatement>();

            // Body
            while (!IsAtEnd() && Peek().Type != TokenType.CloseBrace)
            {
                Token t = Consume();
                SkipWhitespace();
                if (t.Type == TokenType.KeywordProperty)
                {
                    Token propertyName = ConsumeType(TokenType.Symbol, "Expected a property name.");
                    properties.Add(new PropertyStatement(t.Span + propertyName.Span, propertyName));
                    ConsumeType(TokenType.Semicolon, "Expected semicolon after property name.");
                }
                else if (t.Type == TokenType.KeywordMethod)
                {
                    methods.Add(ParseSubroutineStatement(t));
                }
                else
                {
                    Errors.Add(new SyntaxError("Expected property or method.", t.Span));
                }
                SkipWhitespace();
            }

            Token closeBrace = ConsumeType(TokenType.CloseBrace, "Expected a closing brace for class body.");
            return new ClassStatement(classKeyword.Span + closeBrace.Span, name, superclassName, properties, methods);
        }

        private SubroutineStatement ParseSubroutineStatement(Token startKeyword)
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
                Errors.Add(new SyntaxError("Unterminated parameter list by the end of the source.", Tokens.Count > 0 ? Tokens[Tokens.Count - 1].Span.Start.Filename : "<unknown>", CurrentIndex, CurrentLine, 0));
                return new SubroutineStatement(startKeyword.Span + Peek().Span, name, parameters, new BlockStatement(Peek().Span, new List<InstructionStatement>()));
            }
            ConsumeType(TokenType.CloseParenthesis, "Unterminated subroutine parameter list by the end of the source.");
            SkipWhitespace();

            // Body
            BlockStatement body = ParseBlockStatement();
            return new SubroutineStatement(startKeyword.Span + body.Span, name, parameters, body);
        }

        private BlockStatement ParseBlockStatement()
        {
            List<InstructionStatement> instructions = new List<InstructionStatement>();
            Token openBrace = ConsumeType(TokenType.OpenBrace, "Expected an open brace for subroutine body.");
            SkipWhitespace();

            while (!IsAtEnd() && Peek().Type != TokenType.CloseBrace)
            {
                instructions.Add(ParseInstructionStatement());
                SkipWhitespace();
            }
            Token closeBrace = ConsumeType(TokenType.CloseBrace, "Unterminated subroutine body by the end of the source.");

            BlockStatement result = new BlockStatement(openBrace.Span + closeBrace.Span, instructions);
            return result;
        }

        private IfStatement ParseIfStatement()
        {
            Token ifKeyword = ConsumeType(TokenType.KeywordIf, "Expected if keyword.");
            SourceSpan span = ifKeyword.Span;
            SkipWhitespace();
            ConsumeType(TokenType.OpenParenthesis, "Expected an open parenthesis after if keyword.");
            Expression condition = ParseExpression();
            SkipWhitespace();
            ConsumeType(TokenType.CloseParenthesis, "Expected a close parenthesis after if condition.");
            InstructionStatement body = ParseInstructionStatement();
            span = span + body.Span;
            SkipWhitespace();
            IfStatement elseStatement = null;
            if (ConsumeIfType(out Token elseKeyword, TokenType.KeywordElse))
            {
                SkipWhitespace();
                if (Peek().Type == TokenType.KeywordIf)
                {
                    elseStatement = ParseIfStatement();
                }
                else
                {
                    InstructionStatement elseBody = ParseInstructionStatement();
                    elseStatement = new IfStatement(elseKeyword.Span + elseBody.Span, null, elseBody, null);
                }
                span = span + elseStatement.Span;
                SkipWhitespace();
            }
            return new IfStatement(span, condition, body, elseStatement);
        }

        private ForEachStatement ParseForEachStatement()
        {
            Token forEachKeyword = ConsumeType(TokenType.KeywordForEach, "Expected foreach keyword.");
            SourceSpan span = forEachKeyword.Span;
            SkipWhitespace();
            /*span += */ConsumeType(TokenType.OpenParenthesis, "Expected open parenthesis after foreach keyword.")/*.Span*/;
            SkipWhitespace();
            ConsumeType(TokenType.KeywordVar, "Expected var keyword when declaring iteration variable.");
            SkipWhitespace();
            Token variableName = ConsumeType(TokenType.Symbol, "Expected name of iteration variable.");
            //span += variableName.Span;
            SkipWhitespace();
            ConsumeType(TokenType.Colon, "Expected colon after iteration variable.");
            SkipWhitespace();
            Expression collection = ParseExpression();
            SkipWhitespace();
            ConsumeType(TokenType.CloseParenthesis, "Expected close parenthesis after collection.");
            SkipWhitespace();
            InstructionStatement body = ParseInstructionStatement();
            span += body.Span;
            return new ForEachStatement(span, variableName, collection, body);
        }

        private InstructionStatement ParseInstructionStatement()
        {
            // { means block
            SkipWhitespace();
            if (Peek().Type == TokenType.OpenBrace)
            {
                return ParseBlockStatement();
            }
            else if (Peek().Type == TokenType.KeywordIf)
            {
                return ParseIfStatement();
            }
            else if (Peek().Type == TokenType.KeywordForEach)
            {
                return ParseForEachStatement();
            }
            else if (ConsumeIfType(out Token whileKeyword, TokenType.KeywordWhile))
            {
                SkipWhitespace();
                ConsumeType(TokenType.OpenParenthesis, "Expected an open parenthesis after while keyword.");
                Expression condition = ParseExpression();
                SkipWhitespace();
                ConsumeType(TokenType.CloseParenthesis, "Expected a close parenthesis after while condition.");
                InstructionStatement body = ParseInstructionStatement();
                return new WhileStatement(whileKeyword.Span + body.Span, condition, body);
            }
            else if (ConsumeIfType(out Token returnKeyword, TokenType.KeywordReturn))
            {
                if (ConsumeIfType(out _, TokenType.Semicolon))
                {
                    return new ReturnStatement(returnKeyword, null);
                }
                else
                {
                    Expression value = ParseExpression();
                    SkipWhitespace();
                    ConsumeType(TokenType.Semicolon, "Expected a semicolon after expression to return.");
                    return new ReturnStatement(returnKeyword, value);
                }
            }
            else if (ConsumeIfType(out Token varKeyword, TokenType.KeywordVar))
            {
                SkipWhitespace();
                Token name = ConsumeType(TokenType.Symbol, "Expected a name for variable declaration.");
                Expression initializer = null;
                SkipWhitespace();
                if (ConsumeIfType(out _, TokenType.Equals))
                {
                    initializer = ParseExpression();
                }
                SkipWhitespace();
                ConsumeType(TokenType.Semicolon, "Expected a semicolon after variable initializer.");
                return new VariableDeclarationStatement(varKeyword.Span + name.Span, name, initializer);
            }
            else
            {
                Expression value = ParseExpression();
                SkipWhitespace();
                if (ConsumeIfType(out Token equalsOperator, TokenType.Equals))
                {
                    Expression rhs = ParseExpression();
                    SkipWhitespace();
                    ConsumeType(TokenType.Semicolon, "Expected semicolon after assignment statement.");
                    return new AssignmentStatement(value, rhs);
                }
                else
                {
                    ConsumeType(TokenType.Semicolon, "Expected semicolon after expression statement.");
                    return new ExpressionStatement(value);
                }
            }
        }

        // Hack for testing expression parsing by itself
        public Expression ParseExpression(List<Token> tokens)
        {
            this.Tokens = tokens;
            this.CurrentIndex = 0;
            this.CurrentLine = 0;
            Result result = new Result();
            this.Errors = result.Errors;

            return ParseExpression();
        }

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
            else if (t.Type == TokenType.Symbol || t.Type == TokenType.KeywordThis
                || (t.Type >= TokenType.KeywordRGBA && t.Type <= TokenType.KeywordClassID))
                return new VariableExpression(t);
            else if (t.Type == TokenType.OpenParenthesis)
            {
                Expression child = ParseExpression();
                SkipWhitespace();
                Token closeParen = Consume();
                // TODO: Assert that closeParen is, in fact, a close parenthesis
                // once we have a proper panic & sync mechanism
                return new GroupingExpression(t.Span + closeParen.Span, child);
            }
            else // TODO: Add proper panic & sync
            {
                Errors.Add(new SyntaxError("Expected an expression.", t.Span));
                throw new Exception("Expected an expression, found a " + t.Type.ToString());
            }
        }

        private Expression ParseConstructorExpression()
        {
            if (ConsumeIfType(out Token newKeyword, TokenType.KeywordNew))
            {
                SkipWhitespace();
                Token typeName = ConsumeType(TokenType.Symbol, "Expected a type for constructor expression.");
                SkipWhitespace();
                ConsumeType(TokenType.OpenParenthesis, "Expected an open parenthesis after constructor type.");
                // We don't call ParseCallExpression here because that would make the call expression optional. We want to demand the call.
                bool isFirst = true;
                List<Expression> arguments = new List<Expression>();
                while (Peek().Type != TokenType.CloseParenthesis)
                {
                    SkipWhitespace();
                    if (!isFirst)
                        ConsumeType(TokenType.Comma, "Expected a comma between constructor arguments.");

                    Expression argument = ParseExpression();
                    arguments.Add(argument);

                    isFirst = false;
                    SkipWhitespace();
                }

                Token closeParenthesis = ConsumeType(TokenType.CloseParenthesis, "Expected a close parenthesis after constructor call.");
                return new ConstructorExpression(newKeyword.Span + closeParenthesis.Span, typeName, arguments);
            }
            else
            {
                return ParseTerminalExpression();
            }
        }

        // . .$ :: ::$ call [ ]
        private Expression ParseScopeExpression()
        {
            Expression result = ParseConstructorExpression();

            SkipWhitespace();
            while (ConsumeIfType(out Token token, TokenType.Period, TokenType.PeriodDollarSign, TokenType.ColonColon, TokenType.ColonColonDollarSign, TokenType.OpenSquareBracket, TokenType.OpenParenthesis))
            {
                Expression rhs = null;
                if (token.Type == TokenType.OpenSquareBracket)
                {
                    Expression index = ParseExpression();
                    SkipWhitespace();
                    Token closeSquareBracket = Consume();
                    // TODO: Assert that that was actually a close square bracket
                    // once we have a proper panic & sync system
                    result = new ArrayAccessExpression(result.Span + closeSquareBracket.Span, result, index);
                }
                else if (token.Type == TokenType.OpenParenthesis)
                {
                    SkipWhitespace();
                    List<Expression> arguments = new List<Expression>();

                    bool isFirst = true;
                    while (Peek().Type != TokenType.CloseParenthesis)
                    {
                        SkipWhitespace();
                        if (!isFirst)
                            ConsumeType(TokenType.Comma, "Expected a comma between call arguments.");

                        Expression argument = ParseExpression();
                        arguments.Add(argument);

                        isFirst = false;
                        SkipWhitespace();
                    }

                    Token closeParenthesis = ConsumeType(TokenType.CloseParenthesis, "Expected a close parenthesis after function call.");
                    result = new CallExpression(result.Span + closeParenthesis.Span, result, arguments);
                }
                else
                {
                    // . :: .$, ::$
                    if (token.Type == TokenType.PeriodDollarSign || token.Type == TokenType.ColonColonDollarSign)
                    {
                        // Don't allow function calls as part of the lookup expression - this would be ambiguous, consider thing.$foo(bar), is '(bar)' part of the lookup expression, or calling the looked-up function?
                        rhs = ParseTerminalExpression();
                    }
                    else
                    {
                        rhs = ParseTerminalExpression();
                    }
                    result = new BinaryExpression(result, token, rhs);
                }
                SkipWhitespace();
            }
            return result;
        }


        private Expression ParseArrayExpression()
        {
            if (ConsumeIfType(out Token startToken, TokenType.OpenSquareBracket))
            {
                SkipWhitespace();
                List<Expression> elements = new List<Expression>();
                bool firstElement = true;
                while (Peek().Type != TokenType.CloseSquareBracket)
                {
                    SkipWhitespace();
                    if (!firstElement)
                        ConsumeType(TokenType.Comma, "Expected a comma between elements of array expression.");
                    Expression element = ParseExpression();
                    elements.Add(element);
                    firstElement = false;
                    SkipWhitespace();
                }

                SkipWhitespace();
                Token endToken = ConsumeType(TokenType.CloseSquareBracket, "Expected a close brace after array expression.");
                return new ArrayExpression(startToken.Span + endToken.Span, elements);
            }
            else
            {
                return ParseScopeExpression();
            }
        }

        // - ! ~
        private Expression ParseUnaryExpression()
        {
            SkipWhitespace();
            if (ConsumeIfType(out Token token, TokenType.Dash, TokenType.Exclamation, TokenType.Tilde))
            {
                Expression inner = ParseUnaryExpression();
                if (token.Type == TokenType.Dash && inner is LiteralExpression literal && (literal.Value.Type == TokenType.IntegerLiteral || literal.Value.Type == TokenType.FloatLiteral))
                {
                    if (literal.Value.Type == TokenType.IntegerLiteral)
                    {
                        string newContent = (-1 * Int32.Parse(literal.Value.Content)).ToString();
                        return new LiteralExpression(new Token(TokenType.IntegerLiteral, newContent, token.Span.Start.Filename, token.Span.Start.Offset, token.Span.Start.Line, literal.Value.Span.Start.Offset + literal.Value.Span.Length - token.Span.Start.Offset));
                    }
                    else //if (literal.Value.Type == TokenType.FloatLiteral)
                    {
                        string newContent = (-1.0f * Single.Parse(literal.Value.Content)).ToString();
                        return new LiteralExpression(new Token(TokenType.FloatLiteral, newContent, token.Span.Start.Filename, token.Span.Start.Offset, token.Span.Start.Line, literal.Value.Span.Start.Offset + literal.Value.Span.Length - token.Span.Start.Offset));
                    }
                }
                else
                {
                    return new UnaryExpression(inner, token, true);
                }
            }
            return ParseArrayExpression();
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
                return new Token(TokenType.EndOfStream, "", CurrentFilename, CurrentIndex, Tokens.Count > 0 ? Tokens[Tokens.Count - 1].Span.Start.Line : 0, 0);
            }
            return Tokens[CurrentIndex];
        }

        private void SkipWhitespace()
        {
            while (!IsAtEnd() && (Peek().Type == TokenType.Whitespace || Peek().Type == TokenType.Comment || Peek().Type == TokenType.MultilineComment))
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
            Token result = Consume();
            if (result.Type != type)
            {
                this.Errors.Add(new SyntaxError(failureMessage, result.Span));
            }
            return result;
        }

        private Token Consume()
        {
            Token result = Tokens[CurrentIndex++];
            CurrentFilename = result.Span.Start.Filename;
            return result;
        }

        private bool IsAtEnd()
        {
            return CurrentIndex >= Tokens.Count || Tokens[CurrentIndex].Type == TokenType.EndOfStream;
        }
    }
}
