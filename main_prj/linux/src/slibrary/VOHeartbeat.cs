using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
    public class VOHeartbeat
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string DEFAUTL_CONNECTED_MODE = "VDIDisconnect";
        private const long DEFAULT_HEARTBEAT_TIME = 0L;
        private const int DEFAULT_RETRY_TIMES = 3;


        private static string _strEncodedUserID = string.Empty;
        private static string _strClientIP = string.Empty;
        private static string _strLastConnectedMode = DEFAUTL_CONNECTED_MODE;
        private static bool _bIsClientConnected = false;
        private static long _dtLastHeartbeatDt = DEFAULT_HEARTBEAT_TIME;
        private static int _nRetryTimes = DEFAULT_RETRY_TIMES;


        public string UserID
        {
            set { _strEncodedUserID = CryptoManager.EncodingBase64(value); }
            get { return CryptoManager.DecodingBase64(_strEncodedUserID); }
        }
        public string EncodedUserID
        {
            set { _strEncodedUserID = value; }
            get { return _strEncodedUserID; }
        }
        public string ClientIP
        {
            set { _strClientIP = value; }
            get { return _strClientIP; }
        }
        public string LastConnectedMode
        {
            set { _strLastConnectedMode = value; }
            get { return _strLastConnectedMode; }
        }
        public bool IsClientConnected
        {
            set { _bIsClientConnected = value; }
            get { return _bIsClientConnected; }
        }
        public long LastHeartbeatDt
        {
            set { _dtLastHeartbeatDt = value; }
            get { return _dtLastHeartbeatDt; }
        }
        public int RetryTimes
        {
            set { _nRetryTimes = value; }
            get { return _nRetryTimes; }
        }

        public void ClearHeartbeat()
        {
            _strEncodedUserID = string.Empty;
            _strClientIP = string.Empty;
            _strLastConnectedMode = DEFAUTL_CONNECTED_MODE;
            _bIsClientConnected = false;
            _dtLastHeartbeatDt = DEFAULT_HEARTBEAT_TIME;
            _nRetryTimes = DEFAULT_RETRY_TIMES;
        }

        public VOHeartbeat Duplicate()
        {
            VOHeartbeat retVCHeartbeat = new VOHeartbeat();
            {
                retVCHeartbeat.EncodedUserID = _strEncodedUserID;
                retVCHeartbeat.ClientIP = _strClientIP;
                retVCHeartbeat.LastConnectedMode = _strLastConnectedMode;
                retVCHeartbeat.IsClientConnected = _bIsClientConnected;
                retVCHeartbeat.LastHeartbeatDt = _dtLastHeartbeatDt;
                retVCHeartbeat.RetryTimes = _nRetryTimes;
            }

            return retVCHeartbeat;
        }
    }
}
