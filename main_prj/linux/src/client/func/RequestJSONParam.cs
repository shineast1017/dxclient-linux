using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using client;

namespace client.Request
{
    class RequestJSONParam
    {
        //declare for const of response header
        public const string RESPONSE_RESULT_CODE = "resultCode";
        public const string RESPONSE_RESULT_MSG = "resultMessageCode";
        public const string RESPONSE_RESULT_DATA = "resultData";
				public const string RESPONSE_RESULT_MESSAGE = "resultMessage";


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
        public class JSonUserInfo
        {
            public string userId;
            public string password;
            public string newPassword;
            public string clientIp;
            public string clientMac;
            public string clientDeviceType;
            public string clientReleaseVersion;
            public string clientReleaseDt;
        };
        // For DBUS Token, UUID
    public class JSonUserInfoNew : JSonUserInfo
        {
    	public string encMsg;
			public string platformUuid;
        };
        
		public class JSonDesktopPoolInfo
		{
			public string userId;
			public string tenantId;
			public string clientIp;
			public string clientLoc;
		};

		public class JSonRecommandServerInfo
		{
			public string clientIp;
			public string clientLoc;
		}

	public class JSonDesktopInfo
		{
			public string tenantId;
			public string userId;
			public string poolId;
			public string accessDiv;
			public string desktopId;
			public string instanceId;
			public string powerOption;
			public string infoType;
			public string requestProtocol;
			public string displayName;
			public string clientIp;
			public string clientLoc;
		}


        // ------------------------------------------------------------------------------------------
        // request body for HCVKA
        public class JSonRequestParam
        {
            public string clientIP;
            public string clientMAC;
            public string userID;
            public string tenantID;
            public string tenantName;
            public string accessDiv;
            public string serviceProtocol;
            public string desktopId;
        }
		public class JSonOnConnectRequestParam : JSonRequestParam {
			public string clientLoc;

		};

		public class JSonRequestDevice 
		{
			public string clientIP;
			public string userID;
			public string tenantID;
			public string deviceID;
			public string remoteIP;
			public string remotePort;
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
