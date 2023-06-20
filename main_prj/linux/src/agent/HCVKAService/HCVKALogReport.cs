using HCVK.HCVKAService.HCVKARequest;
using HCVK.HCVKSLibrary;
using HCVK.HCVKSLibrary.VO;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKAService
{
    public class HCVKALogReport
    {
        private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private VOBrokerServer _voLogServer = new VOBrokerServer();
        private VOAgentLogData _voLogData = new VOAgentLogData();
        private VOUser _voUser = new VOUser();
        private string _strAgentIp = string.Empty;


        public VOBrokerServer voLogServer
        { set { _voLogServer = value; } }
        public VOAgentLogData voLogData
        { set { _voLogData = value; } }
        public VOUser voUser
        { set { _voUser = value; } }
        public string AgentIp
        { set { _strAgentIp = value; } }

        static string GetNameOf<T>(Expression<Func<T>> property)
        {
            return (property.Body as MemberExpression).Member.Name;
        }


        //---------------------------------------------------------------------------------
        //  Common Action Logging
        public void LogReport_Common_Integrity(string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_AG_COMMON_INTEGRITY;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingAgentIp = _strAgentIp;
                    _voLogData.RequestTenantId = string.Empty;
                    _voLogData.RequestTenantName = string.Empty;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Empty;
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new HCVKARequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_Common_Registration(string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_AG_COMMON_REGISTRATION;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingAgentIp = _strAgentIp;
                    _voLogData.RequestTenantId = string.Empty;
                    _voLogData.RequestTenantName = string.Empty;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Empty;
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new HCVKARequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_Common_ReRegistration(string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_AG_COMMON_REREGISTRATION;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingAgentIp = _strAgentIp;
                    _voLogData.RequestTenantId = string.Empty;
                    _voLogData.RequestTenantName = string.Empty;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Empty;
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new HCVKARequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_Common_DomainJoin(string strDomain, string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_AG_COMMON_REREGISTRATION;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingAgentIp = _strAgentIp;
                    _voLogData.RequestTenantId = string.Empty;
                    _voLogData.RequestTenantName = string.Empty;
                    _voLogData.RequestParam = string.Format("Domain:{0}", strDomain);
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Empty;
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new HCVKARequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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

        //---------------------------------------------------------------------------------
        //  Desktop Action Logging
        public void LogReport_Desktop_Connect(string strDesktopId, string strProtocol, string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_AG_DESKTOP_CONNECT;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingAgentIp = _strAgentIp;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Format("Protocol:{0}", strProtocol);
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new HCVKARequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_Desktop_OnConnect(string strDesktopId, string strProtocol, string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_AG_DESKTOP_ONCONNECT;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingAgentIp = _strAgentIp;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Format("Protocol:{0}", strProtocol);
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new HCVKARequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_Desktop_Reconnect(string strDesktopId, string strProtocol, string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_AG_DESKTOP_RECONNECT;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingAgentIp = _strAgentIp;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Format("Protocol:{0}", strProtocol);
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new HCVKARequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_Desktop_Disconnect(string strDesktopId, string strProtocol, string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_AG_DESKTOP_DISCONNECT;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingAgentIp = _strAgentIp;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Format("Protocol:{0}", strProtocol);
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new HCVKARequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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


        //---------------------------------------------------------------------------------
        //  Callback Logging
        private void Callback_LogReport(JObject resJsonObject, Exception exParam)
        {
            _logger.Debug(string.Format("response : {0}", resJsonObject?.ToString()));

            try
            {
                if (resJsonObject != null)
                {
                    if (resJsonObject != null && resJsonObject[HCVKARequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
                    {
                        // success response
                        // fetch and update auth token
                        //new HCVKCRequestToHCVKB().UpdateAuthToken(_voLogServer, resJsonObject);
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
    }
}
