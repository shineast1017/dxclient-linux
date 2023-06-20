using HCVK.HCVKSLibrary;
using log4net;
using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKAService
{
    public class HCVKAFirewall
    {
        private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        // declare configure values
        private string _strInterfacePort = string.Empty;
        private string _strTestInterfacePort = string.Empty;
        private string _strVDIPort = string.Empty;
        private bool _bIsEnableTestInterfacePort = false;


        // declare firewall rules
        private FirewallManager _fmInterfacePort = new FirewallManager();
        private FirewallManager _fmTestInterfacePort = new FirewallManager();
        private FirewallManager _fmVDIPort = new FirewallManager();

        public HCVKAFirewall()
        {
            ReadConfiguration();

            InitializeFWInterfacePort();
            InitializeFWVDIPort();
        }


        ~HCVKAFirewall()
        {
            DeleteInterfacePort();
            DeleteVDIPort();
        }


        private void ReadConfiguration()
        {
            try
            {
                // read configure
                _strInterfacePort = new ConfigManager() { }.GetAppConfig("InterfacePort");
                _bIsEnableTestInterfacePort = bool.Parse(new ConfigManager() { }.GetAppConfig("TestInterfacePort"));
                _strTestInterfacePort = _bIsEnableTestInterfacePort ? string.Format("{0}", int.Parse(_strInterfacePort) + 1) : string.Empty;
                _strVDIPort = new ConfigManager() { }.GetAppConfig("VDIPort");
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        private void InitializeFWInterfacePort()
        {
            _fmInterfacePort.iNetFwRule.Name = Properties.Resources.FW_RULE_NAME_INTERFACE;
            _fmInterfacePort.iNetFwRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            _fmInterfacePort.iNetFwRule.LocalPorts = _strInterfacePort;
            _fmInterfacePort.iNetFwRule.Enabled = true;
            _fmInterfacePort.iNetFwRule.Grouping = Properties.Resources.FW_GROUP_ENAME;
            _fmInterfacePort.iNetFwRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
            _fmInterfacePort.iNetFwRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;

            if (_bIsEnableTestInterfacePort)
            {
                InitializeFWTestInterfacePort();
            }
        }

        private void InitializeFWTestInterfacePort()
        {
            _fmTestInterfacePort.iNetFwRule.Name = Properties.Resources.FW_RULE_NAME_TEST_INTERFACE;
            _fmTestInterfacePort.iNetFwRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            _fmTestInterfacePort.iNetFwRule.LocalPorts = _strTestInterfacePort;
            _fmTestInterfacePort.iNetFwRule.Enabled = true;
            _fmTestInterfacePort.iNetFwRule.Grouping = Properties.Resources.FW_GROUP_ENAME;
            _fmTestInterfacePort.iNetFwRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
            _fmTestInterfacePort.iNetFwRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
        }


        private void InitializeFWVDIPort()
        {
            _fmVDIPort.iNetFwRule.Name = Properties.Resources.FW_RULE_NAME_VDI;
            _fmVDIPort.iNetFwRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            _fmVDIPort.iNetFwRule.LocalPorts = _strVDIPort;
            _fmVDIPort.iNetFwRule.Enabled = true;
            _fmVDIPort.iNetFwRule.Grouping = Properties.Resources.FW_GROUP_ENAME;
            _fmVDIPort.iNetFwRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
            _fmVDIPort.iNetFwRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
        }



        public void AllowInterfacePort()
        {
            _fmInterfacePort.iNetFwRule.Description = "Allow inbound traffic TCP port " + _strInterfacePort;
            _fmInterfacePort.iNetFwRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            _fmInterfacePort.AddRule(true);

            if (_bIsEnableTestInterfacePort)
            {
                AllowTestInterfacePort();
            }
        }

        private void AllowTestInterfacePort()
        {
            _fmTestInterfacePort.iNetFwRule.Description = "Allow inbound traffic TCP port " + _strTestInterfacePort;
            _fmTestInterfacePort.iNetFwRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            _fmTestInterfacePort.AddRule(true);
        }

        public void BlockInterfacePort()
        {
            _fmInterfacePort.iNetFwRule.Description = "Block inbound traffic TCP port " + _strInterfacePort;
            _fmInterfacePort.iNetFwRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            _fmInterfacePort.AddRule(true);

            if (_bIsEnableTestInterfacePort)
            {
                BlockTestInterfacePort();
            }
        }

        private void BlockTestInterfacePort()
        {
            _fmTestInterfacePort.iNetFwRule.Description = "Allow inbound traffic TCP port " + _strTestInterfacePort;
            _fmTestInterfacePort.iNetFwRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            _fmTestInterfacePort.AddRule(true);
        }

        public void DeleteInterfacePort()
        {
            _fmInterfacePort.RemoveRule();

            if (_bIsEnableTestInterfacePort)
                DeleteTestInterfacePort();
        }

        private void DeleteTestInterfacePort()
        {
            _fmTestInterfacePort.RemoveRule();
        }


        public void AllowVDIPort()
        {
            _fmVDIPort.iNetFwRule.Description = "Allow inbound traffic TCP port " + _strVDIPort;
            _fmVDIPort.iNetFwRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            _fmVDIPort.AddRule(true);
        }

        public void BlockVDIPort()
        {
            _fmVDIPort.iNetFwRule.Description = "Block inbound traffic TCP port " + _strVDIPort;
            _fmVDIPort.iNetFwRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            _fmVDIPort.AddRule(true);
        }

        public void DeleteVDIPort()
        {
            _fmVDIPort.RemoveRule();
        }
    }
}
