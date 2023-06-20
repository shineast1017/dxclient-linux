using HCVK.HCVKAService.HCVKARequest;
using HCVK.HCVKAService.Resources;
using HCVK.HCVKSLibrary.VO;
using HCVK.HCVKSLibrary;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Timers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security;
using System.Management.Automation;
using System.Text;

// ref.
// IPC
//
// powershell 4.0 install for execution power shell
// add references system.managements.automation.dll in solution
// https://social.technet.microsoft.com/wiki/contents/articles/21016.how-to-install-windows-powershell-4-0.aspx
//


namespace HCVK.HCVKAService
{
	public class HCVKAMain
	{
		private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static EventLog _eventLog = new EventLog();


		// declare for broker server
		private VOBrokerServer _voBrokerServer = new VOBrokerServer();
		public VOBrokerServer voBrokerServer
		{
			get { return _voBrokerServer; }
		}

		// declare for logger server
		private VOBrokerServer _voLogServer = new VOBrokerServer();
		public VOBrokerServer voLogServer
		{
			get { return _voLogServer; }
		}




		// declare for rest command fetch handler
		private HCVKAFetchCommand _restFetchCommand = null;
		private string _strBindSecureUrl = string.Empty;
		private string _strBindUrl = string.Empty;
		private bool _bIsEnableTestInterfacePort = false;


		// declare for configure values
		private string _strInterfacePort = string.Empty;
		private string _strVDIPort = string.Empty;
		private string _strSSLKeyFile = string.Empty;
		private string _strEncodedSSLKeyPass = string.Empty;
		public string VDIPort
		{
			get { return _strVDIPort; }
		}


		// declare for HCVKATray
		private Timer _timerCheckHCVKATray = new Timer();
		private const int CHECK_INTERVAL_HCVKA_TRAY = 5000;


		// declare for HCVKAHttpsServer
		private Timer _timerCheckHCVKAHttpsServer = new Timer();
		private const int CHECK_INTERVAL_HTTPS_SERVER = 5000;


		// declare for registered to HCVKB
		private bool _bIsRegisteredToHCVKB = false;
		public bool IsRegisteredToHCVKB
		{
			set { _bIsRegisteredToHCVKB = value; }
			get { return _bIsRegisteredToHCVKB; }
		}


		// declare for re-registration info.
		private bool _bIsReRegistrationInfo = false;
		public bool IsReRegistrationInfo
		{
			set { _bIsReRegistrationInfo = value; }
			get { return _bIsReRegistrationInfo; }
		}

		// declare for flag of ready for rdp services.
		private bool _bIsReadyRDPService = false;
		public bool IsReadyRDPService
		{
			set { _bIsReadyRDPService = value; }
			get { return _bIsReadyRDPService; }
		}


		// declare for Integiry
		private List<VOIntegrity> _listIntegrity = new List<VOIntegrity>();
		private bool _bIsValidateIntegrity = false;
		public List<VOIntegrity> ListIntegrity
		{
			set { _listIntegrity = value; }
			get { return _listIntegrity; }
		}
		public bool IsValidateIntegrity
		{
			set { _bIsValidateIntegrity = value; }
			get { return _bIsValidateIntegrity; }
		}


		// declare for previous status for service
		private bool _bIsPrevReadyForService = false;
		private bool _bIsPrevIsValidateIntegrity = false;
		private bool _bIsPrevIsRegisteredToHCVKB = false;


		// declare for user
		private static VOUser _voUser = new VOUser();
		public VOUser voUser
		{
			get { return _voUser; }
		}
		private string _strServiceProtocol = string.Empty;
		public string ServiceProtocol
		{
			set { _strServiceProtocol = value; }
			get { return _strServiceProtocol; }
		}
		private string _strDesktopID = string.Empty;
		public string DesktopID
		{
			set { _strDesktopID = value; }
			get { return _strDesktopID; }
		}

#if WIN32
		// declare for firewall
		private HCVKAFirewall _hcvkaFirewall = new HCVKAFirewall();
#endif


