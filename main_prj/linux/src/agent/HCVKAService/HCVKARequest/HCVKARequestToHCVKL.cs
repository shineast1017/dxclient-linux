using HCVK.HCVKSLibrary;
using HCVK.HCVKSLibrary.VO;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static HCVK.HCVKAService.HCVKARequest.HCVKARequestJSONParam;

namespace HCVK.HCVKAService.HCVKARequest
{
    class HCVKARequestToHCVKL : HttpsClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //-----------------------------------------------------------------------------------------------
        // logs/type/agent
        public async Task<JObject> RequestLogReport(VOBrokerServer voLogServer, VOAgentLogData voLogData)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voLogServer.BrokerServerIP, voLogServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_LOG_REPORT);

            // add header field of  authentication token
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            //dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voLogServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeName = voLogServer.NodeName,
                requestNodeData = new JSonAgentLogInfo
                {
                    loggingDt = voLogData.LoggingDt,
                    loggingAgentIp = voLogData.LoggingAgentIp,
                    requestTenantId = voLogData.RequestTenantId,
                    requestTenantName = voLogData.RequestTenantName,
                    requestParam = voLogData.RequestParam,
                    resultParam = voLogData.ResultParam,
                    jobStatusCode = voLogData.JobStatusCode,
                    jobCode = voLogData.JobCode,
                    jobObject = voLogData.JobObject,
                    errorMessage = voLogData.ErrorMessage
                }
            };

            return await SendReqeust(strUrl, HTTP_HEADER_METHOD_POST, dicHeader, oParam);
        }
        public void RequestLogReport_AsyncCallback(VOBrokerServer voLogServer, VOAgentLogData voLogData, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, voLogServer.BrokerServerIP, voLogServer.BrokerServerPort, Properties.Resources.HCVKB_REQUEST_LOG_REPORT);

                // add header field of  authentication token
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();
                //dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voLogServer.AuthToken);

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeName = voLogServer.NodeName,
                    requestNodeData = new JSonAgentLogInfo
                    {
                        loggingDt = voLogData.LoggingDt,
                        loggingAgentIp = voLogData.LoggingAgentIp,
                        requestTenantId = voLogData.RequestTenantId,
                        requestTenantName = voLogData.RequestTenantName,
                        requestParam = voLogData.RequestParam,
                        resultParam = voLogData.ResultParam,
                        jobStatusCode = voLogData.JobStatusCode,
                        jobCode = voLogData.JobCode,
                        jobObject = voLogData.JobObject,
                        errorMessage = voLogData.ErrorMessage
                    }
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
