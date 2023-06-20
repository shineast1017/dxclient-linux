using System;
using System.Drawing;
using System.Linq;
using Gtk;
using HCVK.HCVKSLibrary;
using HCVK.HCVKSLibrary.VO;
using Mono.Unix;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class serverlistWidget : Gtk.Bin
    {
		private Menu _contextMenu = null;

		public serverlistWidget()
        {
            this.Build();

			MainFunc.callbackEditServerItem = this.EditServerItem;
			MainFunc.callbackShowServerInfoDetail = this.ShowServerItemDetail;
			MainFunc.callbackExcuteBookmarkServer = this.ExcuteBookmarkServer;
			MainFunc.requestCheckServerState = this.ServerStatusCheck;

			// initialize viewer
			this.scrollServerlist.Hide();

			InitializeServerList();

			if(MainWindow.mainWindow.environment.vOCustomUIProperties.ShowContextMenuInServerItem == true) {
				this._contextMenu = new Menu ();
				MenuItem addServerItem = new MenuItem (Catalog.GetString ("Add Server"));
				addServerItem.ButtonPressEvent += new ButtonPressEventHandler (OnAddServerButtonPressed);
				_contextMenu.Add (addServerItem);
				_contextMenu.ShowAll ();

			}


			//style
			Gdk.Pixbuf ImageAddNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_add_enable.png");
            Gdk.Pixbuf ImageAddDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_add_disable.png");
            Gdk.Pixbuf ImageAddHover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_add_over.png");
            Gdk.Pixbuf ImageAddSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_add_push.png");

			Gdk.Pixmap pixmapNormal, pixmapDisable, pixmapHover, pixmapSelected, pix_mask;
			ImageAddNoraml.RenderPixmapAndMask(out pixmapNormal, out pix_mask, 255);
			ImageAddDisable.RenderPixmapAndMask(out pixmapDisable, out pix_mask, 255);
			ImageAddHover.RenderPixmapAndMask(out pixmapHover, out pix_mask, 255);
			ImageAddSelected.RenderPixmapAndMask(out pixmapSelected, out pix_mask, 255);

            //StyleSheet.SetStyleButton(this.btnAddServer);
			AfterEffectInitializeComponent();
		}



		private int nCnt = 0;
		public void InitializeServerList()
		{
			var brokerServerlist = MainFunc.GetBrokerServerAll ();

			for(int i = 0; i < brokerServerlist.Count; i++) {

				var broker = brokerServerlist [i];

				broker.ConfigIP = broker.BrokerServerIP;

				if (MainWindow.mainWindow.environment.vOGeneralsProperties.UseBrokerFilterTag == true) {
					string strVDI_Connect_MODE = MainWindow.mainWindow.GetVDIConnectMODE ();
					if (strVDI_Connect_MODE.Contains(broker.tag) == false)
						continue;
				} 

				serverItemWidget itemWidget = new serverItemWidget (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowContextMenuInServerItem);

				itemWidget.WidthRequest = 200;
				itemWidget.CanFocus = true;
				itemWidget.Name = string.Format ("serverItemWidget{0}", nCnt++);
				itemWidget.Events |= Gdk.EventMask.EnterNotifyMask | Gdk.EventMask.LeaveNotifyMask;
				this.vboxServerlist.Add (itemWidget);
				global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxServerlist [itemWidget]));
				w1.Position = this.vboxServerlist.Children.Length - 1;
				w1.Expand = false;

				itemWidget.Show ();
				itemWidget.BrokerServerInfo = broker;

			}
			nCnt = this.vboxServerlist.Children.Length;
			if (nCnt > 0) {
				ShowServerlist ();
				//[DXCT-361] broker 서버 수신 가능 상태 요청 
				ServerStatusCheck ();
			}

		}

		public void EditServerItem(int nEditMode, int nIdx, string strServerName, string strServerIP, string strServerPort)
		{
			if(!ValidationUtils.IsIPAddress(strServerIP) && nEditMode != 2)
			{
				ErrorHandlerManager.ErrorMessage(Catalog.GetString("Server IP가 유효하지 않습니다."));
				return;
			}
			if (!ValidationUtils.IsIPPort(strServerPort) && nEditMode != 2)
            {
				ErrorHandlerManager.ErrorMessage(Catalog.GetString("Server Port가 유효하지 않습니다."));
				return;
            }




			if(MainFunc.callbackShowServerListPage != null)
			    MainFunc.callbackShowServerListPage();

			VOBrokerServerNew brokerServer = new VOBrokerServerNew()
            {
				ConfigIP = strServerIP,
                BrokerServerDesc = strServerName,
                BrokerServerIP = strServerIP,
                BrokerServerPort = strServerPort,
                CreateTime = CommonUtils.DateTimeToUnixTime(DateTime.UtcNow),
				LastConnectedTime = 0, //CommonUtils.DateTimeToUnixTime(DateTime.MinValue),
                IsLastConnected = false,
                IsGateway = false,
            };

			var server = MainWindow.mainWindow.environment.BrokerServers.FirstOrDefault(s => s.BrokerServerIP == brokerServer.BrokerServerIP);

			if (server != null && server.ConfigIP != null && server.ConfigIP.Length > 0)
				server.BrokerServerIP = server.ConfigIP;


			if (server != null && nEditMode != 2)
            {
				nEditMode = 1;
				nIdx = MainWindow.mainWindow.environment.BrokerServers.IndexOf(server);
            }

			if (nEditMode == 0) // add
			{
				if (MainFunc.AddBrokerServer(brokerServer))
				{
					ShowServerlist();

					serverItemWidget itemWidget = new serverItemWidget(MainWindow.mainWindow.environment.vOCustomUIProperties.ShowContextMenuInServerItem);
					//DesktopInfoWidget itemWidget = new DesktopInfoWidget();

					itemWidget.WidthRequest = 200;
					itemWidget.CanFocus = true;
					itemWidget.Name = string.Format("serverItemWidget{0}", nCnt++);
					itemWidget.Events |= Gdk.EventMask.EnterNotifyMask | Gdk.EventMask.LeaveNotifyMask;
					this.vboxServerlist.Add(itemWidget);
					global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxServerlist[itemWidget]));
					w1.Position = this.vboxServerlist.Children.Length - 1;
					w1.Expand = false;

					itemWidget.Show();
					itemWidget.BrokerServerInfo = brokerServer;
				}
			}
			else if(nEditMode == 1) // edit
			{
				if(MainFunc.EditBrokerServer(nIdx, brokerServer))
				{
					serverItemWidget itemWidget = (serverItemWidget)this.vboxServerlist.Children[nIdx];
					itemWidget.RefreshInfo();
				}
			}
			else if(nEditMode == 2) //remove
			{
				int nRemoveIdx = MainFunc.RemoveBrokerServer(nIdx, brokerServer);
				if (nRemoveIdx >= 0 )
                {
					this.vboxServerlist.Remove(this.vboxServerlist.Children[nRemoveIdx]);
                }
			}
			MainWindow.mainWindow.SaveConfiguration();
			//[DXCT-361] broker 서버 수신 가능 상태 요청 
			ServerStatusCheck ();
		}

		public void ShowServerItemDetail(int nIdx)
		{
			this.vboxServerlist.Children.Where((x, index) => index != nIdx).ToList().ForEach(s => ((serverItemWidget)s).ShowServerItemDetail(false));
		}

		public void ShowServerlist()
		{
			if(MainWindow.mainWindow.environment.BrokerServers.Count == 0)
			{
                this.scrollServerlist.Hide();
				global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox5[this.btnAddServer]));
                w6.PackType = ((global::Gtk.PackType)(1));
                w6.Position = 1;
                w6.Expand = false;
                w6.Fill = false;
                w6.Padding = ((uint)(157));

            }
			else
			{
				this.scrollServerlist.Show();
				global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox5[this.btnAddServer]));
                w6.PackType = ((global::Gtk.PackType)(1));
                w6.Position = 1;
                w6.Expand = false;
                w6.Fill = false;
                w6.Padding = ((uint)(0));
            }
		}

		protected void OnbtnAddServerClicked(object sender, EventArgs e)
		{
			AddServer();
		}

        private void AddServer()
		{
			if (MainFunc.callbackShowEditServerPage != null)
                MainFunc.callbackShowEditServerPage(0, -1);
		}

		public bool ExcuteBookmarkServer(string strServerIP)
		{
			serverItemWidget widget = (serverItemWidget)this.vboxServerlist.Children.FirstOrDefault(server => ((serverItemWidget)server)._brokerServer.BrokerServerIP == strServerIP);
            if(widget == null)
			{
				//MessageDialog md = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
                //                                     MessageType.Info, ButtonsType.Ok, "Bookmark에 맞는 서버 정보가 존재하지 않습니다.");

                //ResponseType result = (ResponseType)md.Run();
                //md.Destroy();
				ErrorHandlerManager.ErrorMessage(Mono.Unix.Catalog.GetString(Catalog.GetString("Bookmark에 맞는 서버 정보가 존재하지 않습니다.")), this);
                return false;
			}
			if(!widget.IsLogin)
			{
				widget.LogIn();
				return false;
			}

			return true;
		}

		protected void OnScrollServerlistButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			if(this.vboxServerlist.Children != null)
			{
				var last = this.vboxServerlist.Children.Last();
				if (args.Event.X >= 0 && args.Event.X <= last.Allocation.Width
                   && args.Event.Y >= 0 && args.Event.Y <= last.Allocation.Height)
                {
                    return;
                }
			}
			if (args.Event.Button == 3)
			{
				if (this._contextMenu != null)
				{
					_contextMenu.Popup();
				}
			}
		}
		protected void OnAddServerButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
			AddServer();
        }
		public bool IsServerLogIn()
        {
			serverItemWidget widget = (serverItemWidget)this.vboxServerlist.Children.FirstOrDefault(s => ((serverItemWidget)s).IsLogin);

			if (widget == null)
				return false;
			ResponseType result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("기존 로그인 된 서버를 로그아웃 하시겠습니까?"));

            if (result == ResponseType.No)
            {
				return true;
            }

			widget.LogIn(true);
			return false;
        }

        // show up login click event
        public bool ShowLoginPage(int idx)
        {
            if (this.vboxServerlist.Children.Length < idx)
                return false;

            serverItemWidget widget =
             (serverItemWidget)this.vboxServerlist.Children[idx];

            if (widget == null)
                return false;



            widget.LogIn(true);
            return false;
        }

		public void ServerStatusCheck ()
		{
			var brokerServerlist = MainFunc.GetBrokerServerAll ();

			for (int i = 0; i < brokerServerlist.Count; i++) {
				serverItemWidget widget = (serverItemWidget)this.vboxServerlist.Children.FirstOrDefault (server => ((serverItemWidget)server)._brokerServer.BrokerServerIP == brokerServerlist[i].BrokerServerIP);
				if (widget != null) {
					widget.ServerStateRefresh_Async ();
				}

			}

		}


		// For Initialize Component AfterEffect Custom site
		private void AfterEffectInitializeComponent()
        {
            // hide ip address, port
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowAddServerBtnInServerPage == false)
            {
                this.btnAddServer.Hide();
            }


        }
    }
}
