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
        public long StartIndex { get; }
        public long Length { get; }
        public long StartLine { get; }

        public SyntaxError(string message, long startIndex, long length, long startLine)
        {
            this.Message = message;
            this.StartIndex = startIndex;
            this.Length = length;
            this.StartLine = startLine;
        }

        public override string ToString()
        {
            return "[" + StartIndex + "+" + Length + " (line " + StartLine + ")]: " + Message;
        }
    }
}
