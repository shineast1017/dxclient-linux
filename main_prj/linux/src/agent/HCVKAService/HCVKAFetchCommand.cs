using HCVK.HCVKAService;
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
using HCVK.HCVKAService.HCVKARequest;
using Newtonsoft.Json;
using System.Management.Automation;
using System.Security;
using System.Management.Automation.Runspaces;

namespace HCVK.HCVKAService
{
	public class HCVKAFetchCommand : RestfulTemplate
	{
		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		// declare parent
		private HCVKAMain _hcvkaMain = null;


		public HCVKAFetchCommand(object oParent) : base(oParent)
		{
			_hcvkaMain = oParent.GetType() == typeof(HCVKAMain) ? (HCVKAMain)oParent : null;
			_logger.Debug(string.Format("Get Parent Type[{0}]", _hcvkaMain.GetType().ToString()));

			if (_hcvkaMain == null)
				throw new ArgumentException("Invalid Parent Object as HCVKAMain ....");
		}


		public bool IsSupportedNodeVersion(JObject jsonRequestObject)
		{
			bool bSupportedNodeVersion = false;

			try
			{
				// fetch request node info.
				string strRequestNodeName = jsonRequestObject[HCVKARequestJSONParam.REQUEST_NODE_NAME].ToString();
				string strRequestNodeVersion = jsonRequestObject[HCVKARequestJSONParam.REQUEST_NODE_VERSION].ToString();
				_logger.Debug(string.Format("Fetch Node Info. => Name[{0}], Version[{1}] ", strRequestNodeName, strRequestNodeVersion));


				// checking validation
				if (string.IsNullOrEmpty(strRequestNodeName) || string.IsNullOrEmpty(strRequestNodeVersion))
					return bSupportedNodeVersion;

				// convert version string to version object 
				Version verNode = null;
				{
					if (!Version.TryParse(strRequestNodeVersion, out verNode))
						return bSupportedNodeVersion;

					if (verNode == null)
						return bSupportedNodeVersion;
				}


				// comparing node version for HCVKB
				if (strRequestNodeName.Equals(Properties.Resources.NODE_NAME_HCVKB, StringComparison.CurrentCultureIgnoreCase))
				{
					Version verNodeHCVKB = new Version(Properties.Resources.NODE_VERSION_MIN_HCVKB);
					_logger.Debug(string.Format("Comparing HCVKB Version => Base[{0}] : Reqeust[{1}] ", verNodeHCVKB.ToString(), verNode.ToString()));


					// by-pass until implement at HCVKB side
					//if (verNode.CompareTo(verNodeHCVKB) < 0)
					//    return bsupportedNodeVersion;
					_logger.Debug(string.Format("Comparing HCVKB Version => by-pass.. "));


					bSupportedNodeVersion = true;
				}

				// comparing node version for HCVKC
				if (strRequestNodeName.Equals(Properties.Resources.NODE_NAME_HCVKC, StringComparison.CurrentCultureIgnoreCase))
				{
					Version verNodeHCVKC = new Version(Properties.Resources.NODE_VERSION_MIN_HCVKC);
					_logger.Debug(string.Format("Comparing HCVKC Version => Base[{0}] : Reqeust[{1}] ", verNodeHCVKC.ToString(), verNode.ToString()));


					if (verNode.CompareTo(verNodeHCVKC) < 0)
						return bSupportedNodeVersion;


					bSupportedNodeVersion = true;
				}
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}

			return bSupportedNodeVersion;
		}


		//-------------------------------------------------------------------------------------------------------
		//
		//   Response Functions from the Broker Server Request
		//
		[Mapping("/control/health")]
		public object Method_Health(string strRequestBody)
		{
			JSonResult jsonResult = new JSonResult
			{
				resultCode = VOErrorCode._E_CODE_X_X0009999,
				resultMessage = VOErrorCode._E_MSG_FAIL,
				resultData = "/control/health"
			};


