using System;
using Gtk;

namespace GUI.GTK
{
    public partial class MainWindow: Gtk.Dialog
    {
        GUI.Controller c = new GUI.Controller ();
        StatusIcon statusIcon;
        Menu popupMenu;

        public MainWindow ()
        {
            Build ();

            popupMenu = new Menu ();

            ImageMenuItem explore = new ImageMenuItem ("Explore");
            explore.Image = new Image (Stock.Open, IconSize.Menu);
            explore.Activated += Open;
            popupMenu.Add (explore);

            ImageMenuItem quit = new ImageMenuItem ("Exit");
            quit.Image = new Image (Stock.Quit, IconSize.Menu);
            quit.Activated += (object sender, EventArgs e) => {
                Application.Quit ();
            };
            popupMenu.Add (quit);

            statusIcon = new StatusIcon ();
            statusIcon.Pixbuf = c.Icon.Bitmap.ToPixbuf ();
            statusIcon.Activate += Open;
            statusIcon.PopupMenu += (object o, PopupMenuArgs args) => {
                popupMenu.ShowAll ();
                popupMenu.Popup ();
            };
            statusIcon.Tooltip = c.Icon.Tooltip;
            statusIcon.Visible = true;
        }

        protected void Open (object sender, EventArgs args)
        {
            c.Browse ();
        }

        protected void OnDeleteEvent (object sender, EventArgs args)
        {
            Application.Quit ();
        }

        protected void OnConnectButtonClicked (object sender, EventArgs e)
        {
            c.Connect (entry1.Text, entry2.Text);
        }
    }
}
