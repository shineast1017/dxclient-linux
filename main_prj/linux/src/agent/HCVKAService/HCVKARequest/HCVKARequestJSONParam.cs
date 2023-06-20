using HCVK.HCVKSLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKAService.HCVKARequest
{
    public class HCVKARequestJSONParam
    {
        //declare for const of request header
        public const string REQUEST_NODE_NAME = "requestNodeName";
        public const string REQUEST_NODE_VERSION = "requestNodeVersion";
        public const string REQUEST_NODE_DATA = "requestNodeData";

        //declare for const of response header
        public const string RESPONSE_RESULT_CODE = "resultCode";
        public const string RESPONSE_RESULT_MSG = "resultMessageCode";
        public const string RESPONSE_RESULT_DATA = "resultData";


        // ------------------------------------------------------------------------------------------
        // request header
        public class JSonRequest
        {
            public string requestNodeName = Properties.Resources.REQUEST_NODE_NAME;
            public string requestNodeVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            public object requestNodeData;
        }


        // ------------------------------------------------------------------------------------------
        // request body for HCVKB
        public class JSonHealthReport
        {
            public string userId;

            public string sessionClientIP;
            public bool sessionConnected;
            public long sessionLastHeartbeatDt;

            public string agentState;
        }

        public class JSonIntegrityReport
        {
            public string message;
            public string nodeName;
            public string methodName;
            public string expectedHash;
            public string currentHash;
            public string status;
            public string nodeIP;
            public string userID;
        }


        // ------------------------------------------------------------------------------------------
        // request body for HCVKL
        public class JSonLogInfo
        {
            public string loggingDt = string.Empty;
            public string requestTenantId = string.Empty;
            public string requestTenantName = string.Empty;
            public string requestParam = string.Empty;
            public string resultParam = string.Empty;
            public string jobStatusCode = string.Empty;
            public string jobCode = string.Empty;
            public string jobObject = string.Empty;
            public string errorMessage = string.Empty;
        };

        public class JSonClientLogInfo : JSonLogInfo
        {
            public string loggingClientIp = string.Empty;
            public string requestUserId = string.Empty;
        }
        public class JSonAgentLogInfo : JSonLogInfo
        {
            public string loggingAgentIp = string.Empty;
        }

    }
}