			try
			{
				// check to ready for service
				if (!_hcvkaMain.IsReadyForService())
				{
					string strErrorCode = _hcvkaMain.IsReadyForService() ? VOErrorCode._E_CODE_OK : VOErrorCode._E_CODE_A_A0000001;
					{
						// _E_CODE_A_A0000101 or _E_CODE_A_A0000102
						if (!_hcvkaMain.IsRegisteredToHCVKB)
						{
							if (!_hcvkaMain.IsReRegistrationInfo)
							{
								strErrorCode = VOErrorCode._E_CODE_A_A0000101;
							}
							else
							{
								strErrorCode = VOErrorCode._E_CODE_A_A0000102;
							}
						}
						// _E_CODE_X_X0000201
						else if (!_hcvkaMain.IsValidateIntegrity)
						{
							strErrorCode = VOErrorCode._E_CODE_X_X0000201;
						}
						// _E_CODE_A_A0000103
						else if (!_hcvkaMain.IsReadyRDPService)
						{
							strErrorCode = VOErrorCode._E_CODE_A_A0000103;
						}
					}
					jsonResult.resultCode = strErrorCode;
					jsonResult.resultMessage = VOErrorCode._E_MSG_READY_SERVICE;
					return jsonResult;
				}


				if (string.IsNullOrEmpty(strRequestBody))
					new ArgumentException("ResponseBody is null.", "strRequestBody");


				JObject jsonObject = JObject.Parse(strRequestBody);
				_logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


				dicHeaders.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, _hcvkaMain.voBrokerServer.AuthToken);


				// checking supported node version
				if (!IsSupportedNodeVersion(jsonObject))
				{
					jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
					jsonResult.resultMessage = VOErrorCode._E_MSG_NOT_SUPPORT_NODE_VERSION;
					return jsonResult;
				}


				// response with default 
				jsonResult.resultCode = VOErrorCode._E_CODE_OK;
				jsonResult.resultMessage = VOErrorCode._E_MSG_OK;
				jsonResult.resultData = new HCVKARequestJSONParam.JSonHealthReport
				{
					userId = _hcvkaMain.voHeartbeart.EncodedUserID,
					sessionClientIP = _hcvkaMain.voHeartbeart.ClientIP,
					sessionConnected = _hcvkaMain.voHeartbeart.IsClientConnected,
					sessionLastHeartbeatDt = _hcvkaMain.voHeartbeart.LastHeartbeatDt
				};
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

				jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
				jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
			}

			return jsonResult;
		}


		public class ResponseIntergrity
		{
			public List<VOIntegrity> integrityData;
		}
		[Mapping("/control/integrity")]
		public object Method_Integrity(string strRequestBody)
		{
			JSonResult jsonResult = new JSonResult
			{
				resultCode = VOErrorCode._E_CODE_X_X0009999,
				resultMessage = VOErrorCode._E_MSG_FAIL,
				resultData = new List<VOIntegrity>()
			};


			try
			{
				if (string.IsNullOrEmpty(strRequestBody))
					new ArgumentException("ResponseBody is null.", "strRequestBody");


				JObject jsonObject = JObject.Parse(strRequestBody);
				_logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


				// checking supported node version
				if (!IsSupportedNodeVersion(jsonObject))
				{
					jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
					jsonResult.resultMessage = VOErrorCode._E_MSG_NOT_SUPPORT_NODE_VERSION;
					return jsonResult;
				}



				// invoke function
				_hcvkaMain.CheckIntegrity();


				// response with default 
				jsonResult = new JSonResult
				{
					resultCode = _hcvkaMain.IsValidateIntegrity ? VOErrorCode._E_CODE_OK : VOErrorCode._E_CODE_X_X0000201,
					resultMessage = _hcvkaMain.IsValidateIntegrity ? VOErrorCode._E_MSG_OK : VOErrorCode._E_MSG_FAIL,
					resultData = new ResponseIntergrity
					{
						integrityData = _hcvkaMain.ListIntegrity
					}
				};

				jsonResult.resultCode = _hcvkaMain.IsValidateIntegrity ? VOErrorCode._E_CODE_OK : VOErrorCode._E_CODE_X_X0000201;
				jsonResult.resultMessage = _hcvkaMain.IsValidateIntegrity ? VOErrorCode._E_MSG_OK : VOErrorCode._E_MSG_FAIL;
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

				jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
				jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
			}



