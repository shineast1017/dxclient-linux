using System;
namespace client {
	public partial class ChangePasswordDialog : Gtk.Dialog {
		public ChangePasswordDialog (string _strMessage)
		{
			MainFunc.callbackCheckPasswordRule = CheckPasswordRule;
			this.Build ();

			this.Title = "DaaSXpert Client";
			string strMessage = string.Format ("<span foreground=\'blue\' size=\'8000\'> {0} </span>", _strMessage);

			changepasswordwidget1.SetEntryReasonMessage (strMessage);
			changepasswordwidget1.SetEntryErrorMessage ("");


		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			changepasswordwidget1.DoChangePassword ();
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			this.Destroy ();
		}

		private void CheckPasswordRule (string strResultCode,string strResultMessage)
		{
			// broker에게 전달받은 메세지
			if (strResultCode.Equals("0") == false) {

				string strlanguageMessage = MainWindow.mainWindow.GetErrorMessageFromErrorCode (strResultCode);
				string strMessage;
				if (strlanguageMessage != string.Empty) {
					strMessage = string.Format ("<span foreground=\'red\' size=\'8000\'> {0} </span>", strlanguageMessage);
				} else {
					strMessage = string.Format ("<span foreground=\'red\' size=\'8000\'> {0} </span>", strResultMessage);
				}
				Gtk.Application.Invoke (delegate {
					changepasswordwidget1.SetEntryErrorMessage (strMessage);
				});

			} else {
				// 비밀번호 변경 요청 api
				//MainWindow.mainWindow.ChangePasswordLogin (changepasswordwidget.GetEntryCurPassword (), changepasswordwidget.GetEntryNewPassword ());
				this.Destroy ();
			}
		}

		public void SetTitleChangePassword(string strTitle)
		{
			this.Title = strTitle;
		}


	}
}
