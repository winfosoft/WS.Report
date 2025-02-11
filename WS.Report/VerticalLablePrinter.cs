using System;
using System.Drawing;
using System.Windows.Forms;

namespace WS.Report
{
    public class VerticalLablePrinter : Printer
    {
        private string text; // Stores the current text to print
        private string previousText; // Stores the previous text for pagination

        public VerticalLablePrinter(IPrintableControl printControl) : base(printControl)

        {
        }

        private WVerticalLabel Txt
        {
            get
            {
                return Control as WVerticalLabel;
            }
        }

        public PrintManager PMan
        {
            get
            {
                if (Txt?.Section?.Page?.PManager != null)
                    return Txt.Section.Page.PManager;

                throw new InvalidOperationException("PrintManager is not properly initialized.");
            }
        }

        protected internal override bool Print(Graphics g, Rectangle bounds)
        {
            if (Txt == null || Txt.SourceControl == null)
                throw new InvalidOperationException("Source control or vertical label is not properly initialized.");

            Control sourceControl = Txt.SourceControl;

            // Initialize text if not already set
            if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(sourceControl.Text))
            {
                text = sourceControl.Text;
            }

            // Replace placeholders for page numbers if necessary
            if (Txt.IsPageNumber)
            {
                text = text.Replace("[pn]", PMan.CurPage.ToString())
                                     .Replace("[pc]", PMan.PageCount.ToString());
            }

            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    // Measure and format the text
                    StringFormat stringFormat = new StringFormat
                    {
                        LineAlignment = (StringAlignment)Methods.GetYAlign(Txt.TextAlign),
                        Alignment = (StringAlignment)Methods.GetXAlign(Txt.TextAlign),
                        Trimming = StringTrimming.Word
                    };

                    if (Txt.RightToLeft == RightToLeft.Yes)
                    {
                        stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                    }

                    // Measure the text size
                    SizeF textSize = g.MeasureString(text, Txt.Font, bounds.Size, stringFormat, out int charactersFitted, out int linesFilled);
                    Rectangle rect = new Rectangle(bounds.Location, textSize.ToSize());

                    // Adjust rectangle based on alignment and fit settings
                    if (Txt.FitMaxHeight)
                    {
                        rect.Height = bounds.Height;
                        rect.Y = bounds.Top;
                    }

                    if (Txt.FitMaxWidth)
                    {
                        rect.Width = bounds.Width;
                        rect.X = bounds.Left;
                    }

                    // Draw background and border
                    using (Brush brush = new SolidBrush(Txt.BackColor))
                    {
                        g.FillRectangle(brush, rect);
                    }

                    if (Txt.BorderSize > 0)
                    {
                        using (Pen pen = new Pen(Txt.BorderColor, Txt.BorderSize) { DashStyle = Txt.Border })
                        {
                            g.DrawRectangle(pen, rect);
                        }
                    }

                    // Rotate and draw the text vertically
                    g.TranslateTransform(rect.X, rect.Y);
                    g.RotateTransform(270f);
                    g.DrawString(text.Substring(0, charactersFitted), Txt.Font, new SolidBrush(Txt.ForeColor), 0, 0, stringFormat);
                    g.ResetTransform();

                    // Handle pagination
                    if (charactersFitted < text.Length && !Txt.OnePageOnly)
                    {
                        previousText = text;
                        text = text.Substring(charactersFitted);

                        // Avoid infinite loops
                        if (previousText == text)
                        {
                            return true;
                        }

                        return false;
                    }

                    // Reset text for next page
                    text = Txt.Text;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during printing: {ex.Message}");
                    return false;
                }
            }

            return false;
        }
    }
}