using HCVK.HCVKSLibrary.VO;
using HCVK.HCVKSLibrary;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static HCVK.HCVKAService.HCVKARequest.HCVKARequestJSONParam;

namespace HCVK.HCVKAService.HCVKARequest
{
    public partial class HCVKBRequestToHCVKL : HttpsClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public async Task<JObject> RequestHealthReport(VOBrokerServer voBrokerServer, VOHeartbeat voHeartbeat, string strStateCode)
        {
            string strUrl = string.Format("https://{0}:{1}/{2}",
                voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_HEALTH_REPORT);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeName = voBrokerServer.NodeName,
                requestNodeData = new JSonHealthReport
                {
                    userId = voHeartbeat.UserID,
                    sessionClientIP = voHeartbeat.ClientIP,
                    sessionConnected = voHeartbeat.IsClientConnected,
                    sessionLastHeartbeatDt = voHeartbeat.LastHeartbeatDt,
                    agentState = strStateCode
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
        public void RequestHealthReport_AsyncCallback(VOBrokerServer voBrokerServer, VOHeartbeat voHeartbeat, string strStateCode, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("https://{0}:{1}/{2}",
                    voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_HEALTH_REPORT);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();
                dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeName = voBrokerServer.NodeName,
                    requestNodeData = new JSonHealthReport
                    {
                        userId = voHeartbeat.UserID,
                        sessionClientIP = voHeartbeat.ClientIP,
                        sessionConnected = voHeartbeat.IsClientConnected,
                        sessionLastHeartbeatDt = voHeartbeat.LastHeartbeatDt,
                        agentState = strStateCode 
                    }
                };
                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        // -----------------------------------------------------------------------------------------------------
        // /client/reregistration
        public async Task<JObject> RequestReRegistration(VOBrokerServer voBrokerServer)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_RE_REGISTRATION);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();

            JSonRequest oParam = new JSonRequest
            {
                requestNodeName = voBrokerServer.NodeName
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
        public void RequestReRegistration_AsyncCallback(VOBrokerServer voBrokerServer, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                    HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_RE_REGISTRATION);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeName = voBrokerServer.NodeName
                };
                SendReqeust_AsyncCallback(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam, callbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


    }
}
