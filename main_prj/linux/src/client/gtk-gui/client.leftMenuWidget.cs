
// This file has been generated by the GUI designer. Do not modify.
namespace client
{
	public partial class leftMenuWidget
	{
		private global::Gtk.VBox vbox4;

		private global::Gtk.VBox vboxTitle;

		private global::Gtk.Fixed fixed1;

		private global::Gtk.Image image1;

		private global::Gtk.Label labelApplicationName;

		private global::Gtk.EventBox eventboxVersion;

		private global::Gtk.Label labelVersion;

		private global::Gtk.Fixed fixed2;

		private global::Gtk.Label labelCompany;

		private global::Gtk.Label labelURL;

		private global::Gtk.Fixed fixed3;

		private global::Gtk.VBox vbox2;

		private global::client.leftMenuItemWidget MainItem;

		private global::client.leftMenuItemWidget BookmarkItem;

		private global::client.leftMenuItemWidget SettingItem;

		private global::Gtk.Button button14;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget client.leftMenuWidget
			global::Stetic.BinContainer.Attach(this);
			this.WidthRequest = 256;
			this.HeightRequest = 377;
			this.Name = "client.leftMenuWidget";
			// Container child client.leftMenuWidget.Gtk.Container+ContainerChild
			this.vbox4 = new global::Gtk.VBox();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			this.vbox4.BorderWidth = ((uint)(3));
			// Container child vbox4.Gtk.Box+BoxChild
			this.vboxTitle = new global::Gtk.VBox();
			this.vboxTitle.Name = "vboxTitle";
			this.vboxTitle.Spacing = 6;
			// Container child vboxTitle.Gtk.Box+BoxChild
			this.fixed1 = new global::Gtk.Fixed();
			this.fixed1.HeightRequest = 1;
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			this.vboxTitle.Add(this.fixed1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxTitle[this.fixed1]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vboxTitle.Gtk.Box+BoxChild
			this.image1 = new global::Gtk.Image();
			this.image1.Name = "image1";
			this.image1.Pixbuf = global::Gdk.Pixbuf.LoadFromResource("client.Resources.dassxpert_logo.png");
			this.vboxTitle.Add(this.image1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vboxTitle[this.image1]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vboxTitle.Gtk.Box+BoxChild
			this.labelApplicationName = new global::Gtk.Label();
			this.labelApplicationName.Name = "labelApplicationName";
			this.labelApplicationName.LabelProp = global::Mono.Unix.Catalog.GetString("Client Application");
			this.vboxTitle.Add(this.labelApplicationName);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vboxTitle[this.labelApplicationName]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vboxTitle.Gtk.Box+BoxChild
			this.eventboxVersion = new global::Gtk.EventBox();
			this.eventboxVersion.Name = "eventboxVersion";
			// Container child eventboxVersion.Gtk.Container+ContainerChild
			this.labelVersion = new global::Gtk.Label();
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.LabelProp = global::Mono.Unix.Catalog.GetString("VERSION");
			this.eventboxVersion.Add(this.labelVersion);
			this.vboxTitle.Add(this.eventboxVersion);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vboxTitle[this.eventboxVersion]));
			w5.Position = 3;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vboxTitle.Gtk.Box+BoxChild
			this.fixed2 = new global::Gtk.Fixed();
			this.fixed2.HeightRequest = 15;
			this.fixed2.Name = "fixed2";
			this.fixed2.HasWindow = false;
			this.vboxTitle.Add(this.fixed2);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vboxTitle[this.fixed2]));
			w6.Position = 4;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vboxTitle.Gtk.Box+BoxChild
			this.labelCompany = new global::Gtk.Label();
			this.labelCompany.Name = "labelCompany";
			this.labelCompany.LabelProp = global::Mono.Unix.Catalog.GetString("Crossent Co., Ltd.");
			this.vboxTitle.Add(this.labelCompany);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vboxTitle[this.labelCompany]));
			w7.Position = 5;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vboxTitle.Gtk.Box+BoxChild
			this.labelURL = new global::Gtk.Label();
			this.labelURL.Name = "labelURL";
			this.vboxTitle.Add(this.labelURL);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vboxTitle[this.labelURL]));
			w8.Position = 6;
			w8.Expand = false;
			w8.Fill = false;
			// Container child vboxTitle.Gtk.Box+BoxChild
			this.fixed3 = new global::Gtk.Fixed();
			this.fixed3.HeightRequest = 10;
			this.fixed3.Name = "fixed3";
			this.fixed3.HasWindow = false;
			this.vboxTitle.Add(this.fixed3);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vboxTitle[this.fixed3]));
			w9.Position = 7;
			w9.Expand = false;
			w9.Fill = false;
			this.vbox4.Add(this.vboxTitle);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox4[this.vboxTitle]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox();
			this.vbox2.HeightRequest = 170;
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.MainItem = new global::client.leftMenuItemWidget();
			this.MainItem.HeightRequest = 40;
			this.MainItem.Events = ((global::Gdk.EventMask)(256));
			this.MainItem.Name = "MainItem";
			this.MainItem.PageIdx = 0;
			this.vbox2.Add(this.MainItem);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.MainItem]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.BookmarkItem = new global::client.leftMenuItemWidget();
			this.BookmarkItem.HeightRequest = 40;
			this.BookmarkItem.Events = ((global::Gdk.EventMask)(256));
			this.BookmarkItem.Name = "BookmarkItem";
			this.BookmarkItem.PageIdx = 0;
			this.vbox2.Add(this.BookmarkItem);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.BookmarkItem]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.SettingItem = new global::client.leftMenuItemWidget();
			this.SettingItem.HeightRequest = 40;
			this.SettingItem.Events = ((global::Gdk.EventMask)(256));
			this.SettingItem.Name = "SettingItem";
			this.SettingItem.PageIdx = 0;
			this.vbox2.Add(this.SettingItem);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.SettingItem]));
			w13.Position = 2;
			w13.Expand = false;
			w13.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.button14 = new global::Gtk.Button();
			this.button14.CanFocus = true;
			this.button14.Name = "button14";
			this.button14.UseUnderline = true;
			this.button14.Label = "POWER OFF";
			this.vbox2.Add(this.button14);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.button14]));
			w14.Position = 3;
			w14.Expand = false;
			w14.Fill = false;
			this.vbox4.Add(this.vbox2);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox4[this.vbox2]));
			w15.Position = 1;
			w15.Expand = false;
			w15.Fill = false;
			this.Add(this.vbox4);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.Hide();
			this.eventboxVersion.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler(this.OnEventboxVersionButtonPressEvent);
			this.button14.Clicked += new global::System.EventHandler(this.OnPowerOffButtonClicked);
		}
	}
}
