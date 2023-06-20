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
    public class VOBrokerServer : ConfigurationElement
    {
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


        [ConfigurationProperty("IP", IsKey = true, IsRequired = true)]
        public string BrokerServerIP
        {
            get { return (string)base["IP"]; }
            set { base["IP"] = value; }
        }

        [ConfigurationProperty("Port", IsRequired = true)]
        public string BrokerServerPort
        {
            get { return (string)base["Port"]; }
            set { base["Port"] = value; }
        }

        [ConfigurationProperty("Desc", IsRequired = true)]
        public string BrokerServerDesc
        {
            get { return (string)base["Desc"]; }
            set { base["Desc"] = value; }
        }

        [ConfigurationProperty("CreateTime", IsRequired = true)]
        public long CreateTime
        {
            get { return (long)base["CreateTime"]; }
            set { base["CreateTime"] = value; }
        }

        [ConfigurationProperty("LastConnectedTime", IsRequired = true)]
        public long LastConnectedTime
        {
            get { return (long)base["LastConnectedTime"]; }
            set { base["LastConnectedTime"] = value; }
        }

        [ConfigurationProperty("IsLastConnected", IsRequired = true)]
        public bool IsLastConnected
        {
            get { return (bool)base["IsLastConnected"]; }
            set { base["IsLastConnected"] = value; }
        }

        [ConfigurationProperty("IsGateway", IsRequired = true)]
        public bool IsGateway
        {
            get { return (bool)base["IsGateway"]; }
            set { base["IsGateway"] = value; }
        }
    }

	[Serializable()]
	public class VOBrokerServerNew
    {
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


		private string _brokerServerIP = String.Empty;
        public string BrokerServerIP
        {
			get { return _brokerServerIP; }
			set { _brokerServerIP = value; }
        }

		private string _strConfigIP = String.Empty;
		public string ConfigIP {
			get { return _strConfigIP; }
			set { _strConfigIP = value; }
		}
		private string _brokerServerPort = String.Empty;
        public string BrokerServerPort
        {
			get { return _brokerServerPort; }
			set { _brokerServerPort = value; }
        }

		private string _brokerServerDesc = String.Empty;
        public string BrokerServerDesc
        {
			get { return _brokerServerDesc; }
			set { _brokerServerDesc = value; }
        }

		private long _createTime = 0;
        public long CreateTime
        {
			get { return _createTime; }
			set { _createTime = value; }
        }

		private long _lastConnectedTime = 0;
        public long LastConnectedTime
        {
			get { return _lastConnectedTime; }
			set { _lastConnectedTime = value; }
        }

		private bool _isLastConnected = false;
		public bool IsLastConnected
        {
			get { return _isLastConnected; }
			set { _isLastConnected = value; }
        }

		private bool _isGateway = false;
        public bool IsGateway
        {
			get { return _isGateway; }
			set { _isGateway = value; }
        }

		private string _tag = String.Empty;
		public string tag {
			get { return _tag; }
			set { _tag = value; }
		}

		private string _EncodeUserID = String.Empty;
		public string UserID {
			get { return CryptoManager.DecodingBase64(_EncodeUserID); }
			set { _EncodeUserID = CryptoManager.EncodingBase64(value); }
		}

		private List<VOBrokerServerNew> _candidateServers = new List<VOBrokerServerNew>();
        public List<VOBrokerServerNew> CandidateServers
        {
            get { return _candidateServers; }
            set { _candidateServers = value; }
        }

        /// <summary>
        ///  From Server(Broker) Order, Convert TargetClientIP in Rage Rule
        ///  Regex string 
        /// </summary>
        private string _ConvertTargetClientIprangeRegex = "";
        public string ConvertIp_TargetClientIpRangeRule
        {
            set { _ConvertTargetClientIprangeRegex = value; }
            get { return _ConvertTargetClientIprangeRegex; }
        }
        private string _ConvertTargetBrokerIpSubnet = "";
        public string ConvertIp_TargetBrokerIpSubnet
        {
            set { _ConvertTargetBrokerIpSubnet = value; }
            get { return _ConvertTargetBrokerIpSubnet; }
        }
        private string _ConvertTargetDesktopVMIpSubnet = "";
        public string ConvertIp_TargetDesktopVMIpSubnet
        {
            set { _ConvertTargetDesktopVMIpSubnet = value; }
            get { return _ConvertTargetDesktopVMIpSubnet; }
        }
    }
}
