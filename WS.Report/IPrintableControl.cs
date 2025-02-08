using System.Windows.Forms;

namespace WS.Report
{
    public interface IPrintableControl
    {
        WPage Section { get; }

        Printer Printer { get; }

        Control SourceControl { get; set; }

        bool ScaleVertical { get; set; }

        bool ScaleHorizontal { get; set; }

        int StageIndex { get; set; }

        int PrintIndex { get; set; }

        bool OnePageOnly { get; set; }
    }
}