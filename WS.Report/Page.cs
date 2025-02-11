using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace WS.Report
{
    [ToolboxBitmap(typeof(Panel))]
    [ToolboxItem(false)]
    public class Page : UserControl
    {
        private readonly IContainer components;

        internal PrintManager PManager;

        public Page()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ContainerControl, true);
            PManager = new PrintManager(this);
        }

        [Description("Select Page Bounds, yes for inner Bounds, no for outer Bounds")]
        [Category("Printing")]
        public bool MarginBounds { get; set; }

        [Category("Printing")]
        public PrintDocument Document
        {
            get { return PManager.Doc; }
            set { PManager.Doc = value; }
        }

        [Browsable(false)]
        public ReadOnlyCollection<WPage> Sections
        {
            get
            {
                var wpageList = new List<WPage>();
                foreach (Control control in Controls)
                {
                    if (control is WPage page)
                    {
                        wpageList.Add(page);
                    }
                }
                return new ReadOnlyCollection<WPage>(wpageList);
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (!(e.Control is WPage))
            {
                Controls.Remove(e.Control);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // SuspendLayout();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Name = "Page";
            Size = new Size(454, 530);
            ResumeLayout(false);
        }
    }
}