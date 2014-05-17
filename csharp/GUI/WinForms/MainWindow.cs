using System;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public class MainWindow : Form
    {
        private Controller c = new Controller ();
        private NotifyIcon notifyIcon;

        public MainWindow ()
        {
            notifyIcon = new NotifyIcon ();
            notifyIcon.Icon = c.Icon.Bitmap.ToIcon ();
            notifyIcon.Text = c.Icon.Tooltip;
            MenuItem[] menuItems = new MenuItem[] {
                new MenuItem ("&Explore", Open),
                new MenuItem ("E&xit", Exit)
            };
            notifyIcon.ContextMenu = new ContextMenu (menuItems);
            notifyIcon.DoubleClick += Open;
            notifyIcon.Visible = true;

            c.Start ();
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

