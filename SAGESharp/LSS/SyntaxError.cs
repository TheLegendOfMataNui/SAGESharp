using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS
{
    public class SyntaxError
    {
        public long StartIndex { get; }
        public long Length { get; }
        public long StartLine { get; }

        public SyntaxError(long startIndex, long length, long startLine)
        {
            this.StartIndex = startIndex;
            this.Length = length;
            this.StartLine = startLine;
        }
    }
}
