using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS
{
    public class SyntaxError
    {
        public string Message { get; }
        public SourceSpan Span { get; }

        public SyntaxError(string message, string filename, long startIndex, uint startLine, long length)
        {
            this.Message = message;
            this.Span = new SourceSpan(filename, startIndex, startLine, length);
        }

        public SyntaxError(string message, SourceSpan span)
        {
            this.Message = message;
            this.Span = span;
        }

        public override string ToString()
        {
            return "[" + Span.ToString() + "]: " + Message;
        }
    }
}
