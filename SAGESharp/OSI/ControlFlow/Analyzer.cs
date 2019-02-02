using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI.ControlFlow
{
    public class Analyzer
    {
        public SubroutineGraph Graph { get; }

        public Analyzer(SubroutineGraph graph)
        {
            this.Graph = graph;
        }
    }
}
