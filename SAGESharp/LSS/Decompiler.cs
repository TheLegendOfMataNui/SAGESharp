using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAGESharp.OSI;
using SAGESharp.LSS.Statements;
using SAGESharp.LSS.Expressions;

namespace SAGESharp.LSS
{
    public static class Decompiler
    {
        public static SubroutineStatement DecompileSubroutine(OSIFile osi, string name, List<Token> parameters, List<Instruction> instructions, SourceSpan outputSpan)
        {
            List<InstructionStatement> statements = new List<InstructionStatement>();


            BlockStatement body = new BlockStatement(outputSpan, statements);
            return new SubroutineStatement(outputSpan, new Token(TokenType.Symbol, name, outputSpan), parameters, body);
        }
    }
}
