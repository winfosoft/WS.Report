using System;
using System.Drawing;

namespace WS.Report
{
    public class ExGraphics
    {
        private Graphics _graphics;

        public ExGraphics(Graphics graphics)
        {
            _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics), "Graphics reference cannot be null.");
        }

        public Graphics Graphics => _graphics;

        public void DrawString(string text, Font font, Color textColor, Rectangle rect, ContentAlignment align)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be null or empty.", nameof(text));
            if (font == null)
                throw new ArgumentNullException(nameof(font), "Font cannot be null.");

            try
            {
                using (SolidBrush brush = new SolidBrush(textColor))
                {
                    StringFormat format = new StringFormat
                    {
                        Alignment = (StringAlignment)Methods.GetXAlign(align),
                        LineAlignment = (StringAlignment)Methods.GetYAlign(align)
                    };

                    _graphics.DrawString(text, font, brush, rect, format);
                }
            }
            catch (Exception ex)
            {
                // Handle exception or log it
                Console.WriteLine($"Error drawing string: {ex.Message}");
            }
        }

        public void DrawString(string text, Font font, Color textColor, Rectangle rect, ContentAlignment align, StringFormatFlags formatFlags)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be null or empty.", nameof(text));
            if (font == null)
                throw new ArgumentNullException(nameof(font), "Font cannot be null.");

            try
            {
                using (SolidBrush brush = new SolidBrush(textColor))
                {
                    StringFormat format = new StringFormat
                    {
                        Alignment = (StringAlignment)Methods.GetXAlign(align),
                        LineAlignment = (StringAlignment)Methods.GetYAlign(align),
                        FormatFlags = formatFlags
                    };

                    _graphics.DrawString(text, font, brush, rect, format);
                }
            }
            catch (Exception ex)
            {
                // Handle exception or log it
                Console.WriteLine($"Error drawing string: {ex.Message}");
            }
        }

        public Rectangle DrawImage(Bitmap image, Rectangle rect, ref int hSpace, ContentAlignment align = ContentAlignment.MiddleCenter, int vSpace = 4)

        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), "Image cannot be null.");

            Rectangle targetRect = new Rectangle(Point.Empty, image.Size);

            Point location = rect.Location;
            HorizontalAlignment xAlign = Methods.GetXAlign(align);
            VerticalAlignment yAlign = Methods.GetYAlign(align);

            switch (xAlign)
            {
                case HorizontalAlignment.Left:
                    location.X += hSpace;
                    break;

                case HorizontalAlignment.Center:
                    location.X += hSpace + (rect.Width - targetRect.Width) / 2;
                    break;

                case HorizontalAlignment.Right:
                    location.X += rect.Width - targetRect.Width - hSpace;
                    break;
            }

            switch (yAlign)
            {
                case VerticalAlignment.Top:
                    location.Y += vSpace;
                    break;

                case VerticalAlignment.Middle:
                    location.Y += vSpace + (rect.Height - targetRect.Height) / 2;
                    break;

                case VerticalAlignment.Bottom:
                    location.Y += rect.Height - targetRect.Height - vSpace;
                    break;
            }

            targetRect.Location = location;
            _graphics.DrawImage(image, targetRect);

            return targetRect;
        }

        public Rectangle DrawIcon(Icon icon, Rectangle rect, ref int hSpace, ContentAlignment align = ContentAlignment.MiddleCenter, int vSpace = 4)

        {
            if (icon == null)
                throw new ArgumentNullException(nameof(icon), "Icon cannot be null.");

            Rectangle targetRect = new Rectangle(Point.Empty, icon.Size);

            Point location = rect.Location;
            HorizontalAlignment xAlign = Methods.GetXAlign(align);
            VerticalAlignment yAlign = Methods.GetYAlign(align);

            switch (xAlign)
            {
                case HorizontalAlignment.Left:
                    location.X += hSpace;
                    break;

                case HorizontalAlignment.Center:
                    location.X += hSpace + (rect.Width - targetRect.Width) / 2;
                    break;

                case HorizontalAlignment.Right:
                    location.X += rect.Width - targetRect.Width - hSpace;
                    break;
            }

            switch (yAlign)
            {
                case VerticalAlignment.Top:
                    location.Y += vSpace;
                    break;

                case VerticalAlignment.Middle:
                    location.Y += vSpace + (rect.Height - targetRect.Height) / 2;
                    break;

                case VerticalAlignment.Bottom:
                    location.Y += rect.Height - targetRect.Height - vSpace;
                    break;
            }

            targetRect.Location = location;
            _graphics.DrawIcon(icon, targetRect);

            return targetRect;
        }
    }
}