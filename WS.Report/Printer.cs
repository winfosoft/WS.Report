using System.Drawing;

namespace WS.Report
{
    public abstract class Printer
    {
        private IPrintableControl _Control;

        protected internal Printer(IPrintableControl printControl)
        {
            _Control = printControl;
        }

        protected internal virtual IPrintableControl Control
        {
            get
            {
                return _Control;
            }
        }

        protected internal abstract bool Print(Graphics g, Rectangle Bounds);
    }
}