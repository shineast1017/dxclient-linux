
// This file has been generated by the GUI designer. Do not modify.
namespace client
{
	public partial class mainPageWidget
	{
		private global::Gtk.HBox hbox1;

		private global::Gtk.VBox vboxServerlist;

		private global::Gtk.VBox vboxDesktoplist;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget client.mainPageWidget
			global::Stetic.BinContainer.Attach(this);
			this.WidthRequest = 520;
			this.HeightRequest = 377;
			this.Name = "client.mainPageWidget";
			// Container child client.mainPageWidget.Gtk.Container+ContainerChild
			this.hbox1 = new global::Gtk.HBox();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vboxServerlist = new global::Gtk.VBox();
			this.vboxServerlist.WidthRequest = 256;
			this.vboxServerlist.HeightRequest = 377;
			this.vboxServerlist.Name = "vboxServerlist";
			this.vboxServerlist.Spacing = 6;
			this.hbox1.Add(this.vboxServerlist);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.vboxServerlist]));
			w1.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vboxDesktoplist = new global::Gtk.VBox();
			this.vboxDesktoplist.WidthRequest = 256;
			this.vboxDesktoplist.HeightRequest = 377;
			this.vboxDesktoplist.Name = "vboxDesktoplist";
			this.vboxDesktoplist.Spacing = 6;
			this.hbox1.Add(this.vboxDesktoplist);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.vboxDesktoplist]));
			w2.Position = 1;
			this.Add(this.hbox1);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.Hide();
		}
	}
}
