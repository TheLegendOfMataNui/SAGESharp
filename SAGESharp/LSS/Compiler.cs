using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAGESharp.OSI;

namespace SAGESharp.LSS
{
    public class Compiler
    {
        public OSIFile Compile(string source, byte versionMajor = 4, byte versionMinor = 1)
        {
            OSIFile result = new OSIFile(versionMajor, versionMinor);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(source)))
            using (System.IO.StreamReader reader = new System.IO.StreamReader(ms))
            {
                CompileInto(result, reader);
            }

            return result;
        }

        public OSIFile CompileFiles(IEnumerable<string> filenames, byte versionMajor = 4, byte versionMinor = 1)
        {
            OSIFile result = new OSIFile(versionMajor, versionMinor);
            foreach (string filename in filenames)
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(filename))
                {
                    CompileInto(result, reader);
                }
            }
            return result;
        }

        private void CompileInto(OSIFile target, System.IO.StreamReader reader)
        {
            throw new NotImplementedException();
        }

        public string DecompileOSI(OSIFile osi)
        {
            throw new NotImplementedException();
        }

        public string DecompileInstructions(OSIFile osi, List<Instruction> instructions, uint bytecodeOffset)
        {
            SAGESharp.OSI.ControlFlow.SubroutineGraph graph = new OSI.ControlFlow.SubroutineGraph(instructions, bytecodeOffset);
            // TODO
            return "(Not Implemented)";
        }

        public void DecompileOSIProject(OSIFile osi, string outputDirectory)
        {
            throw new NotImplementedException();
        }
    }
}
