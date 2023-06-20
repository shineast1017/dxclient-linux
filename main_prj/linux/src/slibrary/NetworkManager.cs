using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
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

                    if (adapter.Description == "lo")
                    {
                        continue; // skip local lo 
                    }

                    //  adapter status unknown is not down
                    if (adapter.OperationalStatus == OperationalStatus.Up || adapter.OperationalStatus == OperationalStatus.Unknown)
                    //if (adapter.OperationalStatus == OperationalStatus.Up && (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    //     adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
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
#if WIN32
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
#else
				NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    //if (adapter.OperationalStatus == OperationalStatus.Up && (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        foreach (var address in properties.UnicastAddresses)
                        {
                            if (address.Address.AddressFamily.ToString().Equals(ProtocolFamily.InterNetwork.ToString()))
                            {
								if(address.Address.ToString() == strIP)
								{
									PhysicalAddress physicaladdr = adapter.GetPhysicalAddress();
									return string.Join(":", physicaladdr.GetAddressBytes().Select(p => p.ToString("X2")));
								}
                            }
                        }
                    }
                }
#endif
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


        /// <summary>
        /// LocalIP more  pinpoint style method
        /// try connect target dest server/ip/port
        /// get localip from Connect_tryinfo
        /// </summary>
        /// <param name="deskIP"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static string GetLocalIP_FromTargetDestIP_TryConnectInfo(string destIP, string port)
        {
            string ret = "";
            if (destIP.Count() < 1 || port.Count() < 1)
                return ret; // error


            int destPort = Int32.Parse(port);
            if (destPort > 65535) // get over the port range
                return ret;  // error

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {

                socket.Connect(destIP, destPort);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                ret = endPoint.Address.ToString(); // ret localip

            }

            return ret;
        }
    }
}
