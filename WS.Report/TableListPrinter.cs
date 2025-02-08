using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace WS.Report
{
    public class TableListPrinter : Printer
    {
        private int StartIndex;
        private int s1;
        private int HStartIndex;
        private int hs1;
        private bool Sec;

        internal TableListPrinter(IPrintableControl printControl) : base(printControl)
        {
        }

        private WTableList Lvw => (WTableList)Control;

        private ListView Sc => (ListView)Lvw.SourceControl;

        protected internal override bool Print(Graphics g, Rectangle bounds)
        {
            if (Sc.Columns.Count == 0)
                return true;

            var widths = new List<int>();
            if (Lvw.HeaderFont == null)
                Lvw.HeaderFont = Sc.Font;

            for (int i = 0; i < Sc.Columns.Count; i++)
            {
                widths.Add(Sc.AutoSize ? GetBestWidth(GetColumnWidths(i)) : Sc.Columns[i].Width);
            }

            int toHeader = GetToHeader(widths, bounds.Width, HStartIndex);
            var rowsHeights = GetRowsHeights(g, widths);
            int width = Sum(widths.ToArray(), HStartIndex, toHeader, 1);

            if (Lvw.FitMaxWidth && HStartIndex == 0)
            {
                if (bounds.Width > width)
                    widths = SplitWidth(ref widths, Math.Abs(bounds.Width - width), HStartIndex, toHeader);
                width = bounds.Width;
            }

            int itemCount = Lvw.PrintSelection ? Sc.SelectedItems.Count : Sc.Items.Count;
            int toIndex = GetToIndex(rowsHeights, bounds.Height, StartIndex);
            int height = GetTableHeight(rowsHeights, StartIndex, StartIndex == 0 ? toIndex + 1 : toIndex);

            if (Lvw.FitMaxHeight)
                height = bounds.Height;

            ContentAlignment conAlign = Lvw.Align;
            if (HStartIndex > 0)
            {
                VerticalAlignment yAlign = Methods.GetYAlign(conAlign);
                conAlign = Lvw.RightToLeft == RightToLeft.No
                    ? yAlign == VerticalAlignment.Top ? ContentAlignment.TopLeft
                    : yAlign == VerticalAlignment.Middle ? ContentAlignment.MiddleLeft
                    : ContentAlignment.BottomLeft
                    : yAlign == VerticalAlignment.Top ? ContentAlignment.TopRight
                    : yAlign == VerticalAlignment.Middle ? ContentAlignment.MiddleRight
                    : ContentAlignment.BottomRight;
            }

            Size theSize = new Size(width, height);
            Rectangle rect = bounds;
            int vSpace = 0;
            int num5 = 0;
            Rectangle printRect = Methods.GetRect(theSize, rect, conAlign, vSpace, ref num5);

            if (printRect.Bottom > bounds.Bottom && !Lvw.OnePageOnly)
                return false;

            int headerHeight = rowsHeights[0];
            rowsHeights.RemoveAt(0);
            DrawListView(g, printRect, StartIndex, toIndex, HStartIndex, toHeader, widths, rowsHeights, headerHeight);

            HStartIndex = toHeader + 1;
            if (toHeader >= Sc.Columns.Count - 1)
            {
                StartIndex = toIndex + 1;
                HStartIndex = 0;
            }

            bool isComplete = (toIndex >= itemCount - 1 && toHeader >= Sc.Columns.Count - 1) || Lvw.OnePageOnly;
            if (isComplete)
            {
                StartIndex = 0;
                HStartIndex = 0;
            }

            if (Sec && s1 == StartIndex && hs1 == HStartIndex)
                return true;

            s1 = StartIndex;
            hs1 = HStartIndex;
            Sec = true;

            return isComplete;
        }

        protected internal List<int> SplitWidth(ref List<int> widths, int value, int fromI = 0, int toI = -1)
        {
            if (toI == -1)
                toI = widths.Count - 1;

            int sum = Sum(widths.ToArray(), fromI, toI, 1);
            for (int i = fromI; i <= toI; i++)
            {
                widths[i] += (int)Math.Round(value * (double)widths[i] / sum);
            }

            int newSum = Sum(widths.ToArray(), fromI, toI, 1);
            widths[toI] += Math.Abs(newSum - (sum + value));

            return widths;
        }

        protected int Sum(int[] values, int fromI = 0, int toI = -1, int step = 1)
        {
            if (toI == -1)
                toI = values.Length - 1;

            int sum = 0;
            for (int i = fromI; i <= toI; i += step)
            {
                sum += values[i];
            }
            return sum;
        }

        protected int GetBestWidth(List<int> columnWidths)
        {
            return columnWidths.Max();
        }

        protected List<int> GetColumnWidths(int index)
        {
            var widths = new List<int> { GetTextWidth(Sc.Columns[index].Text, Sc.Font) };

            foreach (ListViewItem item in Sc.Items)
            {
                if (item.SubItems.Count > index)
                {
                    var subItem = item.SubItems[index];
                    widths.Add(GetTextWidth(subItem.Text, item.UseItemStyleForSubItems ? item.Font : subItem.Font));
                }
            }

            return widths;
        }

        protected int GetTextWidth(string text, Font font)
        {
            return (int)Math.Round(TextRenderer.MeasureText(text, font).Width * 1.15);
        }

        protected int GetTableHeight(List<int> rowHeights, int start = 0, int toI = -1)
        {
            return Sum(rowHeights.ToArray(), start, toI, 1) + (start > 0 ? rowHeights[0] : 0);
        }

        protected int GetToIndex(List<int> heights, int pageHeight, int index)
        {
            if (GetTableHeight(heights, 0, -1) > pageHeight)
            {
                int sum = 0;
                for (int i = index; i < heights.Count; i++)
                {
                    sum += heights[i];
                    if (sum >= pageHeight)
                    {
                        return i > 0 ? i - 2 : i;
                    }
                }
            }
            return heights.Count - 2;
        }

        protected int GetToHeader(List<int> widths, int pageWidth, int startIndex)
        {
            // If the total width of all columns is less than or equal to the page width, return the last column index.
            if (Sum(widths.ToArray(), 0, widths.Count - 1, 1) <= pageWidth)
            {
                return widths.Count - 1;
            }

            int cumulativeWidth = 0;

            // Iterate through the columns starting from the specified index.
            for (int i = startIndex; i < widths.Count; i++)
            {
                cumulativeWidth += widths[i];

                // If the cumulative width exceeds the page width, return the previous column index.
                if (cumulativeWidth > pageWidth)
                {
                    return i > 0 ? i - 1 : i;
                }
            }

            // If all columns fit, return the last column index.
            return widths.Count - 1;
        }

        protected List<int> GetRowsHeights(Graphics g, List<int> widths)
        {
            var heights = new List<int> { Lvw.HeaderHieght > 0 ? Lvw.HeaderHieght : (int)Math.Round(GetRowHieght(g, -1, widths) * 1.5) };

            var items = Lvw.PrintSelection ? Sc.SelectedItems.Cast<ListViewItem>() : Sc.Items.Cast<ListViewItem>();
            foreach (var item in items)
            {
                heights.Add(Lvw.RowHeight > 0 ? Lvw.RowHeight : (int)Math.Round(GetRowHieght(g, item.Index, widths) * 1.2));
            }

            return heights;
        }

        protected int GetRowHieght(Graphics g, int index, List<int> Widths)
        {
            List<int> heights = new List<int>();
            StringFormat strFormat = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.Word
            };

            if (index == -1)
            {
                // Header row
                for (int i = 0; i < Sc.Columns.Count; i++)
                {
                    strFormat.Alignment = GetAlign(Sc.Columns[i].TextAlign);
                    heights.Add(GetTextHieght(g, Sc.Font, strFormat, Sc.Columns[i].Text, Widths[i]));
                }
            }
            else
            {
                // Data row
                for (int i = 0; i < Sc.Items[index].SubItems.Count; i++)
                {
                    var subItem = Sc.Items[index].SubItems[i];
                    strFormat.Alignment = i < Sc.Columns.Count ? GetAlign(Sc.Columns[i].TextAlign) : GetAlign(System.Windows.Forms.HorizontalAlignment.Center);
                    heights.Add(GetTextHieght(g, subItem.Font, strFormat, subItem.Text, Widths[i]));
                }
            }

            // Return the maximum height
            return heights.Max();
        }

        protected StringAlignment GetAlign(System.Windows.Forms.HorizontalAlignment hAlign)
        {
            switch (hAlign)
            {
                case System.Windows.Forms.HorizontalAlignment.Left: return StringAlignment.Near;
                case System.Windows.Forms.HorizontalAlignment.Right: return StringAlignment.Far;
                case System.Windows.Forms.HorizontalAlignment.Center: return StringAlignment.Center;
                default: return StringAlignment.Near;
            }
        }

        protected int GetTextHieght(Graphics g, Font font, StringFormat format, string text, int width)
        {
            return (int)Math.Round(g.MeasureString(text, font, width, format).Height);
        }

        public void DrawListView(Graphics g, Rectangle listRect, int fromItemIndex, int toItemIndex, int fromHeaderIndex, int toHeaderIndex, List<int> columnsWidths, List<int> itemsHeights, int headerHeight)
        {
            var brush = new SolidBrush(Color.Transparent);
            var pen = new Pen(Color.Transparent, 1f) { Alignment = PenAlignment.Outset };
            var format = new StringFormat();

            int index1, num1, num2;
            if (Sc.RightToLeft == RightToLeft.Yes)
            {
                index1 = toHeaderIndex;
                num1 = fromHeaderIndex;
                num2 = -1;
                format.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            }
            else
            {
                num1 = toHeaderIndex;
                index1 = fromHeaderIndex;
                num2 = 1;
            }

            brush.Color = Lvw.HeaderColor;
            var rect = listRect;
            rect.Height = headerHeight;
            g.FillRectangle(brush, rect);

            var rectangle = listRect;
            for (int i = fromItemIndex; i <= toItemIndex; i++)
            {
                var item = Lvw.PrintSelection ? Sc.SelectedItems[i] : Sc.Items[i];
                if (i == fromItemIndex)
                {
                    rectangle.Height = itemsHeights[i];
                    rectangle.Y += headerHeight;
                    rectangle.Width = columnsWidths[index1];
                }
                else
                {
                    rectangle = GetItemRect(item, itemsHeights[i], listRect.Width, rectangle);
                }

                rectangle.X = listRect.X - rectangle.Width;

                if (item.UseItemStyleForSubItems)
                {
                    brush.Color = item.BackColor;
                    rectangle.Width = listRect.Width;
                    rectangle.X = listRect.X;
                    g.FillRectangle(brush, rectangle);
                }
                else
                {
                    for (int j = index1; j != num1; j += num2)
                    {
                        if (item.SubItems.Count > j)
                        {
                            brush.Color = GetItemBackColor(item, j);
                            rectangle = GetSubItemRect(item, j, columnsWidths[j], rectangle);
                            g.FillRectangle(brush, rectangle);
                        }
                        else if (Sc.RightToLeft == RightToLeft.Yes)
                        {
                            rectangle.X += columnsWidths[j];
                        }
                    }
                }
            }

            if (Lvw.BorderSize > 0)
            {
                pen.DashStyle = Lvw.Border;
                pen.Color = Lvw.BorderColor;
                pen.Width = Lvw.BorderSize;
                g.DrawRectangle(pen, listRect);

                var pt1 = listRect.Location;
                pt1.Y += headerHeight;
                var pt2 = new Point(listRect.Right, pt1.Y);

                if (Lvw.UseBorderStyleForHeader)
                {
                    pen.Color = Lvw.BorderColor;
                    pen.Width = Lvw.BorderSize;
                    pen.DashStyle = Lvw.Border;
                }
                else
                {
                    pen.Color = Lvw.GridColor;
                    pen.Width = Lvw.HGridSize;
                    pen.DashStyle = Lvw.HGridStyle;
                }

                g.DrawLine(pen, pt1, pt2);
            }

            if (Lvw.VGridSize > 0)
            {
                pen.DashStyle = Lvw.VGridStyle;
                pen.Color = Lvw.VGridColor;
                pen.Width = Lvw.VGridSize;

                var pt1 = listRect.Location;
                var pt2 = new Point(pt1.X, listRect.Bottom);

                for (int i = index1; i != num1; i += num2)
                {
                    pt1.X += columnsWidths[i];
                    pt2.X = pt1.X;
                    g.DrawLine(pen, pt1, pt2);
                }

                if (Lvw.UseBorderStyleForHeader)
                {
                    pen.DashStyle = Lvw.Border;
                    pen.Color = Lvw.BorderColor;
                    pen.Width = Lvw.BorderSize;

                    pt1 = listRect.Location;
                    pt2.Y = pt1.Y + headerHeight;

                    for (int i = index1; i != num1; i += num2)
                    {
                        pt1.X += columnsWidths[i];
                        pt2.X = pt1.X;
                        g.DrawLine(pen, pt1, pt2);
                    }
                }
            }

            if (Lvw.HGridSize > 0)
            {
                pen.Color = Lvw.HGridColor;
                pen.Width = Lvw.HGridSize;
                pen.DashStyle = Lvw.HGridStyle;

                var pt1 = new Point(listRect.Left, listRect.Top + headerHeight);
                var pt2 = new Point(listRect.Right, pt1.Y);

                for (int i = fromItemIndex; i < toItemIndex; i++)
                {
                    pt1.Y += itemsHeights[i];
                    pt2.Y = pt1.Y;
                    g.DrawLine(pen, pt1, pt2);
                }

                while (listRect.Bottom - pt1.Y >= Lvw.RowHeight * 2)
                {
                    pt1.Y += Lvw.RowHeight;
                    pt2.Y = pt1.Y;
                    g.DrawLine(pen, pt1, pt2);
                }
            }

            format.LineAlignment = StringAlignment.Center;
            rectangle = listRect;

            for (int i = index1; i != num1; i += num2)
            {
                var column = Sc.Columns[i];
                if (i != index1)
                    rectangle.Location = new Point(rectangle.Right, rectangle.Y);

                rectangle.Width = columnsWidths[i];
                rectangle.Height = headerHeight;
                format.Alignment = (StringAlignment)Methods.GetXAlign(i == 0 ? GetConAlign(Lvw.FirstHeaderAlign) : GetConAlign(column.TextAlign));
                g.DrawString(column.Text, Lvw.HeaderFont, new SolidBrush(Lvw.HeaderForeColor), rectangle, format);
            }

            rectangle = listRect;
            format.LineAlignment = StringAlignment.Center;

            for (int i = fromItemIndex; i <= toItemIndex; i++)
            {
                var item = Lvw.PrintSelection ? Sc.SelectedItems[i] : Sc.Items[i];
                if (i == fromItemIndex)
                {
                    rectangle.Height = itemsHeights[i];
                    rectangle.Y += headerHeight;
                    rectangle.Width = columnsWidths[index1];
                }
                else
                {
                    rectangle = GetItemRect(item, itemsHeights[i], listRect.Width, rectangle);
                }

                rectangle.X = listRect.X - rectangle.Width;

                for (int j = index1; j != num1; j += num2)
                {
                    if (item.SubItems.Count > j)
                    {
                        rectangle = GetSubItemRect(item, j, columnsWidths[j], rectangle);
                        format.Alignment = (StringAlignment)Methods.GetXAlign(j == 0 ? GetConAlign(Lvw.FirstHeaderAlign) : GetConAlign(Sc.Columns[j].TextAlign));
                        g.DrawString(item.SubItems[j].Text, GetItemFont(item, j), new SolidBrush(GetItemForeColor(item, j)), rectangle, format);
                    }
                    else if (Sc.RightToLeft == RightToLeft.Yes)
                    {
                        rectangle.X += columnsWidths[j];
                    }
                }
            }
        }

        protected ContentAlignment GetConAlign(System.Windows.Forms.HorizontalAlignment hAlign)
        {
            switch (hAlign)
            {
                case System.Windows.Forms.HorizontalAlignment.Left: return ContentAlignment.MiddleLeft;
                case System.Windows.Forms.HorizontalAlignment.Right: return ContentAlignment.MiddleRight;
                case System.Windows.Forms.HorizontalAlignment.Center: return ContentAlignment.MiddleCenter;
                default: return ContentAlignment.MiddleLeft;
            }
        }

        private Color GetItemForeColor(ListViewItem item, int index)
        {
            return item.UseItemStyleForSubItems ? item.ForeColor : item.SubItems[index].ForeColor;
        }

        private Font GetItemFont(ListViewItem item, int index)
        {
            return item.UseItemStyleForSubItems ? item.Font : item.SubItems[index].Font;
        }

        private Rectangle GetItemRect(ListViewItem item, int itemHeight, int listWidth, Rectangle prevItemRect)
        {
            return new Rectangle(prevItemRect.X, prevItemRect.Bottom, listWidth, itemHeight);
        }

        private Rectangle GetSubItemRect(ListViewItem item, int index, int width, Rectangle prevSubItemRect)
        {
            return new Rectangle(prevSubItemRect.Right, prevSubItemRect.Y, width, prevSubItemRect.Height);
        }

        private Color GetItemBackColor(ListViewItem item, int index)
        {
            return item.UseItemStyleForSubItems ? GetColor(item.BackColor) : GetColor(item.SubItems[index].BackColor);
        }

        private Color GetColor(Color color)
        {
            return color == SystemColors.Window ? Color.Transparent : color;
        }
    }
}