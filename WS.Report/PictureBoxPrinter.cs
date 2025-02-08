using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WS.Report
{
    public class PictureBoxPrinter : Printer
    {
        private readonly WPictureBox Imb;

        public PictureBoxPrinter(IPrintableControl printControl) : base(printControl)
        {
            Imb = (WPictureBox)printControl;
        }

        protected internal override bool Print(Graphics g, Rectangle Bounds)
        {
            PictureBox sourceControl = (PictureBox)Imb.SourceControl;
            if (sourceControl.Image == null)
                return true;

            Graphics graphics = g;
            Image image = !Imb.ResizeToFit
                ? new Bitmap(sourceControl.Image, Bounds.Size)
                : ResizeImage(sourceControl.Image, Bounds.Width, Bounds.Height);

            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            ExGraphics exGraphics = new ExGraphics(g);
            Bitmap bitmap = (Bitmap)image;
            int hSpace = 0; // Initialize hSpace
            Rectangle rect = exGraphics.DrawImage(bitmap, Bounds, ref hSpace, Imb.Align);

            if (Imb.BorderSize > 0)
            {
                using (Pen pen = new Pen(Imb.BorderColor, Imb.BorderSize))
                {
                    pen.DashStyle = Imb.Border;
                    g.DrawRectangle(pen, rect);
                }
            }

            return true;
        }

        public Image ResizeImage(Image sourceImage, int maxWidth, int maxHeight)
        {
            int width = sourceImage.Width;
            int height = sourceImage.Height;

            int widthDifference = Ds(width, maxWidth);
            int heightDifference = Ds(height, maxHeight);

            if (widthDifference > 0 && heightDifference > 0)
            {
                if (widthDifference > heightDifference)
                {
                    int newHeight = GetSizeByWidth(width, height, maxWidth);
                    return GetNewImage(sourceImage, maxWidth, newHeight);
                }
                else if (heightDifference > widthDifference)
                {
                    int newWidth = GetSizeByHeight(width, height, maxHeight);
                    return GetNewImage(sourceImage, newWidth, maxHeight);
                }
                else
                {
                    int size = Math.Max(maxWidth, maxHeight);
                    return GetNewImage(sourceImage, size, size);
                }
            }
            else if (widthDifference > 0)
            {
                int newHeight = GetSizeByWidth(width, height, maxWidth);
                return GetNewImage(sourceImage, maxWidth, newHeight);
            }
            else if (heightDifference > 0)
            {
                int newWidth = GetSizeByHeight(width, height, maxHeight);
                return GetNewImage(sourceImage, newWidth, maxHeight);
            }
            else
            {
                return sourceImage;
            }
        }

        public int Ds(int w1, int w2)
        {
            return w1 - w2;
        }

        public int GetSizeByWidth(int w1, int h1, int width)
        {
            return (int)Math.Round((double)width / w1 * h1);
        }

        public int GetSizeByHeight(int w1, int h1, int height)
        {
            return (int)Math.Round((double)height / h1 * w1);
        }

        public Image GetNewImage(Image img, int width, int height)
        {
            if (height <= 0) height = 1;
            if (width <= 0) width = 1;
            return new Bitmap(img, width, height);
        }
    }
}