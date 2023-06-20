using HCVK.HCVKSLibrary.VO;
using HCVK.HCVKSLibrary;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static client.Request.RequestJSONParam;


namespace client.Request
{
    public class RequestToHCVKA : HttpsClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<JObject> RequestHeartbeat(string strHCVKAServiceIP, string strHCVKAServicePort, VODesktopPool voDesktopPool, VOUser voUser)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, Properties.Resources.HCVKA_REQUEST_HEARTBEAT);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonRequestParam
                {
                    clientIP = voUser.ClientIP,
                    clientMAC = voUser.ClientMAC,
                    userID = voUser.EncodedUserID,
                    tenantID = voUser.TenantID,
                    tenantName = voUser.TenantName,
                    accessDiv = voDesktopPool.AccessDiv,
                    desktopId = voDesktopPool.Desktop.InstanceID
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
        public void RequestHeartbeat_AsyncCallback(string strHCVKAServiceIP, string strHCVKAServicePort, VODesktopPool voDesktopPool, VOUser voUser, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, Properties.Resources.HCVKA_REQUEST_HEARTBEAT);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonRequestParam
                    {
                        clientIP = voUser.ClientIP,
                        clientMAC = voUser.ClientMAC,
                        userID = voUser.EncodedUserID,
                        tenantID = voUser.TenantID,
                        tenantName = voUser.TenantName,
                        accessDiv = voDesktopPool.AccessDiv,
                        desktopId = voDesktopPool.Desktop.InstanceID
                    }
                };

                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        public async Task<JObject> RequestConnectToHCVKA(string strHCVKAServiceIP, string strHCVKAServicePort, VOUser voUser, VODesktopPool voDesktopPool, string strServiceProtocol)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, Properties.Resources.HCVKA_REQUEST_CONNECT);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonRequestParam
                {
                    clientIP = voUser.ClientIP,
                    clientMAC = voUser.ClientMAC,
                    userID = voUser.EncodedUserID,
                    tenantID = voUser.TenantID,
                    tenantName = voUser.TenantName,
                    accessDiv = voDesktopPool.AccessDiv,
                    desktopId = voDesktopPool.Desktop.InstanceID,
                    serviceProtocol = strServiceProtocol
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
        public void RequestConnectToHCVKA_AsyncCallback(string strHCVKAServiceIP, string strHCVKAServicePort, VOUser voUser, VODesktopPool voDesktopPool, string strServiceProtocol, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, Properties.Resources.HCVKA_REQUEST_CONNECT);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonRequestParam
                    {
                        clientIP = voUser.ClientIP,
                        clientMAC = voUser.ClientMAC,
                        userID = voUser.EncodedUserID,
                        tenantID = voUser.TenantID,
                        tenantName = voUser.TenantName,
                        accessDiv = voDesktopPool.AccessDiv,
                        desktopId = voDesktopPool.Desktop.InstanceID,
                        serviceProtocol = strServiceProtocol
                    }
                };

                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        public async Task<JObject> RequestOnConnectToHCVKA(string strHCVKAServiceIP, string strHCVKAServicePort, VOUser voUser, VODesktopPool voDesktopPool, string strServiceProtocol)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, Properties.Resources.HCVKA_REQUEST_ONCONNECT);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonOnConnectRequestParam
                {
                    clientIP = voUser.ClientIP,
                    clientMAC = voUser.ClientMAC,
                    userID = voUser.EncodedUserID,
                    tenantID = voUser.TenantID,
                    tenantName = voUser.TenantName,
                    accessDiv = voDesktopPool.AccessDiv,
                    desktopId = voDesktopPool.Desktop.InstanceID,
                    serviceProtocol = strServiceProtocol,
										clientLoc = MainWindow.mainWindow._strVDI_Connect_MODE
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
        public void RequestOnConnectConnectToHCVKA_AsyncCallback(string strHCVKAServiceIP, string strHCVKAServicePort, VOUser voUser, VODesktopPool voDesktopPool, string strServiceProtocol, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, Properties.Resources.HCVKA_REQUEST_ONCONNECT);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonRequestParam
                    {
                        clientIP = voUser.ClientIP,
                        clientMAC = voUser.ClientMAC,
                        userID = voUser.EncodedUserID,
                        tenantID = voUser.TenantID,
                        tenantName = voUser.TenantName,
                        accessDiv = voDesktopPool.AccessDiv,
                        desktopId = voDesktopPool.Desktop.InstanceID,
                        serviceProtocol = strServiceProtocol
                    }
                };

                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        public async Task<JObject> RequestReconnectToHCVKA(string strHCVKAServiceIP, string strHCVKAServicePort, VOUser voUser, VODesktopPool voDesktopPool, string strServiceProtocol)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, Properties.Resources.HCVKA_REQUEST_RECONNECT);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonRequestParam
                {
                    clientIP = voUser.ClientIP,
                    clientMAC = voUser.ClientMAC,
                    userID = voUser.EncodedUserID,
                    tenantID = voUser.TenantID,
                    tenantName = voUser.TenantName,
                    accessDiv = voDesktopPool.AccessDiv,
                    desktopId = voDesktopPool.Desktop.InstanceID,
                    serviceProtocol = strServiceProtocol
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
        public void RequestReconnectToHCVKA_AsyncCallback(string strHCVKAServiceIP, string strHCVKAServicePort, VOUser voUser, VODesktopPool voDesktopPool, string strServiceProtocol, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, Properties.Resources.HCVKA_REQUEST_RECONNECT);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonRequestParam
                    {
                        clientIP = voUser.ClientIP,
                        clientMAC = voUser.ClientMAC,
                        userID = voUser.EncodedUserID,
                        tenantID = voUser.TenantID,
                        tenantName = voUser.TenantName,
                        accessDiv = voDesktopPool.AccessDiv,
                        desktopId = voDesktopPool.Desktop.InstanceID,
                        serviceProtocol = strServiceProtocol
                    }
                };

                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        public async Task<JObject> RequestDisconnectFromHCVKA(string strHCVKAServiceIP, string strHCVKAServicePort, VOUser voUser, VODesktopPool voDesktopPool, string strServiceProtocol)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, Properties.Resources.HCVKA_REQUEST_DISCONNECT);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonRequestParam
                {
                    clientIP = voUser.ClientIP,
                    clientMAC = voUser.ClientMAC,
                    userID = voUser.EncodedUserID,
                    tenantID = voUser.TenantID,
                    tenantName = voUser.TenantName,
                    accessDiv = voDesktopPool.AccessDiv,
                    desktopId = voDesktopPool.Desktop.InstanceID,
                    serviceProtocol = strServiceProtocol
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
        public void RequestDisconnectFromHCVKA_AsyncCallback(string strHCVKAServiceIP, string strHCVKAServicePort, VOUser voUser, VODesktopPool voDesktopPool, string strServiceProtocol, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, Properties.Resources.HCVKA_REQUEST_DISCONNECT);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonRequestParam
                    {
                        clientIP = voUser.ClientIP,
                        clientMAC = voUser.ClientMAC,
                        userID = voUser.EncodedUserID,
                        tenantID = voUser.TenantID,
                        tenantName = voUser.TenantName,
                        accessDiv = voDesktopPool.AccessDiv,
                        desktopId = voDesktopPool.Desktop.InstanceID,
                        serviceProtocol = strServiceProtocol
                    }
                };

                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

		// Request USB Device Redirection ( usbip )
		public void RequestDeviceRedirection_AsyncCallback (string strHCVKAServiceIP, string strHCVKAServicePort, VOUser voUser, string strMode, string strDevice, string strDeviceRedirectPort, CallbackResponse callbackResponse)
		{

			string strAPI_Path = Properties.Resources.HCVKA_REQUEST_DEVICEATTACH;
			if (strMode == "detach") {
				strAPI_Path = Properties.Resources.HCVKA_REQUEST_DEVICEDETACH;
			}

			string strUrl = string.Format ("{0}://{1}:{2}/{3}",
				HTTP_PROTOCOL, strHCVKAServiceIP, strHCVKAServicePort, strAPI_Path);

			// add header field of  authentication token
			Dictionary<string, string> dicHeader = new Dictionary<string, string> ();

			JSonRequest oParam = new JSonRequest {
				requestNodeData = new JSonRequestDevice {
					clientIP = voUser.ClientIP,
					userID = voUser.EncodedUserID,
					tenantID = voUser.TenantID,
					deviceID = strDevice,
					remoteIP = voUser.ClientIP,
					remotePort = strDeviceRedirectPort
				}
			};

			SendReqeust_AsyncCallback (strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse, REQUEST_USBIP_TIMEOUT);
		}
	}
}
