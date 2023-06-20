using System;
using Gtk;
using HCVK.HCVKSLibrary;
using HCVK.HCVKSLibrary.VO;
using Mono.Unix;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class serverItemWidget : Gtk.Bin
    {
		public VOBrokerServerNew _brokerServer = null;
		private Menu _contextMenu = null;
		private MenuItem _logInItem = null;
		private MenuItem _logOutItem = null;


		public bool IsLogin { get; set; }

		public VOBrokerServerNew BrokerServerInfo
		{
			get { return this._brokerServer; }
			set
			{
				if(value != this._brokerServer)
				{
					this._brokerServer = value;
					RefreshInfo();
				}
			}
		}

		public serverItemWidget(bool bShowContextMenuEdit = true)
        {
			this.Build();

			this.fixedDetail.Hide();

			this.IsLogin = false;

			// Make ContextMenu
			if (this._contextMenu == null && bShowContextMenuEdit == true)
			{
				this._contextMenu = new Menu();
				_logInItem = new MenuItem (Catalog.GetString ("Log In"));
				_logInItem.ButtonPressEvent += new ButtonPressEventHandler (OnLogInItemButtonPressed);
				_logOutItem = new MenuItem (Catalog.GetString ("Log Out"));
				_logOutItem.ButtonPressEvent += new ButtonPressEventHandler (OnLogInItemButtonPressed);
				this._logOutItem.Hide ();

				MenuItem editItem = new MenuItem (Catalog.GetString ("Edit Server"));
				editItem.ButtonPressEvent += new ButtonPressEventHandler (OnEditItemButtonPressed);
				MenuItem removeItem = new MenuItem (Catalog.GetString ("Remove Server"));
				removeItem.ButtonPressEvent += new ButtonPressEventHandler (OnRemoveItemButtonPressed);

				this._contextMenu.Add(_logInItem);
				this._contextMenu.Add(_logOutItem);
				this._contextMenu.Add(new SeparatorMenuItem());
				this._contextMenu.Add (editItem);
				this._contextMenu.Add (removeItem);

				_contextMenu.ShowAll();

			}

			// background
            SetStyleNormal();
			StyleSheet.SetStyleButton(this.btnRemove);
			StyleSheet.SetStyleButton(this.btnEdit);
			StyleSheet.SetStyleButton(this.btnLogIn);

 			// for only custom site
           	AfterEffectInitializeComponent();
        }

		public void SetServerInfo()
		{
            this.btnLogIn.Click();
        }

		public void RefreshInfo()
		{
			if(this._brokerServer != null)
			{
				this.labelServerName.LabelProp = string.Format("<span size='15000'>{0}</span>", this._brokerServer.BrokerServerDesc);
				this.labelServerIP.LabelProp = this._brokerServer.BrokerServerIP;
				this.labelServerPort.LabelProp = this._brokerServer.BrokerServerPort;
				this.labelConnectedTimeValue.LabelProp = _brokerServer.LastConnectedTime == 0 ? "" : string.Format("<span size='9000'>{0}</span>", CommonUtils.UnixTimeStampToDateTime(_brokerServer.LastConnectedTime).ToLocalTime().ToString());
				this.labelServerState.LabelProp = string.Format("<span size='9000'>{0}</span>", this._brokerServer.IsLastConnected.ToString());
			}
		}

		public void ShowServerItemDetail(bool bShow)
		{
			if (bShow)
				this.fixedDetail.Show();
			else
			{
				this.fixedDetail.Hide();
				SetStyleNormal();
			}
		}

		protected void OnEventboxButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
		{
			// left button down
			if (args.Event.Button == 1)
			{
				if (this.fixedDetail.Visible)
				{
					SetStyleMouseOver();
					this.fixedDetail.Hide();
				}
				else
				{
					SetStyleSelect();
					this.fixedDetail.Show();
					if (MainFunc.callbackShowServerInfoDetail != null)
						MainFunc.callbackShowServerInfoDetail(MainFunc.GetBrokerServerIdx(this._brokerServer));
				}
			}
			// right button down
			else if (args.Event.Button == 3)
			{
				if(_contextMenu != null)
					this._contextMenu.Popup();
			}
		}

        /// <summary>
        /// Ons the button remove clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
		protected void OnBtnRemoveClicked(object sender, EventArgs e)
		{
			RemoveItem();
		}

        /// <summary>
        /// Ons the button edit clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
		protected void OnBtnEditClicked(object sender, EventArgs e)
		{
			EditItem();
		}

        /// <summary>
        /// Ons the button log in clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
		protected void OnBtnLogInClicked(object sender, EventArgs e)
		{
			LogIn();
		}

		public void SetLoginStatus()
		{
			this.IsLogin = true;
			this.btnLogIn.Label = Catalog.GetString("LOGOUT");
			StyleSheet.SetStyleButton(this.btnLogIn);
            
			if(_contextMenu != null) {
				this._logInItem.Hide ();
				this._logOutItem.Show ();
			}

			//Gtk.Application.Invoke(delegate {
			//	((Label)((MenuItem)_contextMenu.Children[0]).Child).Text = "Log Out";
			//});
		}

        protected void OnLogInItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
		{
			MainWindow.mainWindow._voBookmark = null;

			LogIn();
		}
		protected void OnEditItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
			EditItem();
        }

		protected void OnRemoveItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
		{
			RemoveItem();
        }

		public void LogIn(bool bForce = false)
		{
			if (this.IsLogin)
            {
				// logout
				//MessageDialog md = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
				//                                     MessageType.Question, ButtonsType.YesNo, "Logout?");

				//ResponseType result = (ResponseType)md.Run();
				//md.Destroy();
				ResponseType result;
				if (bForce)
					result = ResponseType.Yes;
				else
					result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("Logout 하시겠습니까?"));

                if (result == ResponseType.Yes)
                {
                    MainFunc.callbackChangeLoginStatus = null;
                    this.IsLogin = false;
					this.btnLogIn.Label = Mono.Unix.Catalog.GetString("LOGIN");
					this._brokerServer.IsLastConnected = false;
					StyleSheet.SetStyleButton(this.btnLogIn);
					//Gtk.Application.Invoke(delegate {
					//	((Label)((MenuItem)_contextMenu.Children[0]).Child).Text = "LOGIN";
					//});

					if(_contextMenu != null) {
						this._logInItem.Show ();
						this._logOutItem.Hide ();
					}

					RefreshInfo ();
                    MainWindow.mainWindow.ExcuteLogout();
                }
            }
            else
            {

                MainFunc.callbackChangeLoginStatus = this.SetLoginStatus;

                // if DBusLogin is set, try Autologin
                if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseDBUSLogIn == true)
                {
                    MainWindow.mainWindow.ExcuteLogin(this._brokerServer,"","");
                }


                // login
                if (MainFunc.callbackShowLoginPage != null)
                    MainFunc.callbackShowLoginPage(this._brokerServer);


            }
        }
		private void EditItem()
        {
			if (MainFunc.callbackShowEditServerPage != null)
                MainFunc.callbackShowEditServerPage(1, MainFunc.GetBrokerServerIdx(this._brokerServer));
            //if (MainFunc.callbackEditServerItem != null)
            //  MainFunc.callbackEditServerItem(1, -1, this._brokerServer.BrokerServerDesc, this._brokerServer.BrokerServerIP, this._brokerServer.BrokerServerPort);

        }
		private void RemoveItem()
        {
			//MessageDialog md = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
            //                                     MessageType.Question, ButtonsType.YesNo, "Delete?");

            //ResponseType result = (ResponseType)md.Run();
            //md.Destroy();
			ResponseType result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("서버 정보를 삭제하시겠습니까?"));
                
            if (result == ResponseType.Yes)
            {
                if (MainFunc.callbackEditServerItem != null)
                    MainFunc.callbackEditServerItem(2, -1, this._brokerServer.BrokerServerDesc, this._brokerServer.BrokerServerIP, this._brokerServer.BrokerServerPort);
            }
        }

		protected void OnEventboxEnterNotifyEvent(object o, EnterNotifyEventArgs args)
		{
			if (this.fixedDetail.Visible)
            {
                //SetStyleSelect();
            }
            else
				SetStyleMouseOver();
		}

		protected void OnEventboxLeaveNotifyEvent(object o, LeaveNotifyEventArgs args)
		{
			if (args.Event.X >= 0 && args.Event.X <= this.eventbox.Allocation.Width
               && args.Event.Y >= 0 && args.Event.Y <= this.eventbox.Allocation.Height)
            {
                return;
            }
			if (this.fixedDetail.Visible)
			{
				//SetStyleMouseOver();
			}
			else
				SetStyleNormal();
		}

		private void SetStyleNormal()
		{
			this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.ServerBGEnable);
			this.eventboxPort.ModifyBg(StateType.Normal, StyleSheet.SPortBGEnable);

			this.labelServerName.ModifyFg(StateType.Normal, StyleSheet.SNameFontEnable);
            this.labelServerIP.ModifyFg(StateType.Normal, StyleSheet.SIPFontEnable);
			this.labelServerPort.ModifyFg(StateType.Normal, StyleSheet.SPortFontEnable);

			//default
			this.labelConnectedTime.ModifyFg(StateType.Normal, StyleSheet.ServerLableColor);
			this.labelConnectedTimeValue.ModifyFg(StateType.Normal, StyleSheet.ValueColor);
			this.labelServerStatus.ModifyFg(StateType.Normal, StyleSheet.ServerLableColor);
			this.labelServerState.ModifyFg(StateType.Normal, StyleSheet.ValueColor);

			this.imageServer.Pixbuf = StyleSheet.ImageServerNoraml;
			this.imageArrow.Pixbuf = StyleSheet.ImageArrowNoraml;

		}
		private void SetStyleMouseOver()
        {
			this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.ServerBGHover);
			this.eventboxPort.ModifyBg(StateType.Normal, StyleSheet.SPortBGHover);

            this.labelServerName.ModifyFg(StateType.Normal, StyleSheet.SNameFontHover);
            this.labelServerIP.ModifyFg(StateType.Normal, StyleSheet.SIPFontHover);
            this.labelServerPort.ModifyFg(StateType.Normal, StyleSheet.SPortFontHover);

			this.imageServer.Pixbuf = StyleSheet.ImageServerHover;
			this.imageArrow.Pixbuf = StyleSheet.ImageArrowHover;
        }
		private void SetStyleSelect()
        {
			this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.ServerBGSelected);
			this.eventboxPort.ModifyBg(StateType.Normal, StyleSheet.SPortBGSelected);

            this.labelServerName.ModifyFg(StateType.Normal, StyleSheet.SNameFontSelected);
            this.labelServerIP.ModifyFg(StateType.Normal, StyleSheet.SIPFontSelected);
            this.labelServerPort.ModifyFg(StateType.Normal, StyleSheet.SPortFontSelected);
        
			this.imageServer.Pixbuf = StyleSheet.ImageServerSelected;
			this.imageArrow.Pixbuf = StyleSheet.ImageArrowSelected;
		}


		public async void ServerStateRefresh_Async ()
		{
			ServerStatusManager serverStatusManaer = new ServerStatusManager ();
			string strCode = await serverStatusManaer.GetServerStatusCode_Async (_brokerServer);

			ServerStateUpdate (strCode.Equals ("OK") ? "true" : "false");

		}


		private void ServerStateUpdate(string statusCode)
		{
			Gtk.Application.Invoke(delegate {
				this.labelServerState.LabelProp = string.Format ("<span size='9000'>{0}</span>", statusCode);
			});
		}


		// For Initialize Component AfterEffect Custom site
		private void AfterEffectInitializeComponent()
        {   

            // hide ip address, port
            if (MainWindow.mainWindow.environment.vOGeneralsProperties.SecureUiBroker == true)
            {
                this.labelServerIP.Hide();
                this.labelServerPort.Hide();
            }
            // hide Remove button
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowRemoveBtnInServerItem == false)
            {
                btnRemove.Hide();
            }
            // hide Edit button
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowEditBtnInServerItem == false)
            {
                btnEdit.Hide();
            }

            // adjust position and size 
             if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowRemoveBtnInServerItem == false ||
                MainWindow.mainWindow.environment.vOCustomUIProperties.ShowEditBtnInServerItem == false)
            {
                // ServerItem Width
                float fTotalWidthRequest = (float)this.fixedDefault.WidthRequest;
                float fDivisor = 3.0F;

                if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowRemoveBtnInServerItem == false)
                    fDivisor -= 1.0F;

                if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowEditBtnInServerItem == false)
                    fDivisor -= 1.0F;

                int nFinalWidthRequest = (int)(fTotalWidthRequest / fDivisor);
                int nTargetXpos = 0;
                if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowRemoveBtnInServerItem == true)
                  {
                    this.btnRemove.WidthRequest = nFinalWidthRequest;
                    global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.btnRemove]));
                    w1.X = nTargetXpos;
                    nTargetXpos += nFinalWidthRequest;
                    nTargetXpos -= 1;
                  }
                if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowEditBtnInServerItem == true)
                  {
                    this.btnEdit.WidthRequest = nFinalWidthRequest;
                    global::Gtk.Fixed.FixedChild w2 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.btnEdit]));
                    w2.X = nTargetXpos;
                    nTargetXpos += nFinalWidthRequest;
                    nTargetXpos -= 1;
                  }

                this.btnLogIn.WidthRequest = nFinalWidthRequest;
                global::Gtk.Fixed.FixedChild w3 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.btnLogIn]));
                w3.X = nTargetXpos;

            }



        }
	}
}
