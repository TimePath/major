using System.IO;
using Gdk;
using System.Drawing;
using System.Drawing.Imaging;

internal static class BitmapConverter
{
	internal static Pixbuf ToPixbuf (this Bitmap bmp)
	{
		MemoryStream ms = new MemoryStream ();
		bmp.Save (ms, ImageFormat.Png);
		ms.Position = 0;
		return new Pixbuf (ms); 
	}
}
