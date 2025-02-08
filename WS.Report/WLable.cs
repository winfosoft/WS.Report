using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WS.Report
{
    [ToolboxBitmap(typeof(Label))]
    public class WLabel : Label, IPrintableControl
    {
        private static readonly object _syncLock = new object();
        private static readonly List<WeakReference> _instanceReferences = new List<WeakReference>();

        internal const string PageNumberPlaceholder = "[pn]";
        internal const string PageCountPlaceholder = "[pc]";

        private Label _sourceControl;
        private LabelPrinter _printer;

        public WLabel()
        {
            AddToInstanceList(this);
            InitializeDefaultValues();
        }

        private void InitializeDefaultValues()
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

        private static void AddToInstanceList(object value)
        {
            lock (_syncLock)
            {
                if (_instanceReferences.Count == _instanceReferences.Capacity)
                {
                    CompactInstanceList();
                }
                _instanceReferences.Add(new WeakReference(value));
            }
        }

        private static void CompactInstanceList()
        {
            int activeIndex = 0;
            for (int i = 0; i < _instanceReferences.Count; i++)
            {
                if (_instanceReferences[i].IsAlive)
                {
                    if (i != activeIndex)
                    {
                        _instanceReferences[activeIndex] = _instanceReferences[i];
                    }
                    activeIndex++;
                }
            }
            _instanceReferences.RemoveRange(activeIndex, _instanceReferences.Count - activeIndex);
            _instanceReferences.Capacity = _instanceReferences.Count;
        }

        [Category("Printing Appearance: Common")]
        [Description("Equal width during printing to width during design")]
        public bool FitMaxWidth { get; set; }

        [Category("Printing Appearance: Common")]
        [Description("Equal length during printing to length during design")]
        public bool FitMaxHeight { get; set; }

        [Category("Printing Appearance")]
        [Description("Outer Border shape")]
        public DashStyle Border { get; set; }

        [Category("Printing Appearance")]
        [Description("Outer Border thickness")]
        public int BorderSize { get; set; }

        [Category("Printing Appearance")]
        [Description("Outer Border color")]
        public Color BorderColor { get; set; }

        [Browsable(false)]
        public Control SourceControl
        {
            get => _sourceControl ?? (Label)this;
            set => _sourceControl = (Label)value;
        }

        [Category("Appearance")]
        [Description("To determine whether the field displays the page number, we use the text property for the format, [pn]=Page Number, [pc]=Page Count.")]
        public bool IsPageNumber { get; set; }

        [Browsable(false)]
        public Printer Printer
        {
            get
            {
                if (_printer == null)
                {
                    _printer = new LabelPrinter(this);
                }
                return _printer;
            }
        }

        [Browsable(false)]
        public WPage Section => Parent as WPage;

        [Category("Printing Behavior")]
        [Description("Enable horizontal scale")]
        public bool ScaleHorizontal { get; set; }

        [Category("Printing Behavior")]
        [Description("Enable vertical scale")]
        public bool ScaleVertical { get; set; }

        [Category("Printing Behavior")]
        [Description("Printing stage index")]
        public int StageIndex { get; set; }

        [Category("Printing Behavior")]
        [Description("Index during printing stage")]
        public int PrintIndex { get; set; }

        [Category("Printing Behavior")]
        [Description("Specify whether continues printing on subsequent pages or not.")]
        public bool OnePageOnly { get; set; }

        protected override void OnParentChanged(EventArgs e)
        {
            if (Parent != null && !(Parent is WPage))
            {
                Parent.Controls.Remove(this);
            }
            base.OnParentChanged(e);
        }

        public void ChangeText()
        {
            // Implementation for text change functionality
        }
    }
}