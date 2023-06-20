using System;
namespace client {
	public partial class InfoWindow : Gtk.Window {

		private const string VERSION = "Ver. 3.0";

		public InfoWindow (string ClientVersion = "3.0.0.0", string _BuildDate = "") :
				base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			this.Title = "Version Information";
			this.SetFlag (Gtk.WidgetFlags.Toplevel);
			this.ModifyBg (Gtk.StateType.Normal, new Gdk.Color (57, 58, 63));

			this.labelFileVersion.ModifyFg (Gtk.StateType.Normal, new Gdk.Color (215, 215, 215));
			this.labelDaaSXpertVersion.ModifyFg (Gtk.StateType.Normal, new Gdk.Color (215, 215, 215));
			this.labelDxClientVesion.ModifyFg (Gtk.StateType.Normal, new Gdk.Color (215, 215, 215));

			this.labelDaaSXpertVersion.LabelProp = "DaaSXpert " + VERSION;
			this.labelDxClientVesion.LabelProp = "dxClient " + ClientVersion;
			this.labelFileVersion.LabelProp = "File Version : " + ClientVersion;
			this.labelBuildDate.ModifyFg (Gtk.StateType.Normal, new Gdk.Color (215, 215, 215));

			if(_BuildDate != null && _BuildDate != "" && _BuildDate != "{builddate}") {
				this.labelBuildDate.LabelProp = "Build Date " + _BuildDate;
			} else {
				this.labelBuildDate.LabelProp = "";
			}
		}
	}
}
