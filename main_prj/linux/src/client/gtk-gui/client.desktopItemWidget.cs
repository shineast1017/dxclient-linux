
// This file has been generated by the GUI designer. Do not modify.
namespace client
{
	public partial class desktopItemWidget
	{
		private global::Gtk.EventBox eventbox;

		private global::Gtk.VBox vbox10;

		private global::Gtk.Fixed fixedDefault;

		private global::Gtk.Image imgDesktopIcon;

		private global::Gtk.Label labelDesktopName;

		private global::Gtk.Label labelDesktopIP;

		private global::Gtk.Image imageArrow;

		private global::Gtk.Image imgSharedIcon;

		private global::Gtk.Image imgAutoStartIcon;

		private global::Gtk.Fixed fixedDetail;

		private global::Gtk.Label labelConnectState;

		private global::Gtk.Label labelDesktopState;

		private global::Gtk.Label labelDesktopStateValue;

		private global::Gtk.Button btnProperty;

		private global::Gtk.Button btnEdit;

		private global::Gtk.Button btnConnect;

		private global::Gtk.Label labelConnectStateValue;

		private global::Gtk.Label labelPowerState;

		private global::Gtk.Label labelAgentState;

		private global::Gtk.Label labelPowerStateValue;

		private global::Gtk.Label labelAgentStateValue;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget client.desktopItemWidget
			global::Stetic.BinContainer.Attach(this);
			this.WidthRequest = 200;
			this.Name = "client.desktopItemWidget";
			// Container child client.desktopItemWidget.Gtk.Container+ContainerChild
			this.eventbox = new global::Gtk.EventBox();
			this.eventbox.Name = "eventbox";
			// Container child eventbox.Gtk.Container+ContainerChild
			this.vbox10 = new global::Gtk.VBox();
			this.vbox10.Name = "vbox10";
			this.vbox10.Spacing = 6;
			// Container child vbox10.Gtk.Box+BoxChild
			this.fixedDefault = new global::Gtk.Fixed();
			this.fixedDefault.WidthRequest = 256;
			this.fixedDefault.HeightRequest = 70;
			this.fixedDefault.Name = "fixedDefault";
			this.fixedDefault.HasWindow = false;
			// Container child fixedDefault.Gtk.Fixed+FixedChild
			this.imgDesktopIcon = new global::Gtk.Image();
			this.imgDesktopIcon.Name = "imgDesktopIcon";
			this.imgDesktopIcon.Pixbuf = global::Gdk.Pixbuf.LoadFromResource("client.Resources.icon_broker_enable.png");
			this.fixedDefault.Add(this.imgDesktopIcon);
			global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(this.fixedDefault[this.imgDesktopIcon]));
			w1.X = 10;
			w1.Y = 20;
			// Container child fixedDefault.Gtk.Fixed+FixedChild
			this.labelDesktopName = new global::Gtk.Label();
			this.labelDesktopName.WidthRequest = 160;
			this.labelDesktopName.Name = "labelDesktopName";
			this.labelDesktopName.Xalign = 0F;
			this.labelDesktopName.LabelProp = global::Mono.Unix.Catalog.GetString("<span size=\'15000\'>big text</span>");
			this.labelDesktopName.UseMarkup = true;
			this.fixedDefault.Add(this.labelDesktopName);
			global::Gtk.Fixed.FixedChild w2 = ((global::Gtk.Fixed.FixedChild)(this.fixedDefault[this.labelDesktopName]));
			w2.X = 53;
			w2.Y = 16;
			// Container child fixedDefault.Gtk.Fixed+FixedChild
			this.labelDesktopIP = new global::Gtk.Label();
			this.labelDesktopIP.WidthRequest = 100;
			this.labelDesktopIP.Name = "labelDesktopIP";
			this.labelDesktopIP.Xalign = 0F;
			this.labelDesktopIP.LabelProp = global::Mono.Unix.Catalog.GetString("label3");
			this.fixedDefault.Add(this.labelDesktopIP);
			global::Gtk.Fixed.FixedChild w3 = ((global::Gtk.Fixed.FixedChild)(this.fixedDefault[this.labelDesktopIP]));
			w3.X = 55;
			w3.Y = 40;
			// Container child fixedDefault.Gtk.Fixed+FixedChild
			this.imageArrow = new global::Gtk.Image();
			this.imageArrow.Name = "imageArrow";
			this.imageArrow.Pixbuf = global::Gdk.Pixbuf.LoadFromResource("client.Resources.server_arrow_enable.png");
			this.fixedDefault.Add(this.imageArrow);
			global::Gtk.Fixed.FixedChild w4 = ((global::Gtk.Fixed.FixedChild)(this.fixedDefault[this.imageArrow]));
			w4.X = 227;
			w4.Y = 35;
			// Container child fixedDefault.Gtk.Fixed+FixedChild
			this.imgSharedIcon = new global::Gtk.Image();
			this.imgSharedIcon.WidthRequest = 11;
			this.imgSharedIcon.HeightRequest = 11;
			this.imgSharedIcon.Name = "imgSharedIcon";
			this.fixedDefault.Add(this.imgSharedIcon);
			global::Gtk.Fixed.FixedChild w5 = ((global::Gtk.Fixed.FixedChild)(this.fixedDefault[this.imgSharedIcon]));
			w5.X = 164;
			w5.Y = 53;
			// Container child fixedDefault.Gtk.Fixed+FixedChild
			this.imgAutoStartIcon = new global::Gtk.Image();
			this.imgAutoStartIcon.WidthRequest = 12;
			this.imgAutoStartIcon.HeightRequest = 12;
			this.imgAutoStartIcon.Name = "imgAutoStartIcon";
			this.fixedDefault.Add(this.imgAutoStartIcon);
			global::Gtk.Fixed.FixedChild w6 = ((global::Gtk.Fixed.FixedChild)(this.fixedDefault[this.imgAutoStartIcon]));
			w6.X = 221;
			w6.Y = 52;
			this.vbox10.Add(this.fixedDefault);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox10[this.fixedDefault]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox10.Gtk.Box+BoxChild
			this.fixedDetail = new global::Gtk.Fixed();
			this.fixedDetail.HeightRequest = 75;
			this.fixedDetail.Name = "fixedDetail";
			this.fixedDetail.HasWindow = false;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.labelConnectState = new global::Gtk.Label();
			this.labelConnectState.WidthRequest = 85;
			this.labelConnectState.HeightRequest = 14;
			this.labelConnectState.Name = "labelConnectState";
			this.labelConnectState.Xalign = 0F;
			this.labelConnectState.Yalign = 0F;
			this.labelConnectState.LabelProp = global::Mono.Unix.Catalog.GetString("<span size=\'9000\'>Status</span>");
			this.labelConnectState.UseMarkup = true;
			this.fixedDetail.Add(this.labelConnectState);
			global::Gtk.Fixed.FixedChild w8 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.labelConnectState]));
			w8.Y = 5;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.labelDesktopState = new global::Gtk.Label();
			this.labelDesktopState.WidthRequest = 60;
			this.labelDesktopState.HeightRequest = 14;
			this.labelDesktopState.Name = "labelDesktopState";
			this.labelDesktopState.Xalign = 0F;
			this.labelDesktopState.Yalign = 0F;
			this.labelDesktopState.LabelProp = global::Mono.Unix.Catalog.GetString("<span size=\'9000\'>Desktop</span>");
			this.labelDesktopState.UseMarkup = true;
			this.fixedDetail.Add(this.labelDesktopState);
			global::Gtk.Fixed.FixedChild w9 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.labelDesktopState]));
			w9.Y = 22;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.labelDesktopStateValue = new global::Gtk.Label();
			this.labelDesktopStateValue.Name = "labelDesktopStateValue";
			this.labelDesktopStateValue.Xalign = 0F;
			this.labelDesktopStateValue.Yalign = 0F;
			this.labelDesktopStateValue.LabelProp = global::Mono.Unix.Catalog.GetString("<span size=\'9000\'>Label</span>");
			this.labelDesktopStateValue.UseMarkup = true;
			this.fixedDetail.Add(this.labelDesktopStateValue);
			global::Gtk.Fixed.FixedChild w10 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.labelDesktopStateValue]));
			w10.X = 65;
			w10.Y = 22;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.btnProperty = new global::Gtk.Button();
			this.btnProperty.WidthRequest = 85;
			this.btnProperty.HeightRequest = 36;
			this.btnProperty.CanFocus = true;
			this.btnProperty.Name = "btnProperty";
			this.btnProperty.Label = global::Mono.Unix.Catalog.GetString("POLICY");
			this.fixedDetail.Add(this.btnProperty);
			global::Gtk.Fixed.FixedChild w11 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.btnProperty]));
			w11.Y = 40;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.btnEdit = new global::Gtk.Button();
			this.btnEdit.WidthRequest = 85;
			this.btnEdit.HeightRequest = 36;
			this.btnEdit.CanFocus = true;
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Label = global::Mono.Unix.Catalog.GetString("EDIT");
			this.fixedDetail.Add(this.btnEdit);
			global::Gtk.Fixed.FixedChild w12 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.btnEdit]));
			w12.X = 84;
			w12.Y = 40;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.btnConnect = new global::Gtk.Button();
			this.btnConnect.WidthRequest = 85;
			this.btnConnect.HeightRequest = 36;
			this.btnConnect.CanFocus = true;
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Label = global::Mono.Unix.Catalog.GetString("CONNECT");
			this.fixedDetail.Add(this.btnConnect);
			global::Gtk.Fixed.FixedChild w13 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.btnConnect]));
			w13.X = 169;
			w13.Y = 40;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.labelConnectStateValue = new global::Gtk.Label();
			this.labelConnectStateValue.Name = "labelConnectStateValue";
			this.labelConnectStateValue.Xalign = 0F;
			this.labelConnectStateValue.Yalign = 0F;
			this.labelConnectStateValue.LabelProp = global::Mono.Unix.Catalog.GetString("<span size=\'9000\'>Label</span>");
			this.labelConnectStateValue.UseMarkup = true;
			this.fixedDetail.Add(this.labelConnectStateValue);
			global::Gtk.Fixed.FixedChild w14 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.labelConnectStateValue]));
			w14.X = 65;
			w14.Y = 5;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.labelPowerState = new global::Gtk.Label();
			this.labelPowerState.WidthRequest = 50;
			this.labelPowerState.HeightRequest = 14;
			this.labelPowerState.Name = "labelPowerState";
			this.labelPowerState.Xalign = 0F;
			this.labelPowerState.Yalign = 0F;
			this.labelPowerState.LabelProp = global::Mono.Unix.Catalog.GetString("<span size=\'9000\'>Power</span>");
			this.labelPowerState.UseMarkup = true;
			this.fixedDetail.Add(this.labelPowerState);
			global::Gtk.Fixed.FixedChild w15 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.labelPowerState]));
			w15.X = 130;
			w15.Y = 5;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.labelAgentState = new global::Gtk.Label();
			this.labelAgentState.WidthRequest = 60;
			this.labelAgentState.HeightRequest = 14;
			this.labelAgentState.Name = "labelAgentState";
			this.labelAgentState.Xalign = 0F;
			this.labelAgentState.Yalign = 0F;
			this.labelAgentState.LabelProp = global::Mono.Unix.Catalog.GetString("<span size=\'9000\'>Agent</span>");
			this.labelAgentState.UseMarkup = true;
			this.fixedDetail.Add(this.labelAgentState);
			global::Gtk.Fixed.FixedChild w16 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.labelAgentState]));
			w16.X = 130;
			w16.Y = 22;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.labelPowerStateValue = new global::Gtk.Label();
			this.labelPowerStateValue.Name = "labelPowerStateValue";
			this.labelPowerStateValue.Xalign = 0F;
			this.labelPowerStateValue.Yalign = 0F;
			this.labelPowerStateValue.LabelProp = global::Mono.Unix.Catalog.GetString("<span size=\'9000\'>Label</span>");
			this.labelPowerStateValue.UseMarkup = true;
			this.fixedDetail.Add(this.labelPowerStateValue);
			global::Gtk.Fixed.FixedChild w17 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.labelPowerStateValue]));
			w17.X = 180;
			w17.Y = 5;
			// Container child fixedDetail.Gtk.Fixed+FixedChild
			this.labelAgentStateValue = new global::Gtk.Label();
			this.labelAgentStateValue.Name = "labelAgentStateValue";
			this.labelAgentStateValue.Xalign = 0F;
			this.labelAgentStateValue.Yalign = 0F;
			this.labelAgentStateValue.LabelProp = global::Mono.Unix.Catalog.GetString("<span size=\'9000\'>Label</span>");
			this.labelAgentStateValue.UseMarkup = true;
			this.fixedDetail.Add(this.labelAgentStateValue);
			global::Gtk.Fixed.FixedChild w18 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.labelAgentStateValue]));
			w18.X = 180;
			w18.Y = 22;
			this.vbox10.Add(this.fixedDetail);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox10[this.fixedDetail]));
			w19.Position = 1;
			w19.Expand = false;
			w19.Fill = false;
			this.eventbox.Add(this.vbox10);
			this.Add(this.eventbox);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.imgSharedIcon.Hide();
			this.imgAutoStartIcon.Hide();
			this.Hide();
			this.eventbox.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler(this.OnEventboxButtonPressEvent);
			this.eventbox.EnterNotifyEvent += new global::Gtk.EnterNotifyEventHandler(this.OnEventboxEnterNotifyEvent);
			this.eventbox.LeaveNotifyEvent += new global::Gtk.LeaveNotifyEventHandler(this.OnEventboxLeaveNotifyEvent);
			this.btnProperty.Clicked += new global::System.EventHandler(this.OnBtnPropertyClicked);
			this.btnEdit.Clicked += new global::System.EventHandler(this.OnBtnEditClicked);
			this.btnConnect.Clicked += new global::System.EventHandler(this.OnBtnConnectClicked);
		}
	}
}
