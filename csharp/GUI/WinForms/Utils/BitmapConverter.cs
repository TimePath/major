using System.Drawing;

internal static class BitmapConverter
{
	internal static System.Drawing.Icon ToIcon (this Bitmap bmp)
	{
		return System.Drawing.Icon.FromHandle (bmp.GetHicon ());
	}
}
