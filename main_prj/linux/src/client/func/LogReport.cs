using client.Request;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Reflection;
using HCVK.HCVKSLibrary;
using HCVK.HCVKSLibrary.VO;

namespace client
{
    public class LogReport
    {
        private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private VOBrokerServerNew _voLogServer = new VOBrokerServerNew();
        private VOClientLogData _voLogData = new VOClientLogData();
        private VOUser _voUser = new VOUser();

        public VOBrokerServerNew voLogServer
        { set { _voLogServer = value; } }
        public VOClientLogData voLogData
        { set { _voLogData = value; } }
        public VOUser voUser
        { set { _voUser = value; } }

        //---------------------------------------------------------------------------------
        //  Common Action Logging
        public void LogReport_Common_Integrity(string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_CL_USER_INTEGRITY;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Empty;
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        //  User Action Logging
        public void LogReport_User_Login(string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_CL_USER_LOGIN;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Empty;
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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

        public void LogReport_User_Logout(string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_CL_USER_LOGOUT;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Empty;
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_User_ChangeInfo(string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_CL_USER_CHANGEINFO;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Format("Old Info.:{0}, New Info.:{1}", _voUser.EncodedPassword, _voUser.EncodedNewPassword);
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Empty;
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        //  Desktop Info. Action Logging
        public void LogReport_DesktopInfo_Pool(string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_CL_DESKTOPINFO_POOL;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Empty;
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_DesktopInfo_Status(string strDesktopId, string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_CL_DESKTOPINFO_STATUS;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Empty;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_DesktopInfo_Power(string strDesktopId, string strPower, string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_CL_DESKTOPINFO_POWER;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Format("PowerStat:{0}", strPower);
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_Desktop_ConnectionInfo(string strDesktopId, string strProtocol, string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_CL_DESKTOP_CONNECTIONINFO;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Format("Protocol:{0}", strProtocol);
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
        public void LogReport_Desktop_Connect(string strDesktopId, string strProtocol, string strStatusCode, string strErrorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(_voLogServer.BrokerServerIP))
                    return;

                {
                    _voLogData.JobCode = Properties.Resources.LOG_CL_DESKTOP_CONNECT;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Format("Protocol:{0}", strProtocol); ;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
                    _voLogData.JobCode = Properties.Resources.LOG_CL_DESKTOP_DISCONNECT;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Format("Protocol:{0}", strProtocol); ;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
                    _voLogData.JobCode = Properties.Resources.LOG_CL_DESKTOP_RECONNECT;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Format("Protocol:{0}", strProtocol); ;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
                    _voLogData.JobCode = Properties.Resources.LOG_CL_DESKTOP_ONCONNECT;
                    _voLogData.LoggingDt = CommonUtils.DateTimeToUnixTime(DateTime.Now).ToString();
                    _voLogData.LoggingClientIp = _voUser.ClientIP;
                    _voLogData.RequestEncodedUserId = _voUser.EncodedUserID;
                    _voLogData.RequestTenantId = _voUser.TenantID;
                    _voLogData.RequestTenantName = _voUser.TenantName;
                    _voLogData.RequestParam = string.Format("Protocol:{0}", strProtocol); ;
                    _voLogData.ResultParam = string.Empty;
                    _voLogData.JobStatusCode = strStatusCode;
                    _voLogData.JobObject = string.Format("Instance Id:{0}", strDesktopId);
                    _voLogData.ErrorMessage = strErrorMsg;
                }
                new RequestToHCVKL().RequestLogReport_AsyncCallback(_voLogServer, _voLogData, Callback_LogReport);
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
                    if (resJsonObject != null && resJsonObject[RequestJSONParam.RESPONSE_RESULT_CODE].ToString().Equals("0"))
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
