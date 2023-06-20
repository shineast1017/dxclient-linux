using System;
using System.Linq;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class bookmarkPageWidget : Gtk.Bin
    {
        public bookmarkPageWidget()
        {
            this.Build();

            MainFunc.callbackLoadBookmarkInfo = this.LoadBookmarkInfo;

            LoadBookmarkInfo();
        }

        private int nCnt = 0;
        private void LoadBookmarkInfo()
        {
            if (this.vboxBookmark.Children != null)
                this.vboxBookmark.Children.ToList().ForEach(s => this.vboxBookmark.Remove(s));

            MainWindow.mainWindow.environment.Bookmarks.ForEach(
                bookmark =>
            {
                bookmarkItemWidget itemWidget = new bookmarkItemWidget();

                itemWidget.CanFocus = true;
                itemWidget.Name = string.Format("bookmarkItemWidget{0}", nCnt++);
                itemWidget.Events |= Gdk.EventMask.EnterNotifyMask | Gdk.EventMask.LeaveNotifyMask;
                itemWidget.Show();
                this.vboxBookmark.Add(itemWidget);

                global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxBookmark[itemWidget]));
                w1.Position = this.vboxBookmark.Children.Length - 1;
                w1.Expand = false;

                itemWidget.SetBookmarkInfo(bookmark);
            });
        }
    }
}
