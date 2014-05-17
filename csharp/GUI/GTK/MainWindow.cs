using System;
using Gtk;

namespace GUI.GTK
{
    public partial class MainWindow: Gtk.Window
    {
        GUI.Controller c = new GUI.Controller ();
        StatusIcon statusIcon;
        Menu popupMenu;

        public MainWindow () : base (Gtk.WindowType.Toplevel)
        {
            Build ();

            popupMenu = new Menu ();

            ImageMenuItem explore = new ImageMenuItem ("Explore");
            explore.Image = new Image (Stock.Open, IconSize.Menu);
            explore.Activated += Open;
            popupMenu.Add (explore);

            ImageMenuItem quit = new ImageMenuItem ("Exit");
            quit.Image = new Image (Stock.Quit, IconSize.Menu);
            quit.Activated += delegate {
                Application.Quit ();
            };

            popupMenu.Add (quit);

            statusIcon = new StatusIcon ();
            statusIcon.Pixbuf = c.Icon.Bitmap.ToPixbuf ();
            statusIcon.Activate += Open;
            statusIcon.PopupMenu += delegate {
                popupMenu.ShowAll ();
                popupMenu.Popup ();
            };
            statusIcon.Tooltip = c.Icon.Tooltip;
            statusIcon.Visible = true;

            c.Start ();
        }

        protected void Open (object sender, EventArgs args)
        {
            c.Mount.Open ();
        }

        protected void Exit (object sender, EventArgs args)
        {
            Application.Quit ();
        }

        protected void WindowClose (object sender, DeleteEventArgs args)
        {

        }
    }
}
