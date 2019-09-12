using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.LSS
{
    public class CompileMessage
    {
        public enum MessageSeverity
        {
            Diagnostic,
            Info,
            Warning,
            Error,
            Fatal
        }

        public string Message { get; }
        public string ErrorCode { get; }
        public MessageSeverity Severity { get; }
        public SourceSpan Span { get; }

        public CompileMessage(string message, string errorCode, MessageSeverity severity, string filename, long startIndex, uint startLine, long length)
        {
            this.Message = message;
            this.ErrorCode = errorCode;
            this.Severity = severity;
            this.Span = new SourceSpan(filename, startIndex, startLine, length);
        }

        public CompileMessage(string message, string errorCode, MessageSeverity severity, SourceSpan span)
        {
            this.Message = message;
            this.ErrorCode = errorCode;
            this.Severity = severity;
            this.Span = span;
        }

        public override string ToString()
        {
            // Conforms to the Visual Studio build output format specified at
            // https://docs.microsoft.com/en-us/cpp/build/formatting-the-output-of-a-custom-build-step-or-build-event
            return this.Span.Start.Filename + "(" + this.Span.Start.Line + ") : " + this.Severity.ToString() + " " + this.ErrorCode + ": " + this.Message;
        }
    }
}
