using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WS.Report
{
    
    [ToolboxItemFilter("Common")]
    [ToolboxBitmap(typeof(SplitContainer))]
    public class WPage : Panel
    {
       
        private IContainer components;
        protected internal Rectangle DesignBounds;
        private ReadOnlyCollection<PrintStage> _Stages;

        [DebuggerNonUserCode]
        static WPage()
        {
        }

        public WPage()
        {
          
            Repet = false;
            InitializeComponent();
        }

      

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if ((!disposing || components == null) && !false)
                    return;
                components.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            SuspendLayout();
            BackColor = Color.Transparent;
            BorderStyle = BorderStyle.FixedSingle;
            Name = "Section";
            Size = new Size(400, 480);
            ResumeLayout(false);
        }

        [Browsable(false)]
        public Page Page
        {
            get
            {
                return (Page)this.Parent;
            }
        }

        [Browsable(false)]
        public ReadOnlyCollection<PrintStage> Stages
        {
            get
            {
                OrderStages();
                return _Stages;
            }
        }

        [Category("Printing")]
        public bool Repet { [DebuggerNonUserCode] get; [DebuggerNonUserCode] set; }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control is IPrintableControl)
                return;
            Controls.Remove(e.Control);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (Parent == null || Parent is Page)
                return;
            Parent.Controls.Remove(this);
        }

        private void OrderStages()
        {
            List<PrintStage> printStageList = new List<PrintStage>();
            IEnumerable<int> ints = Controls.Cast<Control>()
                .Where(c => c is IPrintableControl)
                .Select(c => ((IPrintableControl)c).StageIndex)
                .OrderBy(StageIndex => StageIndex)
                .Distinct();

            foreach (int current in ints)
            {
                List<IPrintableControl> printableControlList = Controls.Cast<Control>()
                    .Where(c => c is IPrintableControl)
                    .Select(c => (IPrintableControl)c)
                    .Where(p => p.StageIndex == current)
                    .OrderBy(p => p.PrintIndex)
                    .ToList();

                printStageList.Add(new PrintStage()
                {
                    Controls = printableControlList
                });
            }
            _Stages = new ReadOnlyCollection<PrintStage>(printStageList);
        }
    }
}