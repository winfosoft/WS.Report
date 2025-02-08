using System.Drawing;
using System.Windows.Forms;

namespace WS.Report
{
    public class TextPrinter : Printer
    {
        private string Text;
        private string ptxt;

        public TextPrinter(IPrintableControl printControl) : base(printControl)
        {
        }

        private WText Txt
        {
            get
            {
                return (WText)base.Control;
            }
        }

        public PrintManager PMan
        {
            get
            {
                return Txt.Section.Page.PManager;
            }
        }

        protected internal override bool Print(Graphics g, Rectangle Bounds)
        {
            Control sourceControl = Txt.SourceControl;
            if (Text == null && sourceControl.Text != null)
            {
                Text = sourceControl.Text;
            }

            if (Txt.IsPageNumber)
            {
                Text = Txt.Text
                    .Replace("[pn]", PMan.CurPage.ToString())
                    .Replace("[pc]", PMan.PageCount.ToString());
            }

            if (sourceControl.Text != null)
            {
                Rectangle rectangle = Bounds;
                StringFormat stringFormat = new StringFormat
                {
                    LineAlignment = (StringAlignment)Methods.GetYAlign((ContentAlignment)Txt.TextAlign),
                    Alignment = (StringAlignment)Methods.GetXAlign((ContentAlignment)Txt.TextAlign),
                    Trimming = StringTrimming.Word
                };

                if (Txt.RightToLeft == RightToLeft.Yes)
                {
                    stringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                }

                Size measuredSize = g.MeasureString(Text, Txt.Font, rectangle.Size, stringFormat, out int num, out _).ToSize();
                rectangle.Size = measuredSize;

                string s = Text.Substring(0, num);
                Size size = rectangle.Size;
                Rectangle rect = Bounds;
                ContentAlignment textAlign = (ContentAlignment)Txt.TextAlign;
                int vspace = 0;
                int num3 = 0;
                rectangle = Methods.GetRect(size, rect, textAlign, vspace, ref num3);

                if (Txt.FitMaxHeight)
                {
                    rectangle.Height = Bounds.Height;
                    rectangle.Y = Bounds.Top;
                }

                if (Txt.FitMaxWidth)
                {
                    rectangle.Width = Bounds.Width;
                    rectangle.X = Bounds.X;
                }

                g.FillRectangle(new SolidBrush(Txt.BackColor), rectangle);

                if (Txt.BorderSize > 0)
                {
                    using (Pen borderPen = new Pen(Txt.BorderColor, Txt.BorderSize) { DashStyle = Txt.Border })
                    {
                        g.DrawRectangle(borderPen, rectangle);
                    }
                }

                g.DrawString(s, Txt.Font, new SolidBrush(Txt.ForeColor), rectangle, stringFormat);

                if (num < Text.Length && !Txt.OnePageOnly)
                {
                    ptxt = Text;
                    Text = Text.Substring(num);

                    if (ptxt == Text)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    Text = Txt.Text;
                    return !Txt.IsPageNumber;
                }
            }
            return false;
        }
    }
}