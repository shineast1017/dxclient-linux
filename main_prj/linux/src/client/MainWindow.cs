using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml.Serialization;
using client;
using client.Request;
using Gtk;
using HCVK.HCVKSLibrary;
using HCVK.HCVKSLibrary.VO;
using log4net;
using Mono.Unix;
using Newtonsoft.Json.Linq;
using Mono.Unix.Native;

public partial class MainWindow : Gtk.Window
{
	static public MainWindow mainWindow;
	private int _nToggleButtonState = -1;

	private global::client.mainPageWidget _mainpagewidget = null;
	private global::client.bookmarkPageWidget _bookmarkpagewidget = null;
	private global::client.settingPageWidget _settingkPageWidget = null;

	private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public ClientEnvironment environment { get; set; }

	public VOBrokerServerNew _voSelectedBrokerServer = new VOBrokerServerNew();
    public VOUser _voUser = new VOUser();
    public VOBookmark _voBookmark = null;

	public bool _bRunningAutoStart = false;
    public bool _bRunDirectAutoConnect = false;

	// declare for log report
    private LogReport _logReport = new LogReport();
    public LogReport hcvkcLogReport
    { get { return _logReport; } }

	private string _desktopInstanceID;

	private List<VODesktopPool> _listDesktopPool = new List<VODesktopPool>();

    private bool _xmlSave = true;

    public int FreeRDPVer = 1;

    public string strCurDaaS_USB_REDIRECT = string.Empty;
    private string strPreDaaS_USB_REDIRECT = string.Empty;
	public string _strVDI_Connect_MODE = string.Empty;

    private System.Timers.Timer timerCheckWEBCAMCONTROL = null;
    private const int CHECK_INTERVAL_CHECKWEBCAM = 1000;    // 1 sec.

	// Broker System Notice Message Request
	private System.Timers.Timer _timerRequestCheckedMessage = null;

	// Broker server manager
	private BrokerNoticeMsgManager _NoticeMsgManager = new BrokerNoticeMsgManager();
	// Check new message, while thread Loop Runflag
	private static bool _thread_get_broker_newmsg_run_flag = true;
	private Thread _thread_msg_response = null;

	private static bool _thread_Signal_msg_run_flag = true; // Flag for Signal Thread
    public Thread _thread_Signal_msg_response = null; // Thread for Signal (AutoConnectRDP)

	private string _strConfigFileName = string.Empty;

	public string _curConnectPoolID = string.Empty; // 현재 마지막 접속한  Desktop pool ID;
	public string _strUSBIPConnectStatus = string.Empty; // 현재 usbip와 접속상태

	public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
		startThread_SignalMessage ();
		String [] CommandArgs =  System.Environment.GetCommandLineArgs ();

		ProgramOptions (CommandArgs);

		log4net.Config.XmlConfigurator.Configure ();

		StyleSheet.Initialize();

        mainWindow = this;

        CheckFreeRDPVersion();


		//If not exist config directory, create config directory
		//and load config.ini
		LoadConfiguration ();
        //If not exist config.ini, create config.ini
        CreateEnvironment();

		InitializeSetting ();

		RearrangeBrokerServer ();

		//broker 서버 ip를  ConfigIp로 복구합니다. 
		foreach (VOBrokerServerNew broker in this.environment.BrokerServers) {
			if (broker.ConfigIP != null && broker.ConfigIP.Length > 0)
				broker.BrokerServerIP = broker.ConfigIP;
		}


		Catalog.Init("DaaSXpertClient", "/usr/share/locale");

        // Make MainWindow(MainPageWidget, LeftMenu etc.)
        Build();

        // Make MainPage, BookmarkPage, SettingPage
        MakeMainPages();

        StartNetworkAdaptorCheckThread();

        //MessageBox
        ErrorHandlerManager.Initialize();

        // Enable Bookmark
		if (environment.Bookmarks.Count > 0 && environment.vOGeneralsProperties.IsUseBookmark)
			ChangePage(1);
		else
		    ChangePage(0);