		// declare for health report
		private static VOHeartbeat _voHeartbeart = new VOHeartbeat();
		private Timer _timerCheckHealthReport = new Timer();
		private const int CHECK_INTERVAL_HEALTH_REPORT_INIT = 15000;     // 10 sec.
		private const int CHECK_INTERVAL_HEALTH_REPORT = 60000;     // 60 sec.
		private const int CHECK_INTERVAL_HEARTBEAT = 60000;     // 60 sec.
		public VOHeartbeat voHeartbeart
		{
			get { return _voHeartbeart; }
		}


		// declare for log report
		private HCVKALogReport _hcvkaLogReport = new HCVKALogReport();
		public HCVKALogReport hcvkaLogReport
		{ get { return _hcvkaLogReport; } }






		public HCVKAMain()
		{
			_eventLog.Source = Properties.Resources.EVENT_LOG_NAME;
			ReadConfiguration();
		}

		~HCVKAMain()
		{
			_logger.Debug(string.Format("."));
		}


		public void InitializeHCVKAMain()
		{
			// ready for HCVK VDI Service
			HCVKARDPService.SetVDIPort(_strVDIPort);
			HCVKARDPService.DisableRDPService();

			// check for HCVK VDI Service
			_bIsReadyRDPService = HCVKARDPService.CheckAndStartWindowsRDPService();

			// get network information of HCVKA
			// get local network information
			Dictionary<string, string> mapIPAddress = NetworkManager.GetNetworkAdaptor();
			foreach (KeyValuePair<string, string> item in mapIPAddress)
			{
				// ghwanlim 180628 why??
				//string strClientMAC = NetworkManager.GetMacAddressFromIP(item.Value);

				_hcvkaLogReport.AgentIp = item.Value;
			}


			CheckIntegrity();
			GetReRegistrationInfo();

			InitializeFirewall();
			InitializeHttpServer();
            InitializeTimer();

			//ExecuteGPUpdateForce();

			// previoud registered node name
			_logger.Debug(string.Format("Node Name : {0}", Properties.Settings.Default.NodeName));

			// log to event log as service status 
			_bIsPrevReadyForService = IsReadyForService();
            _eventLog.WriteEntry(_bIsPrevReadyForService ? MultiLang.HCVKA_SERVICE_MSG_0003 : MultiLang.HCVKA_SERVICE_MSG_0004);
		}

		public void FinalizeHCVKAMain()
		{
			_logger.Debug(string.Format("."));
			FinalizeTimer();


			// return to default RDP Service
			HCVKARDPService.EnableRDPService();
			HCVKARDPService.SetDefaultRDPPort();


			FinalizeHttpServer();
			FinalizeFirewall();
		}


		private bool Callback_Integrity(object sender)
		{
			bool bReturn = false;
			try
			{
				string strHCVKAServicePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Properties.Resources.EXECUTE_NAME_FOR_HCVKASERVICE);
				string strHCVKATrayPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Properties.Resources.EXECUTE_NAME_FOR_HCVKATRAY);
				string strHCVKSLibraryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Properties.Resources.EXECUTE_NAME_FOR_HCVKSLIBRARY);


				// make information of integirty at first time 
				if (string.IsNullOrEmpty(Properties.Settings.Default.HCVKAService))
				{
					_logger.Debug(string.Format("Make information of integrity at first time"));

					Properties.Settings.Default.HCVKAService = CryptoManager.MakeHashFromFileBySHA256(strHCVKAServicePath);
					Properties.Settings.Default.HCVKATray = CryptoManager.MakeHashFromFileBySHA256(strHCVKATrayPath);
					Properties.Settings.Default.HCVKSLibrary = CryptoManager.MakeHashFromFileBySHA256(strHCVKSLibraryPath);

					Properties.Settings.Default.Save();
				}


