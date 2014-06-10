using System.Drawing;

internal static class BitmapConverter
{
    internal static Icon ToIcon (this Bitmap bmp)
    {
        return Icon.FromHandle (bmp.GetHicon ());
    }
}
