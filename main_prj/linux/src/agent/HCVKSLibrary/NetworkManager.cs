using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary
{
    public class NetworkManager
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        public static Dictionary<string, string> GetNetworkAdaptor()
        {
            Dictionary<string, string> mapIPAddress = new Dictionary<string, string>();
            try
            {
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    if (adapter.OperationalStatus == OperationalStatus.Up && (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        foreach (var address in properties.UnicastAddresses)
                        {
                            if (address.Address.AddressFamily.ToString().Equals(ProtocolFamily.InterNetwork.ToString()))
                            {
                                _logger.Debug(string.Format("{0}[{1}]", adapter.Name, address.Address.ToString()));
                                mapIPAddress.Add(adapter.Name, address.Address.ToString());
                            }
                        }
                    }
                }
            }
            catch (ArgumentNullException exArgNull)
            {
                _logger.Error(string.Format("ArgException: {0}", exArgNull.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
            return mapIPAddress;
        }


        public static string GetMacAddressFromIP(string strIP)
        {
            string strMACAddress = string.Empty;
            try
            {
                if (strIP == null || strIP.Trim().Length == 0)
                {
                    throw new ArgumentNullException("strIP");
                }


                string strQuery = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled='TRUE'";
                ObjectQuery objectQuery = new System.Management.ObjectQuery(strQuery);
                ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(objectQuery);
                foreach (ManagementObject obj in searcher.Get())
                {
                    string[] ipAddress = (string[])obj["IPAddress"];

                    if (obj["MACAddress"] != null)
                    {
                        // gotcha..
                        strMACAddress = obj["MACAddress"].ToString();
                        _logger.Debug(string.Format("{0}[{1}]", strIP, strMACAddress));
                        break;
                    }
                }
            }
            catch (ArgumentNullException exArgNull)
            {
                _logger.Error(string.Format("ArgException: {0}", exArgNull.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
            return strMACAddress;
        }
    }
}
