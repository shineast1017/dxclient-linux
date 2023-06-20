using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using client.Request;
using Gdk;
using HCVK.HCVKSLibrary;
using HCVK.HCVKSLibrary.VO;
using log4net;
using Newtonsoft.Json.Linq;

namespace client
{
    public partial class ConnectProcess : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // declare for Tag
        private object _oTag;
        public object Tag
        {
            set { _oTag = value; }
            get { return _oTag; }
        }

        // declare for VOs
        public VOUser _voUser = new VOUser();
        public VODesktopPoolEx _voDesktopPoolEx = new VODesktopPoolEx();
        public VOBrokerServerNew _voBrokerServer = new VOBrokerServerNew();
        public string _strServiceProtocol = VOProtocol.VOProtocolType.PROTOCOL_SPICE;

        // declare for HCVKA Info
        private string _strConnectionInfo = string.Empty;
        private string _strAgentServiceIP = string.Empty;
        private string _strAgentServicePort = "4001";
        private string _strAgentVDIIP = string.Empty;
        private string _strAgentVDIPort = string.Empty;
        private string _strTitle = String.Empty;

        // declare for HCVKB
        private RequestToHCVKB _requestDesktopInfo = new RequestToHCVKB();

        // declare HCVKCHeartbeat Timer
        private RequestToHCVKA _reqHeatbeat = new RequestToHCVKA();
        private System.Timers.Timer _timerCheckHCVKCHeartbeat = null;
        private const int CHECK_INTERVAL_HCVKCHEARTBEAT = 10000;    // 10 sec.

		private bool _bIsSkipNotifyHeartbeatError = false;
        private bool _bIsHeartbeatError = false;
				private int _nNotifyHeartbeatErrorCnt = 0; // Error 상태를 특정값(예,10) 까지 기다린후, 에러 다이얼로그를 발생합니다. 

        // declare for retry times
        private const int MAX_RETRY_TIMES = 5;
        private int _nCurrentRetryTimes = 0;
        private object oLock = new object();
        private bool _bIsFirstTime = true;


        private Process _connectProcess = null;
        private bool disposed = false;
        //private static AutoResetEvent _autoEvent = new AutoResetEvent(false);
        //private static int MAX_TIME_OUT = 1000 * 10;    // 10 sec

        // declare for log report
        public LogReport _logReport = new LogReport();

        // declare for HCVKCMain
        // public HCVKCMain _hcvkcMain = null;
        private bool _bIsConnectedSuccess = false;

        // declare for popup message box
        // private HCVKCPopupMsgBox _popupMsgBox = null;
        private bool bIsPopupMessageReconnection = false;

        // ErrorHandler
        //private ErrorHandlerManager _errorHandlerManager = new ErrorHandlerManager();
        private string protocolIP = null;
        private string _strAgentPort = "443";
        private string _strSessionPort = "4172";

        private bool bIsRequestRestart = false;

		//declare Timer for attach command (5 sec. Delay)
		private System.Timers.Timer _timerDelayAttchMessageToAgent = null;
		private const int TIMER_INTERVAL_DELAYMESSAGE = 5000;    // 5 sec.

		public ConnectProcess ()
        {
            _logger.Debug(string.Format("Start --------------"));
            disposed = false;
            //InitializeSpice();
            InitializeHeartbeatTimer();
			MainFunc.CallbackGetConnectedDesktopID = this.GetConnectedDesktopPoolID;
		}

        ~ConnectProcess()
        {
            Dispose();
			_logger.Debug(string.Format("End --------------"));
        }

        public virtual void Dispose()
        {
            if (!disposed)
            {
				KillProcess();
				MainFunc.CallbackGetConnectedDesktopID = null;
                FinalizeHeartbeatTimer();
				StopDelayMessageTimer ();
				//FinalizeSpice();

				if (!_bIsConnectedSuccess)
                {
                    // _hcvkcMain?.SetAutomationState(VOAutomation.AutomationStateEnum.CONNECTION_FAILED);
                }
                // _hcvkcMain?.GetDesktopPoolInfo();
            }

            disposed = true;
            //GC.SuppressFinalize(this);
        }

        public bool GetDisposedState()
        {
            return disposed;
        }


        // ----------------------------------------------------------------------------
        // Heartbeat handling
        private void InitializeHeartbeatTimer()
        {
            try
            {
                _timerCheckHCVKCHeartbeat = new System.Timers.Timer();
                _timerCheckHCVKCHeartbeat.Interval = CHECK_INTERVAL_HCVKCHEARTBEAT;
                _timerCheckHCVKCHeartbeat.Elapsed += new System.Timers.ElapsedEventHandler(timerCheckHCVKCHeartbeat_Tick);
			}
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }




		private void StartHeartbeatTimer()
        {

            if (_reqHeatbeat != null) _reqHeatbeat = new RequestToHCVKA();

            if (_timerCheckHCVKCHeartbeat != null)
            {
                _logger.Debug(string.Format("Start Heartbeat ==============>"));
                _timerCheckHCVKCHeartbeat.Start();
				_nNotifyHeartbeatErrorCnt = 0;
			}

        }

        private void StopHeartbeatTimer()
        {

            if (_reqHeatbeat != null)
                _reqHeatbeat.StopRequest();

            if (_timerCheckHCVKCHeartbeat != null && _timerCheckHCVKCHeartbeat.Enabled)
            {
                _logger.Debug(string.Format("Stop Heartbeat <==============="));
                _timerCheckHCVKCHeartbeat.Stop();
				_nNotifyHeartbeatErrorCnt = 0;
			}

        }