        // style
		this.ModifyBg(StateType.Normal, new Gdk.Color(57, 58, 63));
		this.eventboxState.ModifyBg(StateType.Normal, new Gdk.Color(87, 100, 108));
		this.labelState.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));

		this.eventboxTitlebar.Hide();
        //this.eventboxTitlebar.ModifyBg(StateType.Normal, new Gdk.Color(45, 48, 65));
        //this.labelTitlebar.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 255));

		if (MainWindow.mainWindow.environment.vOGeneralsProperties.UseBrokerFilterTag == true) {
			_logger.Info (string.Format ("Read File lightdm.mode : ") + GetVDIConnectMODE ());
		}


		// kogas thin client Requirements
		if (environment.BrokerServers.Count == 0)
        {
            _mainpagewidget.ShowAddServerPage(0, 1);
        }
        else
        {

            // ShowWindow Login Page default select item 0=1
            _mainpagewidget.ShowLoginPageFromServerList(0);
            /*
            MainFunc.GetBrokerServerAll().ForEach(
                broker =>
                {
                    //_mainpagewidget.ShowLogInPage(broker);
                    //serverItemWidget itemWidget = new serverItemWidget();
                    //itemWidget.BrokerServerInfo = broker;
                    //itemWidget.SetServerInfo();
                });
                */
        }

        InitializeTimers();

        AfterEffectInitializeComponent();




		if (environment.vOUSBDeviceProperties.IsUseAPIWebCamCtrl == true) {


			HancomGooroomDeviceManager.GetInstance ().SetUSBOwnerCondition (environment.vOGeneralsProperties.AppNamePrefix == "BizVDI");
			HancomGooroomDeviceManager.GetInstance ().Setup ();
			HancomGooroomDeviceManager.GetInstance ().takeAction = new HancomGooroomDeviceManager.TakeUSBDeviceCallback (TakeAction);

			if (environment.vOGeneralsProperties.AppNamePrefix == "BizVDI") {
				this.strCurDaaS_USB_REDIRECT = "1";
			} else {
				this.strCurDaaS_USB_REDIRECT = "0";
			}
		}

	}
	public void TakeAction(bool isTake)
	{

		if (mainWindow.environment.vORedirectionProperties.IsLocalVideoDevice == false)
			return;


		string strWebCamDeviceID = GetWebCamDevicdId ();

		if(strWebCamDeviceID != "") {
			if (isTake) {
				this.strCurDaaS_USB_REDIRECT = "1";

				if(mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection == "ALL" ||
				_strVDI_Connect_MODE.Contains(mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection)) {
					CtrlDeviceRedirection ("bind", strWebCamDeviceID);
				} else {
					RestartDesktop ();
				}

			} else {

				this.strCurDaaS_USB_REDIRECT = "0";

				if (mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection == "ALL" ||
						_strVDI_Connect_MODE.Contains (mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection)) {
					CtrlDeviceRedirection ("unbind", strWebCamDeviceID);
				} else {
					RestartDesktop ();
				}
			}

		} else {
			_logger.Debug (string.Format ("[TakeAction] No Webcam Device ID"));


		}



	}

	protected void OnEventboxButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
    {
        if (args.Event.Button == 1)
            DisconnectDesktopAll();
    }

    public void DisconnectDesktopAll()
    {
        if (_listDesktopPool.Count > 0)
        {
            foreach (VODesktopPool vODesktopPool  in _listDesktopPool)
            {
                DisconnectDesktop(vODesktopPool);
            }
        }
    }

    public void StartNetworkAdaptorCheckThread()
    {
        // check network adpator
        Thread networkAdaptorCheckthread = new Thread(new ThreadStart(CheckNetworkAdaptor));
        networkAdaptorCheckthread.Start();
    }

    void CreateEnvironment()
    {
        if (environment.BrokerServers == null) {
			environment.BrokerServers = new List<VOBrokerServerNew> ();
		}
		if (environment.vODisplayProperties == null)
            environment.vODisplayProperties = new VODisplayProperties();
        if (environment.vOAutoLoginProperties == null)
            environment.vOAutoLoginProperties = new VOAutoLoginProperties();
        if (environment.vOPerformanceProperties == null)
        {
            environment.vOPerformanceProperties = new VOPerformanceProperties();
            /*
            if(FreeRDPVer == 2)
            {
                environment.vOPerformanceProperties.EtcProperties = "/gfx-progressive /fonts";
            }
            */
        }
        if (environment.vOGeneralsProperties == null)
            environment.vOGeneralsProperties = new VOGeneralsProperties();
        if (environment.Bookmarks == null)
            environment.Bookmarks = new List<VOBookmark>();

        if (environment.vOGeneralsProperties.SetLanguage == "en-US")
            Environment.SetEnvironmentVariable("LANGUAGE", "en");
        else if (environment.vOGeneralsProperties.SetLanguage == "ko-KR")
            Environment.SetEnvironmentVariable("LANGUAGE", "ko");
    }

    void CheckFreeRDPVersion()
    {
        Process p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = "xfreerdp";
        p.StartInfo.Arguments = "--version";
        p.Start();

        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        // This is FreeRDP version 3.0.0-dev (n/a)
        // This is FreeRDP version 2.0.0-dev (git n/a)
        output = output.Substring(24, 2);

        // add support xfreerdp 3.0.0.0
        if (string.Compare(output, "3.") == 0)
            FreeRDPVer = 3;
        else if (string.Compare(output, "2.") == 0)
            FreeRDPVer = 2;
        else
            FreeRDPVer = 1;

	}

    public void SetTextOfLabelState(String str)
    {
		// Title명 앞에 prefix를 추가합니다. 
		if (MainWindow.mainWindow.environment.vOGeneralsProperties.AppNamePrefix != "") {
			this.labelState.Text = MainWindow.mainWindow.environment.vOGeneralsProperties.AppNamePrefix + "_";
		} else {
			this.labelState.Text = "";
		}

		this.labelState.Text += str;
    }

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
		DisconnectDesktopAll ();

		ErrorHandlerManager.Dispose();



		CloseProcess ();


        Application.Quit();
        a.RetVal = true;

    }

	public void CloseProcess()
	{

		//broker 서버 ip를, 설정에서  load한 값으로 변경하여, 저장합니다 
		foreach (VOBrokerServerNew broker in this.environment.BrokerServers) {
			if (broker.ConfigIP != null && broker.ConfigIP.Length > 0)
				broker.BrokerServerIP = broker.ConfigIP;
		}


		SaveConfiguration ();

		ReleaseAllTimers ();
	}

	private void ReleaseAllTimers()
	{
		if (timerCheckWEBCAMCONTROL != null && timerCheckWEBCAMCONTROL.Enabled) {
			_logger.Debug (string.Format ("Stop check WEBCAM CTRL timer <==============="));
			timerCheckWEBCAMCONTROL.Stop ();
			timerCheckWEBCAMCONTROL.Close ();
			timerCheckWEBCAMCONTROL = null;
		}

		if (_timerCheckBrokerServerState != null) {
			if (_timerCheckBrokerServerState.Enabled == true) {
				_timerCheckBrokerServerState.Stop ();
			}
			_timerCheckBrokerServerState.Close ();
			_timerCheckBrokerServerState = null;
			_logger.Debug (string.Format ("Stop check Broker Server State timer <==============="));

		}

		// Release Thread
		_thread_get_broker_newmsg_run_flag = false;

		ReleaseMessageThread ();

		_thread_Signal_msg_run_flag = false;

		ReleaseSignalMessageThread ();

		StopBrokerSystemMessageReceiver ();

	}


	public void StopBrokerSystemMessageReceiver ()
	{
		// Stop System Message timer
		if (_timerRequestCheckedMessage != null && _timerRequestCheckedMessage.Enabled) {
			_timerRequestCheckedMessage.Stop ();
			_timerRequestCheckedMessage.Close ();
			_timerRequestCheckedMessage = null;
		}

		// Release Thread
		_thread_get_broker_newmsg_run_flag = false;

		ReleaseMessageThread ();

		_thread_Signal_msg_run_flag = false;

		ReleaseSignalMessageThread ();

	}

	public void startAutoLogin()
    {
        // Enable AutoStart
        if (environment.vOAutoLoginProperties.IsAutoLogin)
        {
            string strServerIP = this.environment.vOAutoLoginProperties.ServerIP;
            string strServerPort = this.environment.vOAutoLoginProperties.ServerPort.ToString();

            VOBrokerServerNew brokerServerNew = (VOBrokerServerNew)this.environment.BrokerServers.FirstOrDefault(b => b.BrokerServerIP == strServerIP && b.BrokerServerPort == strServerPort);
            if (brokerServerNew != null)
            {
                _bRunningAutoStart = true;

                this.ExcuteLogin(brokerServerNew, this.environment.vOAutoLoginProperties.UserID, this.environment.vOAutoLoginProperties.UserPW);
            }
        }else if( environment.vOGeneralsProperties.IsAutoConnectRDP == true) {
			// AutoConnectionRDP 은 브로커 서버 정보와, ID/PW가 존재하는 경우 로그인을 진행합니다.
			if( environment.BrokerServers.Count > 0 && environment.vOGeneralsProperties.AutoConnectRDPID != "" && environment.vOGeneralsProperties.AutoConnectRDPPW != "") {
				string strServerIP = environment.BrokerServers [0].BrokerServerIP;
				VOBrokerServerNew brokerServerNew = (VOBrokerServerNew)this.environment.BrokerServers.FirstOrDefault (b => b.BrokerServerIP == strServerIP );
				if (brokerServerNew != null) {
					_bRunningAutoStart = true;
					ExcuteLogin (brokerServerNew, environment.vOGeneralsProperties.AutoConnectRDPID, environment.vOGeneralsProperties.AutoConnectRDPPW);
				}
			}
		}
	}

    public void SetNetworkInfo()
    {
        // get local network information
        KeyValuePair<string, string> item = NetworkManager.GetNetworkAdaptor().ElementAt<KeyValuePair<string, string>>(
            this.environment.vOGeneralsProperties.NetworkAdpatorIdx);

        string strClientMAC = NetworkManager.GetMacAddressFromIP(item.Value);
        _voUser.ClientIP = item.Value;
        _voUser.ClientMAC = strClientMAC;
        _voUser.DeviceType = client.Properties.Resources.MY_DEVICE_TYPE;
        //_voUser.DeviceOSVersion = CommonUtils.GetOSVersion();
    }

    public void CheckNetworkAdaptor()
    {
        int t = 1; // second
        int n = 10; // number
        for (int i = 0; i < n; i++)
        {
            labelState.Text = Catalog.GetString("Network checking...") + " (" + (i + 1) + "/" + n + ")";

            try
            {
                SetNetworkInfo();
            }
            catch (Exception)
            {
                environment.vOGeneralsProperties.NetworkAdpatorIdx = 0;

                if ( i == (n - 1))
                {
                    /*
                    MessageDialog md = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Ok, Catalog.GetString("Please check your network connection. If you check a network status, press refresh button on general tab of setting page."));
                    md.Run();
                    md.Destroy();
                    */
                    labelState.Text = Catalog.GetString("Please check your network connection. After checking network status,\nPress refresh button on general tab of setting page.");
                }
            }

            if (_voUser.ClientIP != "")
            {


				if (MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleEnable == true) {
					MainWindow.mainWindow.SetTextOfLabelState (MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleText);
				} else {
					MainWindow.mainWindow.SetTextOfLabelState ("DaaSXpert Client");
				}

                this._settingkPageWidget.SetNetwork();
                startAutoLogin();
                break;
            }

            Thread.Sleep(t * 1000);
        }
    }

	private void MakeMainPages()
	{
		if (this._mainpagewidget == null)
        {
            this._mainpagewidget = new global::client.mainPageWidget();
            this._mainpagewidget.WidthRequest = 520;
            this._mainpagewidget.HeightRequest = 377;
            this._mainpagewidget.Events = ((global::Gdk.EventMask)(256));
			this._mainpagewidget.Name = "mainpagewidget";
        }
		if (this._bookmarkpagewidget == null)
        {
			this._bookmarkpagewidget = new global::client.bookmarkPageWidget();
			this._bookmarkpagewidget.WidthRequest = 520;
			this._bookmarkpagewidget.HeightRequest = 377;
			this._bookmarkpagewidget.Events = ((global::Gdk.EventMask)(256));
			this._bookmarkpagewidget.Name = "bookmarkpagewidget";
        }
		if (this._settingkPageWidget == null)
        {
			this._settingkPageWidget = new global::client.settingPageWidget();
			this._settingkPageWidget.WidthRequest = 520;
			this._settingkPageWidget.HeightRequest = 377;
			this._settingkPageWidget.Events = ((global::Gdk.EventMask)(256));
			this._settingkPageWidget.Name = "settingpagewidget";
        }
		this.vboxPage.Add(this._mainpagewidget);
		this.vboxPage.Add(this._bookmarkpagewidget);
		this.vboxPage.Add(this._settingkPageWidget);

    }
	public void ChangePage(int nPage)
	{
		if (_nToggleButtonState == nPage)
			return;
		_nToggleButtonState = nPage;

		if (MainFunc.callbackSelectedLeftItem != null)
			MainFunc.callbackSelectedLeftItem(nPage);

		global::Gtk.Box.BoxChild w3;
		switch(_nToggleButtonState)
		{
			case 0:

    			//this.vboxPage.Add(this._mainpagewidget);
                w3 = ((global::Gtk.Box.BoxChild)(this.vboxPage[this._mainpagewidget]));
                w3.Position = 0;

				this._mainpagewidget.Show();
				this._bookmarkpagewidget.Hide();
				this._settingkPageWidget.Hide();
				break;
			case 1:
				w3 = ((global::Gtk.Box.BoxChild)(this.vboxPage[this._bookmarkpagewidget]));
                w3.Position = 0;

				this._mainpagewidget.Hide();
				this._bookmarkpagewidget.Show();
                this._settingkPageWidget.Hide();
				break;
			case 2:
				w3 = ((global::Gtk.Box.BoxChild)(this.vboxPage[this._settingkPageWidget]));
                w3.Position = 0;

				this._mainpagewidget.Hide();
                this._bookmarkpagewidget.Hide();
				this._settingkPageWidget.Show();
                break;

		}
	}

	public void LoadConfiguration()
	{
		string sIniFileName = string.Empty;

		string sIniFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/.config/DaaSXpert";
		string sProgramPath = System.IO.Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);

		if(_strConfigFileName == string.Empty) {
			sIniFileName = sIniFilePath + "/" + ConfigurationManager.AppSettings ["ConfigrationPath"];
			_strConfigFileName = ConfigurationManager.AppSettings ["ConfigrationPath"];
		} else{
			sIniFileName = sIniFilePath + "/" + _strConfigFileName;
		}

		if (Directory.Exists(sIniFilePath) == false)
		{
			Directory.CreateDirectory(sIniFilePath);
		}

		if (File.Exists(sIniFileName) == false)
        {
			try
            {
				File.Create(sIniFileName).Dispose();
				// config 파일이 존재하지 않을때, 실행폴더에 config 파일이 존재한다면,  실행폴더의 파일을 읽고,  저장을 합니다.(용도 브로커 서버 정보를 미리 기입하기 위해..)


				if (File.Exists (sProgramPath + "/"+ _strConfigFileName) == true) {

					XmlSerializer xs = new XmlSerializer (typeof (ClientEnvironment));

					using (var sr = new StreamReader (sProgramPath + "/" + _strConfigFileName)) {
						environment = (ClientEnvironment)xs.Deserialize (sr);
					}

				} else {
					environment = new ClientEnvironment ();
				}
				// 특정프로젝트(행안부)는,  메인윈도우가 보이지 않아 정상적인 종료를 할 수가 없습니다. 따라서, 초기 설정값을 읽고 저장을합니다.  
				SaveConfiguration ();
				return;
            }
      catch (Exception e)
            {
				_logger.Error("configration file can't load." + e.ToString());
				environment = new ClientEnvironment();
				return;
            }
        }

		Stream stm = null;
		try
        {
			if (_xmlSave)
            {
                XmlSerializer xs = new XmlSerializer(typeof(ClientEnvironment));
                using (var sr = new StreamReader(sIniFileName))
                {
                    environment = (ClientEnvironment)xs.Deserialize(sr);
                }
				return;
            }

			stm = File.Open(sIniFileName, FileMode.Open, FileAccess.Read);
            if (stm == null)
            {
				environment = new ClientEnvironment();
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();
            bf = new BinaryFormatter();

            string str = (string)bf.Deserialize(stm);

            byte[] Temp = Convert.FromBase64String(str);

            MemoryStream memStream = new MemoryStream(Temp);

			environment = (ClientEnvironment)bf.Deserialize(memStream);

            memStream.Close();
            stm.Close();

			if (environment == null)
            {
				environment = new ClientEnvironment();
            }
        }
        catch (Exception ex)
        {
			// config 파일이 규칙대로 되어 있지 않는 경우 Exception
			_logger.Debug(string.Format("failed file save [{0}]", ex.ToString()));

            if(stm != null)
			    stm.Close();
			environment = new ClientEnvironment();
			// 특정프로젝트(행안부)는,  메인윈도우가 보이지 않아 정상적인 종료를 할 수가 없습니다. 따라서, 초기 설정값을 읽고 저장을합니다.  
			SaveConfiguration ();

		}
	}
	public void SaveConfiguration()
	{
		string sIniFileName;

        string sIniFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/.config/DaaSXpert";
        _logger.Debug(sIniFilePath);

		if(_strConfigFileName == string.Empty)
			sIniFileName = sIniFilePath + "/" + ConfigurationManager.AppSettings ["ConfigrationPath"];
		else
			sIniFileName = sIniFilePath + "/" + _strConfigFileName;




		if (File.Exists(sIniFileName) == false)
        {
            try
            {
                File.Create(sIniFileName).Dispose();

            }
            catch (Exception)
            {
                _logger.Debug("configration file can't create.");
                return;
            }
        }
		try
        {
			if (_xmlSave)
			{
				XmlSerializer xs = new XmlSerializer(typeof(ClientEnvironment));
				TextWriter tw = new StreamWriter(sIniFileName);
				xs.Serialize(tw, environment);
				tw.Close();
			}
			else
			{
				//File.SetAttributes(sIniFileName, File.GetAttributes(sIniFileName) & ~FileAttributes.ReadOnly);

				Stream stm = File.Open(sIniFileName, FileMode.Create, FileAccess.Write);

				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream memStream = new MemoryStream();

				bf.Serialize(memStream, environment);

				byte[] byteArray = memStream.ToArray();

				string str = Convert.ToBase64String(byteArray);

				bf.Serialize(stm, str);

				stm.Close();
				memStream.Close();
				stm = null;
				memStream = null;
				bf = null;

				/*
				SoapFormatter bf = new SoapFormatter();
				bf.Serialize(stm, environment);

				stm.Close();
				stm = null;
				bf = null;
				*/

				//File.SetAttributes(sIniFileName, File.GetAttributes(sIniFileName) | FileAttributes.ReadOnly);
			}
        }

        catch (Exception ex)
        {
			_logger.Debug(string.Format("failed file save [{0}]", ex.ToString()));
            environment = new ClientEnvironment();
        }
	}


	/// <summary>
	/// Excutes the login.
	/// </summary>
	/// <param name="broker">Broker Info</param>
	/// <param name="strUserID">String user identifier.</param>
	/// <param name="strUserPassword">String user password.</param>
	public void ExcuteLogin(VOBrokerServerNew broker, string strUserID, string strUserPassword)
    {
        try
        {

			// AutoClientIP, AccurateLocalIP option 
			if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseAccurateLocalIP == true)
            {
                 string checkip = NetworkManager.GetLocalIP_FromTargetDestIP_TryConnectInfo(
                      broker.BrokerServerIP,
                      broker.BrokerServerPort);
                 
                // update client ip
                if (checkip != "" && checkip.Contains(_voUser.ClientIP) == false )
                {
                    _voUser.ClientIP = checkip;
                    Console.WriteLine("AccurateLocalIp Apply, Update ClientIP {0}", checkip);
                    _logger.Debug(String.Format("AccurateLocalIp Apply, Update ClientIP {0}", checkip));
                }

            }


            if (this.environment.vOGeneralsProperties.IsUseDBUSLogIn == true)
            {
                QuickDbusMessageClient DbusClient = new QuickDbusMessageClient();
                DbusClient.GetLoginInfoAsync();
                string strLoginIdfromDBUS = DbusClient.GetLoginId();
                _voUser.engMsg = DbusClient.GetEncMsg();

                // Input loginId from Dbus client
                _voUser.UserID = strLoginIdfromDBUS;

#if !DEBUG
				_logger.Debug (string.Format ("[DBUS] Login Id: {0}, and Param Secure Hide", strLoginIdfromDBUS));
#else
				_logger.Debug ("[DBUS] Login Id: " + strLoginIdfromDBUS);
				_logger.Debug ("[DBUS] encMsg: " + _voUser.engMsg);
#endif


			} else
            {
                _voUser.UserID = strUserID;
            }


			//[DXCT-357] 코드에서 MasterUsePin 삭제후 파일에서 로딩하는 기능으로 대체함
			// 파일에서 읽는 MasterUsePin은 plainText을 base64 Encoding된 값입니다
			if(this.environment.vOGeneralsProperties.IsUseMasterKeyLogin == true) {
				_voUser.Password = CryptoManager.DecodingBase64 (CommonUtils.ReadTextFromFile (System.IO.Directory.GetCurrentDirectory () + "/" + this.environment.vOGeneralsProperties.MasterUserPinFileName, true));
			} else {
				_voUser.Password = strUserPassword;
			}


			// _voSelectedBrokerServer.IsGateway = true;
			_voSelectedBrokerServer = broker;

			_voSelectedBrokerServer.AuthToken = string.Empty;

			_logger.Debug("login start");
            //new RequestToHCVKB().RequestLogIn_AsyncCallback(_voSelectedBrokerServer, _voUser, Callback_LoginToBrokerServer);
			new RequestToHCVKB().RequestRecommendServer_AsyncCallback(_voSelectedBrokerServer, _voUser, Callback_RecommendServer);
			_logger.Debug("login end");
        }
        catch (WebException wex)
        {
            //if (_voAutomation.IsAutomation)
            //{
            //    _voAutomation.AutomationState = VOAutomation.AutomationStateEnum.AUTOMATION_STOP;
            //}
            _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));
			//HCVKCErrorHandler.ExceptionHandler(wex);
			ErrorHandlerManager.ExceptionHandler(wex, null, "[LoginW]");

            if (_bRunningAutoStart == true) _bRunningAutoStart = false;
        }
        catch (Exception ex)
        {
            //if (_voAutomation.IsAutomation)
            //{
            //    _voAutomation.AutomationState = VOAutomation.AutomationStateEnum.AUTOMATION_STOP;
            //}
            _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            //HCVKCErrorHandler.ExceptionHandler(ex);
			ErrorHandlerManager.ExceptionHandler(ex, null, "[LoginE]");

            if (_bRunningAutoStart == true) _bRunningAutoStart = false;
        }
    }

    /// <summary>
    /// Callbacks the login to broker server.
    /// </summary>
    /// <param name="resJsonObject">Res json object.</param>
    /// <param name="exParam">Ex parameter.</param>
    private void Callback_LoginToBrokerServer(JObject resJsonObject, Exception exParam)
    {
        _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

        bool bIsConnectedOK = false;
        try
        {
            if (resJsonObject != null)
            {
                if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                {
                    // success response

                    // fetch auth token
                    {
                        JObject oToken = JObject.Parse(resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA]["tokenInfo"].ToString());
                        _voSelectedBrokerServer.AuthToken = oToken["token"].ToString();
                        _voSelectedBrokerServer.Expiration = CommonUtils.UnixTimeStampToDateTime(long.Parse(oToken["expiration"].ToString()));
                        _logger.Debug(string.Format("Token Expiration : {0}", _voSelectedBrokerServer.Expiration.ToLocalTime()));
                        //StartTokenExpirationTimer();
                    }

                    // fetch user info.
                    {
                        JObject oUserInfo = JObject.Parse(resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA]["userInfo"].ToString());

                        _voUser.Domain = oUserInfo["domain"].ToString();
                        _voUser.DomainUserID = oUserInfo["domainUserId"].ToString();
                        _voUser.UserDescription = oUserInfo["userDesc"].ToString();
                        _voUser.UserName = oUserInfo["userName"].ToString();
                        _voUser.OuName = oUserInfo["ouName"].ToString();
                        _voUser.TenantName = oUserInfo["tenantName"].ToString();
                        _voUser.TenantID = oUserInfo["tenantId"].ToString();
                        _voUser.Email = oUserInfo["email"].ToString();
                        _voUser.Phone = oUserInfo["phone"].ToString();
                        _voUser.IsFirstLogin = Convert.ToBoolean(oUserInfo["isFirstLogin"].ToString());
                        _voUser.IsEnabled = Convert.ToBoolean(oUserInfo["isEnabled"].ToString());

                        // check containkey
                        if (oUserInfo.ContainsKey("vmConnectPw"))
                            _voUser.VmConnectPw = oUserInfo["vmConnectPw"].ToString();

                    }

                    // post prcessing
                    {
                        // update last connected time
                        bIsConnectedOK = true;

                        _logReport.voLogServer = _voSelectedBrokerServer;
                        _logReport.voUser = _voUser;

                        // log report
                        _logReport.LogReport_User_Login(VOLogData.STATUS_SUCCEED, string.Empty);
                    }

                    //if (_voUser.IsFirstLogin)
                    //{
                        //if (_voAutomation.IsAutomation)
                        //{
                        //    _voAutomation.AutomationState = VOAutomation.AutomationStateEnum.AUTOMATION_STOP;
                        //}

                        //// change user info.
                        //HCVKCErrorHandler.NotifyHandler(VOErrorCode._E_CODE_C_C0000002);
                        //ChangeUserInformation();
                    //}
                    //else
                    {
                        //if (_voAutomation.IsAutomation)
                        //{
                        //    _voAutomation.AutomationState = VOAutomation.AutomationStateEnum.LOGIN_SUCCESS;
                        //}

                        // get desktop pool info.
                        GetDesktopPoolInfo();
                    }
					Gtk.Application.Invoke(delegate
					{
						if (MainFunc.callbackChangeLoginStatus != null)
                            MainFunc.callbackChangeLoginStatus();
					});
					// Start Broker Msg Receive Start
					runGetNewBrokerMsgTimer();

                }
                else
                {
                    //// failure response
                    //if (_voAutomation.IsAutomation)
                    //{
                    //    _voAutomation.AutomationState = VOAutomation.AutomationStateEnum.AUTOMATION_STOP;
                    //}

                    //// log report
                    _logReport.voLogServer = _voSelectedBrokerServer;
                    _logReport.voUser = _voUser;
                    _logReport.LogReport_User_Login(VOLogData.STATUS_FAILED, resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());

                    //// error handler
                    //HCVKCErrorHandler.ErrorHandler(resJsonObject[HCVKCRequestJSONParam.RESPONSE_RESULT_CODE].ToString());

					if(resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ().Equals ("B0002007") ||
						resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ().Equals ("B0002008") ||
						resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ().Equals ("B0002010")) {
						string strMessage = string.Empty;
						string strResultCode = resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ();
						if (strResultCode == "B0002007")
							strMessage = "비밀번호 유효기간이 초과되어, 재설정이 필요합니다";
						else if (strResultCode == "B0002008")
							strMessage = "비밀번호 유효기간이 초과되어, 재설정이 필요합니다";
						else if (strResultCode == "B0002010")
							strMessage = "비밀번호를 변경해야 합니다.";

						Gtk.Application.Invoke (delegate {
							ChangePasswordDialog changePasswordDialog = new ChangePasswordDialog (strMessage);
							changePasswordDialog.Modal = true;
						});

					} else {
						ErrorHandlerManager.ErrorHandler (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ());
					}

				}
            }
            else
            {
                if (_bRunningAutoStart == true) _bRunningAutoStart = false;
                throw exParam;
            }
        }
        catch (WebException wex)
        {
            //if (_voAutomation.IsAutomation)
            //{
            //    _voAutomation.AutomationState = VOAutomation.AutomationStateEnum.AUTOMATION_STOP;
            //}
            _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));
            //HCVKCErrorHandler.ExceptionHandler(wex);
			ErrorHandlerManager.ExceptionHandler(wex, null, "[LogToBrokerW]");

            if (_bRunningAutoStart == true) _bRunningAutoStart = false;
        }
        catch (Exception ex)
        {
            //if (_voAutomation.IsAutomation)
            //{
            //    _voAutomation.AutomationState = VOAutomation.AutomationStateEnum.AUTOMATION_STOP;
            //}
            _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            //HCVKCErrorHandler.ExceptionHandler(ex);
			ErrorHandlerManager.ExceptionHandler(ex, null, "[LogToBrokerE]");

			if (_bRunningAutoStart == true) _bRunningAutoStart = false;
        }
        finally
        {
            // update last connected time
            {
                _voSelectedBrokerServer.LastConnectedTime = CommonUtils.DateTimeToUnixTime(DateTime.UtcNow);
                _voSelectedBrokerServer.IsLastConnected = bIsConnectedOK;
                //AddServerInfo(_voSelectedBrokerServer);
                // debug???
                MainWindow.mainWindow.SaveConfiguration();
            }
        }
    }
    public void GetDesktopPoolInfo()
    {
        try
        {
            new RequestToHCVKB().RequestGetDesktopPoolInfo_AsyncCallback(_voSelectedBrokerServer, _voUser, Callback_GetDesktopPoolInfo);
            // log report

            _logReport.LogReport_DesktopInfo_Pool(VOLogData.STATUS_START, string.Empty);

            // __Imsi__
            //SetMocUpDesktopPool();
        }
        catch (WebException wex)
        {
            _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));
            if (_bRunningAutoStart == true) _bRunningAutoStart = false;
        }
        catch (Exception ex)
        {
            _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            if (_bRunningAutoStart == true) _bRunningAutoStart = false;
        }
    }

	private void Callback_GetDesktopPoolInfo(JObject resJsonObject, Exception exParam)
    {
        _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));
        try
        {
            if (resJsonObject != null)
            {
                _listDesktopPool.Clear();
                if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                {
                    // success response
                    // fetch and update auth token
                    new RequestToHCVKB().UpdateAuthToken(_voSelectedBrokerServer, resJsonObject);

                    // fetch desktop pool list
                    {
                        JArray oDesktopPoolList = JArray.Parse(resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA]["poolList"].ToString());
                        foreach (JObject oDesktopPool in oDesktopPoolList)
                        {
                            VODesktopPool voDesktopPool = new VODesktopPool();
                            voDesktopPool.Tag = oDesktopPool.ToString();
                            voDesktopPool.PoolID = oDesktopPool["poolId"].ToString();
                            voDesktopPool.PoolName = oDesktopPool["poolName"].ToString();
                            voDesktopPool.PoolDesc = oDesktopPool["poolDesc"].ToString();
                            voDesktopPool.AccessDiv = oDesktopPool["accessDiv"].ToString();
                            voDesktopPool.SupportProtocols = oDesktopPool["supportProtocols"].ToString();
                            voDesktopPool.IsEnabled = Convert.ToBoolean(oDesktopPool["isEnabled"].ToString());

                            // check condition
                            if (oDesktopPool.ContainsKey("availableCount") == true)
                            {
                                voDesktopPool.VmAvailableCount = oDesktopPool["availableCount"].ToString();
                            }

                            // check condition
                            if (oDesktopPool.ContainsKey("totalDesktopCount") == true)
                            {
                                voDesktopPool.VmTotalCount = oDesktopPool["totalDesktopCount"].ToString();
                            }


                            // __Imis__
                            //voDesktopPool.SupportProtocols = VOProtocol.VOProtocolType.PROTOCOL_RDP;

                            // if dedicated desktop, fetch information
                            //if (voDesktopPool.AccessDiv.Equals(client.Properties.Resources.TYPE_ACCESS_DIV_DEDICATED, StringComparison.CurrentCultureIgnoreCase))
                            {
                                JObject oDesktop = JObject.Parse(oDesktopPool["desktop"].ToString());
                                if (oDesktop.GetValue("displayName") != null)
                                    voDesktopPool.Desktop.DesktopName = oDesktop["displayName"].ToString();
                                else
                                    voDesktopPool.Desktop.DesktopName = voDesktopPool.PoolName;

								if (voDesktopPool.AccessDiv.Equals(client.Properties.Resources.TYPE_ACCESS_DIV_DEDICATED, StringComparison.CurrentCultureIgnoreCase))
								{
									voDesktopPool.Desktop.DesktopID = oDesktop["desktopId"].ToString();
									voDesktopPool.Desktop.InstanceID = oDesktop["instanceId"].ToString();
									voDesktopPool.Desktop.CurrentState = oDesktop["desktopCurrentState"].ToString();
									voDesktopPool.Desktop.Status = oDesktop["status"].ToString();
									voDesktopPool.Desktop.PowerState = oDesktop["powerState"].ToString();
									voDesktopPool.Desktop.VMState = oDesktop["vmState"].ToString();
									voDesktopPool.Desktop.AgentState = oDesktop["agentState"].ToString();
									voDesktopPool.Desktop.DesktopIP = oDesktop["ipAddress"].ToString();
									voDesktopPool.Desktop.IsEnabled = Convert.ToBoolean(oDesktop["isEnabled"].ToString());
                                    voDesktopPool.Desktop.Sessionconnected = oDesktop["sessionConnected"].ToString();
								}

                                if (oDesktop.GetValue("desktopTemplate") != null)
                                {
                                    JArray oTemplateList = JArray.Parse(oDesktop["desktopTemplate"].ToString());
                                    foreach (JObject oTemplate in oTemplateList)
                                    {
                                        VODesktopTemplate voDesktopTemplate = new VODesktopTemplate();
                                        voDesktopTemplate.TemplateName = oTemplate["name"].ToString();
                                        voDesktopTemplate.TemplateDescription = oTemplate["description"].ToString();

                                        // JObject oOSTemplate = JObject.Parse(oTemplate["os"].ToString());

                                        JArray oOSTemplateList = JArray.Parse(oTemplate["os"].ToString());
                                        foreach (JObject oOSTemplate in oOSTemplateList)
                                        {
                                            voDesktopTemplate.OSName = oOSTemplate["name"].ToString();
                                            voDesktopTemplate.OSCode = oOSTemplate["code"].ToString();
                                            break;
                                        }

                                        voDesktopPool.Desktop.Templates.Add(voDesktopTemplate);
                                    }
                                }

                                //if (oDesktop.GetValue("desktopPolicies") != null)
                                if (oDesktopPool.GetValue("desktopPolicies") != null)
                                {
                                    JArray oPoliciesList = JArray.Parse(oDesktopPool["desktopPolicies"].ToString());
                                    foreach (JObject oPolicies in oPoliciesList)
                                    {
                                        VODesktopPolicies voDesktopPolicies = new VODesktopPolicies();
                                        voDesktopPolicies.PolicyId = oPolicies["policyId"].ToString();
                                        voDesktopPolicies.PolicyName = oPolicies["policyName"].ToString();
                                        voDesktopPolicies.PolicyType = oPolicies["policyType"].ToString();
                                        voDesktopPolicies.Apply = Convert.ToBoolean(oPolicies["isApply"]);

                                        int nIndex, nCount;
                                        nCount = voDesktopPool.Desktop.Policies.Count;
                                        for (nIndex = 0; nIndex < nCount; nIndex++)
                                        {
                                            if (voDesktopPool.Desktop.Policies[nIndex].PolicyName.CompareTo(voDesktopPolicies.PolicyName) > 0)
                                            {
                                                voDesktopPool.Desktop.Policies.Insert(nIndex, voDesktopPolicies);
                                            }
                                        }
                                        if (nIndex == nCount)
                                        {
                                            voDesktopPool.Desktop.Policies.Add(voDesktopPolicies);
                                        }
                                    }
                                }

								if (oDesktop.GetValue("protocol") != null)
								{
									JArray oProtocolList = JArray.Parse(oDesktop["protocol"].ToString());
									foreach (JObject oProtocol in oProtocolList)
									{
										VOProtocol voProtocol = new VOProtocol();
										voProtocol.ProtocolType = oProtocol["type"].ToString();
										voProtocol.ProtocolIP = oProtocol["ipAddress"].ToString();
										voProtocol.ProtocolPort = oProtocol["port"].ToString();


                                        // Change Convert IP Recommend IpRule Order From Broker
                                        //  apply ip Convert rule
		                        //  Get Desktop Pool VM Info (Show DesktopInfo)
                                        if (_voSelectedBrokerServer.ConvertIp_TargetClientIpRangeRule != "")
                                        {

                                            string change_ip = CommonUtils.ConvertIpRecommendServerAddIpRuleToVM_Ip(
                                                voProtocol.ProtocolIP,
                                                _voSelectedBrokerServer.ConvertIp_TargetClientIpRangeRule,
                                                _voUser.ClientIP,
                                                //"10.11.3.101",
                                                _voSelectedBrokerServer.ConvertIp_TargetDesktopVMIpSubnet);


                                            voProtocol.ProtocolIP = change_ip;
                                            //voDesktopPool.Desktop.DesktopIP = change_ip;
                                            Console.WriteLine("Changed ip {0}", voProtocol.ProtocolIP);
                                        }

                                        

                                       voDesktopPool.Desktop.Protocols.Add(voProtocol);
									}
								}
                                // __Imis__
                                //{
                                //    voDesktopPool.SupportProtocols = VOProtocol.VOProtocolType.PROTOCOL_SPICE + VOProtocol.VOProtocolType.PROTOCOL_SEPARATOR + VOProtocol.VOProtocolType.PROTOCOL_RDP;

                                //    VOProtocol voProtocol = new VOProtocol();
                                //    voProtocol.ProtocolType = VOProtocol.VOProtocolType.PROTOCOL_SPICE;
                                //    voProtocol.ProtocolIP = "192.168.120.2";
                                //    voProtocol.ProtocolPort = "5900";
                                //    voDesktopPool.Desktop.Protocols.Add(voProtocol);
                                //}
                                //{
                                //    VOProtocol voProtocol = new VOProtocol();
                                //    voProtocol.ProtocolType = VOProtocol.VOProtocolType.PROTOCOL_RDP;
                                //    voProtocol.ProtocolIP = voDesktopPool.Desktop.DesktopIP;
                                //    voProtocol.ProtocolPort = "";
                                //    voDesktopPool.Desktop.Protocols.Add(voProtocol);
                                //}
                            }

                            _listDesktopPool.Add(voDesktopPool);
                        }
                    }

                    // TODO : AUTO Quick Step Connect VDI
                    if (_listDesktopPool.Count == 1)
                    {

                        _bRunDirectAutoConnect = true;

                    }

                    try
                    {
                        // update  oderbye OS Type, ascending, orderby  oscode   window OS010164~ OS020564 HAMONIKR
                        _listDesktopPool = _listDesktopPool.OrderBy(item => item.Desktop.Templates[0].OSCode).ToList();
                     } catch
                    {
                        _logger.Warn("Get Pool Desktop Item list orderby ascending, exception");
                    }

                    // log report
                    _logReport.LogReport_DesktopInfo_Pool(VOLogData.STATUS_SUCCEED, string.Empty);
                }
                else
                {
                    // log report
                    _logReport.LogReport_DesktopInfo_Pool(VOLogData.STATUS_FAILED, resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());

                    // failure response
                    // HCVKCErrorHandler.ErrorHandler(resJsonObject[HCVKCRequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                    ErrorHandlerManager.ErrorHandler(resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                }

                // display desktop pool info.
                // DisplayDesktopPool();
				Gtk.Application.Invoke(delegate
                {
                    if (MainFunc.callbackShowDesktopListPage != null)
                        MainFunc.callbackShowDesktopListPage(true);
                });
            }
        }
        catch (WebException wex)
        {
            _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));

            // log report
            _logReport.LogReport_DesktopInfo_Pool(VOLogData.STATUS_FAILED, string.Empty);
            if (_bRunningAutoStart == true) _bRunningAutoStart = false;
        }
        catch (Exception ex)
        {
            _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            // log report
            _logReport.LogReport_DesktopInfo_Pool(VOLogData.STATUS_FAILED, string.Empty);

            // HCVKCErrorHandler.ExceptionHandler(ex);
            ErrorHandlerManager.ExceptionHandler(ex, this, "[DesktopPoolInfo]");

            if (_bRunningAutoStart == true) _bRunningAutoStart = false;
        }
    }

	public VOBrokerServerNew GetSelectedBrokerServer()
	{
		return this._voSelectedBrokerServer;
	}

    public void ExcuteLogout()
    {
        try
        {
            // ClearDesktopViewer();
            new RequestToHCVKB().RequestLogOut_AsyncCallback(_voSelectedBrokerServer, _voUser, Callback_LogoutFromBrokerServer);
        }
        catch (WebException wex)
        {
            _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));
        }
        catch (Exception ex)
        {
            _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
        }

		//[DXCT-361] broker 서버 수신 가능 상태 요청 
		if (MainFunc.requestCheckServerState != null) {
			MainFunc.requestCheckServerState ();
		}

	}

    private void Callback_LogoutFromBrokerServer(JObject resJsonObject, Exception exParam)
    {
        _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

        try
        {
            if (resJsonObject != null)
            {
                // except error code as B0001001, processing to logout 
                if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals(VOErrorCode._E_CODE_OK) ||
                    resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals(VOErrorCode._E_CODE_B_B0003001))
                {
                    // log report
                    _logReport.LogReport_User_Logout(VOLogData.STATUS_SUCCEED, string.Empty);

                    _voSelectedBrokerServer.AuthToken = string.Empty;
                    _voSelectedBrokerServer.Expiration = DateTime.MinValue;

                    _listDesktopPool.Clear();

					// StopTokenExpirationTimer();
					// DisplayBrokerServer();
					Gtk.Application.Invoke(delegate
					{
						if (MainFunc.callbackShowDesktopListPage != null)
							MainFunc.callbackShowDesktopListPage(true);
					});

                    // after changing user info, processing to relogin
                    if (_voUser.IsReLogin)
                    {
                        _voUser.IsReLogin = false;

                        // LoginToBrokerServer();
                    }
                }
                else
                {
                    // log report
                    _logReport.LogReport_User_Logout(VOLogData.STATUS_FAILED, resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());

                    // failure response
                    // HCVKCErrorHandler.ErrorHandler(resJsonObject[HCVKCRequestJSONParam.RESPONSE_RESULT_CODE].ToString());
					ErrorHandlerManager.ErrorHandler(resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());
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
	public List<VODesktopPool> GetListDesktopPool()
    {
		if (this._listDesktopPool == null)
			return new List<VODesktopPool>();
		return new List<VODesktopPool>(this._listDesktopPool);
    }

	private List<ConnectProcess> _ConnectList = new List<ConnectProcess>();


    public void ConnectDesktop(VODesktopPoolEx voDesktopPoolEx)
	{
		ConnectProcess connect = new ConnectProcess();

		connect._strServiceProtocol = voDesktopPoolEx.Protocol;
		connect._voBrokerServer = _voSelectedBrokerServer;
		connect._voUser = _voUser;
		connect._voDesktopPoolEx = voDesktopPoolEx;
		connect._logReport = _logReport;
		connect.Show();

		_curConnectPoolID = voDesktopPoolEx.DesktopPool.PoolID;

		_ConnectList.Add(connect);



		return;
	}





    public void DisconnectDesktop(VODesktopPool vODesktopPool)
	{
		var connect = _ConnectList.FirstOrDefault(s => s._voDesktopPoolEx.DesktopPool.PoolID == vODesktopPool.PoolID);
		if (connect != null)
		{
			connect.DisconnectDesktopView();
			connect.Dispose();
			_ConnectList.Remove(connect);
		}
	}

	public void DeleteConnectList(string PoolId)
    {
		var connect = _ConnectList.FirstOrDefault(s => s._voDesktopPoolEx.DesktopPool.PoolID == PoolId);
        if (connect != null)
        {
            _ConnectList.Remove(connect);
        }
    }

	public void ChangeDesktopName(VODesktopPool voDesktopPool, string strDesktopName)
	{
		try
		{
			var desktop = this._listDesktopPool.First(s => s.Desktop.DesktopID == voDesktopPool.Desktop.DesktopID);
			if(desktop != null)
			{
				desktop.Desktop.DesktopName = strDesktopName;
			}
			new RequestToHCVKB().RequestDesktop_AsyncCallback(_voSelectedBrokerServer, voDesktopPool, _voUser, strDesktopName, Callback_ChangeDesktopName);
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
	private void Callback_ChangeDesktopName(JObject resJsonObject, Exception exParam)
    {
        try
        {
            if (resJsonObject != null)
            {
                if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                {
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
	public void ExcutePowerControl(VODesktopPool voDesktopPool, string strPowerControl)
    {
        try
        {
            // if (!IsTypeBrokerServer())
            {
                // if (listView.SelectedItems.Count > 0)
                {
                    // set selected desktop pool for power control
                    // _voSelectedDesktopPool = (VODesktopPool)listView.SelectedItems[0].Tag;

                    // checking desktop state for ready to power control
                    // pass when destkop state is tasking
                    if (voDesktopPool.Desktop.CurrentState.Equals(VODesktop.DESKTOP_CURRENT_STATE_TASKING, StringComparison.CurrentCultureIgnoreCase))
                        return;

                    // checking current power status if going to set to power on
                    if (strPowerControl.Equals(VODesktop.POWER_DESKTOP_ON, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (voDesktopPool.Desktop.Status.Equals(VODesktop.STATUS_SHUTOFF, StringComparison.CurrentCultureIgnoreCase))
                        {
                            strPowerControl = VODesktop.POWER_DESKTOP_ON;
                        }
                        else if (voDesktopPool.Desktop.Status.Equals(VODesktop.STATUS_PAUSED, StringComparison.CurrentCultureIgnoreCase))
                        {
                            strPowerControl = VODesktop.POWER_DESKTOP_UNPAUSE;
                        }
                        else if (voDesktopPool.Desktop.Status.Equals(VODesktop.STATUS_SUSPENDED, StringComparison.CurrentCultureIgnoreCase))
                        {
                            strPowerControl = VODesktop.POWER_DESKTOP_RESUME;
                        }
                    }

                    // request power control
                    new RequestToHCVKB().RequestDesktopPowerControl_AsyncCallback(
                        _voSelectedBrokerServer, voDesktopPool, _voUser, strPowerControl, Callback_PowerControl);


                    // log report
                    _logReport.LogReport_DesktopInfo_Power(
                       voDesktopPool.Desktop.InstanceID, strPowerControl, VOLogData.STATUS_START, string.Empty);
                    _desktopInstanceID = voDesktopPool.Desktop.InstanceID;
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

    private void Callback_PowerControl(JObject resJsonObject, Exception exParam)
    {
        _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));
        try
        {
            if (resJsonObject != null)
            {
                if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                {
                    // success response
                    // fetch and update auth token
                    new RequestToHCVKB().UpdateAuthToken(_voSelectedBrokerServer, resJsonObject);

                    // log report
                    _logReport.LogReport_DesktopInfo_Power(
                        _desktopInstanceID, string.Empty, VOLogData.STATUS_SUCCEED, string.Empty);
                }
                else
                {
                    // log report
                    _logReport.LogReport_DesktopInfo_Power(
                        _desktopInstanceID, string.Empty, VOLogData.STATUS_FAILED, resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());

                    // failure response
                    //HCVKCErrorHandler.ErrorHandler(resJsonObject[HCVKCRequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                    ErrorHandlerManager.ErrorHandler(resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());
                }
            }
        }
        catch (WebException wex)
        {
            _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));

            // log report
            _logReport.LogReport_DesktopInfo_Power(
                _desktopInstanceID, string.Empty, VOLogData.STATUS_FAILED, string.Empty);
        }
        catch (Exception ex)
        {
            _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));


            // log report
            _logReport.LogReport_DesktopInfo_Power(
                _desktopInstanceID, string.Empty, VOLogData.STATUS_FAILED, string.Empty);
        }
    }
	public void AddBookmarkInfo(VODesktopPool voDesktopPool)
    {
        VOBookmark voBookmark = new VOBookmark();

        voBookmark.ServerIP = _voSelectedBrokerServer.BrokerServerIP;
        voBookmark.ServerName = _voSelectedBrokerServer.BrokerServerDesc;
        voBookmark.ServerPort = _voSelectedBrokerServer.BrokerServerPort;
        voBookmark.DesktopID = voDesktopPool.PoolID;
        voBookmark.DesktopName = voDesktopPool.Desktop.DesktopName;
        voBookmark.DesktopIP = voDesktopPool.Desktop.DesktopIP;

		if (voDesktopPool.Desktop.Templates.Count > 0)
        {
            foreach (VODesktopTemplate voDesktopTemplate in voDesktopPool.Desktop.Templates)
            {
                voBookmark.OSCode = voDesktopTemplate.OSCode;
                break;
            }
        }

		var bookmark = this.environment.Bookmarks.FirstOrDefault(b => b.DesktopIP == voDesktopPool.Desktop.DesktopIP);
		if(bookmark != null)
		{
			this.environment.Bookmarks.Remove(bookmark);
		}
		this.environment.Bookmarks.Add(voBookmark);

		SaveConfiguration();

		if (MainFunc.callbackLoadBookmarkInfo != null)
			MainFunc.callbackLoadBookmarkInfo();
    }
	public void RemoveBookmarkInfo(VOBookmark voBookmark)
	{
		var bookmark = this.environment.Bookmarks.FirstOrDefault(b => b.DesktopIP == voBookmark.DesktopIP);
        if (bookmark != null)
        {
            this.environment.Bookmarks.Remove(bookmark);

			SaveConfiguration();

			if (MainFunc.callbackLoadBookmarkInfo != null)
                MainFunc.callbackLoadBookmarkInfo();

            this._voBookmark = null;
        }
	}
    
	public void ExcuteBookmark(VOBookmark voBookmark)
    {
		ChangePage(0);

		if (MainFunc.callbackExcuteBookmarkServer == null)
			return;

		this._voBookmark = voBookmark;

		if(MainFunc.callbackExcuteBookmarkServer(voBookmark.ServerIP))
		{
			if (MainFunc.callbackExcuteBookmarkDesktop != null)
				MainFunc.callbackExcuteBookmarkDesktop(voBookmark.DesktopID);
		}
    }

	public int CheckAutoStart(VODesktopPool voDesktopPool)
	{
		string strServerIP = this.environment.vOAutoLoginProperties.ServerIP;

        if (string.IsNullOrEmpty(strServerIP))
        {
			return 0;
        }
        else if (strServerIP != this._voSelectedBrokerServer.BrokerServerIP
                || this.environment.vOAutoLoginProperties.ServerPort != Convert.ToInt32(this._voSelectedBrokerServer.BrokerServerPort)
                || this.environment.vOAutoLoginProperties.PoolID != Convert.ToInt32(voDesktopPool.PoolID))
        { // differ
			return 1;
        }

		return 2;
	}
	public bool SetAutoStartInfo(VODesktopPool voDesktopPool)
	{
		int ncheck = CheckAutoStart(voDesktopPool);
		bool bAdd = false;

		if (ncheck == 0)
		{
			bAdd = true;
		}
		else if (ncheck == 1)
		{ // differ
			ResponseType result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("기존 Auto 설정을 해제하고, 재설정하시겠습니까?"));

			if (result == ResponseType.Yes)
			{
				bAdd = true;
			}
		}
        if(bAdd)
		{
			this.environment.vOAutoLoginProperties.ServerIP = this._voSelectedBrokerServer.BrokerServerIP;
			this.environment.vOAutoLoginProperties.ServerPort = Convert.ToInt32(this._voSelectedBrokerServer.BrokerServerPort);

			this.environment.vOAutoLoginProperties.UserID = this._voUser.UserID;
			this.environment.vOAutoLoginProperties.UserPW = this._voUser.Password;

			this.environment.vOAutoLoginProperties.PoolID = Convert.ToInt32(voDesktopPool.PoolID);

			this.environment.vOAutoLoginProperties.ServerName = this._voSelectedBrokerServer.BrokerServerDesc;
			this.environment.vOAutoLoginProperties.DesktopName = voDesktopPool.Desktop.DesktopName;

			SaveConfiguration();

			this._settingkPageWidget.SetSettingPageInfo();

			return true;
		}
		        
		return false;
	}
	public void UnSetAutoStartInfo()
	{
		this.environment.vOAutoLoginProperties.ServerIP = "";
        this.environment.vOAutoLoginProperties.ServerPort = 0;

        this.environment.vOAutoLoginProperties.UserID = "";
        this.environment.vOAutoLoginProperties.UserPW = "";

        this.environment.vOAutoLoginProperties.PoolID = 0;

        this.environment.vOAutoLoginProperties.ServerName = "";
        this.environment.vOAutoLoginProperties.DesktopName = "";

        SaveConfiguration();
    }


    // Recommend Server, AddIpRule Find And Apply From Broker
    // Check Condition, Key and value
    // rule update to ConverIp_xxxx  (Borker VO) 
    private bool RecommendServerAddIpRule(JObject resJsonObject)
    {
        if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA]["addrConvRule"] == null)
        {
            return false; // not support return
        }

        try
        {
            JArray ruleList = JArray.Parse(resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA]["addrConvRule"].ToString());
            string srcExpr = "", br_ip_subnet = "", desk_ip_subnet = ""; // current spec 1 rule

            foreach (JObject rule in ruleList)
            {

                // match client ip = srcIpExpr and update 
                srcExpr = rule["srcIpExpr"].ToString();
                br_ip_subnet = rule["broker"].ToString();
                desk_ip_subnet = rule["desktop"].ToString();
                Console.WriteLine("srcIpExpr {0}, broker {1}, desktop {2}", srcExpr, br_ip_subnet, desk_ip_subnet);


                // match client ip = srcIpExpr and update 
                if (CommonUtils.CheckConditionInRangeIpByRegx(_voUser.ClientIP, srcExpr) == true) // matched
                {

                    Console.WriteLine("IpConvertRule Matched ClientIP {0}, srcIpExpr {1}, broker {2}, desktop {3}",
                        _voUser.ClientIP, srcExpr, br_ip_subnet, desk_ip_subnet);

                    _logger.Info(string.Format("IpConvertRule Matched ClientIP {0}, srcIpExpr {1}, broker {2}, desktop {3}",
                        _voUser.ClientIP, srcExpr, br_ip_subnet, desk_ip_subnet));

                    // find and update 
                    _voSelectedBrokerServer.ConvertIp_TargetClientIpRangeRule = srcExpr;
                    _voSelectedBrokerServer.ConvertIp_TargetBrokerIpSubnet = br_ip_subnet;
                    _voSelectedBrokerServer.ConvertIp_TargetDesktopVMIpSubnet = desk_ip_subnet;

                    // update add convert rule frome broker
                    return true;
                }

            }

            if (srcExpr.Length < 1)
                return false;

        } catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }


        return true;

    }



    private void Callback_RecommendServer(JObject resJsonObject, Exception exParam)
    {
		_logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

        bool bIsConnectedOK = false;
        try
        {
            if (resJsonObject != null)
            {
                if (resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                {
                    // success response
                    JObject oResult = JObject.Parse(resJsonObject[RequestJSONParam.RESPONSE_RESULT_DATA].ToString());

                    // fetch server info.
                    {
                        //_voSelectedBrokerServer.BrokerServerIP = oResult["siblingPublicIp"].ToString();
                        //_voSelectedBrokerServer.BrokerServerDesc = oResult["siblingName"].ToString();
                    }

                    // update last connected time
                    {
                        _voSelectedBrokerServer.LastConnectedTime = CommonUtils.DateTimeToUnixTime(DateTime.UtcNow);
                        _voSelectedBrokerServer.IsLastConnected = bIsConnectedOK;
                        // AddBrokerServer(_voSelectedBrokerServer);
                    }
                    // get cadidate list
                    {
                        if (oResult.GetValue("candidateList") != null)
                        {
                            JArray oCandidateList = JArray.Parse(oResult["candidateList"].ToString());
                            foreach (JObject oCandidate in oCandidateList)
                            {
								_voSelectedBrokerServer.CandidateServers.Add (new VOBrokerServerNew {
									ConfigIP = _voSelectedBrokerServer.ConfigIP,
                                    BrokerServerIP = oCandidate["siblingPublicIp"].ToString(),
                                    BrokerServerDesc = oCandidate["siblingName"].ToString(),
                                });
                            }
                        }
                    }

                    // Find And Apply Recommend Server Add Iprule From Broker
                    RecommendServerAddIpRule(resJsonObject);

                    // update broker server ip RecommendServer AddIpRule
                    //  apply ip Convert rule
                    if (_voSelectedBrokerServer.ConvertIp_TargetClientIpRangeRule != "")
                    {

                        string change_ip = CommonUtils.ConvertIpRecommendServerAddIpRuleToBrokerIp(
                            _voSelectedBrokerServer.BrokerServerIP,
                            _voSelectedBrokerServer.ConvertIp_TargetClientIpRangeRule,
                            _voUser.ClientIP,
                            //"10.11.3.101",
                            _voSelectedBrokerServer.ConvertIp_TargetBrokerIpSubnet);


                        _voSelectedBrokerServer.BrokerServerIP = change_ip;
                        Console.WriteLine("Broker Change IP {0}", change_ip);
                    }

                    MainWindow.mainWindow.SaveConfiguration();

					_voSelectedBrokerServer.BrokerServerIP = oResult["siblingPublicIp"].ToString(); // 저장후 접속 가능한 IP로 변경, win 코드 동일

					// start login process
					// LoginToBrokerServer();
					new RequestToHCVKB ().RequestLogIn_AsyncCallback(_voSelectedBrokerServer, _voUser, Callback_LoginToBrokerServer);
                }
                else
                {
                    // error handler
                    ErrorHandlerManager.ErrorHandler(resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString());
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
            ErrorHandlerManager.ExceptionHandler(wex, this, "[RecommendServerW]");
        }
        catch (Exception ex)
        {
            //if (_voAutomation.IsAutomation)
            //{
            //    _voAutomation.AutomationState = VOAutomation.AutomationStateEnum.AUTOMATION_STOP;
            //}
            _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            ErrorHandlerManager.ExceptionHandler(ex, this, "[RecommendServerE]");
        }
    }

    public void shutDown_Device()
    {
        ResponseType result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("전원을 종료 하시겠습니까?"));

        if (result == ResponseType.Yes)
        {
            ErrorHandlerManager.Dispose();

			CloseProcess ();

            Application.Quit();

            Process process = null;
            process = Process.Start(new ProcessStartInfo()
            {
                FileName = "shutdown",
                Arguments = "-h now",
                RedirectStandardInput = true,
                RedirectStandardOutput = false,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }
    }

	//웹캠 Redirection을 위한 usb bind 요청
	public void CtrlDeviceRedirection(string strMode, string strWebCamDevId)
	{
		_logger.Info (string.Format ("[CtrlDeviceRedirection] Mode: ") + strMode);
		// 웹캠 bus id 획득 

		string strWebCamBusId = CommonUtils.GetWebCamDeviceBusId (strWebCamDevId, MainWindow.mainWindow.environment.vOUSBDeviceProperties.UsbIpModulePath);
		string strWebCamPort = mainWindow.environment.vOUSBDeviceProperties.DeviceRedirectionPort;

		if (strWebCamBusId.Contains ("Error") == true) {
			ErrorHandlerManager.ErrorHandler (VOErrorCode._E_CODE_C_0000012);
			_logger.Error (string.Format ("[CtrlDeviceRedirection] {0}", strWebCamBusId));
			return;
		} else {
			if (strWebCamDevId == "") {
				ErrorHandlerManager.ErrorHandler (VOErrorCode._E_CODE_C_0000011);
				_logger.Debug (string.Format ("No WebCam Dev Id "));
			}
		}

		if (strWebCamPort == null || strWebCamPort == "") {
			ErrorHandlerManager.ErrorHandler (VOErrorCode._E_CODE_C_0000013);
			_logger.Error (string.Format ("[CtrlDeviceRedirection] Fail, unavailable port: ") + strWebCamBusId);
			return;
		}

		if (strWebCamBusId != "") {
			QuickDbusMessageClient DbusCleint = new QuickDbusMessageClient ();
			if (strMode == "bind") {
				// system dbus 웹캠 bind 요청
				_logger.Info (string.Format ("[CtrlDeviceRedirection] bind DBUS API, BusId: ") + strWebCamBusId);

				if (DbusCleint.SetDeviceBindAsync (strWebCamBusId) == true) {
					//  agent 웹캠 리디렉션 연결요청 
					_logger.Info (string.Format ("[CtrlDeviceRedirection] Request Agent, Pool Id: ") + mainWindow._curConnectPoolID);

					var connect = _ConnectList.FirstOrDefault (s => s._voDesktopPoolEx.DesktopPool.PoolID == mainWindow._curConnectPoolID);
					if (connect != null) {
						connect.RequestRedirectionAttachMode (strWebCamBusId, strWebCamPort);
					}
				} else {
					_logger.Info (string.Format ("[CtrlDeviceRedirection] fail bind DBUS API, BusId: ") + strWebCamBusId);

					// unbind 후, 다시 bind
					/*
					_logger.Info (string.Format ("[CtrlDeviceRedirection] try unbind and Request Agent, BusId: ") + strWebCamBusId + " Pool Id: " + mainWindow._curConnectPoolID);
									
					if (DbusCleint.SetDeviceUnbindAsync (strWebCamBusId) == true) {

						_logger.Info (string.Format ("[CtrlDeviceRedirection] Retry bind DBUS API, BusId: ") + strWebCamBusId);
						if (DbusCleint.SetDeviceBindAsync (strWebCamBusId) == true) {
							//  agent 웹캠 리디렉션 연결요청 
							var connect = _ConnectList.FirstOrDefault (s => s._voDesktopPoolEx.DesktopPool.PoolID == mainWindow._curConnectPoolID);
							if (connect != null) {
								connect.RequestRedirectionAttachMode (strWebCamBusId, strWebCamPort);
							}
						}
					}
					*/
				}

			} else if (strMode == "unbind") {
				//  agent 웹캠 리디렉션 해제요청 ->  callback 함수  Dbus unbind -> 파일 쓰기
				var connect = _ConnectList.FirstOrDefault (s => s._voDesktopPoolEx.DesktopPool.PoolID == mainWindow._curConnectPoolID);
				if (connect != null) {
					connect.RequestRedirectionDetachMode (strWebCamBusId, strWebCamPort);
				} else {
					//  agent연결 없을 때, 바로 unbind 요청
					if(DbusCleint.SetDeviceUnbindAsync (strWebCamBusId) == false) {
						_logger.Error (string.Format ("[CtrlDeviceRedirection] Fail, unbind Bus Id: ")+ strWebCamBusId);
					}
				}
			}
		} else {
			_logger.Error (string.Format ("[CtrlDeviceRedirection] No Webcam Bus Id "));
		}

		_strUSBIPConnectStatus = strMode;

	}

	// 웹캠 Redirection을 위한 RDP 재접속 요청
	public void RestartDesktop()
    {
		//pool 
		if (_ConnectList.Count == 0)
		    return;

		_logger.Debug(string.Format("RestartDesktop : ConnectList: {0}", _ConnectList.Count.ToString()));

		int nMethod = 0;

		try {
			nMethod = MainWindow.mainWindow.environment.vOUSBDeviceProperties.ReConnectRDPMethod;
		} catch (Exception ex) {
			_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
		}




		for (int i = 0; i < _ConnectList.Count; i++)
        {
			var connect = _ConnectList[i];
			if (connect != null)
	        {

				connect.SetRequestRestart(true);

				if(nMethod == 0) {
					// method #1
					_logger.Debug (string.Format ("RestartDesktop : Kill Process >>>>>>>>>>>>>>>>>>>>>>>>>"));
					connect.KillProcess ();
					_logger.Debug (string.Format ("RestartDesktop : Connect <<<<<<<<<<<<<<<<<<<<<<<<<"));
					connect.ConnectDesktopView ();
					System.Threading.Thread.Sleep (MainWindow.mainWindow.environment.vOUSBDeviceProperties.RDPRestartDelayMS);

				} else {
					// method #2
					// FreeRDP 종료 요청을 새로 연결후 종료함 
					int pid = connect.GetConnectProcess ().Id;
					_logger.Debug (string.Format ("RestartDesktop : Connect <<<<<<<<<<<<<<<<<<<<<<<<<"));
					connect.ConnectDesktopView ();
					System.Threading.Thread.Sleep (MainWindow.mainWindow.environment.vOUSBDeviceProperties.RDPRestartDelayMS * 10);
					_logger.Debug (string.Format ("RestartDesktop : Kill Process >>>>>>>>>>>>>>>>>>>>>>>>>"));
					KillProcessByPID (pid);
				}

				connect.SetRequestRestart (false);

			}
		}

    }

	private void InitializeTimers()
    {
		runGetNewBrokerMsgTimer ();

		if (MainWindow.mainWindow.environment.vOUSBDeviceProperties.IsUseFileIOWebCamCtrl == true)
        {
    	try
            {
		    timerCheckWEBCAMCONTROL = new System.Timers.Timer();
		    timerCheckWEBCAMCONTROL.Interval = CHECK_INTERVAL_CHECKWEBCAM;
		    timerCheckWEBCAMCONTROL.Elapsed += new System.Timers.ElapsedEventHandler(timerCheck_WEBCAM_CONTROL_Tick);
		    timerCheckWEBCAMCONTROL.Start();
            }
    	catch (Exception ex)
            {
        _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


		startCheckBrokerServerStateTimer ();



	}



	// Event handler for webcam file I/O control timer
	//  타이머 시간 (1초)보다 Restart Delay 시간이 큰 경우를 고려하여, 진입조건strPreDaaS_USB_REDIRECT값을 미리 업데이트 함
	private void timerCheck_WEBCAM_CONTROL_Tick(object sender, EventArgs e)
    {
    try
        {


			if (File.Exists("/var/tmp/GRM-DaaS-USB-REDIRECT-SWITCH") == true) {
				strCurDaaS_USB_REDIRECT = System.IO.File.ReadAllText ("/var/tmp/GRM-DaaS-USB-REDIRECT-SWITCH");
			}

			//_logger.Debug (string.Format (" <<<<<<<<Pre {0} Cur {1} ", strPreDaaS_USB_REDIRECT, strCurDaaS_USB_REDIRECT));

			if (strPreDaaS_USB_REDIRECT.Contains("0") && strCurDaaS_USB_REDIRECT.Contains("1"))
            {
				_logger.Debug (string.Format (" <<<<<<<<<<<<<<<<<<<<<<<<<< Read GRM-DaaS-USB-REDIRECT-SWICH : {0} ", strCurDaaS_USB_REDIRECT));
				strPreDaaS_USB_REDIRECT = strCurDaaS_USB_REDIRECT;

				if (mainWindow.environment.vORedirectionProperties.IsLocalVideoDevice == true) {
					string strWebCamDeviceID = GetWebCamDevicdId ();
					if (strWebCamDeviceID != "") {
						if (mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection == "ALL" ||
								_strVDI_Connect_MODE.Contains (mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection)) {
							CtrlDeviceRedirection ("bind", strWebCamDeviceID);
						} else {
							RestartDesktop ();
						}

					}

				} else {
					_logger.Debug (string.Format ("[WebCamControl] No Webcam Device ID"));
				}

			}
      else if (strPreDaaS_USB_REDIRECT.Contains("1") && strCurDaaS_USB_REDIRECT.Contains("0"))
            {
				_logger.Debug (string.Format (" >>>>>>>>>>>>>>>>>>>>>>>>>> Read GRM-DaaS-USB-REDIRECT-SWICH : {0} ", strCurDaaS_USB_REDIRECT));
				strPreDaaS_USB_REDIRECT = strCurDaaS_USB_REDIRECT;

				if(mainWindow.environment.vORedirectionProperties.IsLocalVideoDevice == true) {
					string strWebCamDeviceID = GetWebCamDevicdId ();
					if (strWebCamDeviceID != "") {
						if (mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection == "ALL" ||
					_strVDI_Connect_MODE.Contains (mainWindow.environment.vOUSBDeviceProperties.UseUsbIpRedirection)) {
							CtrlDeviceRedirection ("unbind", strWebCamDeviceID);
						} else {
							RestartDesktop ();
							_logger.Info (string.Format ("AccessRequest_DaaS2KVM  KVM = 1"));
							DeviceResoureAccessReq.AccessRequest_DaaS2KVM (false, true);
						}
					} else {
								_logger.Debug (string.Format ("[WebCamControl] No Webcam Device ID"));
					}

				} else {
					DeviceResoureAccessReq.AccessRequest_DaaS2KVM (false, true);
				}

			} else {
				strPreDaaS_USB_REDIRECT = strCurDaaS_USB_REDIRECT;
			}


		}
    catch (Exception ex)
        {
    	_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
        }
    }
	// Broker Message Sytem Start : runGetNewBrokerMsgTimer()
	// Broker Message System Stop : StopBrokerSystemMessageReceiver()
	private void runGetNewBrokerMsgTimer()
	{
		if (this.environment.vOBrokerServerSystemMessageProperties.UseBrokerServerSystemMessage == false)
			return;

		try {

			if (_timerRequestCheckedMessage == null)
			{
				_timerRequestCheckedMessage = new System.Timers.Timer ();
				_timerRequestCheckedMessage.Interval = this.environment.vOBrokerServerSystemMessageProperties.InterValSec * 1000;
				_timerRequestCheckedMessage.Elapsed += new System.Timers.ElapsedEventHandler (RunMessageTimer);

				_timerRequestCheckedMessage.Start ();
				// run ui thread
				startThread_Message ();

			}

		} catch (Exception ex) {
			_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
		}

		return;
	}

	// Event handler for request message timer
	private void RunMessageTimer (object sender, EventArgs e)
	{
		// Request for message only during login
		//if(_voSelectedBrokerServer.AuthToken != string.Empty)
		RequestSystemMessage ();
	}
	//request broker server new system message
	private void RequestSystemMessage()
	{
		_NoticeMsgManager.InitServerInfoAndUserInfo (_voSelectedBrokerServer, HttpsClient.HTTP_PROTOCOL, _voUser.UserID, _voUser.ClientIP);
		_NoticeMsgManager.RequestSystemMessage ();
	}
	// Signal Thread start
	private void startThread_SignalMessage()
	{
		_thread_Signal_msg_run_flag = true;

		if(_thread_Signal_msg_response != null) {
			ReleaseSignalMessageThread ();
		}

		TimeSpan ts = new TimeSpan(0, 0, 2);

		Thread signal_thread = new Thread (delegate () {
			// Wait for a unix signal
			while (_thread_Signal_msg_run_flag) {
				int id = UnixSignal.WaitAny (DxSignals.dxSignals.signals, ts);
				if(id == 0) {
					if (MainWindow.mainWindow.environment != null) {
						if (MainWindow.mainWindow.environment.vOGeneralsProperties.AutoRDPConectFromSignal == true) {
							SignalAutoReconnectDesktop ();
						}
					}
				}
			}
		});

		signal_thread.Start ();
	}

	// thread message
	private void startThread_Message()
	{
		_thread_get_broker_newmsg_run_flag = true;

		if (_thread_msg_response != null) {
			ReleaseMessageThread ();
		}

		_thread_msg_response = new Thread (new ThreadStart (delegate {
			//check new message
			while (_thread_get_broker_newmsg_run_flag) {
				//show new message

				ShowMessageDialog ();

				Thread.Sleep (2000);
			}
		}));

		_thread_msg_response.Start ();
	}

	private void SignalAutoReconnectDesktop()
	{
		// 토큰이 있는지 여부확인, 
		if(_voSelectedBrokerServer.AuthToken != "") {
			Console.WriteLine ("Get AutoReconnectDesktop signal, Call DesktopPoolInfo");
			GetDesktopPoolInfo ();
		} else {
			Console.WriteLine ("Get AutoReconnectDesktop signal, no connect VM ");
		}
	}


		private void ShowMessageDialog()
	{
		string get_msg = "";
		try
		{
			get_msg = _NoticeMsgManager.GetResponseMsg ();

			if (get_msg.Length > 0)
			{
				_logger.Debug (string.Format ("response : {0}", get_msg.ToString()));

				SystemMessagePopup (get_msg);
			}


		} catch(Exception ex) {
			_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
		}
	}

	//show system message dialog(modaless)
	private void SystemMessagePopup(string Jdata)
	{

		List<MSG> getMsg = null;
		string seq_data = "";

		getMsg = _NoticeMsgManager.ParserGetmessageFromJsonData (Jdata);

		if (getMsg == null)
			return;
		if (getMsg.Count < 1)
			return;

		foreach(MSG msg in getMsg)
		{
			Gtk.Application.Invoke (delegate
			{
				seq_data += msg.seq_ + ",";
				startNotification (msg.content_, msg.sendTime_, msg.CallbackUrl_arrObj);
				// request Message received
				_NoticeMsgManager.RequestCheckMessage (seq_data);

			});
		}


		getMsg.Clear ();
		getMsg = null;

	}

	private void startNotification(string content, string sendTime, JArray arrData = null)
	{

		PopupMessageDialog _popupMessage = new PopupMessageDialog (arrData);

		_popupMessage.Title = "Message";
		_popupMessage.SetFlag (WidgetFlags.Toplevel);
		_popupMessage.SetPosition (Gtk.WindowPosition.CenterOnParent);
		//SetMessage
		_popupMessage.SetMessage (content, sendTime);

		_popupMessage.ShowAll();

	}

	// Signal Message Thread 종료
	private void ReleaseSignalMessageThread()
	{

		if (_thread_Signal_msg_response == null)
			return;

		Console.WriteLine ("Waiting Release SignalMessageThread");

		if (_thread_Signal_msg_response.Join (3000) == false) {
			_logger.Debug (string.Format (" Signal msg response thread exit timeout "));
		}
		_thread_Signal_msg_response = null;
		Console.WriteLine ("Release SignalMessageThread Complete.");

	}


	private void ReleaseMessageThread()
	{

		if (_thread_msg_response == null)
			return;

		Console.WriteLine ("Waiting Release ReleaseMessageThread");

		if (_thread_msg_response.Join(BrokerServerDefinded.DEF_NEW_GET_MESSAGE_INTERVAL_STOP)== false) {
			_logger.Debug (string.Format (" msg response thread exit timeout "));
		}
		_thread_msg_response = null;
		Console.WriteLine ("Release ReleaseMessageThread Complete.");
		return;
	}


	public void SendConfirmMessageToBroker (object _body, string _url)
	{
		if (_NoticeMsgManager != null) {

			_NoticeMsgManager.SendConfirmMessage (_body, _url);
		}
	}
	public int GetConnectListCount()
	{
		int nRetVal = 0;
		try {
			nRetVal = _ConnectList.Count;
		} catch (Exception) {
			_logger.Debug (string.Format (" Exception Error ConnectList "));
		}
		return nRetVal;
	}

	public void ProgramOptions (string [] options)
	{
		try {
			_strConfigFileName = ConfigurationManager.AppSettings ["ConfigrationPath"];
		} catch {
			_strConfigFileName = "config.ini";
		}

		if (options.Length > 0) {
			string mode = "";
			string value = "";
			for(int idx = 0; idx < options.Length; idx++) {
				mode = options [idx];

				switch(mode) {
					case "--config":
						value = options [++idx];
						value = value.Trim ();
						_strConfigFileName = value;
					break;

					case "--logname":
						var temp = options [++idx];
						temp = temp.Trim ();
						System.Environment.SetEnvironmentVariable ("LOG_SUFFIX", "_" + temp);
					break;

					default:
						break;
				}

			}
		}

		string strUserName = (System.Environment.UserName != "") ?
		System.Environment.UserName : (Environment.GetEnvironmentVariable ("USER") != "") ? Environment.GetEnvironmentVariable ("USER") : Environment.GetEnvironmentVariable ("USERNAME");

		if (strUserName.Equals("root")) {
			System.Environment.SetEnvironmentVariable ("LOG_USER", strUserName);

		} else {
			System.Environment.SetEnvironmentVariable ("LOG_USER", "home/"+strUserName);

		}


	}

	private void KillProcessByPID(int pid)
	{
		Process ps = null;

		try {
			ps = Process.GetProcessById (pid); // 프로세스가 없으면 exception 발생!
			ps.Kill ();
			ps.WaitForExit ();
		} catch (Exception ex) {
			_logger.Error (string.Format ("KillProcessByPID [0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
		}

	}

	public string GetWebCamDevicdId()
	{
		string strWebCamDeviceID = "";
		if (MainWindow.mainWindow.environment.vOUSBDeviceProperties.VideoCaptureDeviceID != "") {
			strWebCamDeviceID = MainWindow.mainWindow.environment.vOUSBDeviceProperties.VideoCaptureDeviceID;
		}

		if (strWebCamDeviceID == "") {
			strWebCamDeviceID = CommonUtils.GetWebCamUsbDeviceId ();
		}
		if (strWebCamDeviceID == "") {
			strWebCamDeviceID = CommonUtils.GetWebCamUsbDeviceForAllDevice ();
		}

		return strWebCamDeviceID;
	}
	/// <summary>
	///  접속환경 (재택,사무실)에 따라 브로커서버 를 선택적으로 표시하기 위해, 미리 정의된 파일에서 접속환경을 확인하기 위한 함수
	/// </summary>
	/// <returns>The VDI onnect mode.</returns>
	public string GetVDIConnectMODE()
	{
		_strVDI_Connect_MODE = CommonUtils.ReadTextFromFile ("/var/tmp/lightdm.mode", true);
		return _strVDI_Connect_MODE;
	}
	/// <summary>
	/// Rearranges the broker server.
	/// </summary>
	private void RearrangeBrokerServer()
	{
		// ClientLoc 업무망/인터넷망 구분없이 업데이트하기위한 함수 호출
		MainWindow.mainWindow.GetVDIConnectMODE ();

		var brokerServerlist = MainFunc.GetBrokerServerAll ();
		List<VOBrokerServerNew> tempResultList = new List<VOBrokerServerNew> ();
		List<VOBrokerServerNew> tempListFilted = new List<VOBrokerServerNew> ();
		List<VOBrokerServerNew> tempListUnFilted = new List<VOBrokerServerNew> ();

		for (int i = 0; i < brokerServerlist.Count; i++) {

			var broker = brokerServerlist [i];
			if (MainWindow.mainWindow.environment.vOGeneralsProperties.UseBrokerFilterTag == true) {
				string strVDI_Connect_MODE = MainWindow.mainWindow.GetVDIConnectMODE ();
				if (strVDI_Connect_MODE.Contains (broker.tag) == false) 
					tempListUnFilted.Add (broker);
				else 
					tempListFilted.Add (broker);
			} else {
				tempListUnFilted.Add (broker);
			}
		}
		tempResultList.AddRange (tempListFilted);
		tempResultList.AddRange (tempListUnFilted);

		MainWindow.mainWindow.environment.BrokerServers = tempResultList;

	}





	private void AfterEffectInitializeComponent ()
	{
		// hide banner imnage
		if (MainWindow.mainWindow.environment.vOCustomUIProperties.ShowBanner == false) {
			vbox3.Remove (image28);
		}

		//  Title명 앞에 prefix를 추가합니다. 
		string sTitle = "";

		if (MainWindow.mainWindow.environment.vOGeneralsProperties.AppNamePrefix != "") {
			sTitle = MainWindow.mainWindow.environment.vOGeneralsProperties.AppNamePrefix + "_";
		} else {
			sTitle = "";
		}
		if (MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleEnable == true) {
			this.Title = sTitle + MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleText;
		} else {
			this.Title = sTitle + "DaaSXpert Client";
		}
		if(MainWindow.mainWindow.environment.vOCustomUIProperties.ShowMainWindow == true) {
			Show ();
		} else {
			Hide (); //this.SetSizeRequest (0, 0);

		}

	}


}

