using System;
using System.Windows.Forms;

namespace GUI.WinForms
{
	public class MainWindow : Form
	{
		private Controller c = new Controller();

		private NotifyIcon notifyIcon;

		public MainWindow ()
		{
			MenuItem[] menuItems = new MenuItem[2];
			menuItems [0] = new MenuItem ("&Explore", Open);
			menuItems [1] = new MenuItem ("E&xit", Exit);

			notifyIcon = new NotifyIcon ();
			notifyIcon.Icon = c.Icon.Bitmap.ToIcon ();
			notifyIcon.Text = c.Icon.Tooltip;
			notifyIcon.ContextMenu = new ContextMenu (menuItems);
			notifyIcon.DoubleClick += Open;
			notifyIcon.Visible = true;
		}

		void Open (object sender, EventArgs e)
		{
			c.Mount.Open ();
		}

		void Exit (object sender, EventArgs e)
		{
			Application.Exit ();
		}

	}
}

