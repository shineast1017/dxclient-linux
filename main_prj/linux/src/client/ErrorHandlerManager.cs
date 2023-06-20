using System;
using HCVK.HCVKSLibrary.VO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
using Mono.Unix;

namespace client
{
    public class ErrorHandlerManager
    {        
        static bool _bIsEnablePopupMsg = true;

		//static Popup_Message _popupMessage = new Popup_Message();
		//static MessageDialog _popupMessage = null;

        public static void Initialize()
        {
            //_popupMessage._PopupType = PopupType.ERROR;
            //_popupMessage.InitializeComponent();
			//_popupMessage = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
            //                                     MessageType.Question, ButtonsType.YesNo, "");
			//_popupMessage.Title = "DaaSXpert";
        }
        public static void Dispose()
		{
			//if (_popupMessage != null)
			//	_popupMessage.Destroy();
		}

        public static void ExceptionHandler(Exception ex, GLib.Object oParent = null, string sPositionCode = null)
        {
			Gtk.Application.Invoke(delegate
			{
				if (_bIsEnablePopupMsg)
				{
					_bIsEnablePopupMsg = false;

					string strExceptionMsg = string.Format("{0}[Code:{1}]", ex.Message, ex.HResult);

					if(sPositionCode != null) {
						strExceptionMsg = string.Format ("{0}{1}[Code:{2}]", sPositionCode, ex.Message, ex.HResult);
					}


					MessageDialog _popupMessage = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
					                                                MessageType.Error, ButtonsType.Ok, strExceptionMsg);
					_popupMessage.Title = "DaaSXpert";

					//_popupMessage.Text = strExceptionMsg;
					_popupMessage.Run();
					_popupMessage.Destroy();

					_bIsEnablePopupMsg = true;
				}
			});
        }

		public static void ErrorHandler(string strErrorCode)
        {
            if (_bIsEnablePopupMsg)
            {
                _bIsEnablePopupMsg = false;

                string strErrorMsg = string.Empty;

                switch (strErrorCode)
                {
                    case VOErrorCode._E_CODE_A_A0000001:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_A0000001, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("데스크톱 접속서비스 상태가 아닙니다."), strErrorCode);
                        break;
                    case VOErrorCode._E_CODE_A_A0000101:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_A0000101, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("데스크톱 접속서비스 등록상태가 아닙니다."), strErrorCode);                        
                        break;

                    case VOErrorCode._E_CODE_B_B0001002:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0001002, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("인증 토큰이 변조되었습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0001003:
                        //strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0001003, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("인증 토큰 유효시간이 만료되었습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0001004:
                        //strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0001004, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("인증 토큰 검증에 실패하였습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0001005:
                        //strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0001005, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("인증 토큰 재발행에 실패하였습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0002001:
                        //strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0002001, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("로그인 인증에 실패했습니다."), strErrorCode);               
                        break;

                    case VOErrorCode._E_CODE_B_B0002002:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0002002, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("접근 제한된 클라이언트 정보입니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0002003:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0002003, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("로그인 인증 제한 회수에 도달했습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0002004:
                        //strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0002004, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("로그인 인증에 실패했습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0002005:
                        //strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0002005, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("로그인 인증에 실패했습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0003001:
                        //strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0003001, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("정상적인 로그아웃 처리에 실패했습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0004001:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0004001, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("계정 인증에 실패했습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0004002:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0004002, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("암호 복잡도를 만족하지 않습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0005001:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0005001, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("로그 기록에 실패했습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0006001:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0006001, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("접속 가능한 데스크톱이 없습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0007001:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0007001, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("데스크톱 전원 제어를 실패했습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_B_B0008001:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_B0008001, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("데스크톱 풀 정보 조회가 실패했습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_G_G0000001:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_G0000001, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("추천 접속서버정보 조회에 실패했습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_X_X0000201:
                        // strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_X0000201, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("무결성 검증에 실패했습니다."), strErrorCode);
                        break;

                    case VOErrorCode._E_CODE_X_X0009999:
                    case VOErrorCode._E_CODE_G_G0009001:
                        strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("데스크톱 접속서비스 상태가 아닙니다."), strErrorCode);
                        break;
                    case VOErrorCode._E_CODE_G_G0009999:
                        //strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_UNKNOWN, strErrorCode);
                        strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("알 수 없는 에러입니다."), strErrorCode);
                        break;
				case VOErrorCode._E_CODE_GPMS_GRTP100:
					strErrorMsg = string.Format ("{0}[ErrCode:{1}]", Catalog.GetString ("로그인 인증에 실패했습니다."), strErrorCode);
					break;
				case VOErrorCode._E_CODE_GPMS_GRTP120:
					strErrorMsg = string.Format ("{0}[ErrCode:{1}]", Catalog.GetString ("로그인 인증에 실패했습니다."), strErrorCode);
					break;
				case VOErrorCode._E_CODE_GPMS_GRTP121:
					strErrorMsg = string.Format ("{0}[ErrCode:{1}]", Catalog.GetString ("로그인 인증에 실패했습니다."), strErrorCode);
					break;
				case VOErrorCode._E_CODE_GPMS_GRTP401:
					strErrorMsg = string.Format ("{0}[ErrCode:{1}]", Catalog.GetString ("로그인 인증에 실패했습니다."), strErrorCode);
					break;
				case VOErrorCode._E_CODE_GPMS_GRTP112:
					strErrorMsg = string.Format ("{0}[ErrCode:{1}]", Catalog.GetString ("로그인 인증에 실패했습니다."), strErrorCode);
					break;
				case VOErrorCode._E_CODE_GPMS_GRTP500:
					strErrorMsg = string.Format ("{0}[ErrCode:{1}]", Catalog.GetString ("로그인 인증에 실패했습니다."), strErrorCode);
					break;

				case VOErrorCode._E_CODE_AU_000001:
				case VOErrorCode._E_CODE_AU_000002:
				case VOErrorCode._E_CODE_AU_000003:
				case VOErrorCode._E_CODE_AU_000101:
				case VOErrorCode._E_CODE_AU_000102:
				case VOErrorCode._E_CODE_AU_000103:
				case VOErrorCode._E_CODE_AU_000201:

					strErrorMsg = string.Format ("{0}[ErrCode:{1}]", Catalog.GetString ("Agent USB 연동 실패하였습니다."), strErrorCode);
					break;

				case VOErrorCode._E_CODE_D_USBE001:
				case VOErrorCode._E_CODE_D_USBE002:
				case VOErrorCode._E_CODE_D_USBE003:
					strErrorMsg = string.Format ("{0}[ErrCode:{1}]", Catalog.GetString ("DBUS USB 연동 실패하였습니다."), strErrorCode);
					break;
				case VOErrorCode._E_CODE_C_0000011:
				case VOErrorCode._E_CODE_C_0000012:
				case VOErrorCode._E_CODE_C_0000013:

					strErrorMsg = string.Format ("{0}[ErrCode:{1}]", Catalog.GetString ("WEBCAM 장치 연동 실패하였습니다."), strErrorCode);
					break;


					default:
                        //strErrorMsg = string.Format("{0}[ErrCode:{1}]", MultiLang.HCVKC_MAIN_MSG_UNKNOWN, strErrorCode);
						strErrorMsg = string.Format("{0}[ErrCode:{1}]", Catalog.GetString("알 수 없는 에러입니다."), strErrorCode);
                        break;
                }

