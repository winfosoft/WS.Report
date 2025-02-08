using System.Collections.Generic;
using System.Diagnostics;

namespace WS.Report
{
    public class PrintStage
    {
        internal List<IPrintableControl> Controls;

        [DebuggerNonUserCode]
        public PrintStage()
        {
        }
    }
}