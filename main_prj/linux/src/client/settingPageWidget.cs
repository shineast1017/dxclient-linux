using System;
using System.Linq;
using System.Collections.Generic;
using HCVK.HCVKSLibrary;
using Gtk;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class settingPageWidget : Gtk.Bin
    {
		List<string> _protocol = new List<string> { "RDP", "Spice", "DXGP" };

        Dictionary<string, string> mapIPAddress;



        public settingPageWidget()
        {
            this.Build();

			SetSettingPageInfo();
			AfterEffectInitializeComponent();

		}

		public void SetSettingPageInfo()
		{
			//Language
			if (MainWindow.mainWindow.environment.vOGeneralsProperties.SetLanguage == "en-US")
				this.cbxLang.Active = 1;
			else
				this.cbxLang.Active = 0;

            // Network
            SetNetwork();

            //Auto Start
            if (MainWindow.mainWindow.environment.vOAutoLoginProperties.IsAutoLogin)
                this.radioAutoStartOn.Active = true;
            else
                this.radioAutoStartOff.Active = true;

            // Server
            if (!string.IsNullOrEmpty(MainWindow.mainWindow.environment.vOAutoLoginProperties.ServerName))
                this.tbxServerName.Text = MainWindow.mainWindow.environment.vOAutoLoginProperties.ServerName;
            if (!string.IsNullOrEmpty(MainWindow.mainWindow.environment.vOAutoLoginProperties.DesktopName))
                this.tbxDesktopName.Text = MainWindow.mainWindow.environment.vOAutoLoginProperties.DesktopName;

            // Enable Bookmark
            if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseBookmark)
                this.radioEnableBookmarkOn.Active = true;
            else
                this.radioEnableBookmarkOff.Active = true;

            //Dual Monitor
            if (MainWindow.mainWindow.environment.vODisplayProperties.IsUseMultiMonitor)
                this.radioDualOn.Active = true;
            else
                this.radioDualOff.Active = true;

            //Audio
            if (MainWindow.mainWindow.environment.vOPerformanceProperties.IsAudio)
                this.radioAudioOn.Active = true;
            else
                this.radioAudioOff.Active = true;

			// Auto Reconnect
			if (MainWindow.mainWindow.environment.vOGeneralsProperties.RdpAutoReconnectEnable)
				this.radioAutoReconnectOn.Active = true;
			else
				this.radioAutoReconnectOff.Active = true;



			this.cbxProtocol.Active = _protocol.IndexOf(MainWindow.mainWindow.environment.vOGeneralsProperties.Protocol);

            this.cbxResolution.Active = MainWindow.mainWindow.environment.vODisplayProperties.Resolution;

			this.cbxColorDepth.Active = MainWindow.mainWindow.environment.vODisplayProperties.ColorDepth;

            this.cbxSpeed.Active = MainWindow.mainWindow.environment.vOPerformanceProperties.BandWidthQoS;

			this.chkClipboard.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsClipBoard;
			this.chkLocalPrinter.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalPrinter;
			this.chkLocalDriver.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalDrive;
			this.chkLocalDevice.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalDevice;
			this.chkLocalPort.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalPort;
			this.chkSmartCard.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsSmartCard;
			this.chkBitmapCaching.Active = MainWindow.mainWindow.environment.vOPerformanceProperties.IsBitmapCaching;

			this.chkMicrophone.Active = MainWindow.mainWindow.environment.vOPerformanceProperties.AudioCapture;

             this.chkAutoClientIP.Active = MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseAccurateLocalIP;

            this.chkVideoCaptureDevice.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalVideoDevice;

            this.chkLocalPort.Sensitive = false;
            this.chkSmartCard.Sensitive = false;

			this.chkUseH264.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsUseH264;
			this.chkExtRDPViewer.Active = MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseExtRDPViewer;
			this.chkUseRDPEcam.Active = MainWindow.mainWindow.environment.vORedirectionProperties.IsUseRDPEcam;
			// 동적해상도 설정
			this.chkDynamicResolutionUpdate.Active = MainWindow.mainWindow.environment.vODisplayProperties.DynamicAutoResolutionUpdate;
		}

        public void SetNetwork()
        {
            this.cbxNetwork.Clear();

            CellRendererText cell = new CellRendererText();
            this.cbxNetwork.PackStart(cell, false);
            this.cbxNetwork.AddAttribute(cell, "text", 0);

            ListStore store = new ListStore(typeof(string));
            this.cbxNetwork.Model = store;

            mapIPAddress = NetworkManager.GetNetworkAdaptor();
            foreach (KeyValuePair<string, string> item in mapIPAddress)
            {
                this.cbxNetwork.AppendText(item.ToString());
            }

            this.cbxNetwork.Active = MainWindow.mainWindow.environment.vOGeneralsProperties.NetworkAdpatorIdx;
        }

        protected void OnCbxLangChanged(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vOGeneralsProperties.SetLanguage = this.cbxLang.ActiveText;
			MainWindow.mainWindow.SaveConfiguration();
		}

		protected void OnCbxNetworkChanged(object sender, EventArgs e)
		{
            MainWindow.mainWindow.environment.vOGeneralsProperties.NetworkAdpatorIdx = this.cbxNetwork.Active;

            try
            {
                MainWindow.mainWindow.SetNetworkInfo();
            }
            catch (Exception)
            {

            }
        }

		protected void OnRadioAutoStartOnGroupChanged(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vOAutoLoginProperties.IsAutoLogin = this.radioAutoStartOn.Active;
			if(!this.radioAutoStartOn.Active)
			{
				this.labelServerName.Hide();
                this.labelDesktopName.Hide();

				this.tbxServerName.Hide();
				this.tbxDesktopName.Hide();

				this.tbxServerName.Text = "";
				this.tbxDesktopName.Text = "";

				//MainWindow.mainWindow.UnSetAutoStartInfo();
				MainWindow.mainWindow.environment.vOAutoLoginProperties.ServerIP = "";
				MainWindow.mainWindow.environment.vOAutoLoginProperties.ServerPort = 0;

				MainWindow.mainWindow.environment.vOAutoLoginProperties.UserID = "";
				MainWindow.mainWindow.environment.vOAutoLoginProperties.UserPW = "";

				MainWindow.mainWindow.environment.vOAutoLoginProperties.PoolID = 0;

				MainWindow.mainWindow.environment.vOAutoLoginProperties.ServerName = "";
				MainWindow.mainWindow.environment.vOAutoLoginProperties.DesktopName = "";
			}
			else
			{
				this.labelServerName.Show();
				this.labelDesktopName.Show();

				this.tbxServerName.Show();
				this.tbxDesktopName.Show();

			}
		}

		protected void OnRadioEnableBookmarkOnGroupChanged(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseBookmark = this.radioEnableBookmarkOn.Active;
			if (MainFunc.callbackEnableBookmark != null)
				MainFunc.callbackEnableBookmark(this.radioEnableBookmarkOn.Active);
		}

		protected void OnRadioDualOnGroupChanged(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vODisplayProperties.IsUseMultiMonitor = this.radioDualOn.Active;
		}

		protected void OnRadioAudioOnGroupChanged(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vOPerformanceProperties.IsAudio = this.radioAudioOn.Active;
		}

		protected void OnCbxProtocolChanged(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vOGeneralsProperties.Protocol = this.cbxProtocol.ActiveText;
		}

		protected void OnCbxResolutionChanged(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vODisplayProperties.Resolution = this.cbxResolution.Active;
		}

		protected void OnCbxSpeedChanged(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vOPerformanceProperties.BandWidthQoS = this.cbxSpeed.Active;
			MainWindow.mainWindow.environment.vOPerformanceProperties.BandWidthQoSName = this.cbxSpeed.ActiveText;
		}

		protected void OnChkClipboardToggled(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vORedirectionProperties.IsClipBoard = this.chkClipboard.Active;
		}

		protected void OnChkLocalPrinterToggled(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalPrinter = this.chkLocalPrinter.Active;
		}

		protected void OnChkLocalDriverToggled(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalDrive = this.chkLocalDriver.Active;
		}

		protected void OnChkLocalDeviceToggled(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalDevice = this.chkLocalDevice.Active;
		}

		protected void OnChkLocalPortToggled(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalPort = this.chkLocalPort.Active;
		}

		protected void OnChkSmartCardToggled(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vORedirectionProperties.IsSmartCard = this.chkSmartCard.Active;
		}

		protected void OnChkBitmapCachingToggled(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vOPerformanceProperties.IsBitmapCaching = this.chkBitmapCaching.Active;
		}

		protected void OnChkMicrophoneToggled(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vOPerformanceProperties.AudioCapture = this.chkMicrophone.Active;
		}

		protected void OnCbxColorDepthChanged(object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vODisplayProperties.ColorDepth = this.cbxColorDepth.Active;
		}

        protected void ClickNetworkRefresh(object sender, EventArgs e)
        {
            SetNetwork();
        }

        protected void OnChkAutoClientIPToggled(object sender, EventArgs e)
        {
            // AutoClientIp Toggled Button Event  Save To App Config 
          MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseAccurateLocalIP =
            this.chkAutoClientIP.Active;
        }
        
    // For Initialize Component AfterEffect Custom site
		private void AfterEffectInitializeComponent()
        {

            // hide resource Tab
			if (MainWindow.mainWindow.environment.vOGeneralsProperties.SettingResourceUi == false)
            {
                this.notebook1.RemovePage(2);
            }

            if (MainWindow.mainWindow.environment.vOGeneralsProperties.ShowBookMarkMenu == false)
            {
                this.radioEnableBookmarkOn.Sensitive = false;
                this.radioEnableBookmarkOff.Sensitive = false;
            }
        }

        protected void OnChkVideoCaptureDeviceToggled(object sender, EventArgs e)
        {
            //  Toggled Button Event  
            MainWindow.mainWindow.environment.vORedirectionProperties.IsLocalVideoDevice = chkVideoCaptureDevice.Active;
        }

		protected void OnChkUseH264Toggled (object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vORedirectionProperties.IsUseH264 = chkUseH264.Active;
		}

		protected void OnChkExtRDPViewerToggled (object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseExtRDPViewer = chkExtRDPViewer.Active;
		}

		protected void OnChkUseRDPEcamToggled (object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vORedirectionProperties.IsUseRDPEcam = chkUseRDPEcam.Active;
		}

		protected void OnRadioAutoReconnectOnGroupChanged (object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vOGeneralsProperties.RdpAutoReconnectEnable = this.radioAutoReconnectOn.Active;
		}

		protected void OnChkDynamicResolutionUpdateToggled (object sender, EventArgs e)
		{
			MainWindow.mainWindow.environment.vODisplayProperties.DynamicAutoResolutionUpdate = this.chkDynamicResolutionUpdate.Active;
		}
	}
}