				// check to integirty
				{
					_logger.Debug(string.Format("Check information of resoures integrity"));

					int nInvalideIntegirty = 0;


					// check target resoures
					{
						_listIntegrity.Clear();

						{
							VOIntegrity voIntegrity = new VOIntegrity();
							voIntegrity.targetName = strHCVKAServicePath;
							voIntegrity.expectedHash = Properties.Settings.Default.HCVKAService;
							voIntegrity.currentHash = CryptoManager.MakeHashFromFileBySHA256(strHCVKAServicePath);
							voIntegrity.isHomogeneous = voIntegrity.expectedHash.Equals(voIntegrity.currentHash) ? "true" : "false";
							nInvalideIntegirty = Convert.ToBoolean(voIntegrity.isHomogeneous) ? 0 : 1;
							_listIntegrity.Add(voIntegrity);

							_logger.Debug(string.Format("Check integrity: [{0}] {1}", voIntegrity.isHomogeneous, voIntegrity.targetName));
						}
						{
							VOIntegrity voIntegrity = new VOIntegrity();
							voIntegrity.targetName = strHCVKATrayPath;
							voIntegrity.expectedHash = Properties.Settings.Default.HCVKATray;
							voIntegrity.currentHash = CryptoManager.MakeHashFromFileBySHA256(strHCVKATrayPath);
							voIntegrity.isHomogeneous = voIntegrity.expectedHash.Equals(voIntegrity.currentHash) ? "true" : "false";
							nInvalideIntegirty = Convert.ToBoolean(voIntegrity.isHomogeneous) ? 0 : 1;
							_listIntegrity.Add(voIntegrity);

							_logger.Debug(string.Format("Check integrity: [{0}] {1}", voIntegrity.isHomogeneous, voIntegrity.targetName));
						}
						{
							VOIntegrity voIntegrity = new VOIntegrity();
							voIntegrity.targetName = strHCVKSLibraryPath;
							voIntegrity.expectedHash = Properties.Settings.Default.HCVKSLibrary;
							voIntegrity.currentHash = CryptoManager.MakeHashFromFileBySHA256(strHCVKSLibraryPath);
							voIntegrity.isHomogeneous = voIntegrity.expectedHash.Equals(voIntegrity.currentHash) ? "true" : "false";
							nInvalideIntegirty = Convert.ToBoolean(voIntegrity.isHomogeneous) ? 0 : 1;
							_listIntegrity.Add(voIntegrity);

							_logger.Debug(string.Format("Check integrity: [{0}] {1}", voIntegrity.isHomogeneous, voIntegrity.targetName));
						}
					}


					// check to pass counter
					if (nInvalideIntegirty == 0)
						bReturn = true;
				}
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception: {0}", ex.ToString()));
			}
			return bReturn;
		}

		public void CheckIntegrity()
		{
			_logger.Debug(string.Format("Start CheckIntegrity"));
#if WIN32
			try
			{
				IntegrityManager integrityMan = new IntegrityManager();
				_bIsValidateIntegrity = integrityMan.CheckIntegrity(new CallbackCheckIntegrity(Callback_Integrity));
				_logger.Debug(string.Format("End CheckIntegrity[{0}]", _bIsValidateIntegrity));
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}
#else
			_bIsValidateIntegrity = true;
#endif
		}


		private void InitializeFirewall()
		{
#if WIN32
			_hcvkaFirewall.AllowInterfacePort();
            _hcvkaFirewall.AllowVDIPort();
#endif
		}

		private void FinalizeFirewall()
		{

#if WIN32
			_logger.Debug(string.Format("."));
            _hcvkaFirewall.DeleteInterfacePort();
            _hcvkaFirewall.DeleteVDIPort();
#endif
		}


		private void ReadConfiguration()
		{
			try
			{
				// read configure
				_strInterfacePort = new ConfigManager() { }.GetAppConfig("InterfacePort");
				_bIsEnableTestInterfacePort = bool.Parse(new ConfigManager() { }.GetAppConfig("TestInterfacePort"));
				_strVDIPort = new ConfigManager() { }.GetAppConfig("VDIPort");
				_strSSLKeyFile = new ConfigManager() { }.GetAppConfig("SSLKeyFile");
				_strEncodedSSLKeyPass = new ConfigManager() { }.GetAppConfig("SSLKeyPass");
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}



			// set to local variables
			_strBindSecureUrl = string.Format("https://*:{0}/", _strInterfacePort);
			_strBindUrl = string.Format("http://*:{0}/", int.Parse(_strInterfacePort) + 1);
		}



		private void InitializeTimer()
		{
			try
			{
#if WIN32
				// check for HCVKA Tray Form
				{
                    _timerCheckHCVKATray.Interval = CHECK_INTERVAL_HCVKA_TRAY;
                    _timerCheckHCVKATray.Elapsed += new ElapsedEventHandler(timerCheckHCVKATray_Tick);
                    _timerCheckHCVKATray.Start();
                }
#endif


                // check for health report
                {
                    _timerCheckHealthReport.Interval = CHECK_INTERVAL_HEALTH_REPORT_INIT;
                    _timerCheckHealthReport.Elapsed += new ElapsedEventHandler(timerCheckHealthReport_Tick);
                    _timerCheckHealthReport.Start();
                }


                // check for https server
                {
                    _timerCheckHCVKAHttpsServer.Interval = CHECK_INTERVAL_HTTPS_SERVER;
                    _timerCheckHCVKAHttpsServer.Elapsed += new ElapsedEventHandler(timerCheckHCVKAHttpsServer_Tick);
                    _timerCheckHCVKAHttpsServer.Start();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        private async void FinalizeTimer()
        {
            _logger.Debug(string.Format("."));
            try
            {
                // Kill Https Server timer 
                {
                    _timerCheckHCVKAHttpsServer.Stop();
                }

                // Kill timer & HCVKA Tray Form
                {
                    _timerCheckHCVKATray.Stop();

                    ProcessManager processMan = new ProcessManager();
                    processMan.KillAllProcessByName(Properties.Resources.PROCESS_NAME_FOR_HCVKATRAY);
                }

                // Kill Health Report timer 
                {
                    _timerCheckHealthReport.Stop();


                    // report health for stopping service of agent
                    {
                        VOHeartbeat voHeartbeat = new VOHeartbeat();
                        lock (_voHeartbeart)
                        {
                            _voHeartbeart.IsClientConnected = false;
                            voHeartbeat = _voHeartbeart.Duplicate();
                        }


                        await new HCVKBRequestToHCVKL().RequestHealthReport(_voBrokerServer, voHeartbeat, VOErrorCode._E_CODE_A_A0000104);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        private void InitializeHttpServer()
        {
            try
            {
                string strCertPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _strSSLKeyFile);
                {
                    // initialize fetch command
                    _restFetchCommand = new HCVKAFetchCommand(this);
                    _restFetchCommand.EncodedSSLKeyPass = _strEncodedSSLKeyPass;
                    _restFetchCommand.InterfacePort = _strInterfacePort;
                    _restFetchCommand.SSLKeyPath = strCertPath;

                    _restFetchCommand.InitializeRestfulTemplate(_bIsEnableTestInterfacePort);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        private void FinalizeHttpServer()
        {
            _logger.Debug(string.Format("."));
            try
            {
                _restFetchCommand.FinalizeRestfulTemplate();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        private string Callback_Request(HttpListenerRequest request)
        {
            _logger.Debug(string.Format("Request Command : [{0}]{1}", request?.HttpMethod, request?.Url));
            return string.Format("<HTML><BODY>HCVKAService Response.<br>Current Time : {0}</BODY></HTML>", DateTime.Now);
        }

        private void ExecuteGPUpdateForce()
        {
            try
            {
                String strGPUpdateForceCmd = "GPUpdate.exe /force";

                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.StandardInput.WriteLine(strGPUpdateForceCmd);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit(1);

                _logger.Debug(string.Format("Result execution of Group Policy update  : [{0}]", cmd.StandardOutput.ReadToEnd()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        private void LaunchProcessHCVKATray()
        {
            //_logger.Debug(string.Format("Launch HCVKATray Form..."));
            try
            {
                string strHCVKATrayPath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                    Properties.Resources.EXECUTE_NAME_FOR_HCVKATRAY);
                //_logger.Debug(string.Format("HCVKATray Form Path : {0}", strHCVKATrayPath));

                ProcessManager processMan = new ProcessManager();
                processMan.LaunchProcessAsCurrentSession(strHCVKATrayPath);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        public bool ExecutDomainJoin(string strDomainName, string strEncodedDomainAdminId, string strEncodedDomainAdminPW)
        {
            bool bReturn = false;

            try
            {
                string strDomainAdminId = CryptoManager.DecodingBase64(strEncodedDomainAdminId);
                string strDomainAdminPW = CryptoManager.DecodingBase64(strEncodedDomainAdminPW);

                _logger.Debug(string.Format("Domain Info. : [{0}\\{1}:{2}]", strDomainName, strDomainAdminId, strDomainAdminPW));


                int nDelayTime = 5; // 5 sec.
                string strDomainAccount = string.Format("{0}\\{1}", strDomainName, strDomainAdminId);
                SecureString securePassword = new SecureString();

                foreach (char c in strDomainAdminPW)
                {
                    securePassword.AppendChar(c);
                }
                PSCredential credential = new PSCredential(strDomainAccount, securePassword);
                using (PowerShell psInstance = PowerShell.Create())
                {
                    // set parameter with credential
                    psInstance.AddCommand("Set-Variable");
                    psInstance.AddParameter("Name", "credential");
                    psInstance.AddParameter("Value", credential);

                    // set script
                    psInstance.AddScript(string.Format("Add-Computer -DomainName {0} -Credential $credential", strDomainName));
                    psInstance.AddScript(string.Format("Restart-Computer"));


                    // execute script
                    var varResults = psInstance.Invoke();
                    if (psInstance.Streams.Error.Count > 0)
                    {
                        Exception psEx = psInstance.Streams.Error[0].Exception;
                        psInstance.Streams.ClearStreams();

                        throw psEx;
                    }

                    // log result
                    StringBuilder strResult = new StringBuilder();
                    foreach(var varLine in varResults)
                    {
                        if (varLine == null)
                            continue;

                        strResult.AppendLine(varLine.ToString());
                    }
                    _logger.Debug(string.Format("Result of execution : [{0}]", strResult.ToString()));

                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }

            return bReturn;
        }


        public bool IsReadyForService()
        {
            return _bIsRegisteredToHCVKB & _bIsValidateIntegrity & _bIsReadyRDPService;
        }

        private void GetReRegistrationInfo()
        {
            try
            {
                // check to have been set to node name in case not registration
                if (!string.IsNullOrEmpty(Properties.Settings.Default.NodeName))
                {
                    _voBrokerServer.NodeName = Properties.Settings.Default.NodeName;
                    _voBrokerServer.BrokerServerIP = Properties.Settings.Default.BrokerServerIP;
                    _voBrokerServer.BrokerServerPort = Properties.Settings.Default.BrokerServerPort;

                    _voLogServer.NodeName = Properties.Settings.Default.NodeName;
                    _voLogServer.BrokerServerIP = Properties.Settings.Default.LogServerIP;
                    _voLogServer.BrokerServerPort = Properties.Settings.Default.LogServerPort;

                    _bIsReRegistrationInfo = true;

                    _logger.Debug(string.Format("Re registration with nodename[{0}] to broker server[{1}:{2}], log report server[{3}:{4}]",
                        _voBrokerServer.NodeName,
                        _voBrokerServer.BrokerServerIP, _voBrokerServer.BrokerServerPort,
                        _voLogServer.BrokerServerIP, _voLogServer.BrokerServerPort));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        // Event handler for timer
        private void timerCheckHCVKATray_Tick(object sender, EventArgs e)
        {
            //_logger.Debug(string.Format("Check to alive HCVKATray Form ... : ServiceStatus[{0}]", IsReadyForService()));
            try
            {
                string strMessage = IsReadyForService() ? MultiLang.HCVKA_SERVICE_MSG_0003 : MultiLang.HCVKA_SERVICE_MSG_0005;
                string strErrorCode = IsReadyForService() ? VOErrorCode._E_CODE_OK : VOErrorCode._E_CODE_A_A0000001;

                // making error message and code.
                {
                    if (!IsReadyForService())
                    {
                        // _E_CODE_A_A0000101 or _E_CODE_A_A0000102
                        if (!_bIsRegisteredToHCVKB)
                        {
                            if (!_bIsReRegistrationInfo)
                            {
                                strMessage = MultiLang.HCVKA_SERVICE_MSG_0006;
                                strErrorCode = VOErrorCode._E_CODE_A_A0000101;
                            }
                            else
                            {
                                strMessage = MultiLang.HCVKA_SERVICE_MSG_0007;
                                strErrorCode = VOErrorCode._E_CODE_A_A0000102;
                            }
                        }
                        // _E_CODE_X_X0000201
                        else if (!_bIsValidateIntegrity)
                        {
                            strMessage = MultiLang.HCVKA_SERVICE_MSG_0008;
                            strErrorCode = VOErrorCode._E_CODE_X_X0000201;
                        }
                        // _E_CODE_A_A0000103
                        else if (!_bIsReadyRDPService)
                        {
                            strMessage = MultiLang.HCVKA_SERVICE_MSG_0009;
                            strErrorCode = VOErrorCode._E_CODE_A_A0000103;
                        }
                    }
                }

                // log to event log when change to service status 
                if (_bIsPrevReadyForService != IsReadyForService())
                {
                    _bIsPrevReadyForService = IsReadyForService();

                    _eventLog.WriteEntry(strMessage);
                }


                // check ti alive process as HCVKATray
                ProcessManager processMan = new ProcessManager();
                if (!processMan.CheckProcessAlive(Properties.Resources.PROCESS_NAME_FOR_HCVKATRAY))
                {
                    //_logger.Debug(string.Format("Not founded the process of HCVKATray, Re-launch HCVKATray.."));

                    LaunchProcessHCVKATray();
                    System.Threading.Thread.Sleep(100);
                }

                // send message to HCVKATray as service status 
                SendServiceStatusToHCVKATray(strErrorCode);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }
        private void timerCheckHealthReport_Tick(object sender, EventArgs e)
        {
            //_logger.Debug(string.Format("Time to report of health ... : ServiceStatus[{0}]", IsReadyForService()));
            try
            {
                // checking to reday for rdp servce
                _bIsReadyRDPService = HCVKARDPService.CheckAndStartWindowsRDPService();


                if (!_bIsRegisteredToHCVKB)
                {
                    // to determine whether to log, compare status for service
                    if (_bIsPrevIsRegisteredToHCVKB != _bIsRegisteredToHCVKB)
                    {
                        _bIsPrevIsRegisteredToHCVKB = _bIsRegisteredToHCVKB;
                        _bIsPrevIsValidateIntegrity = _bIsValidateIntegrity;

                        _logger.Debug(string.Format("Skip.. because of not ready for service..[Registration:{0}]", _bIsRegisteredToHCVKB));
                    }


                    // check to have been set to node name in case not registration and request to re-registration
                    if (_bIsReRegistrationInfo)
                    {
                        new HCVKBRequestToHCVKL().RequestReRegistration_AsyncCallback(_voBrokerServer, Callback_ReRegistration);
                    }
                    else
                    {
                    }
                    return;
                }


                // reset timer for period of health repoort
                if (_timerCheckHealthReport.Interval != CHECK_INTERVAL_HEALTH_REPORT)
                {
                    _timerCheckHealthReport.Stop();

                    _timerCheckHealthReport.Interval = CHECK_INTERVAL_HEALTH_REPORT;
                    _timerCheckHealthReport.Start();
                }


                VOHeartbeat voHeartbeat = new VOHeartbeat();
                lock (_voHeartbeart)
                {
                    // check validation session time 
                    if (_voHeartbeart.IsClientConnected && (CommonUtils.UnixTimeStampToDateTime(_voHeartbeart.LastHeartbeatDt+ CHECK_INTERVAL_HEARTBEAT) > DateTime.Now))
                    {
                        // update heartbeat info.
                        _voHeartbeart.IsClientConnected = true;
                    }
                    else
                    {
                        // update heartbeat info.
                        _voHeartbeart.IsClientConnected = false;
                        //_logger.Debug(string.Format("Time over to be checked from last heartbeat time..."));
                    }

                    voHeartbeat = _voHeartbeart.Duplicate();
                }

                string strMessage = IsReadyForService() ? MultiLang.HCVKA_SERVICE_MSG_0003 : MultiLang.HCVKA_SERVICE_MSG_0005;
                string strErrorCode = IsReadyForService() ? VOErrorCode._E_CODE_OK : VOErrorCode._E_CODE_A_A0000001;

                // making error message and code.
                {
                    if (!IsReadyForService())
                    {
                        // _E_CODE_A_A0000101 or _E_CODE_A_A0000102
                        if (!_bIsRegisteredToHCVKB)
                        {
                            if (!_bIsReRegistrationInfo)
                            {
                                strMessage = MultiLang.HCVKA_SERVICE_MSG_0006;
                                strErrorCode = VOErrorCode._E_CODE_A_A0000101;
                            }
                            else
                            {
                                strMessage = MultiLang.HCVKA_SERVICE_MSG_0007;
                                strErrorCode = VOErrorCode._E_CODE_A_A0000102;
                            }
                        }
                        // _E_CODE_X_X0000201
                        else if (!_bIsValidateIntegrity)
                        {
                            strMessage = MultiLang.HCVKA_SERVICE_MSG_0008;
                            strErrorCode = VOErrorCode._E_CODE_X_X0000201;
                        }
                        // _E_CODE_A_A0000103
                        else if (!_bIsReadyRDPService)
                        {
                            strMessage = MultiLang.HCVKA_SERVICE_MSG_0009;
                            strErrorCode = VOErrorCode._E_CODE_A_A0000103;
                        }
                    }
                }

                // report health
                new HCVKBRequestToHCVKL().RequestHealthReport_AsyncCallback(
                    _voBrokerServer, voHeartbeat, strErrorCode, Callback_CheckHealthReport);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }
        private void Callback_CheckHealthReport(JObject resJsonObject, Exception exParam)
        {
            _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

            try
            {
                if (resJsonObject != null)
                {
                    if (resJsonObject[HCVKARequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                    {
                        // success response
                        // fetch auth token
                        {
                            JObject oToken = JObject.Parse(resJsonObject[HCVKARequestJSONParam.RESPONSE_RESULT_DATA]["tokenInfo"].ToString());

                            _voBrokerServer.AuthToken = oToken["token"].ToString();
                            _voBrokerServer.Expiration = CommonUtils.UnixTimeStampToDateTime(long.Parse(oToken["expiration"].ToString()));

                            //_voLogServer.AuthToken = oToken["token"].ToString();
                            //_voLogServer.Expiration = CommonUtils.UnixTimeStampToDateTime(long.Parse(oToken["expiration"].ToString()));
                        }
                    }
                    else
                    {
                        // failure response
                        // re-registration when time exipired auth token
                        if (resJsonObject[HCVKARequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals(VOErrorCode._E_CODE_B_B0001003))
                        {
                            _logger.Debug(string.Format("Expired time of token : {0}", _voBrokerServer.Expiration.ToLocalTime()));

                            new HCVKBRequestToHCVKL().RequestReRegistration_AsyncCallback(_voBrokerServer, Callback_ReRegistration);
                        }
                    }
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

        private void Callback_ReRegistration(JObject resJsonObject, Exception exParam)
        {
            _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

            try
            {
                if (resJsonObject != null)
                {
                    if (resJsonObject[HCVKARequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                    {
                        // success response
                        // fetch auth token
                        {
                            JObject oToken = JObject.Parse(resJsonObject[HCVKARequestJSONParam.RESPONSE_RESULT_DATA]["tokenInfo"].ToString());

                            _voBrokerServer.AuthToken = oToken["token"].ToString();
                            _voBrokerServer.Expiration = CommonUtils.UnixTimeStampToDateTime(long.Parse(oToken["expiration"].ToString()));

                            _voLogServer.AuthToken = oToken["token"].ToString();
                            _voLogServer.Expiration = CommonUtils.UnixTimeStampToDateTime(long.Parse(oToken["expiration"].ToString()));


                            _logger.Debug(string.Format("Token : {0}", _voBrokerServer.AuthToken));
                            _logger.Debug(string.Format("Next expiration time of token : {0}", _voBrokerServer.Expiration.ToLocalTime()));
                        }

                        _bIsRegisteredToHCVKB = true;
                        _bIsReRegistrationInfo = true;


                        // report health for stopping service of agent
                        {
                            VOHeartbeat voHeartbeat = new VOHeartbeat();
                            lock (_voHeartbeart)
                            {
                                _voHeartbeart.IsClientConnected = false;
                                voHeartbeat = _voHeartbeart.Duplicate();
                            }


                            // reset timer for period of health repoort
                            if (_timerCheckHealthReport.Interval != CHECK_INTERVAL_HEALTH_REPORT)
                            {
                                _timerCheckHealthReport.Stop();

                                _timerCheckHealthReport.Interval = CHECK_INTERVAL_HEALTH_REPORT;
                                _timerCheckHealthReport.Start();
                            }

                            new HCVKBRequestToHCVKL().RequestHealthReport_AsyncCallback(_voBrokerServer, voHeartbeat, VOErrorCode._E_CODE_OK, Callback_CheckHealthReport);
                        }


                        // report log to HCVKL
                        _hcvkaLogReport.LogReport_Common_ReRegistration(VOLogData.STATUS_SUCCEED, string.Empty);
                    }
                    else
                    {
                        // failure response


                        // report log to HCVKL
                        _hcvkaLogReport.LogReport_Common_ReRegistration(
                            VOLogData.STATUS_FAILED, resJsonObject[HCVKARequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                    }
                }
            }
            catch (WebException wex)
            {
                _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));


                // report log to HCVKL
                _hcvkaLogReport.LogReport_Common_ReRegistration(VOLogData.STATUS_FAILED, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));


                // report log to HCVKL
                _hcvkaLogReport.LogReport_Common_ReRegistration(VOLogData.STATUS_FAILED, string.Empty);
            }
        }



        private void timerCheckHCVKAHttpsServer_Tick(object sender, EventArgs e)
        {
            //_logger.Debug(string.Format("Check to alive HttpsServer... : Is running[{0}]", _restFetchCommand.IsRunning));
            try
            {
                if (!_restFetchCommand.IsRunning)
                {
                    _logger.Debug(string.Format("Restart Https Server..."));
                    InitializeHttpServer();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        private void Callback_LogReport(JObject resJsonObject, Exception exParam)
        {
            _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

            try
            {
                if (resJsonObject != null)
                {
                    if (resJsonObject != null && resJsonObject[HCVKARequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                    {
                        // success response
                        // fetch auth token
                        //{
                        //    JObject oToken = JObject.Parse(resJsonObject[HCVKCRequestJSONParam.RESPONSE_RESULT_DATA]["tokenInfo"].ToString());
                        //    _voBrokerServer.AuthToken = oToken["token"].ToString();
                        //    _voLogServer.AuthToken = oToken["token"].ToString();
                        //}
                    }
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


        //-------------------------------------------------------------------------------------
        // IPC between HCVKAService and HCVKATray
        private void SendServiceStatusToHCVKATray(string strErrorCode)
        {
            try
            {
                IPCPipeClient _pipeClient = new IPCPipeClient();
                _pipeClient.Send(strErrorCode, Properties.Resources.IPC_SERVER_NAME, 1000);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }
        //-------------------------------------------------------------------------------------
    }
}
