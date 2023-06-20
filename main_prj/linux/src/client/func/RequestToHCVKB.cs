using HCVK.HCVKSLibrary.VO;
using HCVK.HCVKSLibrary;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using static client.Request.RequestJSONParam;

namespace client.Request
{
    public partial class RequestToHCVKB : HttpsClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private string _RSACertDownloadFilePath = string.Empty;

		public void UpdateAuthToken(VOBrokerServerNew voBrokerServer, JObject resJsonObject)
        {
            // __Imsi__ 
            // remove code when implemented interface for refreshing auth token mechanism
            //return;

            try
            {
                if (resJsonObject != null)
                {
                    if (resJsonObject[RESPONSE_RESULT_CODE].ToString().Equals("0") && resJsonObject [RESPONSE_RESULT_DATA].ToString ().Equals ("") == false)
                    {
                        // success response
                        // fetch auth token
                        {
                            JObject oToken = JObject.Parse(resJsonObject[RESPONSE_RESULT_DATA]["tokenInfo"].ToString());
                            voBrokerServer.AuthToken = oToken["token"].ToString();
                            voBrokerServer.Expiration = CommonUtils.UnixTimeStampToDateTime(long.Parse(oToken["expiration"].ToString()));

                            _logger.Debug(string.Format("Token : {0}", voBrokerServer.AuthToken));
                            _logger.Debug(string.Format("Next expiration time of token : {0}", voBrokerServer.Expiration.ToLocalTime()));
                        }
                    }
                    else
                    {
                        // failure response
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

        //-----------------------------------------------------------------------------------------------
        // client/login
		public async Task<JObject> RequestLogIn(VOBrokerServerNew voBrokerServer, VOUser voUser)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_LOGIN);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonUserInfo
                {
                    userId = voUser.EncodedUserID,
                    password = voUser.EncodedPassword,
                    clientIp = voUser.ClientIP,
                    clientMac = voUser.ClientMAC,
                    clientDeviceType = voUser.DeviceType
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
        public void RequestLogIn_AsyncCallback(VOBrokerServerNew voBrokerServer, VOUser voUser, CallbackResponse callbackResponse)
        {
            try
            {
				string tmp_pwd = string.Empty;
				string tmp_id = string.Empty;
				string tmp_terminalAuth = string.Empty;

				// 비밀번호 RSA encrpyt를 위한 base64로 인코딩된 pem 인증서 다운로드
				if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseRSAEncrpyt) {

					ServerStatusManager serverStatusManaer = new ServerStatusManager ();
					string strRSAPublicKey = serverStatusManaer.GetServerRSACertificate (voBrokerServer);

					if (strRSAPublicKey != string.Empty) 
					{ 
						// system/publickey API로 받은 RSA 공개 키 를 사용
						tmp_id = "RSA|" + CryptoManager.EncryptRSA_CertPublicKey_FromPublicKey (strRSAPublicKey, voUser.EncodedUserID, true);
						tmp_pwd = "RSA|" + CryptoManager.EncryptRSA_CertPublicKey_FromPublicKey (strRSAPublicKey, voUser.EncodedPassword, true);

					} else {

						DownloadRSACertificateFile (voBrokerServer);
						// base64-> rsa -> base64

						tmp_id = "RSA|" + CryptoManager.EncryptRSA_CertPublicKey_FromFile (_RSACertDownloadFilePath, voUser.EncodedUserID);
						tmp_pwd = "RSA|" + CryptoManager.EncryptRSA_CertPublicKey_FromFile (_RSACertDownloadFilePath, voUser.EncodedPassword);

					}


				} else {
					tmp_id = voUser.EncodedUserID;
					tmp_pwd = voUser.EncodedPassword;

				}

				if(MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseTerminalAuth == true) 
				{
					tmp_terminalAuth = CommonUtils.GetPlatfromUUID ();
					_logger.Info (string.Format ("Terminal Auth(UUID) : {0}", tmp_terminalAuth));
				} 

				string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_LOGIN);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();
                dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonUserInfoNew
                    {
                        userId = tmp_id,
                        password = tmp_pwd,
                        clientIp = voUser.ClientIP,
                        clientMac = voUser.ClientMAC,
                        clientDeviceType = voUser.DeviceType,
                        // Assembly.GetExecutingAssembly().GetName().Version.ToString();
                        // clientReleaseVersion = "2.0.2.00",
                        clientReleaseVersion = Properties.Resources.CLIENT_VERSION,
                        clientReleaseDt = "",
                        encMsg = voUser.engMsg,
												platformUuid = tmp_terminalAuth

                    }
                };

                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        //-----------------------------------------------------------------------------------------------
        // client/logout
		public async Task<JObject> RequestLogOut(VOBrokerServerNew voBrokerServer, VOUser voUser)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_LOGOUT);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonUserInfo
                {
                    userId = voUser.EncodedUserID
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
		public void RequestLogOut_AsyncCallback(VOBrokerServerNew voBrokerServer, VOUser voUser, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_LOGOUT);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();
                dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonUserInfo
                    {
                        userId = voUser.EncodedUserID
                    }
                };

                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        //-----------------------------------------------------------------------------------------------
        // client/password
		public async Task<JObject> RequestChangeInformation(VOBrokerServerNew voBrokerServer, VOUser voUser)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_CHANGE_INFO);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonUserInfo
                {
                    userId = voUser.EncodedUserID,
                    password = voUser.EncodedPassword,
                    newPassword = voUser.EncodedNewPassword
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_PUT, dicHeader, oParam);
        }
		public void RequestChangeInformation_AsyncCallback(VOBrokerServerNew voBrokerServer, VOUser voUser, CallbackResponse callbackResponse)
        {
    	try
            {
        string strUrl = string.Format("{0}://{1}:{2}/{3}",
            HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_CHANGE_INFO);

        // add header field of  authentication token
        Dictionary<string, string> dicHeader = new Dictionary<string, string>();
        dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

				string tmp_pwd = string.Empty;
				string tmp_newpwd = string.Empty;
				string tmp_id = string.Empty;

				if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseRSAEncrpyt == true) {

					// 비밀번호 RSA encrpyt를 위한 base64로 인코딩된 pem 인증서 다운로드
					DownloadRSACertificateFile (voBrokerServer);
					tmp_pwd = "RSA|"+CryptoManager.EncryptRSA_CertPublicKey_FromFile (_RSACertDownloadFilePath, voUser.EncodedPassword);
					tmp_newpwd = "RSA|" + CryptoManager.EncryptRSA_CertPublicKey_FromFile (_RSACertDownloadFilePath, voUser.EncodedNewPassword);
					tmp_id = "RSA|" + CryptoManager.EncryptRSA_CertPublicKey_FromFile (_RSACertDownloadFilePath, voUser.EncodedUserID);

				} else {
				 	tmp_pwd = voUser.EncodedPassword;
					tmp_newpwd = voUser.EncodedNewPassword;
					tmp_id = voUser.EncodedUserID;
				}


				JSonRequest oParam = new JSonRequest
                {
	        requestNodeData = new JSonUserInfo
                    {
            userId = tmp_id,
            password = tmp_pwd,
            newPassword = tmp_newpwd
            		}
                };

    		SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_PUT, dicHeader, oParam, callbackResponse);
            }
    	catch (Exception ex)
            {
    		_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }
		//client/password_validate
		public void RequestPasswordValidate_AsyncCallback (VOBrokerServerNew voBrokerServer, VOUser voUser, string strEncPassword,  CallbackResponse callbackResponse)
		{
			try {
				string strUrl = string.Format ("{0}://{1}:{2}/{3}",
					HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_PASSWORD_VALIDATE);

				string tmp_newpwd = string.Empty;
				string tmp_id = string.Empty;

				if (MainWindow.mainWindow.environment.vOGeneralsProperties.IsUseRSAEncrpyt == true) {

					// 비밀번호 RSA encrpyt를 위한 base64로 인코딩된 pem 인증서 다운로드
					DownloadRSACertificateFile (voBrokerServer);
					// base64-> rsa -> base64
					tmp_newpwd = "RSA|"+CryptoManager.EncryptRSA_CertPublicKey_FromFile (_RSACertDownloadFilePath, voUser.EncodedNewPassword);
					tmp_id = "RSA|" + CryptoManager.EncryptRSA_CertPublicKey_FromFile (_RSACertDownloadFilePath, voUser.EncodedUserID);

				} else {
					tmp_newpwd = voUser.EncodedNewPassword;
					tmp_id = voUser.EncodedUserID;
				}


				// add header field of  authentication token
				Dictionary<string, string> dicHeader = new Dictionary<string, string> ();
				dicHeader.Add (Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

				JSonRequest oParam = new JSonRequest {
					requestNodeData = new JSonUserInfo {
						userId = tmp_id,
						newPassword = tmp_newpwd
					}
				};

				SendReqeust_AsyncCallback (strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}
		}


		//-----------------------------------------------------------------------------------------------
		// client/pool
		[Obsolete]
		public async Task<JObject> RequestGetDesktopPoolInfo(VOBrokerServerNew voBrokerServer, VOUser voUser)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_POOL_INFO);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonDesktopPoolInfo
                {
                    userId = voUser.EncodedUserID,
                    tenantId = voUser.TenantID
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }

		public void RequestGetDesktopPoolInfo_AsyncCallback(VOBrokerServerNew voBrokerServer, VOUser voUser, CallbackResponse callbackResponse)
		{
			try
			{
				string strUrl = string.Format("{0}://{1}:{2}/{3}",
												HTTP_PROTOCOL, voBrokerServer.BrokerServerIP,
												voBrokerServer.BrokerServerPort,
												Properties.Resources.HCVKB_REQUEST_POOL_INFO);

				// add header field of  authentication token
				Dictionary<string, string> dicHeader = new Dictionary<string, string>();
				dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

				JSonRequest oParam = new JSonRequest
				{
					requestNodeData = new JSonDesktopPoolInfo
					{
						userId = voUser.EncodedUserID,
						tenantId = voUser.TenantID,
						clientIp = voUser.ClientIP,
						clientLoc = MainWindow.mainWindow._strVDI_Connect_MODE
					}
				};

				SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
			} catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}
        }


		//-----------------------------------------------------------------------------------------------
		// client/desktop/instance
		[Obsolete]
		public async Task<JObject> RequestGetDesktopConnectionInfo(VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool, VOUser voUser, string strRequestProtocol)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_DESKTOP_INFO);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonDesktopInfo
                {
                    tenantId = voUser.TenantID,
                    poolId = voDesktopPool.PoolID,
                    userId = voUser.EncodedUserID,
                    accessDiv = voDesktopPool.AccessDiv,
                    desktopId = voDesktopPool.Desktop.DesktopID,
                    instanceId = voDesktopPool.Desktop.InstanceID,
                    infoType = Properties.Resources.TYPE_INFO_TYPE_CONNECTION,
                    requestProtocol = strRequestProtocol
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }

		public void RequestGetDesktopConnectionInfo_AsyncCallback(VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool, VOUser voUser, string strRequestProtocol, CallbackResponse callbackResponse)
		{
			try
			{
				string strUrl = string.Format("{0}://{1}:{2}/{3}",
								HTTP_PROTOCOL, voBrokerServer.BrokerServerIP,
								voBrokerServer.BrokerServerPort,
								Properties.Resources.HCVKB_REQUEST_DESKTOP_INFO);

				// add header field of  authentication token
				Dictionary<string, string> dicHeader = new Dictionary<string, string>();
				dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

				JSonRequest oParam = new JSonRequest
				{
					requestNodeData = new JSonDesktopInfo
					{
						tenantId = voUser.TenantID,
						poolId = voDesktopPool.PoolID,
						userId = voUser.EncodedUserID,
						accessDiv = voDesktopPool.AccessDiv,
						desktopId = voDesktopPool.Desktop.DesktopID,
						instanceId = voDesktopPool.Desktop.InstanceID,
						infoType = Properties.Resources.TYPE_INFO_TYPE_CONNECTION,
						requestProtocol = strRequestProtocol,
						clientIp = voUser.ClientIP,
						clientLoc = MainWindow.mainWindow._strVDI_Connect_MODE
					}
				};

				SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
			} catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}
		}

		[Obsolete]
		public async Task<JObject> RequestGetDesktopStatusInfo(VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_DESKTOP_INFO);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonDesktopInfo
                {
                    poolId = voDesktopPool.PoolID,
                    accessDiv = voDesktopPool.AccessDiv,
                    desktopId = voDesktopPool.Desktop.DesktopID,
                    instanceId = voDesktopPool.Desktop.InstanceID,
                    infoType = Properties.Resources.TYPE_INFO_TYPE_INFO
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
		[Obsolete]
		public void RequestGetDesktopStatusInfo_AsyncCallback(VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_DESKTOP_INFO);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();
                dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonDesktopInfo
                    {
                        poolId = voDesktopPool.PoolID,
                        accessDiv = voDesktopPool.AccessDiv,
                        desktopId = voDesktopPool.Desktop.DesktopID,
                        instanceId = voDesktopPool.Desktop.InstanceID,
                        infoType = Properties.Resources.TYPE_INFO_TYPE_INFO
                    }
                };

                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }
		// client/desktop/instance
		public void RequestGetDesktopStatusInfo_AsyncCallback (VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool, VOUser vOUser, CallbackResponse callbackResponse)
		{
			try {
				string strUrl = string.Format ("{0}://{1}:{2}/{3}",
						HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_DESKTOP_INFO);

				// add header field of  authentication token
				Dictionary<string, string> dicHeader = new Dictionary<string, string> ();
				dicHeader.Add (Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

				JSonRequest oParam = new JSonRequest {
					requestNodeData = new JSonDesktopInfo {
						tenantId = vOUser.TenantID,
						poolId = voDesktopPool.PoolID,
						userId = vOUser.EncodedUserID,
						accessDiv = voDesktopPool.AccessDiv,
						desktopId = voDesktopPool.Desktop.DesktopID,
						instanceId = voDesktopPool.Desktop.InstanceID,
						infoType = Properties.Resources.TYPE_INFO_TYPE_INFO,
						requestProtocol = "RDP",
						clientIp = vOUser.ClientIP,
						clientLoc = MainWindow.mainWindow._strVDI_Connect_MODE
					}
				};

				SendReqeust_AsyncCallback (strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}
		}
		//-----------------------------------------------------------------------------------------------
		// client/clearSession
		public async Task<JObject> RequestClearSession(VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool, VOUser voUser)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_CLEAR_SESSION);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonDesktopInfo
                {
                    tenantId = voUser.TenantID,
                    poolId = voDesktopPool.PoolID,
                    userId = voUser.EncodedUserID,
                    desktopId = voDesktopPool.Desktop.DesktopID,
                    instanceId = voDesktopPool.Desktop.InstanceID
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_PUT, dicHeader, oParam);
        }
        public void RequestClearSession_AsyncCallback(VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool, VOUser voUser, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_CLEAR_SESSION);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();
                dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonDesktopInfo
                    {
                        tenantId = voUser.TenantID,
                        poolId = voDesktopPool.PoolID,
                        userId = voUser.EncodedUserID,
                        desktopId = voDesktopPool.Desktop.DesktopID,
                        instanceId = voDesktopPool.Desktop.InstanceID
                    }
                };
                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_PUT, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        //-----------------------------------------------------------------------------------------------
        // client/desktop/powerControl
        public async Task<JObject> RequestDesktopPowerControl(VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool, VOUser voUser, string strPowerOption)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_DESKTOP_POWER_CONTROL);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonDesktopInfo
                {
                    tenantId = voUser.TenantID,
                    poolId = voDesktopPool.PoolID,
                    accessDiv = voDesktopPool.AccessDiv,
                    desktopId = voDesktopPool.Desktop.DesktopID,
                    instanceId = voDesktopPool.Desktop.InstanceID,
                    powerOption = strPowerOption
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
        public void RequestDesktopPowerControl_AsyncCallback(VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool, VOUser voUser, string strPowerOption, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_DESKTOP_POWER_CONTROL);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();
                dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonDesktopInfo
                    {
                        tenantId = voUser.TenantID,
                        poolId = voDesktopPool.PoolID,
                        accessDiv = voDesktopPool.AccessDiv,
                        desktopId = voDesktopPool.Desktop.DesktopID,
                        instanceId = voDesktopPool.Desktop.InstanceID,
                        powerOption = strPowerOption
                    }
                };

                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        //-----------------------------------------------------------------------------------------------
        // token/refresh
        public async Task<JObject> RequestTokenRefesh(VOBrokerServerNew voBrokerServer)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_TOKEN_REFRESH);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = null
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_PUT, dicHeader, oParam);
        }
        public void RequestTokenRefesh_AsyncCallback(VOBrokerServerNew voBrokerServer, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_TOKEN_REFRESH);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();
                dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = null
                };

                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_PUT, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

		//-----------------------------------------------------------------------------------------------
		// system/recommendServer
		[Obsolete]
		public async Task<JObject> RequestRecommendServer(VOBrokerServerNew voBrokerServer)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_RECOMMEND_SERVER);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            //dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = null
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }

		public void RequestRecommendServer_AsyncCallback(VOBrokerServerNew voBrokerServer, VOUser voUser, CallbackResponse callbackResponse)
		{
			try
			{
				string strUrl = string.Format("{0}://{1}:{2}/{3}",
													HTTP_PROTOCOL, voBrokerServer.BrokerServerIP,
													voBrokerServer.BrokerServerPort,
													Properties.Resources.HCVKB_REQUEST_RECOMMEND_SERVER);

				// add header field of  authentication token
				Dictionary<string, string> dicHeader = new Dictionary<string, string>();

				//dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

				JSonRequest oParam = new JSonRequest
				{
					requestNodeData = new JSonRecommandServerInfo
					{
						clientIp = voUser.ClientIP,
						clientLoc = MainWindow.mainWindow._strVDI_Connect_MODE

					}
				};


				SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}
		}

        //-----------------------------------------------------------------------------------------------
        // client/desktop       
        public async Task<JObject> RequestDesktop(VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool, VOUser voUser, string strDisplayName)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_DESKTOP);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonDesktopInfo
                {
                    desktopId = voDesktopPool.Desktop.DesktopID,
                    displayName = strDisplayName                    
                }
            };
            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_PUT, dicHeader, oParam);
        }

        public void RequestDesktop_AsyncCallback(VOBrokerServerNew voBrokerServer, VODesktopPool voDesktopPool, VOUser voUser, string strDisplayName, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_DESKTOP);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();
                dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonDesktopInfo
                    {
                        desktopId = voDesktopPool.Desktop.DesktopID,
                        displayName = strDisplayName
                    }
                };
                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_PUT, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

		public void DownloadRSACertificateFile (VOBrokerServerNew voBrokerServer)
		{
			try {
				string certdown_url = string.Format ("https://{0}:{1}", voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort);

				_RSACertDownloadFilePath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal) + "/.config/DaaSXpert/tmp.pem";

				StoredCertManager _downloadCert = new StoredCertManager ();

				_downloadCert.SaveCertificateFile (certdown_url, _RSACertDownloadFilePath);

			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}
		}
	
	}
}
