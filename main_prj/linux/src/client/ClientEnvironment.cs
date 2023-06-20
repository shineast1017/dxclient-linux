using System;
using System.Collections.Generic;
using System.Configuration;
using HCVK.HCVKSLibrary.VO;

namespace client
{
	[Serializable()]
	public sealed class ClientEnvironment
    {
		public ClientEnvironment()
        {
			this.BrokerServers = new List<VOBrokerServerNew>();
			this.vODisplayProperties = new VODisplayProperties();
			this.vOAutoLoginProperties = new VOAutoLoginProperties();
			this.vOPerformanceProperties = new VOPerformanceProperties();
			this.vORedirectionProperties = new VORedirectionProperties();
			this.vOGeneralsProperties = new VOGeneralsProperties();
			this.vOBrokerServerSystemMessageProperties = new VOBrokerServerSystemMessageProperties ();
			this.vOCustomUIProperties = new VOCustomUIProperties ();
			this.vOUSBDeviceProperties = new VOUSBDeviceProperties ();
			this.vORDPViewerOptionProperties = new VORDPViewerOptionProperties ();
			this.Bookmarks = new List<VOBookmark> ();
		}

		public List<VOBrokerServerNew> BrokerServers { get; set; }
		public VODisplayProperties vODisplayProperties { get; set; }
		public VOAutoLoginProperties vOAutoLoginProperties { get; set; }
		public VOPerformanceProperties vOPerformanceProperties { get; set; }
		public VORedirectionProperties vORedirectionProperties { get; set; }
		public VOGeneralsProperties vOGeneralsProperties { get; set; }
		public VOBrokerServerSystemMessageProperties vOBrokerServerSystemMessageProperties { get; set; }
		public VOCustomUIProperties vOCustomUIProperties { get; set; }
		public VOUSBDeviceProperties vOUSBDeviceProperties { get; set; }
		public VORDPViewerOptionProperties vORDPViewerOptionProperties { get; set; }
		public List<VOBookmark> Bookmarks { get; set; }

	}

	[Serializable()]
	public class VODisplayProperties
    {
		public VODisplayProperties()
		{
			Resolution = Int32.Parse(ConfigurationManager.AppSettings["ResolutionIdx"]);
			IsUseMultiMonitor = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["DualMonitor"], TypeCode.Boolean);
			ColorDepth = Int32.Parse(ConfigurationManager.AppSettings["ColorDepthIdx"]);
			DynamicAutoResolutionUpdate = true;
		}

		public int Resolution { get; set; }
		public bool IsAutoResolution { get; set; }

