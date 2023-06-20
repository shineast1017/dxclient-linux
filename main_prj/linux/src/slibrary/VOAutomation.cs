using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
    public delegate void UpdateAutomationState();

    public class VOAutomation
    {
        public enum AutomationStateEnum
        {
            AUTOMATION_START, 
            AUTOMATION_WAITING, 
            AUTOMATION_STOP,

            LOGIN_START,
            LOGIN_FAILED,
            LOGIN_SUCCESS,

            CONNECTION_START,
            CONNECTION_FAILED,
            CONNECTION_SUCCESS
        }

        private string _strNodeName = string.Empty;
        public string NodeName
        {
            set { _strNodeName = value; }
            get { return _strNodeName; }
        }

        private string _strAuthToken = string.Empty;
        public string AuthToken
        {
            set { _strAuthToken = value; }
            get { return _strAuthToken; }
        }

        private DateTime _dtExpiration = DateTime.Now;
        public DateTime Expiration
        {
            set { _dtExpiration = value; }
            get { return _dtExpiration; }
        }

        public UpdateAutomationState updateAutomationState = null;

        private VOAutomation.AutomationStateEnum _strAutomationState = AutomationStateEnum.AUTOMATION_STOP;
        public VOAutomation.AutomationStateEnum AutomationState
        {
            get { return _strAutomationState; }
            set
            {
                _strAutomationState = value;
                if (updateAutomationState != null)
                {
                    updateAutomationState();
                }
            }
        }

        private int _nCurrentRetryTimes = 0;
        public int CurrentRetryTimes
        {
            get { return _nCurrentRetryTimes; }
            set { _nCurrentRetryTimes = value; }
        }

        private bool _bIsAutomation = false;
        public bool IsAutomation
        {
            get { return _bIsAutomation; }
            set { _bIsAutomation = value; }
        }

        private bool _bIsAutoLogin = false;
        public bool IsAutoLogin
        {
            get { return _bIsAutoLogin; }
            set { _bIsAutoLogin = value; }
        }

        private bool _bIsAutoConnection = false;
        public bool IsAutoConnection
        {
            get { return _bIsAutoConnection; }
            set { _bIsAutoConnection = value; }
        }

        private int _nRetryTimes = 0;
        public int RetryTimes
        {
            get { return _nRetryTimes; }
            set { _nRetryTimes = value; }
        }

        private string _strBrokerServerIP = string.Empty;
        public string BrokerServerIP
        {
            get { return _strBrokerServerIP; }
            set { _strBrokerServerIP = value; }
        }

        private int _nBrokerServerPort = 8443;
        public int BrokerServerPort
        {
            get { return _nBrokerServerPort; }
            set { _nBrokerServerPort = value; }
        }

        private bool _bIsGateway = false;
        public bool IsGateway
        {
            get { return _bIsGateway; }
            set { _bIsGateway = value; }
        }

        private string _strEncodedUserID = string.Empty;
        public string UserID
        {
            get { return CryptoManager.DecodingBase64(_strEncodedUserID); }
            set { _strEncodedUserID = CryptoManager.EncodingBase64(value); }
        }
        public string EncodedUserID
        {
            get { return _strEncodedUserID; }
            set { _strEncodedUserID = value; }
        }

        private string _strEncodedUserPW = string.Empty;
        public string UserPW
        {
            get { return CryptoManager.DecodingBase64(_strEncodedUserPW); }
            set { _strEncodedUserPW = CryptoManager.EncodingBase64(value); }
        }
        public string EncodedUserPW
        {
            get { return _strEncodedUserPW; }
            set { _strEncodedUserPW = value; }
        }

        private int _nPoolID = 0;
        public int PoolID
        {
            get { return _nPoolID; }
            set { _nPoolID = value; }
        }
    }
}