			return jsonResult;
		}


		[Mapping("/control/clearSession")]
		public object Method_ClearSession(string strRequestBody)
		{
			JSonResult jsonResult = new JSonResult
			{
				resultCode = VOErrorCode._E_CODE_X_X0009999,
				resultMessage = VOErrorCode._E_MSG_FAIL,
				resultData = "/control/clearSession"
			};


			try
			{
				// check to ready for service
				if (!_hcvkaMain.IsReadyForService())
				{
					_logger.Debug(string.Format("Skip.. because of not ready for serivce."));
					jsonResult.resultCode = VOErrorCode._E_CODE_A_A0000001;
					jsonResult.resultMessage = VOErrorCode._E_MSG_READY_SERVICE;
					return jsonResult;
				}


				if (string.IsNullOrEmpty(strRequestBody))
					new ArgumentException("ResponseBody is null.", "strRequestBody");


				JObject jsonObject = JObject.Parse(strRequestBody);
				_logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


				// checking supported node version
				if (!IsSupportedNodeVersion(jsonObject))
				{
					jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
					jsonResult.resultMessage = VOErrorCode._E_MSG_NOT_SUPPORT_NODE_VERSION;
					return jsonResult;
				}



				// clear heartbeat info.
				lock (_hcvkaMain.voHeartbeart)
				{
					_hcvkaMain.voHeartbeart.ClearHeartbeat();
				}


				// response with default 
				jsonResult.resultCode = VOErrorCode._E_CODE_OK;
				jsonResult.resultMessage = VOErrorCode._E_MSG_OK;
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

				jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
				jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
			}

			return jsonResult;
		}


		[Mapping("/control/registration")]
		public object Method_Registration(string strRequestBody)
		{
			JSonResult jsonResult = new JSonResult
			{
				resultCode = VOErrorCode._E_CODE_OK,
				resultMessage = VOErrorCode._E_MSG_OK,
				resultData = "/control/registration"
			};


			try
			{
				if (string.IsNullOrEmpty(strRequestBody))
					new ArgumentException("ResponseBody is null.", "strRequestBody");


				JObject jsonObject = JObject.Parse(strRequestBody);
				_logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


				// checking supported node version
				if (!IsSupportedNodeVersion(jsonObject))
				{
					jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
					jsonResult.resultMessage = VOErrorCode._E_MSG_NOT_SUPPORT_NODE_VERSION;
					return jsonResult;
				}



				// fetch node info.
				{
					JObject oNodeInfo = JObject.Parse(jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["nodeInfo"].ToString());
					_hcvkaMain.voBrokerServer.NodeName = oNodeInfo["nodeName"].ToString();
					_hcvkaMain.voLogServer.NodeName = oNodeInfo["nodeName"].ToString();

					_logger.Debug(string.Format("Registered node name : {0}", _hcvkaMain.voBrokerServer.NodeName));
				}

				// fetch token info.
				{
					JObject oTokenInfo = JObject.Parse(jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["tokenInfo"].ToString());
					_hcvkaMain.voBrokerServer.AuthToken = oTokenInfo["token"].ToString();
					_hcvkaMain.voBrokerServer.Expiration = CommonUtils.UnixTimeStampToDateTime(long.Parse(oTokenInfo["expiration"].ToString()));
					_hcvkaMain.voLogServer.AuthToken = oTokenInfo["token"].ToString();
					_hcvkaMain.voLogServer.Expiration = CommonUtils.UnixTimeStampToDateTime(long.Parse(oTokenInfo["expiration"].ToString()));

					_hcvkaMain.IsRegisteredToHCVKB = true;

					_logger.Debug(string.Format("Token : {0}", _hcvkaMain.voBrokerServer.AuthToken));
					_logger.Debug(string.Format("Next expiration time of token : {0}", _hcvkaMain.voBrokerServer.Expiration.ToLocalTime()));
				}

				// fetch server info.
				{
					JObject oServerInfo = JObject.Parse(jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["serverInfo"].ToString());
					_hcvkaMain.voBrokerServer.BrokerServerIP = oServerInfo["brokerServerIP"].ToString();
					_hcvkaMain.voBrokerServer.BrokerServerPort = oServerInfo["brokerServerPort"].ToString();
					_hcvkaMain.voLogServer.BrokerServerIP = oServerInfo["logServerIP"].ToString();
					_hcvkaMain.voLogServer.BrokerServerPort = oServerInfo["logServerPort"].ToString();

					_logger.Debug(string.Format("BrokerServer : [{0}:{1}]", _hcvkaMain.voBrokerServer.BrokerServerIP, _hcvkaMain.voBrokerServer.BrokerServerPort));
					_logger.Debug(string.Format("LogServer : [{0}:{1}]", _hcvkaMain.voLogServer.BrokerServerIP, _hcvkaMain.voLogServer.BrokerServerPort));
				}


				// set property of NodeName & ServerInfo.
				Properties.Settings.Default.NodeName = _hcvkaMain.voBrokerServer.NodeName;
				Properties.Settings.Default.BrokerServerIP = _hcvkaMain.voBrokerServer.BrokerServerIP;
				Properties.Settings.Default.BrokerServerPort = _hcvkaMain.voBrokerServer.BrokerServerPort;
				Properties.Settings.Default.LogServerIP = _hcvkaMain.voLogServer.BrokerServerIP;
				Properties.Settings.Default.LogServerPort = _hcvkaMain.voLogServer.BrokerServerPort;
				Properties.Settings.Default.Save();


				// report log to HCVKL
				_hcvkaMain.hcvkaLogReport.voLogServer = _hcvkaMain.voLogServer;
				_hcvkaMain.hcvkaLogReport.LogReport_Common_Registration(VOLogData.STATUS_SUCCEED, string.Empty);
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

				jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
				jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
			}

