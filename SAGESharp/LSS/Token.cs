using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS
{
    public enum TokenType : byte
    {
        Invalid,
        Whitespace,
        KeywordClass,
        KeywordFunction,
        KeywordProperty,
        KeywordMethod,
        KeywordVar,
        KeywordWhile,
        KeywordIf,
        KeywordElse,
        KeywordReturn,
        KeywordThis,
        KeywordTrue,
        KeywordFalse,
        KeywordNull,
        KeywordAnd,
        KeywordOr,
        Comment,
        MultilineComment,
        OpenBrace, // {
        CloseBrace, // }
        OpenParenthesis, // (
        CloseParenthesis, // )
        OpenSquareBracket, // [
        CloseSquareBracket, // ]
        Semicolon, // ;
        Colon, // :
        Period, // .
        DollarSign, // $
        Comma, // ,
        Dash, // -
        Plus, // +
        Slash, // /
        Asterisk, // *
        Percent, // %

        Symbol,
        StringLiteral,
        IntegerLiteral,
        FloatLiteral,

        Exclamation, // !
        ExclamationEquals, // !=
        Equals, // =
        EqualsEquals, // ==
        Greater, // >
        GreaterEquals, // >=
        Less, // <
        LessEquals, // <=
        
        EndOfStream
    }

    public struct TextLocation
    {
        public long Offset { get; }

        public TextLocation(long offset)
        {
            this.Offset = offset;
        }

        public void CalculatePosition(string textContents, out long line, out long column)
        {
            throw new NotImplementedException();
        }

        public static implicit operator long(TextLocation location)
        {
            return location.Offset;
        }
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Content { get; }
        public TextLocation SourceLocation { get; }
        public long SourceLength { get; }

        public Token(TokenType type, string content, long sourceOffset, long sourceLength)
        {
            this.Type = type;
            this.Content = content;
            this.SourceLocation = new TextLocation(sourceOffset);
            this.SourceLength = sourceLength;
        }

        public override string ToString()
        {
            return Type.ToString() + " [" + SourceLocation.Offset + "+" + SourceLength + "] '" + Content + "'";
        }
    }
}
