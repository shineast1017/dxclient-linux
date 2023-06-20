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
}