			return jsonResult;
		}


		[Mapping("/control/domainJoin")]
		public object Method_DomainJoin(string strRequestBody)
		{
			JSonResult jsonResult = new JSonResult
			{
				resultCode = VOErrorCode._E_CODE_X_X0009999,
				resultMessage = VOErrorCode._E_MSG_FAIL,
				resultData = "/control/domainJoin"
			};


			try
			{
				// check to ready for service
				//if (!_hcvkaMain.IsReadyForService())
				//{
				//    _logger.Debug(string.Format("Skip.. because of not ready for serivce."));
				//    jsonResult.resultCode = VOErrorCode._E_CODE_A_A0000001;
				//    jsonResult.resultMessage = VOErrorCode._E_MSG_READY_SERVICE;
				//    return jsonResult;
				//}


				if (string.IsNullOrEmpty(strRequestBody))
					new ArgumentException("ResponseBody is null.", "strRequestBody");


				JObject jsonObject = JObject.Parse(strRequestBody);
				_logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


				// checking supported node version
				if (!IsSupportedNodeVersion(jsonObject))
				{
					jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
					jsonResult.resultMessage = Properties.Resources.NODE_VERSION_MIN_HCVKB;
					return jsonResult;
				}



				string strDomainName = string.Empty;
				string strEncodedDomainAdminId = string.Empty;
				string strEncodedDomainAdminPW = string.Empty;

				// fetch domain info.
				{
					JObject oDomainInfo = JObject.Parse(jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA].ToString());
					strDomainName = oDomainInfo["domainName"].ToString();
					strEncodedDomainAdminId = oDomainInfo["encodedDomainAdminId"].ToString();
					strEncodedDomainAdminPW = oDomainInfo["encodedDomainAdminPW"].ToString();
				}


