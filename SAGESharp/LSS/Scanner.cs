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
        private static string Filename;
        private static int StartIndex = 0;
        private static int CurrentIndex = 0;
        private static uint Line = 0;
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
                _keywords.Add("global", TokenType.KeywordGlobal);
                _keywords.Add("var", TokenType.KeywordVar);
                _keywords.Add("do", TokenType.KeywordDo);
                _keywords.Add("while", TokenType.KeywordWhile);
                _keywords.Add("if", TokenType.KeywordIf);
                _keywords.Add("else", TokenType.KeywordElse);
                _keywords.Add("foreach", TokenType.KeywordForEach);
                _keywords.Add("return", TokenType.KeywordReturn);
                _keywords.Add("this", TokenType.KeywordThis);
                _keywords.Add("true", TokenType.KeywordTrue);
                _keywords.Add("false", TokenType.KeywordFalse);
                _keywords.Add("null", TokenType.KeywordNull);
                _keywords.Add("new", TokenType.KeywordNew);
                //_keywords.Add("and", TokenType.KeywordAnd);
                //_keywords.Add("or", TokenType.KeywordOr);
                _keywords.Add("rgba", TokenType.KeywordRGBA);
                _keywords.Add("__length", TokenType.KeywordLength);
                _keywords.Add("__append", TokenType.KeywordAppend);
                _keywords.Add("__removeat", TokenType.KeywordRemoveAt);
                _keywords.Add("__insertat", TokenType.KeywordInsertAt);
                _keywords.Add("__red", TokenType.KeywordRed);
                _keywords.Add("__green", TokenType.KeywordGreen);
                _keywords.Add("__blue", TokenType.KeywordBlue);
                _keywords.Add("__alpha", TokenType.KeywordAlpha);
                _keywords.Add("__withred", TokenType.KeywordWithRed);
                _keywords.Add("__withgreen", TokenType.KeywordWithGreen);
                _keywords.Add("__withblue", TokenType.KeywordWithBlue);
                _keywords.Add("__withalpha", TokenType.KeywordWithAlpha);
                _keywords.Add("__tostring", TokenType.KeywordToString);
                _keywords.Add("__tofloat", TokenType.KeywordToFloat);
                _keywords.Add("__toint", TokenType.KeywordToInt);
                _keywords.Add("__isint", TokenType.KeywordIsInt);
                _keywords.Add("__isfloat", TokenType.KeywordIsFloat);
                _keywords.Add("__isstring", TokenType.KeywordIsString);
                _keywords.Add("__isobject", TokenType.KeywordIsObject);
                _keywords.Add("__isinstance", TokenType.KeywordIsInstance);
                _keywords.Add("__isarray", TokenType.KeywordIsArray);
                _keywords.Add("__classid", TokenType.KeywordClassID);
            }
            if (_keywords.ContainsKey(contents))
                return _keywords[contents];
            else
                return TokenType.Symbol;
        }

        public static List<Token> Scan(string source, string filename, List<SyntaxError> errors, bool ignoreWhitespace = false, bool ignoreComments = false)
        {
            Source = source;
            Filename = filename;
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

            Result.Add(new Token(TokenType.EndOfStream, "", filename, CurrentIndex, (uint)Line, 0));

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
            {
                if (AdvanceIfMatches(':'))
                {
                    if (AdvanceIfMatches('$'))
                        return FinishToken(TokenType.ColonColonDollarSign);
                    return FinishToken(TokenType.ColonColon);
                }
                return FinishToken(TokenType.Colon);
            }
            else if (start == '.')
            {
                if (AdvanceIfMatches('$'))
                    return FinishToken(TokenType.PeriodDollarSign);
                return FinishToken(TokenType.Period);
            }
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
            else if (start == '&')
            {
                if (AdvanceIfMatches('&'))
                    return FinishToken(TokenType.AmpersandAmpersand);
                return FinishToken(TokenType.Ampersand);
            }
            else if (start == '|')
            {
                if (AdvanceIfMatches('|'))
                    return FinishToken(TokenType.PipePipe);
                return FinishToken(TokenType.Pipe);
            }
            else if (start == '#')
                return FinishToken(TokenType.Octothorpe);
            else if (start == '^')
                return FinishToken(TokenType.Caret);
            else if (start == '~')
                return FinishToken(TokenType.Tilde);
            else if (start == '!')
                return FinishToken(AdvanceIfMatches('=') ? TokenType.ExclamationEquals : TokenType.Exclamation);
            else if (start == '=')
                return FinishToken(AdvanceIfMatches('=') ? TokenType.EqualsEquals : TokenType.Equals);
            else if (start == '>')
            {
                if (AdvanceIfMatches('>'))
                    return FinishToken(TokenType.GreaterGreater);
                if (AdvanceIfMatches('='))
                    return FinishToken(TokenType.GreaterEquals);
                return FinishToken(TokenType.Greater);
            }
            else if (start == '<')
            {
                if (AdvanceIfMatches('<'))
                    return FinishToken(TokenType.LessLess);
                if (AdvanceIfMatches('='))
                    return FinishToken(TokenType.LessEquals);
                return FinishToken(TokenType.Less);
            }
            else if (start == '/')
            {
                if (Peek() == '/')
                {
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                    //CurrentIndex--; // Newline is not part of a comment.
                    return FinishToken(TokenType.Comment);
                }
                else if (Peek() == '*')
                {
                    while ((Peek() != '*' || PeekNext() != '/') && !IsAtEnd())
                        Advance();
                    if (IsAtEnd())
                        return FinishError("Unterminated multiline comment by the end of the source.", TokenType.MultilineComment);
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
                while ((Peek() != '"' || Source[CurrentIndex - 1] == '\\') && !IsAtEnd() && Peek() != '\n')
                {
                    Advance();
                }

                if (IsAtEnd())
                {
                    return FinishError("Unterminated string literal by the end of the source.", TokenType.StringLiteral);
                }
                else if (Peek() == '\n')
                {
                    return FinishError("Unterminated string literal by the end of the line.", TokenType.StringLiteral);
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
                    if (Peek() == '\n')
                        Line++;
                    Advance();
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
                return FinishError("Unknown token.", TokenType.Invalid);
        }

        private static Token FinishToken(TokenType type)
        {
            return new Token(type, Source.Substring(StartIndex, CurrentIndex - StartIndex), Filename, StartIndex, (uint)Line, CurrentIndex - StartIndex);
        }

        private static Token FinishError(string message, TokenType type)
        {
            Errors.Add(new SyntaxError(message, Filename, StartIndex, Line, CurrentIndex - StartIndex));
            return FinishToken(type);
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
