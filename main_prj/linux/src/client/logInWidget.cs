using System;
using GLib;
using HCVK.HCVKSLibrary.VO;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class logInWidget : Gtk.Bin
    {
		private VOBrokerServerNew _brokerServer = null;
		public VOBrokerServerNew BrokerServerInfo
        {
            get { return this._brokerServer; }
            set
            {
                if (value != this._brokerServer)
                {
                    this._brokerServer = value;
					SetServerInfo();
                }
            }
        }

		public logInWidget()
        {
            this.Build();

			if(MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseUserNameAsUseID == true) {
				this.tbxUserID.Text = HCVK.HCVKSLibrary.CommonUtils.GetUserName();
				this.tbxUserID.Sensitive = false;
			}



			StyleSheet.SetStyleCancelButton(this.btnCancel);
			StyleSheet.SetStyleButton(this.btnLogin);
		}

		public void SetServerInfo()
		{
			this.tbxServerName.Text = _brokerServer.BrokerServerDesc;
			this.tbxServerIP.Text = _brokerServer.BrokerServerIP;
			this.tbxServerPort.Text = _brokerServer.BrokerServerPort;

            if (MainWindow.mainWindow.environment.vOGeneralsProperties.SecureUiBroker == true)
            {
                this.tbxServerIP.Visibility = false;
                this.tbxServerPort.Visibility = false;

            }

			// dxClient-W 에 있는, UserID save 기능을 가져옵니다.
			if(string.IsNullOrEmpty(_brokerServer.UserID)) {
				this.chkIDSave.Active = false;
			} else {
				this.chkIDSave.Active = true;
				this.tbxUserID.Text = _brokerServer.UserID;
			}

		}

		protected void OnBtnCancelClicked(object sender, EventArgs e)
		{
			MainWindow.mainWindow._voBookmark = null;

			if (MainFunc.callbackShowDesktopListPage != null)
				MainFunc.callbackShowDesktopListPage(false);
		}

		protected void OnBtnLoginClicked(object sender, EventArgs e)
		{
			LogIn();
		}

		[ConnectBefore]
		protected void OnTbxUserIDKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return)
			{
				LogIn();
			}
		}

		[ConnectBefore]
		protected void OnTbxPasswordKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
        {
			if (args.Event.Key == Gdk.Key.Return)
            {
                LogIn();
            }
        }

		private void LogIn()
		{
            //if (MainFunc.callbackShowDesktopListPage != null)
            //    MainFunc.callbackShowDesktopListPage();
            if (MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleEnable == true)
            {
                MainWindow.mainWindow.SetTextOfLabelState(MainWindow.mainWindow.environment.vOCustomUIProperties.CustomTitleText);
            }
            else
            {
                MainWindow.mainWindow.SetTextOfLabelState("DaaSXpert Client");
            }

			if(this.chkIDSave.Active == true) {
				this._brokerServer.UserID = this.tbxUserID.Text;
			}


			MainWindow.mainWindow.ExcuteLogin(this._brokerServer, this.tbxUserID.Text, this.tbxPassword.Text);

            this.tbxPassword.Text = "";
        }
	}
}