				// execute join to domain
				if (_hcvkaMain.ExecutDomainJoin(strDomainName, strEncodedDomainAdminId, strEncodedDomainAdminPW))
				{
					// response with default 
					jsonResult.resultCode = VOErrorCode._E_CODE_OK;
					jsonResult.resultMessage = VOErrorCode._E_MSG_OK;


					// report log to HCVKL
					_hcvkaMain.hcvkaLogReport.LogReport_Common_DomainJoin(strDomainName, VOLogData.STATUS_SUCCEED, string.Empty);

				}
				else
				{
					jsonResult.resultCode = VOErrorCode._E_CODE_A_A0000105;
					jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;


					// report log to HCVKL
					_hcvkaMain.hcvkaLogReport.LogReport_Common_DomainJoin(strDomainName, VOLogData.STATUS_FAILED, VOErrorCode._E_CODE_A_A0000105);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

				jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
				jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
			}

			return jsonResult;
		}
		//
		//-------------------------------------------------------------------------------------------------------


		//-------------------------------------------------------------------------------------------------------
		//
		//   Response Functions from the HCVKClient Request
		//
		[Mapping("/vdi/heartbeat")]
		public object Method_Heartbeat(string strRequestBody)
		{
			JSonResult jsonResult = new JSonResult
			{
				resultCode = VOErrorCode._E_CODE_X_X0009999,
				resultMessage = VOErrorCode._E_MSG_FAIL,
				resultData = "/vdi/heartbeat"
			};


			try
			{
				// check to ready for service
				if (!_hcvkaMain.IsReadyForService())
				{
					_logger.Debug(string.Format("Skip.. because of not ready for serivce."));
					jsonResult.resultCode = VOErrorCode._E_CODE_A_A0000001;
					jsonResult.resultMessage = VOErrorCode._E_MSG_READY_SERVICE;
					return jsonResult;
				}


				if (string.IsNullOrEmpty(strRequestBody))
					new ArgumentException("ResponseBody is null.", "strRequestBody");


				JObject jsonObject = JObject.Parse(strRequestBody);
				_logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


				// checking supported node version
				if (!IsSupportedNodeVersion(jsonObject))
				{
					jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
					jsonResult.resultMessage = Properties.Resources.NODE_VERSION_MIN_HCVKC;
					return jsonResult;
				}




				// get client info.
				string clientIP = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["clientIP"].ToString();
				string encodedUserId = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["userID"].ToString();

				lock (_hcvkaMain.voHeartbeart)
				{
					// check client info
					if (clientIP.Equals(_hcvkaMain.voUser.ClientIP, StringComparison.InvariantCultureIgnoreCase)
						&& encodedUserId.Equals(_hcvkaMain.voUser.EncodedUserID, StringComparison.InvariantCultureIgnoreCase))
					{
						_hcvkaMain.voHeartbeart.IsClientConnected = true;
					}
					else
					{
						_hcvkaMain.voHeartbeart.IsClientConnected = false;
					}
					_hcvkaMain.voHeartbeart.ClientIP = clientIP;
					_hcvkaMain.voHeartbeart.EncodedUserID = encodedUserId;
					_hcvkaMain.voHeartbeart.LastHeartbeatDt = CommonUtils.DateTimeToUnixTime(DateTime.Now);
					_logger.Debug(string.Format("vcHeartbeart ======>\n{0}", _hcvkaMain.voHeartbeart.ToString()));
				}


				// response with default 
				jsonResult.resultCode = VOErrorCode._E_CODE_OK;
				jsonResult.resultMessage = VOErrorCode._E_MSG_OK;
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

				jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
				jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
			}

			return jsonResult;
		}

		public class ConnectData
		{
			public string vdiPort;
			public string vdiOSVersion;
		}
		[Mapping("/vdi/connect")]
		public object Method_VDIConnect(string strRequestBody)
		{
			JSonResult jsonResult = new JSonResult
			{
				resultCode = VOErrorCode._E_CODE_X_X0009999,
				resultMessage = VOErrorCode._E_MSG_FAIL,
				resultData = new ConnectData()
				{
					vdiPort = _hcvkaMain.VDIPort,
#if WIN32
					vdiOSVersion = CommonUtils.GetOSVersion()
#else
					vdiOSVersion = "16.04"
#endif
				}
            };


