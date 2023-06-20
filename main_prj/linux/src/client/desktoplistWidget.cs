using System;
using System.Linq;
using System.Collections.Generic;
using HCVK.HCVKSLibrary.VO;
using Gtk;
using client.Request;
using System.Net;
using log4net;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Mono.Unix;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class desktoplistWidget : Gtk.Bin
    {
		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private System.Timers.Timer _timerRefreshDesktopStateInfo = new System.Timers.Timer();
		private System.Timers.Timer _timerRefreshDesktopItemStateInfo = null;

		private string _StrRequestDesktopIDs = null;

		public desktoplistWidget()
        {
            this.Build();

			MainFunc.callbackExcuteBookmarkDesktop = this.ExcuteBookmarkDesktop;
			MainFunc.callbackChangeConnectStatus = this.ChangeDesktopConnect;
			MainFunc.callbackShowDesktopInfoDetail = this.ShowDesktopItemDetail;
			MainFunc.callbackRefreshDesktopInfo = this.RefreshDesktop;
			MainFunc.callbackSetDesktopInfo = this.StartRefreshDesktopItemStateInfoTimer;

			_timerRefreshDesktopStateInfo.Interval = 5000;
			_timerRefreshDesktopStateInfo.Elapsed += new System.Timers.ElapsedEventHandler (OnTimer_RefreshDesktopStateInfo);

			this.vboxDesktoplist.Show();
			//this.eventboxDesktoplist.ModifyBg(Gtk.StateType.Normal, new Gdk.Color(0, 0, 255));

			_StrRequestDesktopIDs = string.Empty;

		}

		private int nCnt = 0;
		public void InitializeDesktopList()
		{
			desktopItemWidget autoWidget = null;
			//Automation
            string strDesktopID = string.Empty;
            if (MainWindow.mainWindow._bRunningAutoStart)
            {
                strDesktopID = Convert.ToString(MainWindow.mainWindow.environment.vOAutoLoginProperties.PoolID);
            }
            else if (MainWindow.mainWindow._voBookmark != null)
                strDesktopID = MainWindow.mainWindow._voBookmark.DesktopID;

			// 생성한 DesktopItemWidget이 있다면 먼저 종료하기
			var DesktopList = this.vboxDesktoplist.Children.ToList ();
			Console.WriteLine ("Desktop List Count : " + DesktopList.Count.ToString ());

			for (int i = 0; i < DesktopList.Count; i++) {
				DesktopList [i].Destroy ();
				Console.WriteLine ("DesktopItemWidget Destroy(" + i.ToString () + ")");
			}

			this.vboxDesktoplist.Children.ToList().ForEach(s => this.vboxDesktoplist.Remove(s));

			MainFunc.GetDesktopListAll().ForEach(
                desktop =>
                {
    				desktopItemWidget itemWidget = new desktopItemWidget();

                    itemWidget.WidthRequest = 200;
                    itemWidget.CanFocus = true;
    			    itemWidget.Name = string.Format("desktopItemWidget{0}", nCnt++);
                    itemWidget.Events |= Gdk.EventMask.EnterNotifyMask | Gdk.EventMask.LeaveNotifyMask;
    				itemWidget.Show();
    			    this.vboxDesktoplist.Add(itemWidget);
    		    	
    			    global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxDesktoplist[itemWidget]));
    	    		w1.Position = this.vboxDesktoplist.Children.Length - 1;
                    w1.Expand = false;

					if (!string.IsNullOrEmpty(strDesktopID) && strDesktopID == desktop.PoolID)
						autoWidget = itemWidget;

	    		    itemWidget.SetDesktopPool(desktop, MainWindow.mainWindow.CheckAutoStart(desktop) == 2 ? true : false);
                });

			if (autoWidget != null)
            {
                autoWidget.Connect();
                if (MainWindow.mainWindow._voBookmark != null)
                {
                    MainWindow.mainWindow._voBookmark = null;
                }
            }
		}

		public void ExcuteBookmarkDesktop(string strDesktopID)
		{
			desktopItemWidget widget = (desktopItemWidget)this.vboxDesktoplist.Children.FirstOrDefault(
				desktop => ((desktopItemWidget)desktop)._desktopPoolEx.DesktopPool.PoolID == strDesktopID);

            if(widget == null)
			{
				//MessageDialog md = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
				//                                     MessageType.Info, ButtonsType.Ok, "Bookmark에 맞는 데스크탑 정보가 존재하지 않습니다.");
				ErrorHandlerManager.ErrorMessage(Catalog.GetString("Bookmark에 맞는 데스크탑 정보가 존재하지 않습니다."), this);

                //ResponseType result = (ResponseType)md.Run();
                //md.Destroy();
                return;
			}

			if(!widget._bConnect)
			{
                widget.Connect();
                if (MainWindow.mainWindow._voBookmark != null)
                {
                    MainWindow.mainWindow._voBookmark = null;
                }
            }
		}

		public void ChangeDesktopConnect(string strPoolID, bool bConnect)
		{
			desktopItemWidget widget = (desktopItemWidget)this.vboxDesktoplist.Children.FirstOrDefault(
				desktop => ((desktopItemWidget)desktop)._desktopPoolEx.DesktopPool.PoolID == strPoolID);

			if(widget != null)
			{
				widget.SetConnectStatus(bConnect);
			}
		}

		private void ShowDesktopItemDetail(string strWidgetName)
		{
			this.vboxDesktoplist.Children.Where(x => x.Name != strWidgetName).ToList().ForEach(s => ((desktopItemWidget)s).ShowDesktopItemDetail(false));
		}
		private void OnTimer_RefreshDesktopStateInfo(object sender, EventArgs e)
        {

            _timerRefreshDesktopStateInfo.Stop(); // Server 수정 이후 적용. 
												  // 'client/desktop/instance', 'client/pool'의 상태 정보가 다르게 옴

			this.vboxDesktoplist.Children.ToList().ForEach(s => GetDesktopConnectionInfo(((desktopItemWidget)s)._desktopPoolEx));
        }
		public void GetDesktopConnectionInfo(VODesktopPoolEx voDesktopPoolEx)
        {
            try
            {
                new RequestToHCVKB().RequestGetDesktopConnectionInfo_AsyncCallback(
					MainWindow.mainWindow._voSelectedBrokerServer, voDesktopPoolEx.DesktopPool, MainWindow.mainWindow._voUser, voDesktopPoolEx.Protocol, Callback_GetDesktopConnectionInfo);
            }
            catch (WebException wex)
            {
                _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        private void Callback_GetDesktopConnectionInfo(JObject resJsonObject, Exception exParam)
        {
            try
            {
                if (resJsonObject != null)
                {
                    // fetch and update auth token
					new RequestToHCVKB().UpdateAuthToken(MainWindow.mainWindow._voSelectedBrokerServer, resJsonObject);

                    if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                    {
                        // fetch desktop info
                        {
                            JObject oDesktop = JObject.Parse(resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA]["desktop"].ToString());
                            //_voDesktopPoolEx.DesktopPool.Desktop.DesktopID = oDesktop["desktopId"].ToString();
                            //_voDesktopPoolEx.DesktopPool.Desktop.InstanceID = oDesktop["instanceId"].ToString();
                            //_voDesktopPoolEx.DesktopPool.Desktop.CurrentState = oDesktop["desktopCurrentState"].ToString();
                            //_voDesktopPoolEx.DesktopPool.Desktop.Status = oDesktop["status"].ToString();
                            //_voDesktopPoolEx.DesktopPool.Desktop.PowerState = oDesktop["powerState"].ToString();
                            //_voDesktopPoolEx.DesktopPool.Desktop.VMState = oDesktop["vmState"].ToString();
                            //_voDesktopPoolEx.DesktopPool.Desktop.AgentState = oDesktop["agentState"].ToString();
                            //_voDesktopPoolEx.DesktopPool.Desktop.DesktopIP = oDesktop["ipAddress"].ToString();
                            VODesktop voDesktop = new VODesktop();
                            voDesktop.DesktopID = oDesktop["desktopId"].ToString();
                            voDesktop.InstanceID = oDesktop["instanceId"].ToString();
                            voDesktop.CurrentState = oDesktop["desktopCurrentState"].ToString();
                            voDesktop.Status = oDesktop["status"].ToString();
                            voDesktop.VMState = oDesktop["vmState"].ToString();
                            voDesktop.AgentState = oDesktop["agentState"].ToString();
                            voDesktop.DesktopIP = oDesktop["ipAddress"].ToString();

							this.vboxDesktoplist.Children.Where(
								desktop =>
								((desktopItemWidget)desktop)._desktopPoolEx.DesktopPool.Desktop.DesktopID == voDesktop.DesktopID).ToList().ForEach(
									s => ((desktopItemWidget)s).SetDesktopConnectionInfo(voDesktop));
                        }
                    }
                    else
                    {
                        // failure response
                        // HCVKCErrorHandler.ErrorHandler(resJsonObject[HCVKCRequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                        // ErrorHandlerManager.ErrorHandler(resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString(), this);
                    }
                }
                else
                {
                    throw exParam;
                }
            }
            catch (WebException wex)
            {
                _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));

                // log report
                //_hcvkcLogReport.LogReport_Desktop_ConnectionInfo(
                //    _voDesktopPool.Desktop.InstanceID, VOProtocol.VOProtocolType.PROTOCOL_RDP, VOLogData.STATUS_FAILED, VOErrorCode._E_CODE_C_C0000003);

                // HCVKCErrorHandler.ExceptionHandler(wex);
                // ErrorHandlerManager.ExceptionHandler(wex, this);                
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

                // log report
                //_hcvkcLogReport.LogReport_Desktop_ConnectionInfo(
                //    _voDesktopPool.Desktop.InstanceID, VOProtocol.VOProtocolType.PROTOCOL_RDP, VOLogData.STATUS_FAILED, VOErrorCode._E_CODE_C_C0000003);

                //HCVKCErrorHandler.ExceptionHandler(ex);
                // ErrorHandlerManager.ExceptionHandler(ex, this);

            }
        }
        
		private void RefreshDesktop(string DesktopPoolID)
		{
			this.vboxDesktoplist.Children.Where(
                desktop =>
				((desktopItemWidget)desktop)._desktopPoolEx.DesktopPool.Desktop.DesktopID == DesktopPoolID).ToList().ForEach(
					s => ((desktopItemWidget)s).RefreshInfo());
		}

		public void UpdateConnectStatus()
		{
			if(MainFunc.CallbackGetConnectedDesktopID != null) {
				string ConnectDesktopPoolID = MainFunc.CallbackGetConnectedDesktopID (); // no connected : string.empty

				if(ConnectDesktopPoolID != string.Empty) {
					ChangeDesktopConnect (ConnectDesktopPoolID, true);
				}
			}
		}

		private void RefreshDesktopStatusInfo (string strPoolID)
		{

			desktopItemWidget widget = (desktopItemWidget)this.vboxDesktoplist.Children.FirstOrDefault (
			desktop => ((desktopItemWidget)desktop)._desktopPoolEx.DesktopPool.PoolID == strPoolID);

			if(widget != null)
				GetDesktopStatusInfo (widget._desktopPoolEx);
		}


		public void GetDesktopStatusInfo (VODesktopPoolEx vODesktopPoolEx)
		{
			try {
				new RequestToHCVKB ().RequestGetDesktopStatusInfo_AsyncCallback (
					MainWindow.mainWindow._voSelectedBrokerServer, vODesktopPoolEx.DesktopPool, MainWindow.mainWindow._voUser, Callback_GetDeskopStatusInfo);
			} catch (WebException wex) {
				_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}

		}

		public void Callback_GetDeskopStatusInfo (JObject resJsonObject, Exception exParam)
		{
			try {

				if (resJsonObject != null) {
					if (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ().Equals ("0")) {
						// success response
						JObject oResult = JObject.Parse (resJsonObject [RequestJSONParam.RESPONSE_RESULT_DATA].ToString ());
						VODesktop voDeskTop = new VODesktop ();

						if (oResult.GetValue ("desktop") != null) {

							// fetch desktop info
							JObject oDesktop = JObject.Parse (resJsonObject [RequestJSONParam.RESPONSE_RESULT_DATA] ["desktop"].ToString ());

							voDeskTop.DesktopID = oDesktop ["desktopId"].ToString ();
							voDeskTop.InstanceID = oDesktop ["instanceId"].ToString ();
							voDeskTop.CurrentState = oDesktop ["desktopCurrentState"].ToString ();
							voDeskTop.Status = oDesktop ["status"].ToString ();
							voDeskTop.PowerState = oDesktop ["powerState"].ToString ();
							voDeskTop.VMState = oDesktop ["vmState"].ToString ();
							voDeskTop.AgentState = oDesktop ["agentState"].ToString ();
							voDeskTop.DesktopIP = oDesktop ["ipAddress"].ToString ();
							voDeskTop.IsEnabled = Convert.ToBoolean (oDesktop ["isEnabled"].ToString ());
							voDeskTop.Sessionconnected = oDesktop ["sessionConnected"].ToString ();
						} else {
							voDeskTop.DesktopID = oResult ["desktopId"].ToString ();
							voDeskTop.InstanceID = oResult ["instanceId"].ToString ();
							voDeskTop.CurrentState = oResult ["desktopCurrentState"].ToString ();
							voDeskTop.Status = oResult ["status"].ToString ();
							voDeskTop.PowerState = oResult ["powerState"].ToString ();
							voDeskTop.VMState = oResult ["vmState"].ToString ();
							voDeskTop.AgentState = oResult ["agentState"].ToString ();
							voDeskTop.DesktopIP = oResult ["ipAddress"].ToString ();
							voDeskTop.IsEnabled = Convert.ToBoolean (oResult ["isEnabled"].ToString ());

						}
						String strCurrentState = voDeskTop.CurrentState;

						if (voDeskTop.IsEnabled == false) {
							voDeskTop.CurrentState = "잠김";
						} else {

							if (strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_READY, StringComparison.CurrentCultureIgnoreCase)) //  ready
							{
								if (voDeskTop.AgentState.Equals (VODesktop.DESKTOP_AGENT_STATE_OK, StringComparison.CurrentCultureIgnoreCase)) {
									if (voDeskTop.Sessionconnected.Equals (VODesktop.DESKTOP_SESSION_CONNECTED_TRUE, StringComparison.CurrentCultureIgnoreCase)) {
										voDeskTop.CurrentState = VODesktop.DESKTOP_CURRENT_STATE_CONNECTED;
									} else {
										voDeskTop.CurrentState = VODesktop.DESKTOP_CURRENT_STATE_READY;
									}
								} else {
									voDeskTop.CurrentState = VODesktop.DESKTOP_CURRENT_STATE_READYANDAGENTOFF;
								}
							} else if (strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_CONNECTED, StringComparison.CurrentCultureIgnoreCase)) //  connected
							{
								voDeskTop.CurrentState = VODesktop.DESKTOP_CURRENT_STATE_CONNECTED;
							} else if (strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_PAUSED, StringComparison.CurrentCultureIgnoreCase)) //  PAUSED
							 {
								voDeskTop.CurrentState = VODesktop.DESKTOP_CURRENT_STATE_PAUSED;
							} else if (strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_SHUTOFF, StringComparison.CurrentCultureIgnoreCase)) //  SHUTOFF
							 {
								voDeskTop.CurrentState = VODesktop.DESKTOP_CURRENT_STATE_SHUTOFF;
							} else if (strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_TASKING, StringComparison.CurrentCultureIgnoreCase)) //  TASKING
							{
								voDeskTop.CurrentState = VODesktop.DESKTOP_CURRENT_STATE_TASKING;
							} else if (strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_POWERTASKING, StringComparison.CurrentCultureIgnoreCase)) //  POWERTASKING
							{
								voDeskTop.CurrentState = VODesktop.DESKTOP_CURRENT_STATE_POWERTASKING;
							} else if (strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_PROVISIONING, StringComparison.CurrentCultureIgnoreCase)) //  PROVISIONING
							{
								voDeskTop.CurrentState = VODesktop.DESKTOP_CURRENT_STATE_PROVISIONING;
							} else {
								voDeskTop.CurrentState = VODesktop.DESKTOP_CURRENT_STATE_UNKNOWN;
							}

						}




						//if (MainFunc.callbackSetDesktopInfo != null)
						//	MainFunc.callbackSetDesktopInfo (voDeskTop);
						Gtk.Application.Invoke (delegate {
							this.vboxDesktoplist.Children.Where (
							desktop =>
							((desktopItemWidget)desktop)._desktopPoolEx.DesktopPool.Desktop.DesktopID == voDeskTop.DesktopID).ToList ().ForEach (
							s => ((desktopItemWidget)s).SetDesktopConnectionInfo (voDeskTop));

						});

						strCurrentState = voDeskTop.CurrentState;

						if (strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_READY, StringComparison.CurrentCultureIgnoreCase) ||
							strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_CONNECTED, StringComparison.CurrentCultureIgnoreCase) ||
							strCurrentState.Equals (VODesktop.DESKTOP_CURRENT_STATE_SHUTOFF, StringComparison.CurrentCultureIgnoreCase)) {

							Console.WriteLine ("OnTimer_RefreshDesktopItemStateInfo() Stop " + strCurrentState);
							_timerRefreshDesktopItemStateInfo.Stop ();
							_StrRequestDesktopIDs = string.Empty;

						} else {
							Console.WriteLine ("OnTimer_RefreshDesktopItemStateInfo() ... CurrentState: " + strCurrentState);

						}


					} else {
						// error handler
						ErrorHandlerManager.ErrorHandler (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ());
					}
				} else {
					throw exParam;
				}
			} catch (WebException wex) {
				_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}
		}


		public void SetDesktopConnectionStatus(VODesktop _voDesktop)
		{

			this.vboxDesktoplist.Children.Where (
				desktop =>
				((desktopItemWidget)desktop)._desktopPoolEx.DesktopPool.Desktop.DesktopID == _voDesktop.DesktopID).ToList ().ForEach (
					s => ((desktopItemWidget)s).SetDesktopConnectionInfo (_voDesktop));
		}

		/// <summary>
		///  데스크탑 상태 표시 업데이트를 위한 타이머를 시작합니다.
		/// </summary>
		private void StartRefreshDesktopItemStateInfoTimer (string strRequestDesktopID)
		{
			Console.WriteLine ("StartRefreshDesktopItemStateInfoTimer() DesktopID: " + strRequestDesktopID);

			_StrRequestDesktopIDs = strRequestDesktopID;

			if (_timerRefreshDesktopItemStateInfo == null) {
				_timerRefreshDesktopItemStateInfo = new System.Timers.Timer ();
				_timerRefreshDesktopItemStateInfo.Interval = 5000; // 5초
				_timerRefreshDesktopItemStateInfo.Elapsed += new System.Timers.ElapsedEventHandler (OnTimer_RefreshDesktopItemStateInfo);
				_timerRefreshDesktopItemStateInfo.Start ();

			} else {

				if (_timerRefreshDesktopItemStateInfo.Enabled == false) {
					_timerRefreshDesktopItemStateInfo.Start ();
				}

			}
		}

		private void OnTimer_RefreshDesktopItemStateInfo (object sender, EventArgs e)
		{
			if(_StrRequestDesktopIDs != string.Empty) {
				RefreshDesktopStatusInfo (_StrRequestDesktopIDs);
			} else {
				_timerRefreshDesktopItemStateInfo.Stop ();
			}
		}
	}
}