                //_popupMessage.Text = strErrorMsg;
                //_popupMessage.Run();

                Gtk.Application.Invoke(delegate
				{
					MessageDialog _popupMessage = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
					                                                MessageType.Error, ButtonsType.Ok, strErrorMsg);
					_popupMessage.Title = "DaaSXpert";

					_popupMessage.Run();
					_popupMessage.Destroy();

					_bIsEnablePopupMsg = true;
				});
            }
        }

        public static void ErrorMessage(string strMessage, GLib.Object oParent = null)
        {
            if (_bIsEnablePopupMsg)
            {
                _bIsEnablePopupMsg = false;

				//_popupMessage.MessageType = MessageType.Error;
				//_popupMessage.Text = strMessage;
				//_popupMessage.Run();

				Gtk.Application.Invoke(delegate
				{
					MessageDialog _popupMessage = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
					                                                MessageType.Error, ButtonsType.Ok, strMessage);
					_popupMessage.Title = "DaaSXpert";

					_popupMessage.Run();
					_popupMessage.Destroy();

					_bIsEnablePopupMsg = true;
				});
            }
        }

		public static ResponseType QuestionMessage(string strMessage)
		{
			ResponseType result = ResponseType.Cancel;
			if (_bIsEnablePopupMsg)
            {
                _bIsEnablePopupMsg = false;

				//_popupMessage.MessageType = MessageType.Question;
				//_popupMessage.Text = strMessage;
				//result = (ResponseType)_popupMessage.Run();
				//Gtk.Application.Invoke(delegate
				//{
					MessageDialog _popupMessage = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
																					MessageType.Question, ButtonsType.YesNo, strMessage);
					_popupMessage.Title = "DaaSXpert";

					result = (ResponseType)_popupMessage.Run();
					_popupMessage.Destroy();

				//});
				_bIsEnablePopupMsg = true;
            }
			return result;
		}
    }
}
