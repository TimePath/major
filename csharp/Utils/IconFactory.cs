using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GUI
{
    public class IconFactory
    {
        static Font myFont = new Font ("Arial Black", 12, FontStyle.Bold, GraphicsUnit.Pixel);

        public static Bitmap CreateIcon (string s, Color ballColor)
        {
            int w = 16, h = 16;
            int x = 0, y = 0;

            Bitmap bm = new Bitmap (w, h);

            Graphics g = Graphics.FromImage ((Image)bm);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawBall (g, new Rectangle (0, 0, w - 1, h - 1), ballColor);
            if (OS.IsWindows) {
                g.DrawString (s, myFont, Brushes.Black, new Point (x + 1, y + 1));
                g.DrawString (s, myFont, Brushes.White, new Point (x, y));
            }

            return bm;
        }

        private static void DrawBall (Graphics g, Rectangle rect, Color c)
        {
            GraphicsPath path = new GraphicsPath ();
            path.AddEllipse (rect);

            PathGradientBrush pgbrush = new PathGradientBrush (path);
            pgbrush.CenterPoint = new Point ((rect.Right - rect.Left) / 3 + rect.Left, (rect.Bottom - rect.Top) / 3 + rect.Top);
            pgbrush.CenterColor = Color.White;
            pgbrush.SurroundColors = new Color[] { c };

            g.FillEllipse (pgbrush, rect);
            g.DrawEllipse (new Pen (c), rect);
        }
    }
}

