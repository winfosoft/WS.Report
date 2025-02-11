using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WS.Report
{
    [ToolboxBitmap(typeof(Label))]
    public class WVerticalLabel : Label, IPrintableControl
    {
        internal const string pn = "[pn]";
        internal const string pc = "[pc]";

        private Label _SourceControl;
        private VerticalLablePrinter _Printer;

        [DebuggerNonUserCode]
        static WVerticalLabel()
        {
        }

        public WVerticalLabel()
        {
            FitMaxWidth = true;
            FitMaxHeight = true;
            Border = DashStyle.Solid;
            BorderSize = 0;
            BorderColor = Color.Black;
            IsPageNumber = false;
            ScaleHorizontal = true;
            ScaleVertical = true;
            OnePageOnly = false;
            AutoSize = false;
        }

        [Description("Equal width during printing to width during design")]
        [Category("Printing Appearance: Common")]
        public bool FitMaxWidth { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Equal length during printing to length during design")]
        [Category("Printing Appearance: Common")]
        public bool FitMaxHeight { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Outer Border shape")]
        [Category("Printing Appearance")]
        public DashStyle Border { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Appearance")]
        [Description("Outer Border thickness")]
        public int BorderSize { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Outer Border color")]
        [Category("Printing Appearance")]
        public Color BorderColor { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Browsable(false)]
        public Control SourceControl
        {
            get
            {
                if (_SourceControl == null)
                    _SourceControl = this;
                return _SourceControl;
            }
            set
            {
                _SourceControl = (Label)value;
            }
        }

        [Description("To determine whether the field displays the page number, we use the text property for the format, [pn]=Page Number, [pc]=Page Count.")]
        [Category("Appearance")]
        public bool IsPageNumber { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Browsable(false)]
        public Printer Printer
        {
            get
            {
                if (_Printer == null)
                    _Printer = new VerticalLablePrinter(this);
                return _Printer;
            }
        }

        [Browsable(false)]
        public WPage Section
        {
            get
            {
                return (WPage)Parent;
            }
        }

        [Category("Printing Behavior")]
        [Description("Enable horizontal scale")]
        public bool ScaleHorizontal { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Enable vertical scale")]
        [Category("Printing Behavior")]
        public bool ScaleVertical { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Behavior")]
        [Description("Printing stage index")]
        public int StageIndex { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Behavior")]
        [Description("Index during printing stage")]
        public int PrintIndex { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Specify whether continues printing on subsequent pages or not.")]
        [Category("Printing Behavior")]
        public bool OnePageOnly { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        protected override void OnParentChanged(EventArgs e)
        {
            if (Parent == null || Parent is WPage)
                return;
            Parent.Controls.Remove(this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Get the graphics object
            Graphics g = e.Graphics;

            // text to display
            string displayText = Text;

            // StringFormat for text alignment
            StringFormat stringFormat = new StringFormat
            {
                LineAlignment = (StringAlignment)Methods.GetYAlign(TextAlign),
                Alignment = (StringAlignment)Methods.GetXAlign(TextAlign),
                Trimming = StringTrimming.Word
            };

            if (RightToLeft == RightToLeft.Yes)
            {
                stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }

            // Measure the text size
            SizeF textSize = g.MeasureString(displayText, Font, ClientRectangle.Size, stringFormat);

            // Create a rectangle for the text
            Rectangle rect = new Rectangle(0, 0, (int)textSize.Height, (int)textSize.Width);

            // if AutoSize is enabled
            if (AutoSize)
            {
                Size = new Size(rect.Width, rect.Height);
                AutoSize = false;
            }

            // Draw background and border
            using (Brush brush = new SolidBrush(BackColor))
            {
                g.FillRectangle(brush, ClientRectangle);
            }

            if (BorderSize > 0)
            {
                using (Pen pen = new Pen(BorderColor, BorderSize) { DashStyle = Border })
                {
                    g.DrawRectangle(pen, ClientRectangle);
                }
            }

            // Save the current graphics state
            GraphicsState state = g.Save();

            // Rotate and draw the text vertically
            g.TranslateTransform(Width / 2, Height / 2);
            g.RotateTransform(270);
            g.TranslateTransform(-textSize.Height / 2, -textSize.Width / 2);

            // Draw the text
            g.DrawString(displayText, Font, new SolidBrush(ForeColor), new RectangleF(0, 0, textSize.Height, textSize.Width), stringFormat);

            // Restore the graphics state
            g.Restore(state);
        }

        public void ChangeText()
        {
        }
    }
}