            try
            {
                // check to ready for service
                if (!_hcvkaMain.IsReadyForService())
                {
                    _logger.Debug(string.Format("Skip.. because of not ready for serivce."));
                    jsonResult.resultCode = VOErrorCode._E_CODE_A_A0000001;
                    jsonResult.resultMessage = VOErrorCode._E_MSG_READY_SERVICE;
                    return jsonResult;
                }


                if (string.IsNullOrEmpty(strRequestBody))
                    new ArgumentException("ResponseBody is null.", "strRequestBody");


                JObject jsonObject = JObject.Parse(strRequestBody);
                _logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


                // checking supported node version
                if (!IsSupportedNodeVersion(jsonObject))
                {
                    jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
                    jsonResult.resultMessage = Properties.Resources.NODE_VERSION_MIN_HCVKC;
                    return jsonResult;
                }




                // get client info.
                _hcvkaMain.voUser.ClientIP = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["clientIP"].ToString();
                _hcvkaMain.voUser.UserID = CryptoManager.DecodingBase64(jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["userID"].ToString());
                _hcvkaMain.voUser.TenantID = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["tenantID"].ToString();
                _hcvkaMain.voUser.TenantName = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["tenantName"].ToString();
                _hcvkaMain.ServiceProtocol = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["serviceProtocol"].ToString();
                _hcvkaMain.DesktopID = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["desktopId"].ToString();



                if (_hcvkaMain.ServiceProtocol.Equals(VOProtocol.VOProtocolType.PROTOCOL_RDP, StringComparison.CurrentCultureIgnoreCase))
                {
                    // wait for client connecting to rdp service
                    HCVKARDPService.EnableRDPService();
                }


                // response with default 
                jsonResult.resultCode = VOErrorCode._E_CODE_OK;
                jsonResult.resultMessage = VOErrorCode._E_MSG_OK;


                // report log to HCVKL
                _hcvkaMain.hcvkaLogReport.voUser = _hcvkaMain.voUser;
                _hcvkaMain.hcvkaLogReport.LogReport_Desktop_Connect(_hcvkaMain.DesktopID, _hcvkaMain.ServiceProtocol, VOLogData.STATUS_SUCCEED, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

                jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
                jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
            }

            return jsonResult;
        }

        [Mapping("/vdi/disconnect")]
        public object Method_VDIDisconnect(string strRequestBody)
        {
            JSonResult jsonResult = new JSonResult
            {
                resultCode = VOErrorCode._E_CODE_X_X0009999,
                resultMessage = VOErrorCode._E_MSG_FAIL,
                resultData = "/vdi/disconnect"
            };


            try
            {
                // check to ready for service
                if (!_hcvkaMain.IsReadyForService())
                {
                    _logger.Debug(string.Format("Skip.. because of not ready for serivce."));
                    jsonResult.resultCode = VOErrorCode._E_CODE_A_A0000001;
                    jsonResult.resultMessage = VOErrorCode._E_MSG_READY_SERVICE;
                    return jsonResult;
                }


                if (string.IsNullOrEmpty(strRequestBody))
                    new ArgumentException("ResponseBody is null.", "strRequestBody");


                JObject jsonObject = JObject.Parse(strRequestBody);
                _logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


                // checking supported node version
                if (!IsSupportedNodeVersion(jsonObject))
                {
                    jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
                    jsonResult.resultMessage = Properties.Resources.NODE_VERSION_MIN_HCVKC;
                    return jsonResult;
                }


                // report log to HCVKL
                _hcvkaMain.hcvkaLogReport.LogReport_Desktop_Disconnect(_hcvkaMain.DesktopID, _hcvkaMain.ServiceProtocol, VOLogData.STATUS_SUCCEED, string.Empty);



                if (_hcvkaMain.ServiceProtocol.Equals(VOProtocol.VOProtocolType.PROTOCOL_RDP, StringComparison.CurrentCultureIgnoreCase))
                {
                    // closing rdp service from another client connecting
                    HCVKARDPService.DisableRDPService();
                }

                // clear heartbeat info.
                lock (_hcvkaMain.voHeartbeart)
                {
                    _hcvkaMain.voHeartbeart.ClearHeartbeat();
                }

                // clear user info.
                _hcvkaMain.voUser.ClientIP = string.Empty;
                _hcvkaMain.voUser.UserID = string.Empty;
                _hcvkaMain.voUser.TenantID = string.Empty;
                _hcvkaMain.voUser.TenantName = string.Empty;
                _hcvkaMain.ServiceProtocol = string.Empty;
                _hcvkaMain.DesktopID = string.Empty;


                // response with default 
                jsonResult.resultCode = VOErrorCode._E_CODE_OK;
                jsonResult.resultMessage = VOErrorCode._E_MSG_OK;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

                jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
                jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
            }

