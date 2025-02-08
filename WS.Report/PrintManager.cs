using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace WS.Report
{
    public class PrintManager
    {
        public readonly Page Page;
        private PrintDocument _Doc;
        private float sx;
        private float sy;
        private List<List<PrintStage>> Sections;
        private List<List<PrintStage>> RSections;
        internal int CurPage;
        internal int PageCount;
        private Graphics gg;

        internal virtual PrintDocument Doc
        {
            [DebuggerNonUserCode]
            get
            {
                return _Doc;
            }
            [DebuggerNonUserCode, MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                QueryPageSettingsEventHandler settingsEventHandler = new QueryPageSettingsEventHandler(Doc_QueryPageSettings);
                PrintPageEventHandler pageEventHandler = new PrintPageEventHandler(Doc_PrintPage);
                PrintEventHandler printEventHandler = new PrintEventHandler(Doc_BeginPrint);

                if (_Doc != null)
                {
                    _Doc.QueryPageSettings -= settingsEventHandler;
                    _Doc.PrintPage -= pageEventHandler;
                    _Doc.BeginPrint -= printEventHandler;
                }

                _Doc = value;

                if (_Doc != null)
                {
                    _Doc.QueryPageSettings += settingsEventHandler;
                    _Doc.PrintPage += pageEventHandler;
                    _Doc.BeginPrint += printEventHandler;
                }
            }
        }

        public PrintManager(Page tPage)
        {
            Page = tPage ?? throw new ArgumentNullException(nameof(tPage));
        }

        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Print(e);
        }

        private int GetPageCount()
        {
            ResetRSections();
            ResetSections();
            int num = 0;
            PrintPageEventArgs e = new PrintPageEventArgs(Graphics.FromImage(new Bitmap(Doc.DefaultPageSettings.Bounds.Width, Doc.DefaultPageSettings.Bounds.Height)), GetMarginBounds(), Doc.DefaultPageSettings.Bounds, Doc.DefaultPageSettings)
            {
                HasMorePages = true
            };
            while (e.HasMorePages)
            {
                num++;
                e.HasMorePages = false;
                Print(e);
                Application.DoEvents();
            }
            return num;
        }

        private void Print(PrintPageEventArgs e)
        {
            if (gg == null)
                gg = e.Graphics;

            CurPage++;

            bool flag = false;

            foreach (var current1 in RSections)
            {
                foreach (var current2 in current1)
                {
                    current2.Controls.Reverse();
                    for (int index = current2.Controls.Count - 1; index >= 0; index--)
                    {
                        Printer printer = current2.Controls[index].Printer;
                        Rectangle scaledBounds = GetScaledBounds(printer.Control);
                        if (printer.Print(e.Graphics, scaledBounds))
                            current2.Controls.Remove(printer.Control);
                    }

                    if (current2.Controls.Count <= 0)
                    {
                        if (current1 == RSections.Last())
                        {
                            flag = true;
                            break;
                        }
                    }
                    else
                        break;
                }
            }

            if (flag)
                ResetRSections();

            foreach (var current1 in Sections)
            {
                foreach (var current2 in current1)
                {
                    current2.Controls.Reverse();
                    for (int index = current2.Controls.Count - 1; index >= 0; index--)
                    {
                        Printer printer = current2.Controls[index].Printer;
                        Rectangle scaledBounds = GetScaledBounds(printer.Control);
                        if (printer.Print(e.Graphics, scaledBounds))
                            current2.Controls.Remove(printer.Control);
                    }

                    e.HasMorePages = e.HasMorePages || current2.Controls.Count > 0;
                    if (current2.Controls.Count > 0)
                        break;
                }
            }
        }

        private void ResetRSections()
        {
            if (RSections == null)
                RSections = new List<List<PrintStage>>();
            RSections.Clear();

            foreach (var current in Page.Sections)
            {
                if (current.Repet)
                    RSections.Add(current.Stages.ToList());
            }
        }

        private void ResetSections()
        {
            if (Sections == null)
                Sections = new List<List<PrintStage>>();
            Sections.Clear();

            foreach (var current in Page.Sections)
            {
                if (!current.Repet)
                    Sections.Add(current.Stages.ToList());
            }
        }

        private void Doc_BeginPrint(object sender, PrintEventArgs e)
        {
            if (Doc.DefaultPageSettings.Landscape)
            {
                Rectangle bounds = Page.Bounds;
                Page.Width = bounds.Height;
                Page.Height = bounds.Height;
            }

            Scale();
            PageCount = GetPageCount();
            CurPage = 0;
            ResetRSections();
            ResetSections();
        }

        private Rectangle GetScaledBounds(IPrintableControl pc)
        {
            float scaleX = pc.ScaleHorizontal ? sx : 1f;
            float scaleY = pc.ScaleVertical ? sy : 1f;

            Rectangle bounds = ((Control)pc).Bounds;
            bounds.Location = GetLocation((Control)pc);
            Rectangle scaledBound = Methods.GetScaledBound(bounds, scaleX, scaleY);

            if (Page.MarginBounds)
            {
                scaledBound.X += Doc.DefaultPageSettings.Margins.Left;
                scaledBound.Y += Doc.DefaultPageSettings.Margins.Top;
            }

            return scaledBound;
        }

        private Point GetLocation(Control c)
        {
            return new Point(c.Left + c.Parent.Left, c.Top + c.Parent.Top);
        }

        public Rectangle GetMarginBounds()
        {
            Rectangle bounds = Doc.DefaultPageSettings.Bounds;
            Margins margins = Doc.DefaultPageSettings.Margins;
            bounds.X += margins.Left;
            bounds.Y += margins.Top;
            bounds.Width -= margins.Right + margins.Left;
            bounds.Height -= margins.Bottom + margins.Top;
            return bounds;
        }

        private void Scale()
        {
            Rectangle rectangle = Page.MarginBounds ? GetMarginBounds() : Doc.DefaultPageSettings.Bounds;
            sx = (float)Page.Width / rectangle.Width;
            sy = (float)Page.Height / rectangle.Height;
        }

        private void Doc_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            Scale();
        }
    }
}