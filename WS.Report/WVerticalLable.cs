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
        private VerticalPrinter _Printer;

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
                    _Printer = new VerticalPrinter(this);
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

        public void ChangeText()
        {
        }
    }
}