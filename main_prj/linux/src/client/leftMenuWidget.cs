using System;
using System.Drawing;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class leftMenuWidget : Gtk.Bin
    {
		#region Properties

        #endregion

		public leftMenuWidget()
        {
			MainFunc.callbackEnableBookmark = this.SetEnableBookmark;
			MainFunc.callbackSelectedLeftItem = this.SetSelectedItem;

			this.Build();

            // TODO : after init effect change logo, for custom, mois
            // do not edit code in mehtod (Build())
            // this.image1.Pixbuf = global::Gdk.Pixbuf.LoadFromResource("client.Resources.mois_logo.png");



            this.MainItem.Initialize(0);
			this.BookmarkItem.Initialize(1);
			this.SettingItem.Initialize(2);

            Color color = ColorTranslator.FromHtml("#9a9a9a");
			this.labelApplicationName.ModifyFg(Gtk.StateType.Normal, new Gdk.Color(color.R, color.G, color.B));
			color = ColorTranslator.FromHtml("#d6d6d6");
			this.labelVersion.ModifyFg(Gtk.StateType.Normal, new Gdk.Color(color.R, color.G, color.B));
			color = ColorTranslator.FromHtml("#717171");
			this.labelCompany.ModifyFg(Gtk.StateType.Normal, new Gdk.Color(color.R, color.G, color.B));
			color = ColorTranslator.FromHtml("#828282");
            this.labelURL.ModifyFg(Gtk.StateType.Normal, new Gdk.Color(color.R, color.G, color.B));

            StyleSheet.SetStyleCancelButton(this.button14);

            AfterEffectInitializeComponent();
        }

		protected void OnTogglebtnMainClicked(object sender, EventArgs e)
		{
			MainWindow.mainWindow.ChangePage(0);
		}

		protected void OnTogglebtnBookmarkClicked(object sender, EventArgs e)
		{
            MainWindow.mainWindow.ChangePage(1);
		}

		protected void OnTogglebtnSettingClicked(object sender, EventArgs e)
		{
            MainWindow.mainWindow.ChangePage(2);
		}

		public void SetSelectedItem(int nIdx)
		{
			if (nIdx == 0)
			{
				this.MainItem.StateType = Gtk.StateType.Selected;
				if (this.BookmarkItem.StateType != Gtk.StateType.Insensitive)
					this.BookmarkItem.StateType = Gtk.StateType.Normal;
				this.SettingItem.StateType = Gtk.StateType.Normal;
			}
			else if(nIdx == 1)
            {
				this.MainItem.StateType = Gtk.StateType.Normal;
                if (this.BookmarkItem.StateType != Gtk.StateType.Insensitive)
					this.BookmarkItem.StateType = Gtk.StateType.Selected;
                this.SettingItem.StateType = Gtk.StateType.Normal;
			}
			else if (nIdx == 2)
            {
				this.MainItem.StateType = Gtk.StateType.Normal;
                if (this.BookmarkItem.StateType != Gtk.StateType.Insensitive)
                    this.BookmarkItem.StateType = Gtk.StateType.Normal;
				this.SettingItem.StateType = Gtk.StateType.Selected;
            }
            
		}

		public void SetEnableBookmark(bool bEnable)
		{
			if (bEnable)
				this.BookmarkItem.StateType = Gtk.StateType.Normal;
			else
				this.BookmarkItem.StateType = Gtk.StateType.Insensitive;
		}

        protected void OnPowerOffButtonClicked(object sender, EventArgs e)
        {
            MainWindow.mainWindow.shutDown_Device();
        }

		private void AfterEffectInitializeComponent ()
		{
			// TODO : after init effect change logo, for custom, mois
			// do not edit code in method (Build())
			global::Gdk.Pixbuf logImage = null;

			try {
				logImage = new global::Gdk.Pixbuf ("/usr/lib/DaaSXpertClient/client_logo.png");

			} catch {

			}

			if (logImage != null) {
				this.image1.Pixbuf = logImage;

			} else {
				this.image1.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("client.Resources.leftMenu_logo.png");
			}

			if (MainWindow.mainWindow.environment.vOGeneralsProperties.ShowSettingMenu == false) {
				this.vbox2.Remove (this.SettingItem);
			}
			if (MainWindow.mainWindow.environment.vOGeneralsProperties.ShowBookMarkMenu == false)
			{ 
				this.vbox2.Remove(this.BookmarkItem);
            }
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowPowerOffBtn == false)
            {
                this.vbox2.Remove(this.button14);
            }
            // hide banner imnage
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowCompanyName == false)
            {
                this.vboxTitle.Remove(this.labelCompany);
            }

			this.labelVersion.LabelProp = Properties.Resources.CLIENT_VERSION;

			this.eventboxVersion.ModifyBg (Gtk.StateType.Normal, new Gdk.Color (57, 58, 63));


        }

		protected void OnEventboxVersionButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			InfoWindow versionInfoWindow = new InfoWindow (System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(), MainWindow.mainWindow.environment.vOGeneralsProperties.BuildDate);

			versionInfoWindow.SetPosition (Gtk.WindowPosition.CenterAlways);
			versionInfoWindow.Modal = true;

		}
	}
}