		public bool IsConnectionBar { get; set; }
		public bool IsUseMultiMonitor { get; set; }
		public int ColorDepth { get; set; }
		public bool DynamicAutoResolutionUpdate { get; set; }
	}

    [Serializable()]
	public class VOAutoLoginProperties
    {
		public VOAutoLoginProperties()
		{
			try {
				IsAutoLogin = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["AutoStart"], TypeCode.Boolean);
			} catch {
				IsAutoLogin = false;
			}
						RetryTimes = 0;
            IsGstewayServer = false;
            ServerIP = "";
            ServerPort = 0;
            ServerName = "";
            DesktopName = "";
            UserID = "";
            UserPW = "";
            PoolID = 0;

        }

		public bool IsAutoLogin { get; set; }
		public bool IsAutoConnection { get; set; }
		public int RetryTimes { get; set; }

		public bool IsGstewayServer { get; set; }
		public string ServerIP { get; set; }
		public int ServerPort { get; set; }
		public string ServerName { get; set; }
        public string DesktopName { get; set; }
		//public string EnocdedUserID { get; set; }
		public string UserID { get; set; }
		//public string EnocdedUserPW { get; set; }
		public string UserPW { get; set; }
		public int PoolID { get; set; }
    }

	[Serializable()]
    public class VOPerformanceProperties
    {
		public VOPerformanceProperties()
		{
            try
            {
                IsAudio = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["Audio"], TypeCode.Boolean);
            }
            catch
            {
                IsAudio = false;
            }
            try
            {
                BandWidthQoS = Int32.Parse(ConfigurationManager.AppSettings["SpeedIdx"]);
            }
            catch
            {
                BandWidthQoS = 0;
            }
            try
            {
                IsBitmapCaching = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsBitmapCaching"], TypeCode.Boolean);
            }
            catch
            {
                IsBitmapCaching = false;
            }

			try {
				AudioCapture = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["AudioCapture"], TypeCode.Boolean);
			} catch {
				AudioCapture = true;
			}

				BandWidthQoSName = "auto";
		}

		public bool IsAudio { get; set; }
		public bool IsBitmapCaching { get; set; }
		public int BandWidthQoS { get; set; }
		public string BandWidthQoSName { get; set; }
		//public int AudioPlayback { get; set; }
		public bool AudioCapture { get; set; }
		//public int AudioQoS { get; set; }
        public string EtcProperties { get; set; }
    }

	[Serializable()]
    public class VORedirectionProperties
    {
		public VORedirectionProperties()
		{
            try
            {
                IsLocalVideoDevice = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsLocalVideoDevice"], TypeCode.Boolean);
            }
            catch
            {
                IsLocalVideoDevice = false;
            }
            try
            {
                IsLocalPrinter = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsLocalPrinter"], TypeCode.Boolean);
            }
            catch
            {
                IsLocalPrinter = false;
            }
            try
            {
                IsClipBoard = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsClipBoard"], TypeCode.Boolean);
            }
            catch
            {
                IsClipBoard = false;
            }
            try
            {
                IsSmartCard = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsSmartCard"], TypeCode.Boolean);
            }
            catch
            {
                IsSmartCard = false;
            }
            try
            {
                IsLocalPort = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsLocalPort"], TypeCode.Boolean);
            }
            catch
            {
                IsLocalPort = false;
            }
            try
            {
                IsLocalDrive = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsLocalDrive"], TypeCode.Boolean);
            }
            catch
            {
                IsLocalDrive = false;
            }
            try
            {
                IsLocalVideoDevice = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsLocalVideoDevice"], TypeCode.Boolean);
            }
            catch
            {
                IsLocalVideoDevice = false;
            }
            try
            {
                IsLocalDevice = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsLocalDevice"], TypeCode.Boolean);
            }
            catch
            {
                IsLocalDevice = false;
            }

			IsUseH264 = true;
			IsUseRDPEcam = false;

		}
          
		public bool IsLocalPrinter { get; set; }
		public bool IsClipBoard { get; set; }
		public bool IsSmartCard { get; set; }
		public bool IsLocalPort { get; set; }
		public bool IsLocalDrive { get; set; }
		public bool IsLocalDevice { get; set; }
        public bool IsLocalVideoDevice { get; set; }
		public bool IsUseH264 { get; set; } // Thincast client 웹캠 옵션 h264 사용 유무 (네트워크 트래픽 영향)
		public bool IsUseRDPEcam { get; set; } // freerdp rdpecam 용 옵션

	}

	[Serializable()]
    public class VOGeneralsProperties
    {
		public VOGeneralsProperties()
		{
			SetLanguage = ConfigurationManager.AppSettings["Language"];
			Protocol = ConfigurationManager.AppSettings["Protocol"];
			IsUseBookmark = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["Bookmark"], TypeCode.Boolean);

            // rdp autoreconnect cnt option  is valid use
            try 
            { 
                RdpAutoReconnectCnt = ConfigurationManager.AppSettings["AutoReconnectRDPtryCnt"];
                RdpAutoReconnectEnable = 
                    (bool)Convert.ChangeType(ConfigurationManager.AppSettings["AutoRDPReconnectEnable"], TypeCode.Boolean);
            }
            catch
            {
                RdpAutoReconnectCnt = "0"; // zero is not use
                RdpAutoReconnectEnable = false;
            }

            try
            {
                IsUseAccurateLocalIP = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["AccurateLocalIP"], TypeCode.Boolean);
            }
            catch
            {
                IsUseAccurateLocalIP = false;
            }
            try
            {
                SecureUiBroker = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["SecureUiBroker"], TypeCode.Boolean);
            }
            catch
            {
                SecureUiBroker = false;
            }
		
			try
            {
				SettingResourceUi = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["SettingResourceUi"], TypeCode.Boolean);
            }
    	catch
            {
				SettingResourceUi = true;
            }


            try
            {
                IsUseDBUSLogIn = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["IsUseDBUSLogIn"], TypeCode.Boolean);
            }
            catch
            {
                IsUseDBUSLogIn = false;
            }




			try
			{
				ShowBookMarkMenu = (bool)Convert.ChangeType(ConfigurationManager.AppSettings["ShowBookMarkMenu"], TypeCode.Boolean);
            }
      catch
            {
				ShowBookMarkMenu = false;
            }

			try 
			{
				ShowSettingMenu = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowSettingMenu"], TypeCode.Boolean);
			} 
			catch 
			{
				ShowSettingMenu = false;
			}

			try
			{
				IsUseMasterKeyLogin = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["IsUseMasterKeyLogin"], TypeCode.Boolean);
			} catch {
				IsUseMasterKeyLogin = false;
			}
			try {
				RDPExtraOption = ConfigurationManager.AppSettings ["RDPExtraOption"];
			} catch {
				RDPExtraOption = "";
			}


			try {
				AppNamePrefix = ConfigurationManager.AppSettings ["AppNamePrefix"];
			} catch {
				AppNamePrefix = "";
			}
			try {
				ViewerNamePrefix = ConfigurationManager.AppSettings ["ViewerNamePrefix"];
			} catch {
				ViewerNamePrefix = "";
			}
			try {
				AutoRDPConectFromSignal = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["AutoRDPConectFromSignal"], TypeCode.Boolean);
			} catch {
				AutoRDPConectFromSignal = false;
			}
			try {

				AgentMsgSkipCnt = Int32.Parse (ConfigurationManager.AppSettings ["AgentMsgSkipCnt"]); 
			} catch {
				AgentMsgSkipCnt = 10;
			}
			try {
				UseBrokerFilterTag = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["UseBrokerFilterTag"], TypeCode.Boolean);
			} catch {
				UseBrokerFilterTag = false;
			}
			try {
				MasterUserPinFileName = ConfigurationManager.AppSettings ["MasterUserPinFileName"];
			} catch {
				MasterUserPinFileName = "";
			}
			try {
				IsUseRDPDefaultOption = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["IsUseRDPDefaultOption"], TypeCode.Boolean);
			} catch {
				IsUseRDPDefaultOption = true;
			}
			try {
				IsUseRDPToolbar = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["IsUseRDPToolbar"], TypeCode.Boolean);
			} catch {
				IsUseRDPToolbar = true;
			}
			try {
				RDPGfxMode = Int32.Parse (ConfigurationManager.AppSettings ["RDPGfxMode"]);
			} catch {
				RDPGfxMode = 0;
			}
			try {
				IsUseRSAEncrpyt = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["IsUseRSAEncrpyt"], TypeCode.Boolean);
			} catch {
				IsUseRSAEncrpyt = false;
			}
			try {
				IsUseExtRDPViewer = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["IsUseExtRDPViewer"], TypeCode.Boolean);
			} catch {
				IsUseExtRDPViewer = false;
			}
			try {
				GDIRendering = ConfigurationManager.AppSettings ["GDIRendering"];
			} catch {
				GDIRendering = "hw";
			}
			try {
				IsUseTerminalAuth = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["IsUseTerminalAuth"], TypeCode.Boolean);
			} catch {
				IsUseTerminalAuth = false;
			}
			try {
				IsUseUserNameAsUseID = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["IsUseUserNameAsUseID"], TypeCode.Boolean);
			} catch {
				IsUseUserNameAsUseID = false;
			}

			IsAutoConnectRDP = false;
			AutoConnectRDPID = "";
			AutoConnectRDPPW = "";
			BuildDate = "";
		}

		public bool ShowBookMarkMenu { get; set; } // 메뉴 북마크 표시 유무
		public bool ShowSettingMenu { get; set; } // 메뉴 세팅 표시 유무
		public string SetLanguage { get; set; }
		public int NetworkAdpatorIdx { get; set; }
        public bool IsUseAccurateLocalIP { get; set;  } //  Client Network Interface IP Detect AutoMode 
        public string Protocol { get; set; }
		public bool IsUseBookmark { get; set; }
		public bool SecureUiBroker { get; set; } // IP, Port Secure Hide and password * display
        public bool SettingResourceUi { get; set; } // Resource Tab Hide

        
        public bool RdpAutoReconnectEnable { get; set; }
        public string RdpAutoReconnectCnt { get; set; }

        public bool IsUseDBUSLogIn { get; set; }
            
			public bool IsUseMasterKeyLogin { get; set; }
			public string RDPExtraOption { get; set; }
			public string AppNamePrefix { get; set; } // Title, RDP Viewer  접두어
			public string ViewerNamePrefix { get; set; } // RDP Viewer  접두어
			public bool AutoRDPConectFromSignal { get; set; } // signal 신호에 따라 자동 RDP 접속 유무
			public int AgentMsgSkipCnt { get; set; } // Agent HeartBeat Skip Count
			public bool IsAutoConnectRDP { get; set; }// 자동 RDP 접속 기능 (브로커 정보 있을때)
			public string AutoConnectRDPID { get; set; }// 자동 RDP 접속용 ID
			public string AutoConnectRDPPW { get; set; }// 자동 RDP 접속용 PW
		public bool UseBrokerFilterTag { get; set; }
		public string MasterUserPinFileName { get; set; }
		public bool IsUseRDPDefaultOption { get; set; }
		public bool IsUseRDPToolbar { get; set; }
		public int RDPGfxMode { get; set; } // gfx 모드 0: gfx:rfx 1: gfx-progressive 2: gfx-small-cache 3.gfx-thin-client 4. none
		public bool IsUseRSAEncrpyt { get; set; } // 비밀번호 변경시 공개키 암호시스템(RSA)
		public bool IsUseExtRDPViewer { get; set; } // thincast client 사용 여부
		public string GDIRendering { get; set; } // gdi:sw 사용여부 (하드웨어 가속 사용안함)
		public bool IsUseTerminalAuth { get; set; } // 단말기 인증 사용 여부
		public bool IsUseUserNameAsUseID { get; set; } // username을 userid로 고정하는 기능 사용 여부 
		public string BuildDate { get; set; } // 빌드 날짜 표기

	}

	[Serializable ()]
	public class VOBrokerServerSystemMessageProperties {
		public VOBrokerServerSystemMessageProperties ()
		{
			try {
				InterValSec = Int32.Parse (ConfigurationManager.AppSettings ["InterValSec"]);

			} catch {
				InterValSec = 60; // initial value is 60 second
			}

			try {
				UseBrokerServerSystemMessage = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["UseBrokerServerSystemMessage"], TypeCode.Boolean);
			} catch {
				UseBrokerServerSystemMessage = false;
			}


		}

		public bool UseBrokerServerSystemMessage { get; set; }
		public int InterValSec { get; set; }

	}

	[Serializable ()]
	public class VOCustomUIProperties {
		public VOCustomUIProperties ()
		{
			try {
				ShowRemoveBtnInServerItem = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowRemoveBtnInServerItem"], TypeCode.Boolean);
			} catch {
				ShowRemoveBtnInServerItem = true;
			}
			try {
				ShowEditBtnInServerItem = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowEditBtnInServerItem"], TypeCode.Boolean);
			} catch {
				ShowEditBtnInServerItem = true;
			}
			try {
				ShowAddServerBtnInServerPage = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowAddServerBtnInServerPage"], TypeCode.Boolean);
			} catch {
				ShowAddServerBtnInServerPage = true;
			}
			try {
				ShowPolicyBtnInDesktopItem = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowPolicyBtnInDesktopItem"], TypeCode.Boolean);
			} catch {
				ShowPolicyBtnInDesktopItem = true;
			}
			try {
				ShowEditBtnInDesktopItem = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowEditBtnInDesktopItem"], TypeCode.Boolean);
			} catch {
				ShowEditBtnInDesktopItem = true;
			}
			try {
				ShowPowerOffBtn = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowPowerOffBtn"], TypeCode.Boolean);
			} catch {
				ShowPowerOffBtn = false;
			}

			try {
				ShowBanner = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowBanner"], TypeCode.Boolean);
			} catch {
				ShowBanner = true;
			}
			try {
				ShowCompanyName = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowCompanyName"], TypeCode.Boolean);
			} catch {
				ShowCompanyName = true;
			}
			try {
				ShowMainWindow = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowMainWindow"], TypeCode.Boolean);
			} catch {
				ShowMainWindow = true;
			}
			// custom title option  is valid use
			try {
				CustomTitleText = ConfigurationManager.AppSettings ["CustomTitleText"];
				CustomTitleEnable =
					(bool)Convert.ChangeType (ConfigurationManager.AppSettings ["CustomTitleEnable"], TypeCode.Boolean);
			} catch {
				CustomTitleText = "DaaSXpert Client"; // zero is not use
				CustomTitleEnable = false;
			}
			try {
				ShowContextMenuInServerItem = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["ShowContextMenuInServerItem"], TypeCode.Boolean);
			} catch {
				ShowContextMenuInServerItem = true;
			}
		}

		public bool ShowRemoveBtnInServerItem { get; set; } // don't use or use Remove button in Server Item
		public bool ShowEditBtnInServerItem { get; set; } // don't use or use Edit button in Server Item
		public bool ShowContextMenuInServerItem { get; set; } // don't use or use ContextMenu in Server Item

		public bool ShowAddServerBtnInServerPage { get; set; }
		public bool ShowPolicyBtnInDesktopItem { get; set; }
		public bool ShowEditBtnInDesktopItem { get; set; }
		public bool ShowPowerOffBtn { get; set; }
		public bool ShowBanner { get; set; }
		public bool ShowCompanyName { get; set; }
		public bool ShowMainWindow { get; set; } // 메인윈도우 Show/Hide 유무


		public bool CustomTitleEnable { get; set; }
		public string CustomTitleText { get; set; }


	}

	[Serializable ()]
	public class VOUSBDeviceProperties {
		public VOUSBDeviceProperties ()
		{
			try {
				IsUseFileIOWebCamCtrl = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["IsUseFileIOWebCamCtrl"], TypeCode.Boolean);
			} catch {
				IsUseFileIOWebCamCtrl = false;
			}
			try {
				IsUseAPIWebCamCtrl = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["IsUseAPIWebCamCtrl"], TypeCode.Boolean);
			} catch {
				IsUseAPIWebCamCtrl = false;
			}



			try {
				RDPRestartDelayMS = Int32.Parse (ConfigurationManager.AppSettings ["RDPRestartDelayMS"]);

			} catch {
				RDPRestartDelayMS = 1000; // initial value is 1 second
			}
			try {
				UseUsbIpRedirection = ConfigurationManager.AppSettings ["UseUsbIpRedirection"];
			} catch {
				UseUsbIpRedirection = "NONE";
			}
			try {
				DeviceRedirectionPort = ConfigurationManager.AppSettings ["DeviceRedirectionPort"];
			} catch {
				DeviceRedirectionPort = "3240";
			}
			try {
				UsbIpModulePath = ConfigurationManager.AppSettings ["UsbIpModulePath"];
			} catch {
				UsbIpModulePath = "/usr/sbin";
			}


			ReConnectRDPMethod = 0;
			VideoCaptureDeviceID = "";
			LocalDeviceDeviceID = "";


		}
		public string VideoCaptureDeviceID { get; set; }
		public string LocalDeviceDeviceID { get; set; }
		public int ReConnectRDPMethod { get; set; } // 웹캠 제어권 변경 관련 FreeRDP 재접속 방법 0: 기존RDP kill -> 신규RDP 접속 1: 신규RDP 접속 -> 기존 RDP kill
		public bool IsUseFileIOWebCamCtrl { get; set; }
		public bool IsUseAPIWebCamCtrl { get; set; }
		public int RDPRestartDelayMS { get; set; }
		public string UseUsbIpRedirection { get; set; }// usb device 리디렉션을 IP로 연결 (usbip 이용) all: 전체, internal : 업무망에서만 external: 인터넷망만 none: 사용안함 / 판단기준 /var/tmp/ligtdm.mode 파일 값
		public string DeviceRedirectionPort { get; set; } // usb device ip 리디렉션용  포트 (Default:3240)
		public string UsbIpModulePath { get; set; } 
	
	}

	[Serializable ()]
	public class VORDPViewerOptionProperties {
		public VORDPViewerOptionProperties ()
		{
			try {
				RDPDomain = ConfigurationManager.AppSettings ["RDPDomain"];
			} catch {
				RDPDomain = "";
			}
			try {
				IsUseRDPViewerTitleUpdate = (bool)Convert.ChangeType (ConfigurationManager.AppSettings ["IsUseRDPViewerTitleUpdate"], TypeCode.Boolean);
			} catch {
				IsUseRDPViewerTitleUpdate = false;
			}
		}

		public string RDPDomain { get; set; }
		public bool IsUseRDPViewerTitleUpdate { get; set; }

	}

}
