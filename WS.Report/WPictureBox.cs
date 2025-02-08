using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WS.Report
{
    [ToolboxBitmap(typeof(PictureBox))]
    public class WPictureBox : PictureBox, IPrintableControl
    {
        private PictureBox _SourceControl;
        private PictureBoxPrinter _Printer;

        [DebuggerNonUserCode]
        static WPictureBox()
        {
        }

        public WPictureBox()
        {
            ResizeToFit = false;
            Border = DashStyle.Solid;
            BorderSize = 0;
            BorderColor = Color.Black;
            Align = ContentAlignment.TopLeft;
            ScaleHorizontal = true;
            ScaleVertical = true;
            OnePageOnly = false;
        }

        [Category("Printing Appearance")]
        [Description("If the image area is larger than the WPictureBox area, its area will be changed so that it fills within the WPictureBox area.")]
        public bool ResizeToFit { get; set; }

        [Description("Outer Border shape")]
        [Category("Printing Appearance")]
        public DashStyle Border { get; set; }

        [Category("Printing Appearance")]
        [Description("Outer Border thickness")]
        public int BorderSize { get; set; }

        [Category("Printing Appearance")]
        [Description("Outer Border color")]
        public Color BorderColor { get; set; }

        [Description("Picture alignment")]
        [Category("Printing Appearance")]
        public ContentAlignment Align { get; set; }

        [Browsable(false)]
        public Control SourceControl
        {
            get { return _SourceControl ?? (_SourceControl = this); }
            set { _SourceControl = (PictureBox)value; }
        }

        [Browsable(false)]
        public Printer Printer
        {
            get { return _Printer ?? (_Printer = new PictureBoxPrinter(this)); }
        }

        [Browsable(false)]
        public WPage Section
        {
            get { return (WPage)Parent; }
        }

        [Description("Enable horizontal scale")]
        [Category("Printing Behavior")]
        public bool ScaleHorizontal { get; set; }

        [Category("Printing Behavior")]
        [Description("Enable vertical scale")]
        public bool ScaleVertical { get; set; }

        [Description("Printing stage index")]
        [Category("Printing Behavior")]
        public int StageIndex { get; set; }

        [Description("Index during printing stage")]
        [Category("Printing Behavior")]
        public int PrintIndex { get; set; }

        [Description("Specify whether continues printing on subsequent pages or not.")]
        [Category("Printing Behavior")]
        public bool OnePageOnly { get; set; }

        protected override void OnParentChanged(EventArgs e)
        {
            if (Parent != null && !(Parent is WPage))
            {
                Parent.Controls.Remove(this);
            }
        }
    }
}