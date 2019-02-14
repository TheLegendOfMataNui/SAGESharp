using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS
{
    public static class Scanner
    {
        // Wow, this really abuses ugly global state. I thought I was better than this...
        private static string Source;
        private static int StartIndex = 0;
        private static int CurrentIndex = 0;
        private static int Line = 0;
        private static List<Token> Result = new List<Token>();
        private static List<SyntaxError> Errors;

        private static Dictionary<string, TokenType> _keywords = null;

        private static TokenType GetKeywordType(string contents)
        {
            if (_keywords == null)
            {
                _keywords = new Dictionary<string, TokenType>();
                _keywords.Add("class", TokenType.KeywordClass);
                _keywords.Add("function", TokenType.KeywordFunction);
                _keywords.Add("property", TokenType.KeywordProperty);
                _keywords.Add("method", TokenType.KeywordMethod);
                _keywords.Add("var", TokenType.KeywordVar);
                _keywords.Add("while", TokenType.KeywordWhile);
                _keywords.Add("if", TokenType.KeywordIf);
                _keywords.Add("else", TokenType.KeywordElse);
                _keywords.Add("return", TokenType.KeywordReturn);
                _keywords.Add("this", TokenType.KeywordThis);
                _keywords.Add("true", TokenType.KeywordTrue);
                _keywords.Add("false", TokenType.KeywordFalse);
                _keywords.Add("null", TokenType.KeywordNull);
                _keywords.Add("and", TokenType.KeywordAnd);
                _keywords.Add("or", TokenType.KeywordOr);
            }
            if (_keywords.ContainsKey(contents))
                return _keywords[contents];
            else
                return TokenType.Symbol;
        }

        public static List<Token> Scan(string source, List<SyntaxError> errors, bool ignoreWhitespace = false, bool ignoreComments = false)
        {
            Source = source;
            Errors = errors;
            StartIndex = 0;
            CurrentIndex = 0;
            Line = 0;
            Result = new List<Token>();

            while (!IsAtEnd())
            {
                StartIndex = CurrentIndex;
                Token result = ScanToken();
                if (((result.Type != TokenType.Comment && result.Type != TokenType.MultilineComment) || !ignoreComments) && (result.Type != TokenType.Whitespace || !ignoreWhitespace))
                    Result.Add(result);
            }

            Result.Add(new Token(TokenType.EndOfStream, "", CurrentIndex, 0));

            return Result;
        }

        private static Token ScanToken()
        {
            char start = Advance();
            if (start == '{')
                return FinishToken(TokenType.OpenBrace);
            else if (start == '}')
                return FinishToken(TokenType.CloseBrace);
            else if (start == '(')
                return FinishToken(TokenType.OpenParenthesis);
            else if (start == ')')
                return FinishToken(TokenType.CloseParenthesis);
            else if (start == '[')
                return FinishToken(TokenType.OpenSquareBracket);
            else if (start == ']')
                return FinishToken(TokenType.CloseSquareBracket);
            else if (start == ';')
                return FinishToken(TokenType.Semicolon);
            else if (start == ':')
                return FinishToken(TokenType.Colon);
            else if (start == '.')
                return FinishToken(TokenType.Period);
            else if (start == '$')
                return FinishToken(TokenType.DollarSign);
            else if (start == ',')
                return FinishToken(TokenType.Comma);
            else if (start == '-')
                return FinishToken(TokenType.Dash);
            else if (start == '+')
                return FinishToken(TokenType.Plus);
            else if (start == '*')
                return FinishToken(TokenType.Asterisk);
            else if (start == '%')
                return FinishToken(TokenType.Percent);
            else if (start == '!')
                return FinishToken(AdvanceIfMatches('=') ? TokenType.ExclamationEquals : TokenType.Exclamation);
            else if (start == '=')
                return FinishToken(AdvanceIfMatches('=') ? TokenType.EqualsEquals : TokenType.Equals);
            else if (start == '>')
                return FinishToken(AdvanceIfMatches('=') ? TokenType.GreaterEquals : TokenType.Greater);
            else if (start == '<')
                return FinishToken(AdvanceIfMatches('=') ? TokenType.LessEquals : TokenType.Less);
            else if (start == '/')
            {
                if (Peek() == '/')
                {
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                    return FinishToken(TokenType.Comment);
                }
                else if (Peek() == '*')
                {
                    while ((Peek() != '*' || PeekNext() != '/') && !IsAtEnd())
                        Advance();
                    if (IsAtEnd())
                        return FinishError();
                    else
                    {
                        CurrentIndex += 2; // Include the closing '*/' we peeked above
                        return FinishToken(TokenType.MultilineComment);
                    }
                }
                else
                {
                    return FinishToken(TokenType.Slash);
                }
            }
            else if (start == '"')
            {
                while (Peek() != '"' && !IsAtEnd() && Peek() != '\n')
                {
                    Advance();
                }

                if (IsAtEnd())
                {
                    return FinishError(); // Unterminated string at end of file
                }
                else if (Peek() == '\n')
                {
                    return FinishError(); // Unterminated string at end of line
                }
                else
                {
                    Advance(); // TODO: Assert this is '"'

                    return FinishToken(TokenType.StringLiteral);
                }
            }
            else if (IsDigit(start))
            {
                bool wholeNumber = true;
                while (IsDigit(Peek()))
                    Advance();
                if (Peek() == '.' && IsDigit(PeekNext()))
                {
                    wholeNumber = false;
                    Advance(); // TODO: Assert this is '.'

                    while (IsDigit(Peek()))
                        Advance();
                }

                return FinishToken(wholeNumber ? TokenType.IntegerLiteral : TokenType.FloatLiteral);
            }
            else if (Char.IsWhiteSpace(start))
            {
                if (start == '\n')
                    Line++;
                while (Char.IsWhiteSpace(Peek()))
                {
                    Advance();
                    if (Peek() == '\n')
                        Line++;
                }
                return FinishToken(TokenType.Whitespace);
            }
            else if (IsIdentifierStart(start))
            {
                while (IsIdentifierBody(Peek()))
                    Advance();

                return FinishToken(GetKeywordType(Source.Substring(StartIndex, CurrentIndex - StartIndex)));
            }
            else
                return FinishError();
        }

        private static Token FinishToken(TokenType type)
        {
            return new Token(type, Source.Substring(StartIndex, CurrentIndex - StartIndex), StartIndex, CurrentIndex - StartIndex);
        }

        private static Token FinishError()
        {
            Errors.Add(new SyntaxError(StartIndex, CurrentIndex - StartIndex, Line));
            return FinishToken(TokenType.Invalid);
        }

        private static bool AdvanceIfMatches(char character)
        {
            if (IsAtEnd()) return false;
            if (Source[CurrentIndex] != character) return false;

            CurrentIndex++;

            return true;
        }

        private static char Advance()
        {
            if (IsAtEnd()) return '\0';
            return Source[CurrentIndex++];
        }

        private static char Peek()
        {
            if (IsAtEnd()) return '\0';
            return Source[CurrentIndex];
        }

        private static char PeekNext()
        {
            if (CurrentIndex + 1 >= Source.Length) return '\0';
            return Source[CurrentIndex + 1];
        }

        private static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private static bool IsIdentifierStart(char ch)
        {
            return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '_';
        }

        private static bool IsIdentifierBody(char ch)
        {
            return IsIdentifierStart(ch) || IsDigit(ch);
        }

        private static bool IsAtEnd()
        {
            return CurrentIndex >= Source.Length;
        }
    }
}
