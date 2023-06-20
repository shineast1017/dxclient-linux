using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
    public class VOLogData
    {
        public const string STATUS_START = "0";
        public const string STATUS_SUCCEED = "1";
        public const string STATUS_FAILED = "2";

        public string _loggingDt = string.Empty;
        public string _loggingClientIp = string.Empty;
        public string _requestEncodedUserId = string.Empty;
        public string _requestTenantId = string.Empty;
        public string _requestTenantName = string.Empty;
        public string _requestParam = string.Empty;
        public string _resultParam = string.Empty;
        public string _jobStatusCode = string.Empty;
        public string _jobCode = string.Empty;
        public string _jobObject = string.Empty;
        public string _errorMessage = string.Empty;


        public string LoggingDt
        {
            set { _loggingDt = value; }
            get { return _loggingDt; }
        }
        public string LoggingClientIp
        {
            set { _loggingClientIp = value; }
            get { return _loggingClientIp; }
        }
        public string RequestEncodedUserId
        {
            set { _requestEncodedUserId = value; }
            get { return _requestEncodedUserId; }
        }
        public string RequestTenantId
        {
            set { _requestTenantId = value; }
            get { return _requestTenantId; }
        }
        public string RequestTenantName
        {
            set { _requestTenantName = value; }
            get { return _requestTenantName; }
        }
        public string RequestParam
        {
            set { _requestParam = value; }
            get { return _requestParam; }
        }
        public string ResultParam
        {
            set { _resultParam = value; }
            get { return _resultParam; }
        }
        public string JobStatusCode
        {
            set { _jobStatusCode = value; }
            get { return _jobStatusCode; }
        }
        public string JobCode
        {
            set { _jobCode = value; }
            get { return _jobCode; }
        }
        public string JobObject
        {
            set { _jobObject = value; }
            get { return _jobObject; }
        }
        public string ErrorMessage
        {
            set { _errorMessage = value; }
            get { return _errorMessage; }
        }

    }
    public class VOAgentLogData : VOLogData
    {
        public string _loggingAgentIp = string.Empty;


        public string LoggingAgentIp
        {
            set { _loggingAgentIp = value; }
            get { return _loggingAgentIp; }
        }
    }
    public class VOClientLogData : VOLogData
    {
        public string _loggingClientIp = string.Empty;
        public string _requestEncodedUserId = string.Empty;

        public string LoggingClientIp
        {
            set { _loggingClientIp = value; }
            get { return _loggingClientIp; }
        }
        public string RequestEncodedUserId
        {
            set { _requestEncodedUserId = value; }
            get { return _requestEncodedUserId; }
        }
    }
}
