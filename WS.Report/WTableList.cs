using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WS.Report
{
    [ToolboxBitmap(typeof(ListView))]
    public class WTableList : ListView, IPrintableControl
    {
        private int _RowHeight;
        private int _HeaderHeight;
        private Font _HeaderFont;
        private ListView _SourceControl;
        private TableListPrinter _Printer;

        [DebuggerNonUserCode]
        static WTableList()
        {
        }

        public WTableList()
        {
            UseBorderStyleForHeader = true;
            FitMaxWidth = true;
            FitMaxHeight = true;
            _RowHeight = 30;
            _HeaderHeight = 30;
            Border = DashStyle.Solid;
            VGridStyle = DashStyle.Solid;
            HGridStyle = DashStyle.Solid;
            BorderSize = 1;
            VGridSize = 1;
            Align = ContentAlignment.TopLeft;
            HGridSize = 1;
            HeaderColor = Color.White;
            HeaderForeColor = Color.Black;
            BorderColor = Color.Black;
            VGridColor = Color.Black;
            HGridColor = Color.Black;
            FirstHeaderAlign = System.Windows.Forms.HorizontalAlignment.Left;
            _HeaderFont = Font;
            ScaleHorizontal = true;
            ScaleVertical = true;
            OnePageOnly = false;
        }

        [Description("Use outline formatting for table column headers")]
        [Category("Printing Appearance: Headers")]
        public bool UseBorderStyleForHeader { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Appearance: Common")]
        [Description("Equal width during printing to width during design")]
        public bool FitMaxWidth { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Equal height during printing to height during design")]
        [Category("Printing Appearance: Common")]
        public bool FitMaxHeight { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Table row height")]
        [Category("Printing Appearance: Common")]
        public int RowHeight
        {
            get
            {
                return _RowHeight;
            }
            set
            {
                if ((double)value < (double)Font.GetHeight())
                    value = checked((int)Math.Round((double)Font.GetHeight()));
                _RowHeight = value;
            }
        }

        [Category("Printing Appearance: Headers")]
        [Description("Column header hieght")]
        public int HeaderHieght
        {
            get
            {
                return _HeaderHeight;
            }
            set
            {
                if (value < (double)Font.GetHeight())
                    value = checked((int)Math.Round(Font.GetHeight()));
                _HeaderHeight = value;
            }
        }

        [Category("Printing Appearance: Border")]
        [Description("Outer Border shape")]
        public DashStyle Border { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Appearance: Grid")]
        [Description("Vertical table lines shape")]
        public DashStyle VGridStyle { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Appearance: Grid")]
        [Description("Horizontal table lines shape")]
        public DashStyle HGridStyle { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Outer Border thickness")]
        [Category("Printing Appearance: Border")]
        public int BorderSize { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Appearance: Grid")]
        [Description("Table line thickness")]
        public int GridSize
        {
            get
            {
                if (HGridSize == VGridSize)
                    return HGridSize;
                return -1;
            }
            set
            {
                HGridSize = value;
                VGridSize = value;
            }
        }

        [Description("Table line shape")]
        [Category("Printing Appearance: Grid")]
        public DashStyle GridStyle
        {
            get
            {
                if (HGridStyle == VGridStyle)
                    return HGridStyle;
                return DashStyle.Solid;
            }
            set
            {
                HGridStyle = value;
                VGridStyle = value;
            }
        }

        [Category("Printing Appearance: Grid")]
        [Description("Thickness of Vertical table lines")]
        public int VGridSize { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Table alignment")]
        [Category("Printing Appearance: Common")]
        public ContentAlignment Align { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Appearance: Grid")]
        [Description("Thickness of Horizontal table lines")]
        public int HGridSize { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Appearance: Headers")]
        [Description("Header color")]
        public Color HeaderColor { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Header forecolor")]
        [Category("Printing Appearance: Headers")]
        public Color HeaderForeColor { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Appearance: Border")]
        [Description("Outer border color")]
        public Color BorderColor { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Table line color")]
        [Category("Printing Appearance: Grid")]
        public Color GridColor
        {
            get
            {
                if (VGridColor == HGridColor)
                    return HGridColor;
                return Color.Transparent;
            }
            set
            {
                VGridColor = value;
                HGridColor = value;
            }
        }

        [Category("Printing Appearance: Grid")]
        [Description("Table  Vertical line color")]
        public Color VGridColor { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Table Horizontal line color")]
        [Category("Printing Appearance: Grid")]
        public Color HGridColor { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Category("Printing Appearance: Headers")]
        [Description("Align the text of the first column (because it does not change through the design)")]
        public System.Windows.Forms.HorizontalAlignment FirstHeaderAlign { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Description("Table header font ")]
        [Category("Printing Appearance: Headers")]
        public Font HeaderFont
        {
            get
            {
                if (_HeaderFont == null)
                    _HeaderFont = Font;
                return _HeaderFont;
            }
            set
            {
                _HeaderFont = value;
            }
        }

        [DefaultValue(false)]
        [Category("Printing Data")]
        [Description("Print only selected items from the list")]
        public bool PrintSelection { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        [Browsable(false)]
        public Control SourceControl
        {
            get
            {
                if (_SourceControl == null)
                    _SourceControl = (ListView)this;
                return (Control)_SourceControl;
            }
            set
            {
                _SourceControl = (ListView)value;
            }
        }

        [Browsable(false)]
        public Printer Printer
        {
            get
            {
                if (_Printer == null)
                    _Printer = new TableListPrinter((IPrintableControl)this);
                return (Printer)_Printer;
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

        [Category("Printing Behavior")]
        [Description("Enable vertical scale")]
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
            Parent.Controls.Remove((Control)this);
        }
    }
}