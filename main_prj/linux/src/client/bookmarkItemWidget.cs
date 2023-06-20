using System;
using Gtk;
using HCVK.HCVKSLibrary.VO;
using Mono.Unix;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class bookmarkItemWidget : Gtk.Bin
    {
		private VOBookmark _voBookmark = null;
		private Menu _contextmenu = null;

		private Gdk.Pixbuf _desktopIconNormal = null;
        private Gdk.Pixbuf _desktopIconDisbable = null;
        private Gdk.Pixbuf _desktopIconHover = null;
        private Gdk.Pixbuf _desktopIconSelected = null;

		public bookmarkItemWidget()
        {
            this.Build();

			// background
            SetStyleNormal();
		}

		public void SetBookmarkInfo(VOBookmark bookmark)
		{
			_voBookmark = bookmark;

			this.labelServerName.Text = bookmark.ServerName;
			this.labelDesktopName.Text = bookmark.DesktopName;
			this.labelDesktopIP.Text = bookmark.DesktopIP;

			if (string.IsNullOrEmpty(_voBookmark.OSCode))
			{
				_desktopIconNormal = StyleSheet.ImageWin7Noraml;
                _desktopIconDisbable = StyleSheet.ImageWin7Disable;
                _desktopIconHover = StyleSheet.ImageWin7Hover;
                _desktopIconSelected = StyleSheet.ImageWin7Selected;
			}
			else if (_voBookmark.OSCode.Equals("OS010232") == true || _voBookmark.OSCode.Equals("OS010264") == true)
            {
                _desktopIconNormal = StyleSheet.ImageWin10Noraml;
                _desktopIconDisbable = StyleSheet.ImageWin10Disable;
                _desktopIconHover = StyleSheet.ImageWin10Hover;
                _desktopIconSelected = StyleSheet.ImageWin10Selected;
            }
			else if (_voBookmark.OSCode.Equals("OS020132") == true || _voBookmark.OSCode.Equals("OS020132") == true)
            {
                _desktopIconNormal = StyleSheet.ImageUbuntuNoraml;
                _desktopIconDisbable = StyleSheet.ImageUbuntuDisable;
                _desktopIconHover = StyleSheet.ImageUbuntuHover;
                _desktopIconSelected = StyleSheet.ImageUbuntuSelected;
            }
			else if (_voBookmark.OSCode.Equals("OS020232") == true || _voBookmark.OSCode.Equals("OS020264") == true)
            {
                _desktopIconNormal = StyleSheet.ImageCentOSNoraml;
                _desktopIconDisbable = StyleSheet.ImageCentOSDisable;
                _desktopIconHover = StyleSheet.ImageCentOSHover;
                _desktopIconSelected = StyleSheet.ImageCentOSSelected;
            }
            else if (_voBookmark.OSCode.Equals("OS020364") == true )
            {
                // TMAXOS  OSCODE OS020364
                _desktopIconNormal = StyleSheet.ImageTmaxNoraml;
                _desktopIconDisbable = StyleSheet.ImageTmaxDisable;
                _desktopIconHover = StyleSheet.ImageTmaxHover;
                _desktopIconSelected = StyleSheet.ImageTmaxSelected;
            }
            else if (_voBookmark.OSCode.Equals("OS020464") == true)
            {
                // HANCOM  OSCODE OS020464
                _desktopIconNormal = StyleSheet.ImageHancomNoraml;
                _desktopIconDisbable = StyleSheet.ImageHancomDisable;
                _desktopIconHover = StyleSheet.ImageHancomHover;
                _desktopIconSelected = StyleSheet.ImageHancomSelected;
            }
            else if (_voBookmark.OSCode.Equals("OS020564") == true)
            {
                // HAMONIKR  OSCODE OS020564
                _desktopIconNormal = StyleSheet.ImageHamonikrNoraml;
                _desktopIconDisbable = StyleSheet.ImageHamonikrDisable;
                _desktopIconHover = StyleSheet.ImageHamonikrHover;
                _desktopIconSelected = StyleSheet.ImageHamonikrSelected;
            }
            else
            {
                _desktopIconNormal = StyleSheet.ImageWin7Noraml;
                _desktopIconDisbable = StyleSheet.ImageWin7Disable;
                _desktopIconHover = StyleSheet.ImageWin7Hover;
                _desktopIconSelected = StyleSheet.ImageWin7Selected;
            }

			this.imageDesktop.Pixbuf = this._desktopIconNormal;
		}

		protected void OnEventboxButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
		{
			if(args.Event.Button == 1)
			    MainWindow.mainWindow.ExcuteBookmark(this._voBookmark);
			else if(args.Event.Button == 3)
			{
				ContextMenu();
			}
		}

		private void ContextMenu()
		{
			if(this._contextmenu == null)
			{
				this._contextmenu = new Menu();

				MenuItem removeItem = new MenuItem(Catalog.GetString("Remove Bookmark"));
				removeItem.ButtonPressEvent += new ButtonPressEventHandler(OnRemoveItemButtonPressed);

				this._contextmenu.Add(removeItem);
				this._contextmenu.ShowAll();
			}

			this._contextmenu.Popup();
		}

		protected void OnRemoveItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
		{
			MainWindow.mainWindow.RemoveBookmarkInfo(this._voBookmark);
		}

		protected void OnEventboxEnterNotifyEvent(object o, EnterNotifyEventArgs args)
		{
			SetStyleMouseOver();
		}

		protected void OnEventboxLeaveNotifyEvent(object o, LeaveNotifyEventArgs args)
		{
			SetStyleNormal();
		}
		private void SetStyleNormal()
        {
            this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.ServerBGEnable);

            this.labelServerName.ModifyFg(StateType.Normal, StyleSheet.SNameFontEnable);
			this.labelDesktopName.ModifyFg(StateType.Normal, StyleSheet.DNameFontEnable);
			this.labelDesktopIP.ModifyFg(StateType.Normal, StyleSheet.DIPFontEnable);


			this.imageServer.Pixbuf = StyleSheet.ImageServerNoraml;
			this.imageDesktop.Pixbuf = this._desktopIconNormal;
			this.imageArrow.Pixbuf = StyleSheet.ImageBookmarkArrowNoraml;

        }
        private void SetStyleMouseOver()
        {
            this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.ServerBGHover);

            this.labelServerName.ModifyFg(StateType.Normal, StyleSheet.SNameFontHover);
			this.labelDesktopName.ModifyFg(StateType.Normal, StyleSheet.DNameFontHover);
			this.labelDesktopIP.ModifyFg(StateType.Normal, StyleSheet.DIPFontHover);

            this.imageServer.Pixbuf = StyleSheet.ImageServerHover;
			this.imageArrow.Pixbuf = StyleSheet.ImageBookmarkArrowHover;
        }
	}
}
