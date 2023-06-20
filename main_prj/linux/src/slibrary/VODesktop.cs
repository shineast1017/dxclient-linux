using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
    public class VODesktop
    {
        // declare for power command
        public static string POWER_DESKTOP_UNSTATE = "Unknown";
        public static string POWER_DESKTOP_ON = "START";
        public static string POWER_DESKTOP_OFF = "STOP";
        public static string POWER_DESKTOP_SUSPEND = "SUSPEND";
        public static string POWER_DESKTOP_RESUME = "RESUME";
        public static string POWER_DESKTOP_PAUSE = "PAUSE";
        public static string POWER_DESKTOP_UNPAUSE = "UNPAUSE";
        public static string POWER_DESKTOP_RESET = "RESET";
        public static string POWER_GUEST_SHUTDOWN = "SHUTDOWN";
        public static string POWER_GUEST_REBOOT = "REBOOT";

        // declare for desktop status
        public static string STATUS_ACTIVE = "ACTIVE";
        public static string STATUS_SHUTOFF = "SHUTOFF";
        public static string STATUS_PAUSED = "PAUSED";
        public static string STATUS_SUSPENDED = "SUSPENDED";

        // declare for desktop current state
        public static string DESKTOP_CURRENT_STATE_READY = "READY";
        public static string DESKTOP_CURRENT_STATE_POWERTASKING = "POWERTASKING";
        public static string DESKTOP_CURRENT_STATE_SHUTOFF = "SHUTOFF";
        public static string DESKTOP_CURRENT_STATE_PAUSED = "PAUSED";
        public static string DESKTOP_CURRENT_STATE_SUSPENDED = "SUSPENDED";
        public static string DESKTOP_CURRENT_STATE_CONNECTED = "CONNECTED";
        public static string DESKTOP_CURRENT_STATE_PROVISIONING = "PROVISIONING";
        public static string DESKTOP_CURRENT_STATE_UNKNOWN = "UNKNOWN";
		public static string DESKTOP_CURRENT_STATE_TASKING = "TASKING"; 
		public static string DESKTOP_CURRENT_STATE_READYANDAGENTOFF = "WAITING TO BE READY";

		public static string DESKTOP_AGENT_STATE_OK = "0";
		public static string DESKTOP_AGENT_STATE_SYSTEM_ERROR = "A0000001";
		public static string DESKTOP_AGENT_STATE_WAIT_REG = "A0000101";
		public static string DESKTOP_AGENT_STATE_WAIT_REREG = "A0000102";
		public static string DESKTOP_AGENT_STATE_RDP_ERROR = "A0000103";
		public static string DESKTOP_AGENT_STATE_SHUTDOWN = "A0000104";
		public static string DESKTOP_AGENT_STATE_INTERGRITY = "A0000201";
		public static string DESKTOP_AGENT_STATE_CHECKING = "A0000202";

		public static string DESKTOP_SESSION_CONNECTED_TRUE = "TRUE";
		public static string DESKTOP_SESSION_CONNECTED_FALSE = "FALSE";


				private string _strDesktopIP = string.Empty;
        private string _strDesktopName = string.Empty;
        private string _strDesktopState = string.Empty;
        private string _strDesktopID = string.Empty;
        private string _strDesktopFQDN = string.Empty;
        private string _strInstanceID = string.Empty;
        private string _strCurrentState = string.Empty;
        private string _strStatus = string.Empty;
        private string _strPowerState = "0";
        private string _strVMState = string.Empty;
        private string _strAgentState = string.Empty;
        private List<VOProtocol> _listProtocol = new List<VOProtocol>();
        private bool _bIsEnabled = false;
		private List<VODesktopPolicies> _listPolicies = new List<VODesktopPolicies>();
		private List<VODesktopTemplate> _listTemplate = new List<VODesktopTemplate>();
        private string _strSessionconnected = string.Empty;

        public string DesktopIP
        {
            set { _strDesktopIP = value; }
            get { return _strDesktopIP; }
        }
        public string DesktopName
        {
            set { _strDesktopName = value; }
            get { return _strDesktopName; }
        }
        public string DesktopState
        {
            set { _strDesktopState = value; }
            get { return _strDesktopState; }
        }
        public string DesktopID
        {
            set { _strDesktopID = value; }
            get { return _strDesktopID; }
        }
        public string DesktopFQDN
        {
            set { _strDesktopFQDN = value; }
            get { return _strDesktopFQDN; }
        }
        public string InstanceID
        {
            set { _strInstanceID = value; }
            get { return _strInstanceID; }
        }
        public string CurrentState
        {
            set { _strCurrentState = value; }
            get { return _strCurrentState; }
        }
        public string Status
        {
            set { _strStatus = value; }
            get { return _strStatus; }
        }
        public string PowerState
        {
            set { _strPowerState = value; }
            get { return _strPowerState; }
        }
        public string PowerStateText
        {
            get { return GetPowerTextFromPowerState(_strPowerState); }
        }
        public string VMState
        {
            set { _strVMState = value; }
            get { return _strVMState; }
        }
        public string AgentState
        {
            set { _strAgentState = value; }
            get { return _strAgentState; }
        }
        public string AgentStateText
        {
            get { return GetAgentTextFromAgentState(_strAgentState); }
        }
        public List<VOProtocol> Protocols
        {
            set { _listProtocol = value; }
            get { return _listProtocol; }
        }
        public bool IsEnabled
        {
            set { _bIsEnabled = value; }
            get { return _bIsEnabled; }
        }
		public List<VODesktopTemplate> Templates
        {
            set { _listTemplate = value; }
            get { return _listTemplate; }
        }
		public List<VODesktopPolicies> Policies
        {
            set { _listPolicies = value; }
            get { return _listPolicies; }
        }

        public string Sessionconnected
        {
            set { _strSessionconnected = value; }
            get { return _strSessionconnected; }
        }

        private static string GetPowerTextFromPowerState(string strPowerState)
        {
            string strPowerText = POWER_DESKTOP_UNSTATE;

            switch (Convert.ToInt32(strPowerState))
            {
                case 0:
                    strPowerText = POWER_DESKTOP_UNSTATE;
                    break;
                case 1:
                    strPowerText = POWER_DESKTOP_ON;
                    break;
                case 3:
                    strPowerText = POWER_DESKTOP_PAUSE;
                    break;
                case 4:
                    strPowerText = POWER_DESKTOP_OFF;
                    break;
                case 7:
                    strPowerText = POWER_DESKTOP_SUSPEND;
                    break;
            }


            return strPowerText;
        }

        private static string GetAgentTextFromAgentState(string strAgentState)
        {
            string strAgentText = VOErrorCode._E_MSG_CHECKING;

            if (strAgentState.Equals(VOErrorCode._E_CODE_OK))
            {
                strAgentText = VOErrorCode._E_MSG_OK;
            }

            return strAgentText;
        }
    }
}
