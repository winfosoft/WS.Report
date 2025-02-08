using System.Drawing;
using System.Windows.Forms;

namespace WS.Report
{
    public class LabelPrinter : Printer
    {
        private string Text;
        private string ptxt;

        public LabelPrinter(IPrintableControl printControl) : base(printControl)
        {

        }

        private WLabel Txt
        {
            get
            {
                return (WLabel)this.Control;
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
                Text = sourceControl.Text;

            if (Txt.IsPageNumber)
                Text = Txt.Text.Replace("[pn]", PMan.CurPage.ToString()).Replace("[pc]", PMan.PageCount.ToString());

            if (sourceControl.Text != null)
            {
                Rectangle rect = Bounds;
                using (StringFormat stringFormat = new StringFormat())
                {
                    stringFormat.LineAlignment = (StringAlignment)Methods.GetYAlign(Txt.TextAlign);
                    stringFormat.Alignment = (StringAlignment)Methods.GetXAlign(Txt.TextAlign);
                    stringFormat.Trimming = StringTrimming.Word;

                    if (Txt.RightToLeft == RightToLeft.Yes)
                        stringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;

                    rect.Size = g.MeasureString(Text, Txt.Font, rect.Size, stringFormat, out int charactersFitted, out int linesFilled).ToSize();
                    string s = Text.Substring(0, charactersFitted);

                    Size size = rect.Size;
                    Rectangle Rect = Bounds;
                    int textAlign = (int)Txt.TextAlign;
                    int VSpace = 0;
                    int num = 0;
                    rect = Methods.GetRect(size, Rect, (ContentAlignment)textAlign, VSpace, ref num);

                    if (Txt.FitMaxHeight)
                    {
                        rect.Height = Bounds.Height;
                        rect.Y = Bounds.Top;
                    }

                    if (Txt.FitMaxWidth)
                    {
                        rect.Width = Bounds.Width;
                        rect.X = Bounds.X;
                    }

                    using (SolidBrush backgroundBrush = new SolidBrush(Txt.BackColor))
                    {
                        g.FillRectangle(backgroundBrush, rect);
                    }

                    if (Txt.BorderSize > 0)
                    {
                        using (Pen borderPen = new Pen(Txt.BorderColor, Txt.BorderSize))
                        {
                            borderPen.DashStyle = Txt.Border;
                            g.DrawRectangle(borderPen, rect);
                        }
                    }

                    using (SolidBrush textBrush = new SolidBrush(Txt.ForeColor))
                    {
                        g.DrawString(s, Txt.Font, textBrush, (RectangleF)rect, stringFormat);
                    }

                    if (charactersFitted < Text.Length && !Txt.OnePageOnly)
                    {
                        ptxt = Text;
                        Text = Text.Substring(charactersFitted);
                        if (ptxt == Text)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Text = Txt.Text;
                        return true;
                    }
                }
            }

            return false; // Default return value if sourceControl.Text is null
        }
    }
}