            return jsonResult;
        }

        [Mapping("/vdi/reconnect")]
        public object Method_VDIReconnect(string strRequestBody)
        {
            JSonResult jsonResult = new JSonResult
            {
                resultCode = VOErrorCode._E_CODE_X_X0009999,
                resultMessage = VOErrorCode._E_MSG_FAIL,
                resultData = "/vdi/reconnect"
            };


            try
            {
                // check to ready for service
                if (!_hcvkaMain.IsReadyForService())
                {
                    _logger.Debug(string.Format("Skip.. because of not ready for serivce."));
                    jsonResult.resultCode = VOErrorCode._E_CODE_A_A0000001;
                    jsonResult.resultMessage = VOErrorCode._E_MSG_READY_SERVICE;
                    return jsonResult;
                }


                if (string.IsNullOrEmpty(strRequestBody))
                    new ArgumentException("ResponseBody is null.", "strRequestBody");


                JObject jsonObject = JObject.Parse(strRequestBody);
                _logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


                // checking supported node version
                if (!IsSupportedNodeVersion(jsonObject))
                {
                    jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
                    jsonResult.resultMessage = Properties.Resources.NODE_VERSION_MIN_HCVKC;
                    return jsonResult;
                }




                // get client info.
                _hcvkaMain.voUser.ClientIP = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["clientIP"].ToString();
                _hcvkaMain.voUser.UserID = CryptoManager.DecodingBase64(jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["userID"].ToString());
                _hcvkaMain.voUser.TenantID = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["tenantID"].ToString();
                _hcvkaMain.voUser.TenantName = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["tenantName"].ToString();
                _hcvkaMain.ServiceProtocol = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["serviceProtocol"].ToString();
                _hcvkaMain.DesktopID = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["desktopId"].ToString();

                if (_hcvkaMain.ServiceProtocol.Equals(VOProtocol.VOProtocolType.PROTOCOL_RDP, StringComparison.CurrentCultureIgnoreCase))
                {
                    // wait for client connecting to rdp service
                    HCVKARDPService.EnableRDPService();
                }


                // response with default 
                jsonResult = new JSonResult
                {
                    resultCode = VOErrorCode._E_CODE_OK,
                    resultMessage = VOErrorCode._E_MSG_OK,
                    resultData = "/vdi/reconnect"
                };


                // report log to HCVKL
                _hcvkaMain.hcvkaLogReport.LogReport_Desktop_Reconnect(_hcvkaMain.DesktopID, _hcvkaMain.ServiceProtocol, VOLogData.STATUS_SUCCEED, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));


                // response with error
                jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
                jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
            }

            return jsonResult;
        }

        [Mapping("/vdi/onconnect")]
        public object Method_VDIOnConnect(string strRequestBody)
        {
            JSonResult jsonResult = new JSonResult
            {
                resultCode = VOErrorCode._E_CODE_X_X0009999,
                resultMessage = VOErrorCode._E_MSG_FAIL,
                resultData = "/vdi/onconnect"
            };