        private void FinalizeHeartbeatTimer()
        {
            try
            {
                StopHeartbeatTimer();

                _timerCheckHCVKCHeartbeat = null;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


		private void StartDelayMessageTimer ()
		{
			if (_timerDelayAttchMessageToAgent == null) {
				_timerDelayAttchMessageToAgent = new System.Timers.Timer ();
				_timerDelayAttchMessageToAgent.Interval = TIMER_INTERVAL_DELAYMESSAGE;
				_timerDelayAttchMessageToAgent.Elapsed += new System.Timers.ElapsedEventHandler (timerDelayMessage_Tick);
			} else if (_timerDelayAttchMessageToAgent.Enabled == true) {
				_timerDelayAttchMessageToAgent.Stop ();
			}
			_timerDelayAttchMessageToAgent.Start ();
		}

		private void StopDelayMessageTimer ()
		{
			if (_timerDelayAttchMessageToAgent == null)
				return;

			if (_timerDelayAttchMessageToAgent.Enabled) {
				_timerDelayAttchMessageToAgent.Stop ();
			}
			_timerDelayAttchMessageToAgent.Dispose ();
			_timerDelayAttchMessageToAgent = null;
		}
		// Event handler for DelayMessage timer
		private void timerDelayMessage_Tick (object sender, EventArgs e)
		{
			string strWebCamDevId = MainWindow.mainWindow.GetWebCamDevicdId ();
			string strWebCamBusId = CommonUtils.GetWebCamDeviceBusId (strWebCamDevId, MainWindow.mainWindow.environment.vOUSBDeviceProperties.UsbIpModulePath);
			string strWebCamPort = MainWindow.mainWindow.environment.vOUSBDeviceProperties.DeviceRedirectionPort;

			if (strWebCamBusId.Contains ("Error") == true) {
				ErrorHandlerManager.ErrorHandler (VOErrorCode._E_CODE_C_0000012);
				_logger.Error (string.Format ("[WebCamBusId] {0}", strWebCamBusId));
			} else {

				if (strWebCamDevId != "") {
					if (MainWindow.mainWindow.environment.vOUSBDeviceProperties.IsUseFileIOWebCamCtrl == true ||
							 MainWindow.mainWindow.environment.vOUSBDeviceProperties.IsUseAPIWebCamCtrl == true) {
						if (MainWindow.mainWindow.strCurDaaS_USB_REDIRECT.Contains ("1")) {
							RequestRedirectionAttachMode (strWebCamBusId, strWebCamPort);
						} else {
							_logger.Info (string.Format ("[ConnectDesktopView] No Attach Device"));
						}
					} else {
						// Request 'usbip bind' instead of FileIO or API 
						MainWindow.mainWindow.CtrlDeviceRedirection ("bind", strWebCamDevId);
					}
				} else {
					ErrorHandlerManager.ErrorHandler (VOErrorCode._E_CODE_C_0000011);
					_logger.Debug (string.Format ("[ConnectDesktopView] No Webcam Dev Id"));
				}
			}
			StopDelayMessageTimer ();

		}
		// Event handler for heartbeat timer
		private void timerCheckHCVKCHeartbeat_Tick(object sender, EventArgs e)
        {
            try
            {

				if (_reqHeatbeat != null)
                    _reqHeatbeat.RequestHeartbeat_AsyncCallback(_strAgentServiceIP, _strAgentServicePort, _voDesktopPoolEx.DesktopPool, _voUser, Callback_Heartbeat);
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

        private void Callback_Heartbeat(JObject resJsonObject, Exception exParam)
        {
            _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

            try
            {
                if (resJsonObject != null)
                {
                    _bIsSkipNotifyHeartbeatError = false;
                    _bIsHeartbeatError = false;

                    if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                    {
                        // by pass..
                        bIsPopupMessageReconnection = false;
						_nNotifyHeartbeatErrorCnt = 0;
					}
                    else
                    {
                        // failure response
                        string resultCode = resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString();

                        switch (resultCode)
                        {
                            case VOErrorCode._E_CODE_A_A0000001:    // skip error message as "Not ready to service" after reboot desktop.
                            case VOErrorCode._E_CODE_G_G0009001:    // skip error message as "Request timeout with hcVKA" after reboot desktop.
                                if (!bIsPopupMessageReconnection)
                                {
                                    bIsPopupMessageReconnection = true;
                                    // ShowPopupMessage(MultiLang.HCVKC_MAIN_MSG_0120, 30);
                                }
                                break;

                            default:
                                // HCVKCErrorHandler.ErrorHandler(resJsonObject[HCVKCRequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                                ErrorHandlerManager.ErrorHandler(resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                                break;
                        }
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

                if (_bIsSkipNotifyHeartbeatError)
                {
                    // by pass..
                }
                else
                {
                    if (wex.Status == WebExceptionStatus.ConnectFailure/* 0x80131509 */)
                    {
                    }
                }


                if (!_bIsSkipNotifyHeartbeatError && _nNotifyHeartbeatErrorCnt++ > MainWindow.mainWindow.environment.vOGeneralsProperties.AgentMsgSkipCnt)
                {
                    // HCVKCErrorHandler.ExceptionHandler(wex);
                    ErrorHandlerManager.ExceptionHandler(wex,null,"[HeartbeatW]");

                    // terminating a viewer process
                    if (_connectProcess != null && !_connectProcess.HasExited)
                    {
                        _bIsHeartbeatError = true;
                        _connectProcess.CloseMainWindow();
                    }
				} else {
					Console.WriteLine ("Skip Heartbeart Error cnt: " + _nNotifyHeartbeatErrorCnt) ;
				}
			}
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
                // HCVKCErrorHandler.ExceptionHandler(ex);

				if(_nNotifyHeartbeatErrorCnt++ > MainWindow.mainWindow.environment.vOGeneralsProperties.AgentMsgSkipCnt) {
					ErrorHandlerManager.ExceptionHandler (ex, null, "[HeartbeatE]");

					// terminating a viewer process
					if (_connectProcess != null && !_connectProcess.HasExited) {
						_bIsHeartbeatError = true;
						_connectProcess.CloseMainWindow ();
					}
				} else {
					Console.WriteLine ("Skip Heartbeart Error cnt: " + _nNotifyHeartbeatErrorCnt);
				}
			}
        }
        // ----------------------------------------------------------------------------


        // ----------------------------------------------------------------------------
        // Retry times handling
        private void ResetRetryTimes()
        {
            lock (oLock)
            {
                _nCurrentRetryTimes = 0;
            }
        }
        private void IncreamentRetryTimes()
        {
            lock (oLock)
            {
                _nCurrentRetryTimes++;
            }
        }
        private bool IsRetryTimeout()
        {
            bool bReturn = false;

            lock (oLock)
            {
                bReturn = MAX_RETRY_TIMES < _nCurrentRetryTimes;
            }

            return bReturn;
        }
        // ----------------------------------------------------------------------------


        private bool IsValidServiceProtocolNetwork()
        {
            bool bIsValidServiceProtocolNetwork = false;
            foreach (VOProtocol voProtocol in _voDesktopPoolEx.DesktopPool.Desktop.Protocols)
            {
                if (voProtocol.ProtocolType.Equals(_strServiceProtocol))
                {
                    bIsValidServiceProtocolNetwork = !string.IsNullOrEmpty(voProtocol.ProtocolIP);
                    break;
                }
            }
            return bIsValidServiceProtocolNetwork;
        }

        private bool IsReadyForConnectToDesktop()
        {
            bool bIsNetwork = !string.IsNullOrEmpty(_voDesktopPoolEx.DesktopPool.Desktop.DesktopIP);
            bool bIsValidServiceProtocolNetwork = IsValidServiceProtocolNetwork();
            bool bIsOKStatus = _voDesktopPoolEx.DesktopPool.Desktop.Status.Equals(VODesktop.STATUS_ACTIVE, StringComparison.CurrentCultureIgnoreCase);
            bool bIsOKCurrentState = _voDesktopPoolEx.DesktopPool.Desktop.CurrentState.Equals(VODesktop.DESKTOP_CURRENT_STATE_READY, StringComparison.CurrentCultureIgnoreCase);
            bool bIsOKAgentState = _voDesktopPoolEx.DesktopPool.Desktop.AgentState.Equals(VOErrorCode._E_CODE_OK, StringComparison.CurrentCultureIgnoreCase);
            bool bIsServiceReadyOK = bIsNetwork && bIsValidServiceProtocolNetwork && bIsOKStatus && bIsOKCurrentState && bIsOKAgentState & !_bIsFirstTime;

            _logger.Debug(string.Format("bIsServiceReadyOK[{0}] ==> ", bIsServiceReadyOK));
            _logger.Debug(string.Format("\t IsNetwork                     : [{0}]", bIsNetwork));
            _logger.Debug(string.Format("\t IsValidServiceProtocolNetwork : [{0}]", bIsValidServiceProtocolNetwork));
            _logger.Debug(string.Format("\t IsOKStatus                    : [{0}]", bIsOKStatus));
            _logger.Debug(string.Format("\t IsOKCurrentState              : [{0}]", bIsOKCurrentState));
            _logger.Debug(string.Format("\t IsOKAgentState                : [{0}]", bIsOKAgentState));
            _logger.Debug(string.Format("\t IsPassFirstTime               : [{0}]", !_bIsFirstTime));

            return bIsServiceReadyOK;
        }


        private void GetDesktopInfo()
        {
            try
            {
                if (IsReadyForConnectToDesktop())
                {
                    ConnectDesktop();
                }
                else
                {
                    _bIsFirstTime = false;
                    IncreamentRetryTimes();
                    new RequestToHCVKB().RequestGetDesktopConnectionInfo_AsyncCallback(_voBrokerServer, _voDesktopPoolEx.DesktopPool, _voUser, _strServiceProtocol, Callback_GetDesktopConnectionInfo);
                }
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
            _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

            try
            {
                if (resJsonObject != null)
                {
                    // fetch and update auth token
                    new RequestToHCVKB().UpdateAuthToken(_voBrokerServer, resJsonObject);

                    if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                    {
                        
                        // fetch desktop info
                        {
                            JObject oDesktop = JObject.Parse(resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA]["desktop"].ToString());
                            _voDesktopPoolEx.DesktopPool.Desktop.DesktopID = oDesktop["desktopId"].ToString();
                            _voDesktopPoolEx.DesktopPool.Desktop.InstanceID = oDesktop["instanceId"].ToString();
                            _voDesktopPoolEx.DesktopPool.Desktop.CurrentState = oDesktop["desktopCurrentState"].ToString();
                            _voDesktopPoolEx.DesktopPool.Desktop.Status = oDesktop["status"].ToString();
                            _voDesktopPoolEx.DesktopPool.Desktop.PowerState = oDesktop["powerState"].ToString();
                            _voDesktopPoolEx.DesktopPool.Desktop.VMState = oDesktop["vmState"].ToString();
                            _voDesktopPoolEx.DesktopPool.Desktop.AgentState = oDesktop["agentState"].ToString();
                            _voDesktopPoolEx.DesktopPool.Desktop.DesktopIP = oDesktop["ipAddress"].ToString();
                            _voDesktopPoolEx.DesktopPool.Desktop.Sessionconnected = oDesktop["sessionConnected"].ToString();

                            // remove desktop protocol objects
                            _voDesktopPoolEx.DesktopPool.Desktop.Protocols.Clear();


                            // add new desktop protocol object
                            JArray oProtocolList = JArray.Parse(oDesktop["protocol"].ToString());
                            foreach (JObject oProtocol in oProtocolList)
                            {
                                VOProtocol voProtocol = new VOProtocol();
                                voProtocol.ProtocolType = oProtocol["type"].ToString();
                                voProtocol.ProtocolIP = oProtocol["ipAddress"].ToString();
                                voProtocol.ProtocolPort = oProtocol["port"].ToString();


                                // Change Convert IP Recommend IpRule Order From Broker
                                //  apply ip Convert rule
                                if (_voBrokerServer.ConvertIp_TargetClientIpRangeRule != "")
                                {

                                    string change_ip = CommonUtils.ConvertIpRecommendServerAddIpRuleToVM_Ip(
                                        voProtocol.ProtocolIP,
                                        _voBrokerServer.ConvertIp_TargetClientIpRangeRule,
                                        _voUser.ClientIP,
                                        //"10.11.3.101",
                                        _voBrokerServer.ConvertIp_TargetDesktopVMIpSubnet);


                                    voProtocol.ProtocolIP = change_ip;
                                }

                                _voDesktopPoolEx.DesktopPool.Desktop.Protocols.Add(voProtocol);
                            }
                            // __Imsi__
                            //{
                            //    VOProtocol voProtocol = new VOProtocol();
                            //    voProtocol.ProtocolType = _strServiceProtocol;
                            //    voProtocol.ProtocolIP = "192.168.120.2";
                            //    voProtocol.ProtocolPort = "5900";
                            //    _voDesktopPoolEx.DesktopPool.Desktop.Protocols.Add(voProtocol);
                            //}
                        }


                        // check to status of ready for connect to desktop 
                        {
                            if (IsReadyForConnectToDesktop())
                            {
                                // log report
                                _logReport.LogReport_Desktop_ConnectionInfo(
                                    _voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_SUCCEED, string.Empty);

                                // connect desktop
                                ConnectDesktop();
                            }
                            else
                            {
                                // retry to get desktop info.
                                if (IsRetryTimeout())
                                {
                                    // log report
                                    _logReport.LogReport_Desktop_ConnectionInfo(
                                           _voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_FAILED, VOErrorCode._E_CODE_C_C0000003);

                                    //HCVKCErrorHandler.ErrorHandler(VOErrorCode._E_CODE_C_C0000003);
                                    ErrorHandlerManager.ErrorHandler(VOErrorCode._E_CODE_C_C0000003);
                                    DisconnectDesktopView();
                                }
                                else
                                {
                                    GetDesktopInfo();
                                }
                            }
                        }
                    }
                    else
                    {
                        //ConnectDesktop();
                        // log report
                        _logReport.LogReport_Desktop_ConnectionInfo(
                            _voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_FAILED, VOErrorCode._E_CODE_C_C0000003);

                        // failure response
                        //HCVKCErrorHandler.ErrorHandler(resJsonObject[HCVKCRequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                        ErrorHandlerManager.ErrorHandler(resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                        DisconnectDesktopView();
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
                _logReport.LogReport_Desktop_ConnectionInfo(
                    _voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_FAILED, VOErrorCode._E_CODE_C_C0000003);

                // HCVKCErrorHandler.ExceptionHandler(wex);
                DisconnectDesktopView();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

                // log report
                _logReport.LogReport_Desktop_ConnectionInfo(
                    _voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_FAILED, VOErrorCode._E_CODE_C_C0000003);

                //HCVKCErrorHandler.ExceptionHandler(ex);
                ErrorHandlerManager.ExceptionHandler(ex, null, "[DesktopConnectionInfo]");
                DisconnectDesktopView();
            }
        }

        private void GetClientSize(ref int nWidth, ref int nHeight)
        {
            int nResolution = _voDesktopPoolEx.ResolutionIndex;

            switch (nResolution)
            {
                case 0:
                    nWidth = 600; nHeight = 480;
                    break;
                case 1:
                    nWidth = 800; nHeight = 600;
                    break;
                case 2:
                    nWidth = 1024; nHeight = 768;
                    break;
                case 4:
                    nWidth = 1280; nHeight = 1024;
                    break;
                case 5:
                    nWidth = 1440; nHeight = 900;
                    break;
                case 6:
                    nWidth = 1600; nHeight = 900;
                    break;
                case 7:
                    nWidth = 1600; nHeight = 1200;
                    break;
                case 8:
                    nWidth = 1920; nHeight = 1080;
                    break;
                case 9:
                    //_axRdpView.FullScreen = true;
                    //WindowState = FormWindowState.Maximized;
                    //_bIsStartFullScreenMode = true;
                    break;
                case 3:
                default:
                    nWidth = 1280; nHeight = 768;
                    break;
            }

            //if (_bIsStartFullScreenMode)
            //  ClientSize = new Size(1280, 768);
            //else _sizePrevClientSize = GetClientSize();
        }

		// Thincast 프로그램의 중요노출방지를 위해서, 입력 파라메터를 개행으로 구분된 string로 반환합니다.
		private string InitConnectionStringsforExtViewer()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			// 접속 정보 
			// more priority option, if Contains Vmconnect value use to Vmconnect
			string temp_pw = ""; // not safe
			if (_voUser.VmConnectPw != string.Empty && _voUser.VmConnectPw.Length > 0) {
				temp_pw = _voUser.VmConnectPw;
			} else {
				temp_pw = _voUser.Password;
			}
			sb.AppendLine (string.Format ("/v:{0}:{1}", _strAgentVDIIP, _strAgentVDIPort));
			sb.AppendLine (string.Format ("/p:{0}", temp_pw));
			sb.AppendLine (string.Format ("/u:{0}", _voUser.DomainUserID));

			if (MainWindow.mainWindow.environment.vORDPViewerOptionProperties.RDPDomain != "") {
				sb.AppendLine(string.Format ("/d:{0}", MainWindow.mainWindow.environment.vORDPViewerOptionProperties.RDPDomain));
			} else {
				sb.AppendLine(string.Format ("/d:{0}", _voUser.Domain));
			}

			// RDP 기능
			if (MainWindow.mainWindow.environment.vODisplayProperties.IsUseMultiMonitor) {
				sb.AppendLine("/multimon");
			} else {

				if (MainWindow.mainWindow.environment.vODisplayProperties.DynamicAutoResolutionUpdate) {
					// Thincast에서는 동적해상도를 위해, /fitsession 옵션을 지원합니다.  
					sb.AppendLine ("/fitsession");
				}

			}
			if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseRDPDefaultOption == true) {
				sb.AppendLine ("/rfx");
				sb.AppendLine ("/rfx-mode:video");
				sb.AppendLine ("+fonts");
			}
			if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseRDPToolbar == true) {
				sb.AppendLine ("/floatbar:sticky:off,default:visible,show:always");
			}

			switch (MainWindow.mainWindow.environment.vOGeneralsProperties.RDPGfxMode) {
			case 0:
				sb.AppendLine ("/gfx:rfx");
				break;
			case 1:
				sb.AppendLine ("/gfx-progressive");
				break;
			case 2:
				sb.AppendLine ("/gfx-small-cache");
				break;
			case 3:
				sb.AppendLine ("/gfx-thin-client");
				break;
			case 4:
				break;
			default:
				sb.AppendLine ("/gfx:rfx");
				break;
			}

			sb.AppendLine ("/cert:ignore");


			if (_voDesktopPoolEx.ResolutionIndex == 9) {
				sb.AppendLine ("/f");
			}else {
				int nWidth = 0, nHeight = 0;
				GetClientSize (ref nWidth, ref nHeight);
				sb.AppendLine (string.Format ("/w:{0}", nWidth));
				sb.AppendLine (string.Format ("/h:{0}", nHeight));
			}

			if (MainWindow.mainWindow.environment.vODisplayProperties.IsUseMultiMonitor) {
				sb.AppendLine ("/multimon");
			}
			if (_voDesktopPoolEx.Audio) {
				sb.AppendLine ("/sound:latency:60");
			}

			switch (_voDesktopPoolEx.SpeedIndex) {
			case 1:
				sb.AppendLine ("/network:modem");
				break;
			case 2:
				sb.AppendLine ("/network:broadband");
				break;
			case 3:
				sb.AppendLine ("/network:broadband-low");
				break;
			case 4:
				sb.AppendLine ("/network:broadband-high");
				break;
			case 5:
				sb.AppendLine ("/network:wan");
				break;
			case 6:
				sb.AppendLine ("/network:lan");
				break;
			}

			switch (MainWindow.mainWindow.environment.vODisplayProperties.ColorDepth) {
			case 0:
				sb.AppendLine ("/bpp:15");
				break;
			case 1:
				sb.AppendLine ("/bpp:16");
				break;
			case 2:
				sb.AppendLine ("/bpp:24");
				break;
			case 3:
				sb.AppendLine ("/bpp:32");
				break;
			}

			if (MainWindow.mainWindow.environment.vOGeneralsProperties.GDIRendering == "sw") {
				sb.AppendLine ("/gdi:sw");
			}

			// 리다이렉션 정보
			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsClipBoard) {
				sb.AppendLine ("+clipboard"); 
			} else {
				sb.AppendLine ("-clipboard");
			}

			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalPrinter) {
				sb.AppendLine ("/printers");
			}

			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalDrive) {
				sb.AppendLine ("/home-drive");
			}

			string strDeviceIDs = string.Empty;

			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalDevice) {
				if (MainWindow.mainWindow.environment.vOUSBDeviceProperties.LocalDeviceDeviceID != "") {
					strDeviceIDs = MainWindow.mainWindow.environment.vOUSBDeviceProperties.LocalDeviceDeviceID;
				}
				if (strDeviceIDs != string.Empty) {
					sb.AppendLine ("/usb:id:" + strDeviceIDs);
				}
			}
			// 체크 우선순위 1. 웹캠리디렉션 2. 웹캠 devid 검색 3. 웹캠 제어권 파일 사용 4.  파일 값이 1인지 5. usbip 사용
			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalVideoDevice) {
				string strWebCamDeviceID = MainWindow.mainWindow.GetWebCamDevicdId ();

				if (MainWindow.mainWindow.environment.vORedirectionProperties.IsUseH264 == true) {
					if (strWebCamDeviceID != "") {
						sb.AppendLine (string.Format ("/dvc:rdpecam,dev:{0},format:h264", strWebCamDeviceID));
					} else {
						sb.AppendLine (string.Format ("/dvc:rdpecam,device:*,format:h264"));
					}
				} else {
					if (strWebCamDeviceID != "") {
						sb.AppendLine (string.Format ("/dvc:rdpecam,dev:{0}", strWebCamDeviceID));
					} else {
						sb.AppendLine (string.Format ("/dvc:rdpecam,device:*"));
					}
				}
				sb.AppendLine ("/mic");

			}


			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsSmartCard) {
				sb.AppendLine ("/smartcard");
			}

			if (MainWindow.mainWindow.environment.vOPerformanceProperties.AudioCapture) {
				sb.AppendLine ("/microphone");
			}

			if (MainWindow.mainWindow.environment.vOPerformanceProperties.IsBitmapCaching) {
				sb.AppendLine ("-bitmap-cache");
			}

			// set enable rdp auto reconnect option
			if (MainWindow.mainWindow.environment.vOGeneralsProperties.RdpAutoReconnectEnable == true) {
				sb.AppendLine ("+auto-reconnect");

				if(MainWindow.mainWindow.environment.vOGeneralsProperties.RdpAutoReconnectCnt != "0") {
					sb.AppendLine ("/auto-reconnect-max-retries:" + MainWindow.mainWindow.environment.vOGeneralsProperties.RdpAutoReconnectCnt);
				} else {
					sb.AppendLine ("/auto-reconnect-max-retries:100");
				}


			}

			if (MainWindow.mainWindow.environment.vOGeneralsProperties.RDPExtraOption != "") {
				string [] sArrTemp = MainWindow.mainWindow.environment.vOGeneralsProperties.RDPExtraOption.Split (' ');
				for (int i = 0; i < sArrTemp.Length; i++) {
					sb.AppendLine (sArrTemp [i]);
				}
			}

			if (!string.IsNullOrEmpty (_strTitle)) {
				sb.AppendLine(string.Format ("/t:{0}", _strTitle));
			}

			// thincast input parameter
			_logger.Debug (string.Format ("_strConnectionInfo(ThinCast) ==> "));
			_logger.Debug (sb.ToString ());

			return (sb.ToString ());
		}

		private void InitConnectionInfoRDP()
        {
            _strConnectionInfo = "";

            // connection settings
            /*
            _strConnectionInfo = string.Format("/v:{0} /port:{1}",
                                               _strAgentVDIIP,
                                               _strAgentVDIPort);
                                               */


            // more priority option, if Contains Vmconnect value use to Vmconnect
            string temp_pw = ""; // not safe
            if (_voUser.VmConnectPw != string.Empty &&
                 _voUser.VmConnectPw.Length > 0) {

                temp_pw = _voUser.VmConnectPw;
            } else {

                temp_pw = _voUser.Password;
            }
			/*
            _strConnectionInfo = string.Format("/v:{0} /port:{1} /u:{2}@{3} /p:'{4}' ",
                                                   _strAgentVDIIP,
                                                   _strAgentVDIPort,
                                                   _voUser.DomainUserID,
                                                   _voUser.Domain,
                                                   temp_pw);
												   */
			_strConnectionInfo = string.Format ("/v:{0} /port:{1}  /p:'{2}' ",_strAgentVDIIP, _strAgentVDIPort,	temp_pw);

			if (MainWindow.mainWindow.environment.vORDPViewerOptionProperties.RDPDomain != "") {
				_strConnectionInfo += string.Format (" /u:{0} /d:{1}" ,
												   _voUser.DomainUserID,
												   MainWindow.mainWindow.environment.vORDPViewerOptionProperties.RDPDomain);
			} else {
				_strConnectionInfo += string.Format (" /u:{0}@{1}",_voUser.DomainUserID,_voUser.Domain);

			}


			if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseRDPDefaultOption == true) {
				_strConnectionInfo += " /rfx /rfx-mode:video  +fonts ";
			}

			if (MainWindow.mainWindow.environment.vODisplayProperties.DynamicAutoResolutionUpdate == true) {
				_strConnectionInfo += " /dynamic-resolution ";
			}

			if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseRDPToolbar == true) {
				_strConnectionInfo += " /floatbar:sticky:off,default:visible,show:always ";
			}
			switch (MainWindow.mainWindow.environment.vOGeneralsProperties.RDPGfxMode) {
			case 0:
				_strConnectionInfo += " /gfx:rfx";
				break;
			case 1:
				_strConnectionInfo += " /gfx-progressive";
				break;
			case 2:
				_strConnectionInfo += " /gfx-small-cache";
				break;
			case 3:
				_strConnectionInfo += " /gfx-thin-client";
				break;
			case 4:
				break;
			default:
				_strConnectionInfo += " /gfx:rfx";
				break;
			}



			if (MainWindow.mainWindow.FreeRDPVer == 3) {
				_strConnectionInfo += " /cert:ignore";
			} else if (MainWindow.mainWindow.FreeRDPVer == 2) {
				_strConnectionInfo += " /cert-ignore";
			} else {
				_strConnectionInfo += " /cert-ignore";
			}

			if (!string.IsNullOrEmpty (_strTitle)) {
				_strConnectionInfo += string.Format (" /t:\"{0}\"", _strTitle);
			}

			// display settings
			if (_voDesktopPoolEx.ResolutionIndex == 9)
                _strConnectionInfo += string.Format(" /f");
            else
            {
                int nWidth = 0, nHeight = 0;
                GetClientSize(ref nWidth, ref nHeight);
                _strConnectionInfo += string.Format(" /w:{0} /h:{1}", nWidth, nHeight);
            }


            if (MainWindow.mainWindow.environment.vODisplayProperties.IsUseMultiMonitor)
                _strConnectionInfo += " /multimon";

            // performance settings
            if (_voDesktopPoolEx.Audio)
            {
                if (MainWindow.mainWindow.FreeRDPVer == 2)
                    _strConnectionInfo += " /sound:latency:60";
                else
                    _strConnectionInfo += " /sound:latency:60";
            }

            switch (_voDesktopPoolEx.SpeedIndex)
            {
                case 1:
                    _strConnectionInfo += " /network:modem";
                    break;
                case 2:
                    _strConnectionInfo += " /network:broadband";
                    break;
                case 3:
                    _strConnectionInfo += " /network:broadband-low";
                    break;
                case 4:
                    _strConnectionInfo += " /network:broadband-high";
                    break;
                case 5:
                    _strConnectionInfo += " /network:wan";
                    break;
                case 6:
                    _strConnectionInfo += " /network:lan";
                    break;
            }
            switch (MainWindow.mainWindow.environment.vODisplayProperties.ColorDepth)
            {
                case 0:
                    _strConnectionInfo += " /bpp:15";
                    break;
                case 1:
                    _strConnectionInfo += " /bpp:16";
                    break;
                case 2:
                    _strConnectionInfo += " /bpp:24";
                    break;
                case 3:
                    _strConnectionInfo += " /bpp:32";
                    break;
            }

			if (MainWindow.mainWindow.environment.vOGeneralsProperties.GDIRendering == "sw") {
				_strConnectionInfo += " /gdi:sw";
			}


			/*
            this.chkClipboard.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsClipBoard;
            this.chkLocalPrinter.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalPrinter;
            this.chkLocalDriver.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalDrive;
            this.chkLocalDevice.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalDevice;
            this.chkLocalPort.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalPort;
            this.chkSmartCard.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsSmartCard;
            this.chkBitmapCaching.Active = MainWindow.mainWindow.environment.vOPerformanceProperties.IsBitmapCaching;
            */

			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsClipBoard) {
				_strConnectionInfo += " +clipboard";
			} else {
				_strConnectionInfo += " -clipboard";
			}

			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalPrinter)
            {
				string strGetPrinterTotalNames = CommonUtils.GetPrinterNames ();
				if (strGetPrinterTotalNames != "") {
					string [] strPrinterNames = strGetPrinterTotalNames.Split (',');
					for (int i = 0; i < strPrinterNames.Length; i++) {
						_strConnectionInfo += " /printer:" + strPrinterNames [i];
					}
					_strConnectionInfo += " ";
				}
			}

            if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalDrive) {
				_strConnectionInfo += " /home-drive";
			}

			string strDeviceIDs = string.Empty;

			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalDevice) {
				if (MainWindow.mainWindow.environment.vOUSBDeviceProperties.LocalDeviceDeviceID != "") {
					strDeviceIDs = MainWindow.mainWindow.environment.vOUSBDeviceProperties.LocalDeviceDeviceID;
				}
			}
			// 체크 우선순위 1. 웹캠리디렉션 2. 웹캠 devid 검색 3. 웹캠 제어권 파일 사용 4.  파일 값이 1인지 5. usbip 사용
			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalVideoDevice) {
				//freerdp-rdpecam프로그램을 이용하면, usb 리디렉션방식이 아닌, /webcam 파라메터로 연결합니다.
				if(MainWindow.mainWindow.environment.vORedirectionProperties.IsUseRDPEcam == true) {
					_strConnectionInfo += " /webcam:format:mjpg,res:320x240,fps:30,quality:high,videodevice:/dev/video0 /mic ";
				} else {
					string strWebCamDeviceID = MainWindow.mainWindow.GetWebCamDevicdId ();
					if (strWebCamDeviceID != "") {
						if (MainWindow.mainWindow.environment.vOUSBDeviceProperties.IsUseFileIOWebCamCtrl == true ||
						 MainWindow.mainWindow.environment.vOUSBDeviceProperties.IsUseAPIWebCamCtrl == true) {
							if (MainWindow.mainWindow.strCurDaaS_USB_REDIRECT.Contains ("1")) {
								// Usbip 이용하지 않을 경우에 옵션을 추가합니다. OnConnect 콜백함수에서 RDP 연결후, usbip로 연결
								if (MainWindow.mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection != "ALL" &&
										MainWindow.mainWindow._strVDI_Connect_MODE.Contains (MainWindow.mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection) == false) {
									if (strDeviceIDs != string.Empty) strDeviceIDs += "#";
									strDeviceIDs += strWebCamDeviceID + " /mic ";
								}
							}
						} else {
							// Usbip 이용하지 않을 경우에 옵션을 추가합니다. OnConnect 콜백함수에서 RDP 연결후, usbip로 연결
							if (MainWindow.mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection != "ALL" &&
									MainWindow.mainWindow._strVDI_Connect_MODE.Contains (MainWindow.mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection) == false) {
								if (strDeviceIDs != string.Empty) strDeviceIDs += "#";
								strDeviceIDs += strWebCamDeviceID + " /mic ";
							}
						}
					}
				}
			}

			if (strDeviceIDs != string.Empty) {
				_strConnectionInfo += " /usb:id:" + strDeviceIDs;
				_strConnectionInfo += " ";
			}
            _strConnectionInfo += " ";

            if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalPort)
                _strConnectionInfo += " ";

            if (MainWindow.mainWindow.environment.vORedirectionProperties.IsSmartCard)
                _strConnectionInfo += " /smartcard";

    		if (MainWindow.mainWindow.environment.vOPerformanceProperties.AudioCapture) {
				_strConnectionInfo += " /microphone:sys:alsa,format:1,quality:high ";
			}
			
            // freerdp version 3 bitmap on off option type
            if (MainWindow.mainWindow.FreeRDPVer == 3)
            {
                if (MainWindow.mainWindow.environment.vOPerformanceProperties.IsBitmapCaching)
                    _strConnectionInfo += " /cache:bitmap:on";
                else
                    _strConnectionInfo += " /cache:bitmap:off";

            }
            else if (MainWindow.mainWindow.FreeRDPVer == 2)
            {
                if (MainWindow.mainWindow.environment.vOPerformanceProperties.IsBitmapCaching)
                    _strConnectionInfo += " -bitmap-cache";
            }
            else
            {
                if (MainWindow.mainWindow.environment.vOPerformanceProperties.IsBitmapCaching)
                    _strConnectionInfo += " -bitmap-cache:on";
                else
                    _strConnectionInfo += " -bitmap-cache:off";
            }

            if (!string.IsNullOrEmpty(MainWindow.mainWindow.environment.vOPerformanceProperties.EtcProperties))
                _strConnectionInfo += " " + MainWindow.mainWindow.environment.vOPerformanceProperties.EtcProperties;


            // set enable rdp auto reconnect option
            if (MainWindow.mainWindow.environment.vOGeneralsProperties.RdpAutoReconnectEnable == true){

				if (MainWindow.mainWindow.environment.vOGeneralsProperties.RdpAutoReconnectCnt != "0") {
					_strConnectionInfo += " +auto-reconnect /auto-reconnect-max-retries:" +
						MainWindow.mainWindow.environment.vOGeneralsProperties.RdpAutoReconnectCnt;
				} else {
					_strConnectionInfo += " +auto-reconnect /auto-reconnect-max-retries:100";
				}



			}

			if(MainWindow.mainWindow.environment.vOGeneralsProperties.RDPExtraOption != "") {
				_strConnectionInfo += " " + MainWindow.mainWindow.environment.vOGeneralsProperties.RDPExtraOption + " ";
			}

