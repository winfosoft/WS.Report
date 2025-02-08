using System;
using System.Drawing;

namespace WS.Report
{
    internal sealed class Methods
    {
        public static Rectangle GetScaledBound(Rectangle Bound, float sx, float sy)
        {
            return new Rectangle(
                (int)Math.Round(Bound.X / (double)sx),
                (int)Math.Round(Bound.Y / (double)sy),
                (int)Math.Round(Bound.Width / (double)sx),
                (int)Math.Round(Bound.Height / (double)sy)
            );
        }

        // Overloads for GetRect to handle optional parameters
        public static Rectangle GetRect(Size theSize, Rectangle Rect)
        {
            return GetRect(theSize, Rect, ContentAlignment.MiddleCenter);
        }

        public static Rectangle GetRect(Size theSize, Rectangle Rect, ContentAlignment Align)
        {
            return GetRect(theSize, Rect, Align, 4);
        }

        public static Rectangle GetRect(Size theSize, Rectangle Rect, ContentAlignment Align, int VSpace)
        {
            int defaultHSpace = 4;
            return GetRect(theSize, Rect, Align, VSpace, ref defaultHSpace);
        }

        public static Rectangle GetRect(Size theSize, Rectangle Rect, ContentAlignment Align, int VSpace, ref int HSpace)
        {
            Rectangle rectangle = new Rectangle(new Point(0, 0), theSize);
            Point location = Rect.Location;
            HorizontalAlignment xalign = GetXAlign(Align);
            VerticalAlignment yalign = GetYAlign(Align);

            switch (xalign)
            {
                case HorizontalAlignment.Left:
                    location.X += HSpace;
                    break;

                case HorizontalAlignment.Center:
                    location.X += HSpace + (Rect.Width - rectangle.Width) / 2;
                    break;

                case HorizontalAlignment.Right:
                    location.X += Rect.Width - rectangle.Width - HSpace;
                    break;
            }

            switch (yalign)
            {
                case VerticalAlignment.Top:
                    location.Y += VSpace;
                    break;

                case VerticalAlignment.Middle:
                    location.Y += VSpace + (Rect.Height - rectangle.Height) / 2;
                    break;

                case VerticalAlignment.Bottom:
                    location.Y += Rect.Height - rectangle.Height - VSpace;
                    break;
            }

            rectangle.Location = location;
            return rectangle;
        }

        internal static StringAlignment GetLineAlign(ContentAlignment ConAlign)
        {
            return (StringAlignment)GetYAlign(ConAlign);
        }

        internal static StringAlignment GetTextAlign(ContentAlignment ConAlign)
        {
            return (StringAlignment)GetXAlign(ConAlign);
        }

        internal static VerticalAlignment GetYAlign(ContentAlignment ConAlign)
        {
            int[] topValues = new int[] { 1, 2, 4 };
            int[] middleValues = new int[] { 16, 32, 64 };
            int[] bottomValues = new int[] { 256, 512, 1024 };
            int conAlignValue = (int)ConAlign;

            if (Array.IndexOf(topValues, conAlignValue) != -1)
                return VerticalAlignment.Top;
            if (Array.IndexOf(middleValues, conAlignValue) != -1)
                return VerticalAlignment.Middle;
            if (Array.IndexOf(bottomValues, conAlignValue) != -1)
                return VerticalAlignment.Bottom;

            return VerticalAlignment.Top; // Default fallback
        }

        internal static HorizontalAlignment GetXAlign(ContentAlignment ConAlign)
        {
            int[] leftValues = new int[] { 1, 16, 256 };
            int[] centerValues = new int[] { 2, 32, 512 };
            int[] rightValues = new int[] { 4, 64, 1024 };
            int conAlignValue = (int)ConAlign;

            if (Array.IndexOf(leftValues, conAlignValue) != -1)
                return HorizontalAlignment.Left;
            if (Array.IndexOf(centerValues, conAlignValue) != -1)
                return HorizontalAlignment.Center;
            if (Array.IndexOf(rightValues, conAlignValue) != -1)
                return HorizontalAlignment.Right;

            return HorizontalAlignment.Left; // Default fallback
        }
    }
}