            try
            {
                // check to ready for service
                if (!_hcvkaMain.IsReadyForService())
                {
                    _logger.Debug(string.Format("Skip.. because of not ready for serivce."));
                    jsonResult.resultCode = VOErrorCode._E_CODE_A_A0000001;
                    jsonResult.resultMessage = VOErrorCode._E_MSG_READY_SERVICE;
                    return jsonResult;
                }


                if (string.IsNullOrEmpty(strRequestBody))
                    new ArgumentException("ResponseBody is null.", "strRequestBody");


                JObject jsonObject = JObject.Parse(strRequestBody);
                _logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


                // checking supported node version
                if (!IsSupportedNodeVersion(jsonObject))
                {
                    jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
                    jsonResult.resultMessage = Properties.Resources.NODE_VERSION_MIN_HCVKC;
                    return jsonResult;
                }




                if (_hcvkaMain.ServiceProtocol.Equals(VOProtocol.VOProtocolType.PROTOCOL_RDP, StringComparison.CurrentCultureIgnoreCase))
                {
                    // closing rdp service from another client connecting
                    HCVKARDPService.DisableRDPService();
                }


                // response with default 
                jsonResult.resultCode = VOErrorCode._E_CODE_OK;
                jsonResult.resultMessage = VOErrorCode._E_MSG_OK;


                // report log to HCVKL
                _hcvkaMain.hcvkaLogReport.LogReport_Desktop_OnConnect(_hcvkaMain.DesktopID, _hcvkaMain.ServiceProtocol, VOLogData.STATUS_SUCCEED, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

                jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
                jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
            }

            return jsonResult;
        }
        //
        //-------------------------------------------------------------------------------------------------------


        [Mapping("/control/management")]
        public object Method_VDIManage(string strRequestBody)
        {
            JSonResult jsonResult = new JSonResult
            {
                resultCode = VOErrorCode._E_CODE_X_X0009999,
                resultMessage = VOErrorCode._E_MSG_FAIL,
                resultData = "/control/management"
            };


            try
            {
                if (string.IsNullOrEmpty(strRequestBody))
                    new ArgumentException("ResponseBody is null.", "strRequestBody");


                JObject jsonObject = JObject.Parse(strRequestBody);
                _logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject?.ToString()));


                // checking supported node version
                if (!IsSupportedNodeVersion(jsonObject))
                {
                    jsonResult.resultCode = VOErrorCode._E_CODE_X_X0000203;
                    jsonResult.resultMessage = string.Format("{0} : HCVKB >= {1}, HCVKC >= {2}", VOErrorCode._E_MSG_NOT_SUPPORT_NODE_VERSION, 
                        Properties.Resources.NODE_VERSION_MIN_HCVKB, Properties.Resources.NODE_VERSION_MIN_HCVKC);
                    return jsonResult;
                }




                // get command for rdp management.
                string strCommand = jsonObject[HCVKARequestJSONParam.REQUEST_NODE_DATA]["command"].ToString();

                switch (strCommand)
                {
                    case "enableRDP":
                        // opening rdp service to wait for client connecting
                        HCVKARDPService.EnableRDPService();
                        jsonResult.resultMessage = VOErrorCode._E_MSG_OK;
                        break;

                    case "disableRDP":
                        // closing rdp service from another client connecting
                        HCVKARDPService.DisableRDPService();
                        jsonResult.resultMessage = VOErrorCode._E_MSG_OK;
                        break;

                    case "bypassRegistration":
                        // Bypass to check to registered with HCVKB
                        _hcvkaMain.IsRegisteredToHCVKB = true;
                        jsonResult.resultMessage = VOErrorCode._E_MSG_OK;
                        break;

                    case "bypassIntegrity":
                        // Bypass to check to integrity validation
                        _hcvkaMain.IsValidateIntegrity = true;
                        jsonResult.resultMessage = VOErrorCode._E_MSG_OK;
                        break;

                    case "defaultSetRDPPort":
                        // Restart to Default RDP Service
                        HCVKARDPService.SetDefaultRDPPort();
                        HCVKARDPService.EnableRDPService();
                        jsonResult.resultMessage = VOErrorCode._E_MSG_OK;
                        break;

                    default:
                        jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
                        break;
                }


                // response with default 
                jsonResult.resultCode = VOErrorCode._E_CODE_OK;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));

                jsonResult.resultCode = VOErrorCode._E_CODE_X_X0009999;
                jsonResult.resultMessage = VOErrorCode._E_MSG_FAIL;
            }

            return jsonResult;
        }

    }
}
