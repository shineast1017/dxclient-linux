using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
    public class VOProtocol
    {
        public class VOProtocolType
        {
            public const char PROTOCOL_SEPARATOR = '|';

            public const string PROTOCOL_RDP = "RDP";
            public const string PROTOCOL_SPICE = "SPICE";
        }

        private string _strProtocolType = string.Empty;
        private string _strProtocolIP = string.Empty;
        private string _strProtocolPort = string.Empty;


        public string ProtocolType
        {
            set { _strProtocolType = value; }
            get { return _strProtocolType; }
        }
        public string ProtocolIP
        {
            set { _strProtocolIP = value; }
            get { return _strProtocolIP; }
        }
        public string ProtocolPort
        {
            set { _strProtocolPort = value; }
            get { return _strProtocolPort; }
        }
    }
}
