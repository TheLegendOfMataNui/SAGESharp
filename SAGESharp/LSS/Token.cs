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
        KeywordGlobal,
        KeywordVar,
        KeywordWhile,
        KeywordIf,
        KeywordElse,
        KeywordForEach,
        KeywordReturn,
        KeywordThis,
        KeywordTrue,
        KeywordFalse,
        KeywordNull,
        KeywordNew,
        //KeywordAnd,
        //KeywordOr,
        KeywordRGBA,
        KeywordLength,
        KeywordAppend,
        KeywordRemoveAt,
        KeywordInsertAt,
        KeywordRed,
        KeywordGreen,
        KeywordBlue,
        KeywordAlpha,
        KeywordToString,
        KeywordToFloat,
        KeywordToInt,
        KeywordIsInt,
        KeywordIsFloat,
        KeywordIsString,
        KeywordIsObject,
        KeywordIsInstance,
        KeywordIsArray,
        KeywordClassID,
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
        Ampersand, // &
        Pipe, // |
        Octothorpe, // #
        Caret, // ^
        Tilde, // ~
        PeriodDollarSign, // .$
        //PlusPlus, // ++
        //DashDash, // --
        ColonColon, // ::
        PipePipe, // ||
        AmpersandAmpersand, // &&
        LessLess, // <<
        GreaterGreater, // >>
        ColonColonDollarSign, // ::$

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

    public struct SourceLocation
    {
        public string Filename { get; }
        public long Offset { get; }
        public uint? Line { get; }

        public SourceLocation(string filename, long offset, uint? line)
        {
            this.Filename = filename;
            this.Offset = offset;
            this.Line = line;
        }

        public override string ToString()
        {
            return Filename + ":" + (Line.HasValue ? (Line.Value + 1).ToString() : "?");
        }
    }

    public struct SourceSpan
    {
        public SourceLocation Start { get; }
        public long Length { get; }
        public long End => Start.Offset + Length;

        public SourceSpan(string filename, long offset, uint? line, long length)
        {
            this.Start = new SourceLocation(filename, offset, line);
            this.Length = length;
        }

        public override string ToString()
        {
            return Start.ToString(); // TODO: Somehow get column. If we can get column, then also show length.
        }

        public static SourceSpan operator+(SourceSpan start, SourceSpan end)
        {
            return new SourceSpan(start.Start.Filename, start.Start.Offset, start.Start.Line, end.End - start.Start.Offset);
        }
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Content { get; }
        public SourceSpan Span { get; }

        public Token(TokenType type, string content, string filename, long sourceOffset, uint? sourceLine, long sourceLength)
        {
            this.Type = type;
            this.Content = content;
            this.Span = new SourceSpan(filename, sourceOffset, sourceLine, sourceLength);
        }

        public override string ToString()
        {
            return Type.ToString() + " [" + Span.ToString() + "] '" + Content + "'";
        }
    }
}
