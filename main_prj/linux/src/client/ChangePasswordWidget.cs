using System;
using GLib;

namespace client {
	[System.ComponentModel.ToolboxItem (true)]
	public partial class ChangePasswordWidget : Gtk.Bin {
		public ChangePasswordWidget ()
		{
			this.Build ();

			entry_CurPassword.Visibility = false;
			entry_NewPassword.Visibility = false;
			entry_ConfirmNewPassword.Visibility = false;
		}

		public string GetEntryCurPassword()
		{
			return entry_CurPassword.Text;
		}

		public string GetEntryNewPassword()
		{
			return entry_NewPassword.Text;
		}

		public string GetEntryConfirmNewPassword()
		{
			return entry_ConfirmNewPassword.Text;
		}

		public void SetEntryErrorMessage(string strMessage)
		{
			lb_ErrorMessage.LabelProp = strMessage;
		}

		public void SetEntryReasonMessage (string strMessage)
		{
			lb_ReasonMessage.LabelProp = strMessage;
		}

		public void DoChangePassword()
		{
			if(GetEntryNewPassword () != GetEntryConfirmNewPassword ())
				SetEntryErrorMessage ("<span foreground=\'red\' size=\'8000\'> 비밀번호가 일치하지 않습니다. </span>");
			else {
				// 비밀번호 유효성 확인
				MainWindow.mainWindow.ChangePasswordLogin (GetEntryCurPassword (), GetEntryNewPassword ());

			}

		}
		[ConnectBefore]
		protected void OnEntryConfirmNewPasswordKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return) {
				DoChangePassword ();
			}
		}
	}
}
