using System;
using System.Net;
using client;
using client.Request;
using Newtonsoft.Json.Linq;
using HCVK.HCVKSLibrary;
using HCVK.HCVKSLibrary.VO;
using Mono.Unix;
using Gtk;
using System.IO;

public partial class MainWindow {

	private void InitializeSetting()
	{
		// Thincast Client 초기설정 파일 (RDC.conf) 이 없으면, 초기 설정파일을 복사합니다. 
		if(environment.vOGeneralsProperties.IsUseExtRDPViewer == true) {
			// 파일이 있는지 확인
			string sSourceConfFileName = "/usr/lib/DaaSXpertClient/RDC.conf";
			string sConfFilePath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal) + "/.config/Thincast";
			string sDestConfFileName = sConfFilePath + "/RDC.conf";

			if (File.Exists(sSourceConfFileName) && !File.Exists (sDestConfFileName)) {
				try {
					// 폴더가 없으면 디렉토리 생성
					if (Directory.Exists (sConfFilePath) == false) {
						Directory.CreateDirectory (sConfFilePath);
					}
					// 설정 파일 복사
					File.Copy (sSourceConfFileName, sDestConfFileName, true);

				} catch (Exception ex) {
					_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
				}
			}
		}
	}

	public void ChangePasswordLogin (string strPassword, string strNewPassword)
	{

		try {

			_voUser.Password = strPassword; // base64 encrpyt
			_voUser.NewPassword = strNewPassword; // base64 encrpyt

			new RequestToHCVKB ().RequestPasswordValidate_AsyncCallback (_voSelectedBrokerServer, _voUser, strNewPassword, Callback_PasswordValidate);

		} catch (WebException wex) {

			_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
			ErrorHandlerManager.ExceptionHandler (wex, null, "[ChangePasswordLoginW]");

			if (_bRunningAutoStart == true) _bRunningAutoStart = false;
		} catch (Exception ex) {

			_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			ErrorHandlerManager.ExceptionHandler (ex, null, "[ChangePasswordLoginE]");

		}


	}

	public void RequestCheckPasswordValidate (string strNewPassword)
	{
		try {

			_voUser.NewPassword = strNewPassword; // base64 encrpyt

			new RequestToHCVKB ().RequestPasswordValidate_AsyncCallback (_voSelectedBrokerServer, _voUser, strNewPassword, Callback_PasswordValidate);

		} catch (WebException wex) {

			_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
			ErrorHandlerManager.ExceptionHandler (wex, null, "[ChangePasswordLoginW]");

			if (_bRunningAutoStart == true) _bRunningAutoStart = false;
		} catch (Exception ex) {

			_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			ErrorHandlerManager.ExceptionHandler (ex, null, "[ChangePasswordLoginE]");

		}
	}

	private void Callback_PasswordValidate (JObject resJsonObject, Exception exParam)
	{
		_logger.Debug (string.Format ("response : {0}", resJsonObject?.ToString ()));

		try {
			if (resJsonObject != null) {
				if (resJsonObject.ContainsKey (RequestJSONParam.RESPONSE_RESULT_CODE) == true) {

					try {
						if (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ().Equals ("0") == true) {

							new RequestToHCVKB ().RequestChangeInformation_AsyncCallback (_voSelectedBrokerServer, _voUser, Callback_ChangePassword);
						} else {
							// 에러 메세지 발생 or 비밀번호 변경 창 닫기
							if (MainFunc.callbackCheckPasswordRule != null) {
								MainFunc.callbackCheckPasswordRule (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString (),
																										resJsonObject [RequestJSONParam.RESPONSE_RESULT_MESSAGE].ToString ());
							}

						}


					} catch (WebException wex) {

						_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
						ErrorHandlerManager.ExceptionHandler (wex, null, "[ChangePasswordLoginW]");

						if (_bRunningAutoStart == true) _bRunningAutoStart = false;
					} catch (Exception ex) {

						_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
						ErrorHandlerManager.ExceptionHandler (ex, null, "[ChangePasswordLoginE]");

					}


				}
			}
		} catch (WebException wex) {
			_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
		} catch (Exception ex) {
			_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
		}
	}

	private void Callback_ChangePassword (JObject resJsonObject, Exception exParam)
	{
		_logger.Debug (string.Format ("response : {0}", resJsonObject?.ToString ()));

		try {
			if (resJsonObject != null) {
				if (resJsonObject.ContainsKey (RequestJSONParam.RESPONSE_RESULT_CODE) == true) {
					try {
						if (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ().Equals ("0") == true) {

							Gtk.Application.Invoke (delegate
							{
								MessageDialog _popupMessage = new MessageDialog (MainWindow.mainWindow, DialogFlags.DestroyWithParent,
																				MessageType.Info, ButtonsType.Ok, "비밀번호 변경을 성공하였습니다");
								_popupMessage.Title = "DaaSXpert";

								_popupMessage.Run ();
								_popupMessage.Destroy ();
							});

							_mainpagewidget.ShowLoginPageFromServerList (MainFunc.GetBrokerServerIdx (_voSelectedBrokerServer));

							// 비밀번호 변경 창 닫기
							if (MainFunc.callbackCheckPasswordRule != null) {
								Gtk.Application.Invoke (delegate {
									MainFunc.callbackCheckPasswordRule (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString (), "");
								});
							}

						} else {
							// 에러 메세지 발생
							if (MainFunc.callbackCheckPasswordRule != null) {
								Gtk.Application.Invoke (delegate {
									MainFunc.callbackCheckPasswordRule (resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString (),
																											resJsonObject [RequestJSONParam.RESPONSE_RESULT_MESSAGE].ToString ());
								});
							}
						
						}

					} catch (WebException wex) {

						_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
						ErrorHandlerManager.ExceptionHandler (wex, null, "[ChangePasswordW]");

						if (_bRunningAutoStart == true) _bRunningAutoStart = false;
					} catch (Exception ex) {

						_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
						ErrorHandlerManager.ExceptionHandler (ex, null, "[ChangePasswordE]");

					}

				}

			}

		} catch (WebException wex) {
			_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
		} catch (Exception ex) {
			_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
		}

	}




	public string GetErrorMessageFromErrorCode (string strErrorCode)
	{
		string strErrorMsg = string.Empty;

		switch (strErrorCode) 
		{
			case VOErrorCode._E_CODE_B_B0004001:
				strErrorMsg = string.Format ("{0}", Catalog.GetString ("Please enter your current password correctly."));
				break;
			case VOErrorCode._E_CODE_B_B0004002:
				strErrorMsg = string.Format ("{0}", Catalog.GetString ("Password must be rewritten (at least 9 characters, use upper and lower case letters, numbers and special characters).") );
				break;
			case VOErrorCode._E_CODE_B_B0004003:
				strErrorMsg = string.Format ("{0}", Catalog.GetString ("Password must be rewritten (repeatedly typing the same letters and numbers)."));
				break;
			case VOErrorCode._E_CODE_B_B0004004:
				strErrorMsg = string.Format ("{0}", Catalog.GetString ("Password needs to be rewritten (sequential input of consecutive letters or numbers on the keyboard)."));
				break;
			case VOErrorCode._E_CODE_B_B0004005:
				strErrorMsg = string.Format ("{0}", Catalog.GetString ("Same as the current password."));
				break;
			case VOErrorCode._E_CODE_B_B0004006:
				strErrorMsg = string.Format ("{0}", Catalog.GetString ("Same as the user ID."));
				break;
			case VOErrorCode._E_CODE_B_B0004007:
				strErrorMsg = string.Format ("{0}", Catalog.GetString ("Same as the initial password."));
				break;
			case VOErrorCode._E_CODE_B_B0004008:
				strErrorMsg = string.Format ("{0}", Catalog.GetString ("Last used password. It needs to be rewritten."));
				break;
			case VOErrorCode._E_CODE_B_B0004009:
				strErrorMsg = string.Format ("{0}", Catalog.GetString ("An error occurred when processing the password change."));
				break;

		}

		return strErrorMsg;
	}


}