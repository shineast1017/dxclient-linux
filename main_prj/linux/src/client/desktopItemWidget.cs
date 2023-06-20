using System;
using Gtk;
using HCVK.HCVKSLibrary.VO;
using Mono.Unix;

namespace client
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class desktopItemWidget : Gtk.Bin
	{


		public VODesktopPoolEx _desktopPoolEx = null;

		private Menu _contextMenu = null;
		private MenuItem _connectItem = null;
		private MenuItem _disconnectItem = null;
		private MenuItem _powerOnItem = null;
		private MenuItem _powerOffItem = null;
		private MenuItem _addBookmarkItem = null;
		private MenuItem _autoStartItem = null;

		public bool _bConnect = false;
		private bool _bAutoStart = false;


        private Gdk.Pixbuf _desktopIconNormal = null;
        private Gdk.Pixbuf _desktopIconDisbable = null;
        private Gdk.Pixbuf _desktopIconHover = null;
        private Gdk.Pixbuf _desktopIconSelected = null;

		public desktopItemWidget()
		{
			this.Build();

			this.fixedDetail.Hide();

			this._desktopPoolEx = new VODesktopPoolEx();

            _desktopPoolEx.ResolutionIndex = MainWindow.mainWindow.environment.vODisplayProperties.Resolution;
            _desktopPoolEx.SpeedIndex = MainWindow.mainWindow.environment.vOPerformanceProperties.BandWidthQoS;
            _desktopPoolEx.Protocol = MainWindow.mainWindow.environment.vOGeneralsProperties.Protocol;
            _desktopPoolEx.Audio = MainWindow.mainWindow.environment.vOPerformanceProperties.IsAudio;

            // background
            SetStyleNormal();
			StyleSheet.SetStyleButton(this.btnProperty);
            StyleSheet.SetStyleButton(this.btnEdit);
			StyleSheet.SetStyleButton(this.btnConnect);
			
			AfterEffectInitializeComponent();

		}

		public void SetDesktopPool(VODesktopPool desktopPool, bool bAutoStart)
		{
			if (this._desktopPoolEx == null)
				this._desktopPoolEx = new VODesktopPoolEx();

			_desktopPoolEx.DesktopPool = desktopPool;
			_bAutoStart = bAutoStart;

			// protocol 
			if (string.IsNullOrEmpty(_desktopPoolEx.Protocol))
				_desktopPoolEx.Protocol = MainWindow.mainWindow.environment.vOGeneralsProperties.Protocol;

			// resolution
			//if (_desktopPoolEx.ResolutionIndex < 0)
			// always adjust resolution based on setting 
            _desktopPoolEx.ResolutionIndex = MainWindow.mainWindow.environment.vODisplayProperties.Resolution;

			// speed
			if (_desktopPoolEx.SpeedIndex < 0)
				_desktopPoolEx.SpeedIndex = MainWindow.mainWindow.environment.vOPerformanceProperties.BandWidthQoS;

			//Audio
			_desktopPoolEx.Audio = MainWindow.mainWindow.environment.vOPerformanceProperties.IsAudio;

            // desktop Icon
            if (_desktopPoolEx.DesktopPool.Desktop.Templates.Count > 0)
            {
                foreach (VODesktopTemplate voDesktopTemplate in _desktopPoolEx.DesktopPool.Desktop.Templates)
                {
                    if (voDesktopTemplate.OSCode.Equals("OS010232") == true || voDesktopTemplate.OSCode.Equals("OS010264") == true)
                    {
                        _desktopIconNormal = StyleSheet.ImageWin10Noraml;
                        _desktopIconDisbable = StyleSheet.ImageWin10Disable;
                        _desktopIconHover = StyleSheet.ImageWin10Hover;
                        _desktopIconSelected = StyleSheet.ImageWin10Selected;
                    }
                    else if (voDesktopTemplate.OSCode.Equals("OS020132") == true || voDesktopTemplate.OSCode.Equals("OS020132") == true)
                    {
                        _desktopIconNormal = StyleSheet.ImageUbuntuNoraml;
                        _desktopIconDisbable = StyleSheet.ImageUbuntuDisable;
                        _desktopIconHover = StyleSheet.ImageUbuntuHover;
                        _desktopIconSelected = StyleSheet.ImageUbuntuSelected;
                    }
                    else if (voDesktopTemplate.OSCode.Equals("OS020232") == true || voDesktopTemplate.OSCode.Equals("OS020264") == true)
                    {
                        _desktopIconNormal = StyleSheet.ImageCentOSNoraml;
                        _desktopIconDisbable = StyleSheet.ImageCentOSDisable;
                        _desktopIconHover = StyleSheet.ImageCentOSHover;
                        _desktopIconSelected = StyleSheet.ImageCentOSSelected;
                    }
                    else if (voDesktopTemplate.OSCode.Equals("OS020364") == true )
                    {
                        // TMAXOS  OSCODE OS020364
                        _desktopIconNormal = StyleSheet.ImageTmaxNoraml;
                        _desktopIconDisbable = StyleSheet.ImageTmaxDisable;
                        _desktopIconHover = StyleSheet.ImageTmaxHover;
                        _desktopIconSelected = StyleSheet.ImageTmaxSelected;
                    }
                    else if (voDesktopTemplate.OSCode.Equals("OS020464") == true)
                    { 
                        // HANCOM  OSCODE OS020464
                        _desktopIconNormal = StyleSheet.ImageHancomNoraml;
                        _desktopIconDisbable = StyleSheet.ImageHancomDisable;
                        _desktopIconHover = StyleSheet.ImageHancomHover;
                        _desktopIconSelected = StyleSheet.ImageHancomSelected;
                    }
                    else if (voDesktopTemplate.OSCode.Equals("OS020564") == true)
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
                }
            }
            else
            {
                _desktopIconNormal = StyleSheet.ImageWin7Noraml;
                _desktopIconDisbable = StyleSheet.ImageWin7Disable;
                _desktopIconHover = StyleSheet.ImageWin7Hover;
                _desktopIconSelected = StyleSheet.ImageWin7Selected;
            }

            this.imgDesktopIcon.Pixbuf = _desktopIconNormal;
            RefreshInfo();
		}

		public void RefreshInfo()
		{
			if (this._desktopPoolEx != null)
			{
				this.labelDesktopName.LabelProp = string.Format("<span size='15000'>{0}</span>", this._desktopPoolEx.DesktopPool.Desktop.DesktopName);
				this.labelDesktopIP.Text = this._desktopPoolEx.DesktopPool.Desktop.DesktopIP;

				this.labelConnectStateValue.LabelProp = string.Format("<span size='9000'>{0}</span>", this._desktopPoolEx.DesktopPool.Desktop.Status);
				this.labelPowerStateValue.LabelProp = string.Format("<span size='9000'>{0}</span>", this._desktopPoolEx.DesktopPool.Desktop.PowerStateText);
				this.labelDesktopStateValue.LabelProp = string.Format("<span size='9000'>{0}</span>", this._desktopPoolEx.DesktopPool.Desktop.CurrentState);
				this.labelAgentStateValue.LabelProp = string.Format("<span size='9000'>{0}</span>", this._desktopPoolEx.DesktopPool.Desktop.AgentStateText);

                                if (MainWindow.mainWindow.CheckAutoStart(this._desktopPoolEx.DesktopPool) != 2)
                                        _bAutoStart = false;
				


                                if (this._desktopPoolEx.DesktopPool.AccessDiv == Properties.Resources.TYPE_ACCESS_DIV_SHARED &&
                                                this._desktopPoolEx.DesktopPool.VmTotalCount.Length > 0)
                                {

                                        // this.labelConnectState.Visible = false;
                                        this.labelPowerState.Visible = false;
                                        this.labelDesktopState.Visible = false;
                                        this.labelAgentState.Visible = false;

                                        // this.labelConnectStateValue.Visible = false;
                                        this.labelPowerStateValue.Visible = false; 
                                        this.labelAgentStateValue.Visible = false;
                                        this.labelDesktopStateValue.Visible = false;

                                        // avaliable vm 
                                        // label text change
                                        this.labelConnectState.LabelProp = global::Mono.Unix.Catalog.GetString("<span size=\'9000\'>Desktop</span>");
                                        this.labelConnectStateValue.LabelProp = 
                                            string.Format("<span size='9000'>{0}</span>", this._desktopPoolEx.DesktopPool.VmAvailableCount);
                                        


                                } else {

                                }

				
				ShowIcon();

        // TODO : check Auto Login
        if (MainWindow.mainWindow._bRunDirectAutoConnect == true)
                {
					// Call Refresh ChangeStatus();
					MainFunc.callbackChangeLoginStatus ();
					MainWindow.mainWindow._bRunDirectAutoConnect = false;

					//check Connection status.. 
					//if (MainFunc.CallbackGetConnectedDesktopID == null) {
					if(MainWindow.mainWindow.GetConnectListCount() == 0) { 
						Connect ();
					}

				}


            }

			// DesktopItemWidget에 할당된 desktop이 현재 ready, connected, shutoff 상태가 아니라면, 상태 갱신 요청을 합니다. 
			string strCurrentState = this._desktopPoolEx.DesktopPool.Desktop.CurrentState;

			if (strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_READY, StringComparison.CurrentCultureIgnoreCase) == false &&
				strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_CONNECTED, StringComparison.CurrentCultureIgnoreCase) == false &&
				strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_SHUTOFF, StringComparison.CurrentCultureIgnoreCase) == false &&
				strCurrentState.Equals ("") == false ) {
				Console.WriteLine ("[RefreshInfo] Requeset Update Status Now: " + strCurrentState);
				StartRefreshDesktopItem ();
			}

		}
		public void SetDesktopConnectionInfo(VODesktop voDesktop)
        {
			this.labelConnectStateValue.LabelProp = string.Format ("<span size='9000'>{0}</span>", voDesktop.Status);
			this.labelPowerStateValue.LabelProp = string.Format ("<span size='9000'>{0}</span>", voDesktop.PowerStateText);
			this.labelDesktopStateValue.LabelProp = string.Format ("<span size='9000'>{0}</span>", voDesktop.CurrentState);
			this.labelAgentStateValue.LabelProp = string.Format ("<span size='9000'>{0}</span>", voDesktop.AgentStateText);

			//string strMsg;
			//strMsg = "SetDesktopConnectionInfo, PowerStateValue= " + this.labelPowerStateValue.Text;
			//System.Diagnostics.Debug.WriteLine(strMsg);
		}

		public void SetEditItem(VODesktopPoolEx vODesktopPoolEx)
		{
			this._desktopPoolEx = vODesktopPoolEx;
			RefreshInfo();

			MainWindow.mainWindow.ChangeDesktopName(vODesktopPoolEx.DesktopPool, this.labelDesktopName.Text);
		}

		private void ShowIcon()
		{
            int nXpos = 55;
            if(this._desktopPoolEx.DesktopPool.AccessDiv == Properties.Resources.TYPE_ACCESS_DIV_SHARED)
            {
				this.imgSharedIcon.Show();
                global::Gtk.Fixed.FixedChild w5 = ((global::Gtk.Fixed.FixedChild)(this.fixedDefault[this.imgSharedIcon]));
                w5.X = nXpos;
                w5.Y = 40;
                nXpos += 15;
            }
            else
            {
				this.imgSharedIcon.Hide();
            }
            
            if(_bAutoStart)
            {
                this.imgAutoStartIcon.Visible = true;
                global::Gtk.Fixed.FixedChild w5 = ((global::Gtk.Fixed.FixedChild)(this.fixedDefault[this.imgAutoStartIcon]));
                w5.X = nXpos;
                w5.Y = 40;
                nXpos += 15;
            }
            else
            {
                this.imgAutoStartIcon.Visible = false;
            }
            global::Gtk.Fixed.FixedChild w6 = ((global::Gtk.Fixed.FixedChild)(this.fixedDefault[this.labelDesktopIP]));
            w6.X = nXpos;
            w6.Y = 40;
		}

		public void ShowDesktopItemDetail(bool bShow)
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
					if (MainFunc.callbackShowDesktopInfoDetail != null)
						MainFunc.callbackShowDesktopInfoDetail(this.Name);
				}
			}
			else if (args.Event.Button == 3)
			{
				SetContextMenu();
				this._contextMenu.Popup();
			}
		}

		/// <summary>
		/// Ons the button property clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnPropertyClicked(object sender, EventArgs e)
		{
			ShowProperty();
		}

		/// <summary>
		/// Ons the button edit clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnEditClicked(object sender, EventArgs e)
		{
			Edit();
		}

		/// <summary>
		/// Ons the button connect clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnConnectClicked(object sender, EventArgs e)
		{
			Connect();
		}
		public void SetConnectStatus(bool bConnect)
		{
            _bConnect = bConnect;
			if (bConnect)
			{
				//this._bConnect = true;
				this.btnConnect.Label = Catalog.GetString("DISCONNECT");
				StyleSheet.SetStyleButton(this.btnConnect);
			}
			else
			{
				//this._bConnect = false;
				this.btnConnect.Label = Catalog.GetString("CONNECT");
				StyleSheet.SetStyleButton(this.btnConnect);
			}
		}

		private void SetContextMenu()
		{
			if (_contextMenu == null)
			{
				this._contextMenu = new Menu();


				_connectItem = new MenuItem(Catalog.GetString("Connect Desktop"));
				_connectItem.ButtonPressEvent += new ButtonPressEventHandler(OnConnectItemButtonPressed);
				_disconnectItem = new MenuItem(Catalog.GetString("Disconnect Desktop"));
				_disconnectItem.ButtonPressEvent += new ButtonPressEventHandler(OnConnectItemButtonPressed);
				MenuItem editItem = new MenuItem(Catalog.GetString("Edit Desktop Attributes"));
				editItem.ButtonPressEvent += new ButtonPressEventHandler(OnEditItemButtonPressed);
				MenuItem viewPolicyItem = new MenuItem(Catalog.GetString("View Desktop Properties"));
				viewPolicyItem.ButtonPressEvent += new ButtonPressEventHandler(OnViewPolicyItemButtonPressed);
				MenuItem refreshItem = new MenuItem(Catalog.GetString("Refresh Desktop Status"));
				refreshItem.ButtonPressEvent += new ButtonPressEventHandler(OnRefreshItemButtonPressed);


				MenuItem powerItem = new MenuItem(Catalog.GetString("Power Control"));
				Menu powerSubItem = new Menu();
				powerItem.Submenu = powerSubItem;

				_powerOnItem = new MenuItem(Catalog.GetString("Power On"));
				_powerOnItem.ButtonPressEvent += new ButtonPressEventHandler(OnPowerOnItemButtonPressed);
				_powerOffItem = new MenuItem(Catalog.GetString("Power Off"));
				_powerOffItem.ButtonPressEvent += new ButtonPressEventHandler(OnPowerOffItemButtonPressed);

                MenuItem systemResetItem = new MenuItem(Catalog.GetString("System Reset"));
                systemResetItem.ButtonPressEvent += new ButtonPressEventHandler(OnSystemResetItemButtonPressed);

                powerSubItem.Add(_powerOnItem);
                powerSubItem.Add(_powerOffItem);
                powerSubItem.Add(systemResetItem);


				MenuItem systemItem = new MenuItem(Catalog.GetString("System Control"));
				Menu systemSubItem = new Menu();
				systemItem.Submenu = systemSubItem;

				MenuItem systemShutDownItem = new MenuItem(Catalog.GetString("System Shutdown"));
				systemShutDownItem.ButtonPressEvent += new ButtonPressEventHandler(OnShutDownItemButtonPressed);
				MenuItem systemRebootItem = new MenuItem(Catalog.GetString("System Reboot"));
				systemRebootItem.ButtonPressEvent += new ButtonPressEventHandler(OnRebootItemButtonPressed);
				systemSubItem.Add(systemShutDownItem);
				systemSubItem.Add(systemRebootItem);

				_addBookmarkItem = new MenuItem(Catalog.GetString("Add to Bookmark"));
				_addBookmarkItem.ButtonPressEvent += new ButtonPressEventHandler(OnAddBookmarkItemButtonPressed);

				_autoStartItem = new MenuItem(Catalog.GetString("Set to Auto Start"));
				_autoStartItem.ButtonPressEvent += new ButtonPressEventHandler(OnSetAutoStartItemButtonPressed);

				this._contextMenu.Add(_connectItem);
				this._contextMenu.Add(_disconnectItem);
				this._contextMenu.Add(editItem);
				this._contextMenu.Add(viewPolicyItem);
				this._contextMenu.Add(refreshItem);
				this._contextMenu.Add(new SeparatorMenuItem());

				this._contextMenu.Add(powerItem);
				this._contextMenu.Add(systemItem);
				this._contextMenu.Add(_addBookmarkItem);
				this._contextMenu.Add(_autoStartItem);
				_contextMenu.ShowAll();
			}
			if (this._bConnect)
			{
				this._connectItem.Hide();
				this._disconnectItem.Show();
			}
			else
			{
				this._connectItem.Show();
				this._disconnectItem.Hide();
			}
			if (this.labelPowerStateValue.Text.Equals(VODesktop.POWER_DESKTOP_ON, StringComparison.CurrentCultureIgnoreCase))
			{
				this._powerOnItem.Hide();
				this._powerOffItem.Show();
			}
			else
			{
				this._powerOnItem.Show();
				this._powerOffItem.Hide();
			}

			if (MainWindow.mainWindow.environment.vOAutoLoginProperties.IsAutoLogin)
				this._autoStartItem.Show();
			else
				this._autoStartItem.Hide();

			if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseBookmark)
				this._addBookmarkItem.Show();
			else
				this._addBookmarkItem.Hide();
		}

		protected void OnConnectItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
		{
			Connect();
		}
		protected void OnEditItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
			Edit();
        }
		protected void OnViewPolicyItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
			ShowProperty();
        }
		protected void OnRefreshItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
			MainWindow.mainWindow.GetDesktopPoolInfo();
        }
		protected void OnPowerOnItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
            ResponseType result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("전원 켜기를 실행 하시겠습니까?"));

            if (result == ResponseType.Yes)
            {
				StartRefreshDesktopItem ();
                MainWindow.mainWindow.ExcutePowerControl(this._desktopPoolEx.DesktopPool, VODesktop.POWER_DESKTOP_ON);
            }
        }
		protected void OnPowerOffItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
            ResponseType result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("전원 끄기를 실행 하시겠습니까?"));

            if (result == ResponseType.Yes)
            {
				StartRefreshDesktopItem ();
                MainWindow.mainWindow.ExcutePowerControl(this._desktopPoolEx.DesktopPool, VODesktop.POWER_DESKTOP_OFF);
            }
        }
		protected void OnSystemResetItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
            ResponseType result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("전원 리셋을 실행 하시겠습니까?"));

            if (result == ResponseType.Yes)
            {
				StartRefreshDesktopItem ();
                MainWindow.mainWindow.ExcutePowerControl(this._desktopPoolEx.DesktopPool, VODesktop.POWER_DESKTOP_RESET);
            }
        }
		protected void OnShutDownItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
            ResponseType result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("시스템 종료를 실행 하시겠습니까?"));

            if (result == ResponseType.Yes)
            {
				StartRefreshDesktopItem ();
                MainWindow.mainWindow.ExcutePowerControl(this._desktopPoolEx.DesktopPool, VODesktop.POWER_GUEST_SHUTDOWN);
            }
        }
		protected void OnRebootItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
            ResponseType result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("시스템 리부팅을 실행 하시겠습니까?"));

            if (result == ResponseType.Yes)
            {
				StartRefreshDesktopItem ();
                MainWindow.mainWindow.ExcutePowerControl(this._desktopPoolEx.DesktopPool, VODesktop.POWER_GUEST_REBOOT);
            }
        }
		protected void OnAddBookmarkItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
			//MessageDialog md = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
			//                                     MessageType.Question, ButtonsType.YesNo, "Bookmark를 추가하시겠습니까?");

			ResponseType result = (ResponseType)ErrorHandlerManager.QuestionMessage(Catalog.GetString("Bookmark를 추가하시겠습니까?"));
            //md.Destroy();

            if (result == ResponseType.Yes)
            {
				MainWindow.mainWindow.AddBookmarkInfo(this._desktopPoolEx.DesktopPool);
            }
        }
		protected void OnSetAutoStartItemButtonPressed(object o, Gtk.ButtonPressEventArgs args)
        {
			if(MainWindow.mainWindow.SetAutoStartInfo(this._desktopPoolEx.DesktopPool))
			{
				_bAutoStart = true;
				ShowIcon();
			}
        }

		public void Connect()
		{
            if (!_bConnect)
                MainWindow.mainWindow.ConnectDesktop(this._desktopPoolEx);
            else
            {
                this._bConnect = true;
                this.btnConnect.Label = Catalog.GetString("CONNECT");
                StyleSheet.SetStyleButton(this.btnConnect);

                MainWindow.mainWindow.DisconnectDesktop(this._desktopPoolEx.DesktopPool);
            }
        }
        private void Edit()
		{
			MainFunc.callbackEditDesktopItem = this.SetEditItem;
            if (MainFunc.callbackShowEditDesktopPage != null)
                MainFunc.callbackShowEditDesktopPage(_desktopPoolEx);
		}

        private void ShowProperty()
		{
			if (MainFunc.callbackShowDesktopPolicyPage != null)
				MainFunc.callbackShowDesktopPolicyPage(this._desktopPoolEx.DesktopPool.Desktop.Policies);
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
            this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.DesktopBGEnable);

			this.labelDesktopName.ModifyFg(StateType.Normal, StyleSheet.DNameFontEnable);
			this.labelDesktopIP.ModifyFg(StateType.Normal, StyleSheet.DIPFontEnable);

            //default
			this.labelConnectState.ModifyFg(StateType.Normal, StyleSheet.DesktopLableColor);
			this.labelConnectStateValue.ModifyFg(StateType.Normal, StyleSheet.ValueColor);
			this.labelPowerState.ModifyFg(StateType.Normal, StyleSheet.DesktopLableColor);
			this.labelPowerStateValue.ModifyFg(StateType.Normal, StyleSheet.ValueColor);
			this.labelDesktopState.ModifyFg(StateType.Normal, StyleSheet.DesktopLableColor);
			this.labelDesktopStateValue.ModifyFg(StateType.Normal, StyleSheet.ValueColor);
			this.labelAgentState.ModifyFg(StateType.Normal, StyleSheet.DesktopLableColor);
			this.labelAgentStateValue.ModifyFg(StateType.Normal, StyleSheet.ValueColor);

			this.imgDesktopIcon.Pixbuf = this._desktopIconNormal;
            this.imageArrow.Pixbuf = StyleSheet.ImageArrowNoraml;

			this.imgSharedIcon.Pixbuf = StyleSheet.ImageSharedNoraml;
			this.imgAutoStartIcon.Pixbuf = StyleSheet.ImageAutoStartNoraml;

        }
        private void SetStyleMouseOver()
        {
            this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.DesktopBGHover);

			this.labelDesktopName.ModifyFg(StateType.Normal, StyleSheet.DNameFontHover);
			this.labelDesktopIP.ModifyFg(StateType.Normal, StyleSheet.DIPFontHover);

			this.imgDesktopIcon.Pixbuf = _desktopIconHover;
            this.imageArrow.Pixbuf = StyleSheet.ImageArrowHover;

			this.imgSharedIcon.Pixbuf = StyleSheet.ImageSharedHover;
			this.imgAutoStartIcon.Pixbuf = StyleSheet.ImageAutoStartHover;
		}
        private void SetStyleSelect()
        {
            this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.ServerBGSelected);

			this.labelDesktopName.ModifyFg(StateType.Normal, StyleSheet.DNameFontSelected);
			this.labelDesktopIP.ModifyFg(StateType.Normal, StyleSheet.DIPFontSelected);

			this.imgDesktopIcon.Pixbuf = _desktopIconSelected;
            this.imageArrow.Pixbuf = StyleSheet.ImageArrowSelected;

			this.imgSharedIcon.Pixbuf = StyleSheet.ImageSharedSelected;
			this.imgAutoStartIcon.Pixbuf = StyleSheet.ImageAutoStartSelected;
		}

		private void StartRefreshDesktopItem ()
		{
			if(MainFunc.callbackSetDesktopInfo != null)
				MainFunc.callbackSetDesktopInfo (_desktopPoolEx.DesktopPool.PoolID);

			this.labelDesktopStateValue.LabelProp = "<span size='9000'>tasking</span>";
		}


		// For Initialize Component AfterEffect Custom site
		private void AfterEffectInitializeComponent()
        {

          
            // hide ip address, port
            if (MainWindow.mainWindow.environment.vOGeneralsProperties.SecureUiBroker == true)
            {
                this.labelDesktopIP.Hide();
            }
            // hide Remove button
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowPolicyBtnInDesktopItem == false)
            {
                this.btnProperty.Hide();
            }
            // hide Edit button
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowEditBtnInDesktopItem == false)
            {
                this.btnEdit.Hide();
            }

            // adjust position and size 
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowPolicyBtnInDesktopItem == false ||
               MainWindow.mainWindow.environment.vOCustomUIProperties.ShowEditBtnInDesktopItem == false)
            {
                // ServerItem Width
                float fTotalWidthRequest = (float)this.fixedDefault.WidthRequest;
                float fDivisor = 3.0F;

                if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowPolicyBtnInDesktopItem == false)
                    fDivisor -= 1.0F;

                if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowEditBtnInDesktopItem == false)
                    fDivisor -= 1.0F;

                int nFinalWidthRequest = (int)(fTotalWidthRequest / fDivisor);
                int nTargetXpos = 0;
                if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowRemoveBtnInServerItem == true)
                {
                    this.btnProperty.WidthRequest = nFinalWidthRequest;
                    global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.btnProperty]));
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

                this.btnConnect.WidthRequest = nFinalWidthRequest;
                global::Gtk.Fixed.FixedChild w3 = ((global::Gtk.Fixed.FixedChild)(this.fixedDetail[this.btnConnect]));
                w3.X = nTargetXpos;


            }



        }
    }
}
