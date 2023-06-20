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
using static client.Request.RequestJSONParam;
using client;

namespace client.Request
{
    class RequestToHCVKL : HttpsClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //-----------------------------------------------------------------------------------------------
        // logs/type/client
        public async Task<JObject> RequestLogReport(VOBrokerServerNew voBrokerServer, VOClientLogData voLogData)
        {
            string strUrl = string.Format("{0}://{1}:{2}/{3}",
                HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKL_REQUEST_LOG_REPORT);

            // add header field of  authentication token
            // 2017.11.10   no more used authToken for logging
            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            //dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

            JSonRequest oParam = new JSonRequest
            {
                requestNodeData = new JSonClientLogInfo
                {
                    loggingDt = voLogData.LoggingDt,
                    loggingClientIp = voLogData.LoggingClientIp,
                    requestUserId = voLogData.RequestEncodedUserId,
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
        public void RequestLogReport_AsyncCallback(VOBrokerServerNew voBrokerServer, VOClientLogData voLogData, CallbackResponse callbackResponse)
        {
            try
            {
                string strUrl = string.Format("{0}://{1}:{2}/{3}",
                        HTTP_PROTOCOL, voBrokerServer.BrokerServerIP, voBrokerServer.BrokerServerPort, Properties.Resources.HCVKL_REQUEST_LOG_REPORT);

                // add header field of  authentication token
                // 2017.11.10   no more used authToken for logging
                Dictionary<string, string> dicHeader = new Dictionary<string, string>();
                //dicHeader.Add(Properties.Resources.HCVK_HEADER_AUTH_TOKEN, voBrokerServer.AuthToken);

                JSonRequest oParam = new JSonRequest
                {
                    requestNodeData = new JSonClientLogInfo
                    {
                        loggingDt = voLogData.LoggingDt,
                        loggingClientIp = voLogData.LoggingClientIp,
                        requestUserId = voLogData.RequestEncodedUserId,
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