#if !DEBUG
			_logger.Debug (string.Format ("_strConnectionInfo ==> Param Secure Hide"));
#else
			_logger.Debug (string.Format ("_strConnectionInfo ==> {0}", _strConnectionInfo));
#endif

		}

		private void InitConnectionInfoSpice()
        {
            // fetch information
            _strConnectionInfo = string.Empty;

            string _customTitle = "";
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleEnable == true)
            {
                _customTitle = MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleText;
            }
            else
            {
                _customTitle = "DaaSXpert";
            }


            foreach (VOProtocol voProtocol in _voDesktopPoolEx.DesktopPool.Desktop.Protocols)
            {
                if (voProtocol.ProtocolType.Equals(_strServiceProtocol, StringComparison.CurrentCultureIgnoreCase))
                {
                    _strAgentServiceIP = _voDesktopPoolEx.DesktopPool.Desktop.DesktopIP;
                    // string strTitle = string.Format("{0}:{1} [ Login ID : {2} ]", MultiLang.HCVKC_MAIN_MSG_9999, _voDesktopPoolEx.DesktopPool.PoolName, _voUser.DomainUserID);



                    _strTitle = string.Format("{0}:{1} [ Login ID : {2} ]", _customTitle, _voDesktopPoolEx.DesktopPool.PoolName, _voUser.DomainUserID);
                    _strConnectionInfo = string.Format("spice://{0}:{1} --title=\"{2}\"", voProtocol.ProtocolIP, voProtocol.ProtocolPort, _strTitle);

                    
                    _logger.Debug(string.Format("_strConnectionInfo ==> {0}", _strConnectionInfo));
                    break;
                }
            }
        }

        private void InitConnectionInfoDXGP()
        {
            // fetch information
            _strConnectionInfo = string.Empty;
            string _customTitle = "";
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleEnable == true)
            {
                _customTitle = MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleText;
            }
            else
            {
                _customTitle = "DaaSXpert";
            }
            foreach (VOProtocol voProtocol in _voDesktopPoolEx.DesktopPool.Desktop.Protocols)
            {
                if (voProtocol.ProtocolType.Equals(_strServiceProtocol, StringComparison.CurrentCultureIgnoreCase))
                {
                    _strAgentServiceIP = _voDesktopPoolEx.DesktopPool.Desktop.DesktopIP;

                    _strTitle = string.Format("{0}:{1} [ Login ID : {2} ]", _customTitle, _voDesktopPoolEx.DesktopPool.PoolName, _voUser.DomainUserID);
                    /*_strConnectionInfo = string.Format("--op connect -i {0} -p {1} --session-port {2} -u {3} --password {4}  --domain {5}",
                            voProtocol.ProtocolIP, _strAgentPort, _strSessionPort, _voUser.DomainUserID, _voUser.Password, _voUser.Domain);*/
                    protocolIP = voProtocol.ProtocolIP;
                    break;
                }
            }
        }

        private void ConnectDesktop()
        {
            try
            {
                ResetRetryTimes();

                // fetch information
                _strConnectionInfo = string.Empty;
                _strAgentServiceIP = string.Empty;
                string _customTitle = "";

				// RDP 창 전환을 위해, Title명 앞에 prefix를 추가합니다.
				if (MainWindow.mainWindow.environment.vOGeneralsProperties.ViewerNamePrefix != "") {
					_customTitle = MainWindow.mainWindow.environment.vOGeneralsProperties.ViewerNamePrefix + "_";
				} else {
					_customTitle = "";
				}


				if (MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleEnable == true)
                {
                    _customTitle += MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleText;
                }
                else
                {
                    _customTitle += "DaaSXpert";
                }
                foreach (VOProtocol voProtocol in _voDesktopPoolEx.DesktopPool.Desktop.Protocols)
                {
                    if (voProtocol.ProtocolType.Equals(_strServiceProtocol))
                    {
                        if (_strServiceProtocol == "RDP")
                        {
                            _strAgentServiceIP = voProtocol.ProtocolIP;
                            _strAgentVDIIP = voProtocol.ProtocolIP;
                            _strTitle = string.Format("{0}:{1} [ Login ID : {2} ]", _customTitle, _voDesktopPoolEx.DesktopPool.PoolName, _voUser.DomainUserID);
                            break;
                        }
                        else if (_strServiceProtocol == "SPICE")
                        {
                            _strAgentServiceIP = _voDesktopPoolEx.DesktopPool.Desktop.DesktopIP;
                            //_strAgentVDIIP = voProtocol.ProtocolIP;
                            _strTitle = string.Format("{0}:{1} [ Login ID : {2} ]", "DaaSXpert", _voDesktopPoolEx.DesktopPool.PoolName, _voUser.DomainUserID);
                            break;
                        }
                        else if (_strServiceProtocol == "DXGP")
                        {
                            _strAgentServiceIP = _voDesktopPoolEx.DesktopPool.Desktop.DesktopIP;
                            //_strAgentVDIIP = voProtocol.ProtocolIP;
                            _strTitle = string.Format("{0}:{1} [ Login ID : {2} ]", "DaaSXpert", _voDesktopPoolEx.DesktopPool.PoolName, _voUser.DomainUserID);
                            break;
                        }
                    }
                }

                if (_strServiceProtocol == "DXGP")
                    InitConnectionInfoDXGP();

                // validating information.
                if (string.IsNullOrEmpty(_strAgentServiceIP))
                {
                    // HCVKCErrorHandler.ErrorHandler(VOErrorCode._E_CODE_C_C0000004);
                    ErrorHandlerManager.ErrorHandler(VOErrorCode._E_CODE_C_C0000004);
                    _logger.Error(string.Format("VOErrorCode._E_CODE_C_C0000004"));
                    return;
                }

                // request to hcvkagent for ready to connecting
                new RequestToHCVKA().RequestConnectToHCVKA_AsyncCallback(_strAgentServiceIP, _strAgentServicePort, _voUser, _voDesktopPoolEx.DesktopPool, _strServiceProtocol, Callback_ConnectDesktop);
            }
            catch (WebException wex)
            {
                _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));

                DisconnectDesktopView();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }
        private void Callback_ConnectDesktop(JObject resJsonObject, Exception exParam)
        {
            _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

            try
            {
                if (resJsonObject != null)
                {
                    if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                    {

                        Object agentPort = resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA]["agentPort"];
                        if (agentPort != null)
                        {
                            _strAgentPort = agentPort.ToString();
                        }
                        Object sessionPort = resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA]["vdiPort"];
                        if (agentPort != null)
                        {
                            _strSessionPort = sessionPort.ToString();
                        }
                        _strAgentVDIPort = resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA]["vdiPort"].ToString();

                        // log report
                        _logReport.LogReport_Desktop_Connect(
                            _voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_SUCCEED, string.Empty);

                        if (_strServiceProtocol.Equals("DXGP"))
                        {
                            _strConnectionInfo = string.Format("--op connect -i {0} -p {1} --session-port {2} -u {3} --password {4} --domain {5} ",
                            protocolIP, _strAgentPort, _strSessionPort, _voUser.DomainUserID, _voUser.Password, _voUser.Domain);

                            if (_voDesktopPoolEx.ResolutionIndex == 9)
                                _strConnectionInfo += string.Format(" --full-screen");
                            else
                            {
                                int nWidth = 0, nHeight = 0;
                                GetClientSize(ref nWidth, ref nHeight);
                                _strConnectionInfo += string.Format(" --set-host-resolution {0}x{1}", nWidth ,nHeight);
                            }

                            _logger.Debug(string.Format("_strConnectionInfo ==> {0}", _strConnectionInfo));
                        }

                        ConnectDesktopView();
                        StartHeartbeatTimer();

                        //_hcvkcMain?.SetAutomationState(VOAutomation.AutomationStateEnum.CONNECTION_SUCCESS);
                        //_hcvkcMain?.GetDesktopPoolInfo();
                    }
                    else
                    {
                        // log report
                        _logReport.LogReport_Desktop_Connect(_voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_FAILED, string.Empty);

                        // failure response
                        // HCVKCErrorHandler.ErrorHandler(resJsonObject[HCVKCRequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                        ErrorHandlerManager.ErrorHandler(resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());

                        // disconnect from desktop
                        DisconnectDesktopView();
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
                _logReport.LogReport_Desktop_Connect(_voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_FAILED, string.Empty);

                //HCVKCErrorHandler.ExceptionHandler(wex);
                ErrorHandlerManager.ExceptionHandler(wex, null, "[DesktopConnectW]");

                DisconnectDesktopView();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

                // log report
                _logReport.LogReport_Desktop_Connect(_voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_FAILED, string.Empty);

                //HCVKCErrorHandler.ExceptionHandler(ex);
                ErrorHandlerManager.ExceptionHandler(ex, null, "[DesktopConnectE]");
                DisconnectDesktopView();
            }
        }

		public void ConnectDesktopView ()
		{
			if (MainFunc.callbackChangeConnectStatus != null) {
				Gtk.Application.Invoke (delegate {
					MainFunc.callbackChangeConnectStatus (_voDesktopPoolEx.DesktopPool.PoolID, true);
				});
			}

			//StartSpiceSession(strConnectionInfo);

			_bIsConnectedSuccess = true;

			if (_strServiceProtocol == "RDP")
				InitConnectionInfoRDP ();
			else if (_strServiceProtocol == "SPICE")
				InitConnectionInfoSpice ();

			// launching viewer
			if (_strServiceProtocol != "DXGP") {

				string sFileName, sArguments;
				bool bUseStandardInput = false;
				// AppImage 파일이 존재하면, AppImage로 실행하고, 없다면, xfreerdp로 연결합니다. 
				if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseExtRDPViewer == true &&
					System.IO.File.Exists ("/usr/lib/DaaSXpertClient/rdc/rdc.AppImage") == true) {
					sFileName = "/usr/lib/DaaSXpertClient/rdc/rdc.AppImage";
					sArguments = "/args-from:stdin";
					bUseStandardInput = true;
				} else {
					sFileName = _strServiceProtocol == "RDP" ? "xfreerdp" : "remote-viewer";
					sArguments = _strConnectionInfo;
				}

				_connectProcess = Process.Start (new ProcessStartInfo () {

					FileName = sFileName,
					Arguments = sArguments,
					RedirectStandardInput = bUseStandardInput,
					RedirectStandardOutput = true, 
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				});


				if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseExtRDPViewer == true &&
					System.IO.File.Exists ("/usr/lib/DaaSXpertClient/rdc/rdc.AppImage") == true) {

					string sArrArguments = InitConnectionStringsforExtViewer ();
					System.IO.StreamWriter sw = _connectProcess.StandardInput;
					sw.WriteLine (sArrArguments);
					sw.Flush ();
					sw.Close ();

				}

			} else {
				string path = "/usr/local/dxgp_viewer/dxgp_viewer";
				_connectProcess = Process.Start (new ProcessStartInfo () {
					FileName = path,
					//Arguments = _strConnectionInfo,
					Arguments = "--stdin-args",
					RedirectStandardInput = true,
					RedirectStandardOutput = false,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				});
				System.IO.StreamWriter sw = _connectProcess.StandardInput;
				sw.WriteLine (_strConnectionInfo);
				sw.Flush ();
				sw.Close ();
			}

			_connectProcess.EnableRaisingEvents = true;
			_connectProcess.Exited += Exit_Process;

			if (bIsRequestRestart == false) {
				DoOnConnected ();
			}

			// Thincast Client 창의 이름을 변경합니다(ID 중요정보 노출방지) (-> 파라메터 적용 전까지)
			if(MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseExtRDPViewer == true) {
				while(_bIsConnectedSuccess && MainWindow.mainWindow.environment.vORDPViewerOptionProperties.IsUseRDPViewerTitleUpdate == true) {
					// Connecting 타이틀 창은 제외 (자동으로 종료)
					string sWindowTitle = CommonUtils.GetWindowIDFromPID (_connectProcess.Id, "Connecting");
					// pid에 해당하는 windows 생성되었다면 Title 값을 가져옴
					if (sWindowTitle != "") {
						CommonUtils.SetWindowTitleWithWMCTRL (sWindowTitle, _strTitle);
						break;
					}
					System.Threading.Thread.Sleep (1000);
				}
			}

			//웹캠리디렉션과 usbip를 이용하면, 여기서, attach를 합니다. 
			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalVideoDevice == true &&
				(MainWindow.mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection == "ALL" ||
									MainWindow.mainWindow._strVDI_Connect_MODE.Contains (MainWindow.mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection) == true)) {

				StartDelayMessageTimer ();

			}
		}

		// 윈도우 RDP의 경우, 연결후이벤트에서 vdi/onconnect 전달하나, DxClient-Lin에서는 Callback_ConnectDesktop 함수에서 프로세스 실행 후 바로 vdi/onconnected 를 전달합니다. 
		public async void DoOnConnected ()
		{
			try {

				_logger.Debug (string.Format ("On Connected.."));





				_logReport.LogReport_Desktop_OnConnect (_voDesktopPoolEx.DesktopPool.Desktop.InstanceID, VOProtocol.VOProtocolType.PROTOCOL_RDP, VOLogData.STATUS_SUCCEED, string.Empty);
				await new RequestToHCVKA ().RequestOnConnectToHCVKA (_strAgentServiceIP, _strAgentServicePort, _voUser, _voDesktopPoolEx.DesktopPool, _strServiceProtocol);

			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}
		}


		public void DisconnectDesktopView()
        {
            if (MainFunc.callbackChangeConnectStatus != null)
            {
                Gtk.Application.Invoke(delegate
                {
                    MainFunc.callbackChangeConnectStatus(_voDesktopPoolEx.DesktopPool.PoolID, false);
                });
            }

            StopHeartbeatTimer();
			StopDelayMessageTimer ();

			// skiping reqeuest of disconnection to HCVKA
			if (string.IsNullOrEmpty(_strAgentServiceIP) || _bIsHeartbeatError)
            {
                //if (IsReadyForConnectToDesktop())
                if (_bIsConnectedSuccess)
                {
                    new RequestToHCVKB().RequestClearSession_AsyncCallback(_voBrokerServer, _voDesktopPoolEx.DesktopPool, _voUser, Callback_RequestClearSession);
                    _bIsConnectedSuccess = false;
                }
                return;
            }

            // log report
            _logReport.LogReport_Desktop_Disconnect(_voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_SUCCEED, string.Empty);

            new RequestToHCVKA().RequestDisconnectFromHCVKA_AsyncCallback(_strAgentServiceIP, _strAgentServicePort, _voUser, _voDesktopPoolEx.DesktopPool, _strServiceProtocol, Callback_RequestDisconnect);
        }

        private void Callback_RequestDisconnect(JObject resJsonObject, Exception exParam)
        {
            _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

            try
            {
                if (resJsonObject != null)
                {
                    //if (IsReadyForConnectToDesktop())
                    if (_bIsConnectedSuccess)
                    {
                        new RequestToHCVKB().RequestClearSession_AsyncCallback(_voBrokerServer, _voDesktopPoolEx.DesktopPool, _voUser, Callback_RequestClearSession);
                        _bIsConnectedSuccess = false;
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
                // HCVKCErrorHandler.ExceptionHandler(wex);
                //ErrorHandlerManager.ExceptionHandler(wex);

                //_connectProcess.CloseMainWindow();

                if (_bIsConnectedSuccess)
                {
                    new RequestToHCVKB().RequestClearSession_AsyncCallback(_voBrokerServer, _voDesktopPoolEx.DesktopPool, _voUser, Callback_RequestClearSession);
                    _bIsConnectedSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
                // HCVKCErrorHandler.ExceptionHandler(ex);
                ErrorHandlerManager.ExceptionHandler(ex, null, "[RequestDisconnect]");
            }
            finally
            {
            }
        }

        private void Callback_RequestClearSession(JObject resJsonObject, Exception exParam)
        {
            _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

            try
            {
                if (resJsonObject != null)
                {
                    // fetch and update auth token
                    new RequestToHCVKB().UpdateAuthToken(_voBrokerServer, resJsonObject);
                }
                else
                {
                    throw exParam;
                }
            }
            catch (WebException wex)
            {
                _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));
                //HCVKCErrorHandler.ExceptionHandler(wex);
                ErrorHandlerManager.ExceptionHandler(wex, null, "[RequestClearSessionW]");
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
                //HCVKCErrorHandler.ExceptionHandler(ex);
                ErrorHandlerManager.ExceptionHandler(ex, null, "[RequestClearSessionE]");
            }
            finally
            {
                //_autoEvent.Set();
                Dispose();
                MainWindow.mainWindow.DeleteConnectList(this._voDesktopPoolEx.DesktopPool.PoolID);
            }
        }

        public void Show()
        {
            try
            {
                // log report
                _logReport.LogReport_Desktop_ConnectionInfo(
                    _voDesktopPoolEx.DesktopPool.Desktop.InstanceID, _strServiceProtocol, VOLogData.STATUS_START, string.Empty);
                GetDesktopInfo();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        public void Activate()
        {
            //ShowWindow(_connectProcess.MainWindowHandle.ToInt32(), SW_SHOWNORMAL);
        }

        public void Exit_Process(object sender, EventArgs e)
        {
            if (bIsRequestRestart)
                return;

			_connectProcess.WaitForExit();
            _logger.Debug(string.Format("Exit_Process --------------"));
            if (!_connectProcess.ExitCode.Equals(null))
            {
                _logger.Debug(string.Format("ExitCode : " + _connectProcess.ExitCode));
            }

            DisconnectDesktopView();

			// thincast client kill, flatpak 외 AppImage 형태이면, flatpak kill 명령을 진행하지 않습니다. 
			if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseExtRDPViewer == true) {
				if (System.IO.File.Exists ("/usr/lib/DaaSXpertClient/rdc/AppRun") == false) {
					CommonUtils.KillflatpakApplication (((Process)sender).Id);
				}
			}
			//  usbip를 이용하면 종료시에 detach를 합니다. 현재상태가 bind상태에는 detach만 호출합니다.
			if (MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalVideoDevice == true &&
			  		(MainWindow.mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection == "ALL" ||
							MainWindow.mainWindow._strVDI_Connect_MODE.Contains (MainWindow.mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection))){

				string strWebCamDevId = MainWindow.mainWindow.GetWebCamDevicdId ();
				string strWebCamBusId = CommonUtils.GetWebCamDeviceBusId (strWebCamDevId, MainWindow.mainWindow.environment.vOUSBDeviceProperties.UsbIpModulePath);
				string strWebCamPort = MainWindow.mainWindow.environment.vOUSBDeviceProperties.DeviceRedirectionPort;

				if (strWebCamBusId.Contains ("Error") == true) {
					ErrorHandlerManager.ErrorHandler (VOErrorCode._E_CODE_C_0000012);
					_logger.Error (string.Format ("[WebCamBusId] {0}", strWebCamBusId));
				} else {
					if (strWebCamDevId != "") {
						RequestRedirectionDetachMode (strWebCamBusId, strWebCamPort);
					} else {
						ErrorHandlerManager.ErrorHandler (VOErrorCode._E_CODE_C_0000011);
						_logger.Debug (string.Format ("No Request detach usbip"));
					}
				}

			}


		}

        public void KillProcess()
        {
            // checking alive viewer
            if (_connectProcess != null && !_connectProcess.HasExited)
            {
                _connectProcess.Kill();
                _connectProcess.WaitForExit();

                //Thread.Sleep(MAX_TIME_OUT);
                //_autoEvent.WaitOne(MAX_TIME_OUT);
            }
        }

        public void SetRequestRestart(bool bSet)
        {
            bIsRequestRestart = bSet;
        }

		public string GetConnectedDesktopPoolID()
		{
			string retVal = string.Empty;

			if(_bIsConnectedSuccess == true) {
				retVal = _voDesktopPoolEx.DesktopPool.PoolID;
			}

			return retVal;
		}
		public Process GetConnectProcess ()
		{
			return _connectProcess;
		}

		public void RequestRedirectionAttachMode (string strBusId, string remotePort)
		{
			_logger.Info (string.Format ("[RequestRedirectionAttachMode] BusId: ") + strBusId + " Port: " +remotePort);

			new RequestToHCVKA ().RequestDeviceRedirection_AsyncCallback (_strAgentServiceIP, _strAgentServicePort, _voUser, "attach", strBusId, remotePort, Callback_DeviceRedirectionAttachMode);

		}
		public void RequestRedirectionDetachMode (string strBusId, string remotePort)
		{
			_logger.Info (string.Format ("[RequestRedirectionDetachMode] BusId: ") + strBusId + " Port: " + remotePort);

			new RequestToHCVKA ().RequestDeviceRedirection_AsyncCallback (_strAgentServiceIP, _strAgentServicePort, _voUser, "detach", strBusId, remotePort, Callback_DeviceRedirectionDetachMode);

		}
		private void Callback_DeviceRedirectionAttachMode (JObject resJsonObject, Exception exParam)
		{
			_logger.Debug (string.Format ("response : {0}", resJsonObject?.ToString ()));

			try {
				if (resJsonObject != null) {

					if (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ().Equals ("0") != true) {
						string resultCode = resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ();
						_logger.Debug (string.Format ("DeviceRedirection failure response : {0}", resultCode));
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
		private void Callback_DeviceRedirectionDetachMode (JObject resJsonObject, Exception exParam)
		{
			_logger.Debug (string.Format ("response : {0}", resJsonObject?.ToString ()));

			try {
				if (resJsonObject != null) {

					if (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ().Equals ("0") != true) {
						string resultCode = resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ();
						_logger.Debug (string.Format ("DeviceRedirection failure response : {0}", resultCode));
						ErrorHandlerManager.ErrorHandler (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ());
					}

					// 요청상태가 unbind 일때,  callback 메세지가 오류메세지여도, unbind를  dbus로 요청하고 이후 제어권을  반납합니다.
					if (MainWindow.mainWindow._strUSBIPConnectStatus == "unbind" || 
					  (MainWindow.mainWindow.environment.vOUSBDeviceProperties.IsUseAPIWebCamCtrl == false &&
							MainWindow.mainWindow.environment.vOUSBDeviceProperties.IsUseAPIWebCamCtrl == false)) {
						string strWebCamDevId = MainWindow.mainWindow.GetWebCamDevicdId ();
						string strWebCamBusId = CommonUtils.GetWebCamDeviceBusId (strWebCamDevId, MainWindow.mainWindow.environment.vOUSBDeviceProperties.UsbIpModulePath);
						// system dbus 웹캠 unbind 요청
						QuickDbusMessageClient DbusCleint = new QuickDbusMessageClient ();
						_logger.Info (string.Format ("DeviceRedirection unbind DBUS API busId: ") + strWebCamBusId);
						if (DbusCleint.SetDeviceUnbindAsync (strWebCamBusId) == true) {
							if(MainWindow.mainWindow.environment.vOUSBDeviceProperties.IsUseFileIOWebCamCtrl == true) {
								_logger.Info (string.Format ("AccessRequest_DaaS2KVM  KVM = 1"));
								DeviceResoureAccessReq.AccessRequest_DaaS2KVM (false, true);
							}
						}
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
		// ------------------------------------------------------------------------------------------------
		// declare for interfaces of HCVKCWrapperSpice.dll
		//[DllImport(@"HCVKCWrapperSpice.dll", CallingConvention = CallingConvention.Cdecl)]
		//private static extern void InitializeSpice();

		//[DllImport(@"HCVKCWrapperSpice.dll", CallingConvention = CallingConvention.Cdecl)]
		//private static extern void FinalizeSpice();

		//[DllImport(@"HCVKCWrapperSpice.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		//private static extern bool StartSpiceSession(string strConnectionInfo);
	}
}
