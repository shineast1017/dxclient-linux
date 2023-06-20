using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HCVK.HCVKAService
{
	public class HCVKARDPService
	{
		private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private const string DEFAULT_RDP_PORT = "3389";

		private static void SetRDPService(bool bEnable)
		{
#if WIN32
			try
			{
				RegistryKey fDenyTSConnections = Registry.LocalMachine.OpenSubKey(Properties.Resources.REG_RDP_SUBKEY, true);

				if (fDenyTSConnections != null)
				{
					//_logger.Debug(string.Format("Before Set Value: DenyTSConnection = {0}", fDenyTSConnections.GetValue("fDenyTSConnections").ToString()));
					fDenyTSConnections.SetValue("fDenyTSConnections", (!bEnable ? "1" : "0"), RegistryValueKind.DWord);
					_logger.Debug(string.Format("Current Set Value: DenyTSConnection = {0}", fDenyTSConnections.GetValue("fDenyTSConnections").ToString()));

					fDenyTSConnections.Close();
					Thread.Sleep(100);
				}
				else
				{
					_logger.Debug(string.Format("There is no key: fDenyTSConnections"));
				}
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception: {0}", ex.Message));
			}
#else
			// 1st Approach: Start/Stop xrdp Service - sudo systemctl start/stop xrdp

			// 2nd Approach: Allow/Disallow rdp port - sudo ufw allow/deny port/tcp (ubuntu), sudo firewall-cmd --permanent --add-port=port/tcp
#endif
		}


		public static void EnableRDPService()
		{
			SetRDPService(true);
		}


		public static void DisableRDPService()
		{
			SetRDPService(false);
		}



		private static string GetRDPPort()
		{
			string strRDPPort = string.Empty;
			try
			{
				RegistryKey portNumber = Registry.LocalMachine.OpenSubKey(Properties.Resources.REG_RDP_PORT_SUBKEY, true);

				if (portNumber != null)
				{
					strRDPPort = portNumber.GetValue("PortNumber").ToString();
					portNumber.Close();
				}
				else
				{
					_logger.Debug(string.Format("There is no key: PortNumber"));
				}
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception: {0}", ex.Message));
			}

			return strRDPPort;
		}
		private static void SetRDPPort(string strRDPPort)
		{
			try
			{
				RegistryKey portNumber = Registry.LocalMachine.OpenSubKey(Properties.Resources.REG_RDP_PORT_SUBKEY, true);

				if (portNumber != null)
				{
					portNumber.SetValue("PortNumber", Int32.Parse(strRDPPort), RegistryValueKind.DWord);
					_logger.Debug(string.Format("Current Set Value: PortNumber = {0}", portNumber.GetValue("PortNumber").ToString()));

					portNumber.Close();
					Thread.Sleep(100);
				}
				else
				{
					_logger.Debug(string.Format("There is no key: PortNumber"));
				}
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception: {0}", ex.Message));
			}
		}

		public static void SetVDIPort(string strVDIPort)
		{
#if WIN32
			if (strVDIPort.Equals(DEFAULT_RDP_PORT))
                return;

            SetRDPPort(strVDIPort);

            // rdp service restart
            RestartWindowsRDPService();
#endif
		}

		public static void SetDefaultRDPPort()
		{

#if WIN32
            if (GetRDPPort().Equals(DEFAULT_RDP_PORT))
                return;


            SetRDPPort(DEFAULT_RDP_PORT);

            // rdp service restart
            RestartWindowsRDPService();
#endif
		}


		public static void RestartWindowsRDPService()
		{
#if WIN32
			try
            {
                ServiceController serviceRDP = new ServiceController(Properties.Resources.WINDOWS_RDP_SERVICE_NAME);

                serviceRDP.Refresh();
                _logger.Debug(string.Format("Check to RDP Service Status:{0}", serviceRDP.Status.ToString()));


                if (serviceRDP.Status == ServiceControllerStatus.Running)
                {
                    // start service of rdp usermode port redirection
                    ServiceController serviceUmRdp = new ServiceController(Properties.Resources.WINDOWS_USER_MODE_RDP_SERVICE_NAME);
                    serviceUmRdp.Refresh();
                    _logger.Debug(string.Format("Check to RDP Usermode Service Status:{0}", serviceUmRdp.Status.ToString()));
                    if (serviceUmRdp.Status != ServiceControllerStatus.Running)
                    {
                        serviceUmRdp.Stop();
                        Thread.Sleep(500);
                        serviceUmRdp.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(10000));
                    }

                    // start service of rdp configuration
                    ServiceController serviceSessionEnv = new ServiceController(Properties.Resources.WINDOWS_SESSION_RDP_SERVICE_NAME);
                    serviceSessionEnv.Refresh();
                    _logger.Debug(string.Format("Check to RDP Session Service Status:{0}", serviceSessionEnv.Status.ToString()));
                    if (serviceSessionEnv.Status != ServiceControllerStatus.Running)
                    {
                        serviceSessionEnv.Stop();
                        Thread.Sleep(500);
                        serviceSessionEnv.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(10000));
                    }

                    serviceRDP.Stop();
                    Thread.Sleep(500);
                    serviceRDP.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(10000));

                    serviceUmRdp.Refresh();
                    _logger.DebugFormat("Stop to RDP Service..Status[{0}]", serviceRDP.Status.ToString());
                }

                {
                    _logger.DebugFormat("Start to RDP Service Group..");

                    serviceRDP.Start();
                    Thread.Sleep(500);
                    serviceRDP.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(10000));

                    // start service of rdp usermode port redirection
                    ServiceController serviceUmRdp = new ServiceController(Properties.Resources.WINDOWS_USER_MODE_RDP_SERVICE_NAME);
                    serviceUmRdp.Refresh();
                    if (serviceUmRdp.Status != ServiceControllerStatus.Running)
                    {
                        serviceUmRdp.Start();
                        Thread.Sleep(500);
                        serviceUmRdp.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(10000));
                    }

                    // start service of rdp configuration
                    ServiceController serviceSessionEnv = new ServiceController(Properties.Resources.WINDOWS_SESSION_RDP_SERVICE_NAME);
                    serviceSessionEnv.Refresh();
                    if (serviceSessionEnv.Status != ServiceControllerStatus.Running)
                    {
                        serviceSessionEnv.Start();
                        Thread.Sleep(500);
                        serviceSessionEnv.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(10000));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception: {0}", ex.Message));
            }
#endif
		}


		public static bool CheckAndStartWindowsRDPService()
		{
#if WIN32
			bool bRunningRDP = false;

            try
            {
                ServiceController serviceRDP = new ServiceController(Properties.Resources.WINDOWS_RDP_SERVICE_NAME);

                serviceRDP.Refresh();
                _logger.Debug(string.Format("Check to RDP Service Status:{0}", serviceRDP.Status.ToString()));


                if (serviceRDP.Status == ServiceControllerStatus.Running)
                {
                    bRunningRDP = true;
                }
                else
                {
                    _logger.DebugFormat("Start to RDP Service Group..");

                    serviceRDP.Start();
                    Thread.Sleep(500);
                    serviceRDP.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(10000));

                    // start service of rdp usermode port redirection
                    ServiceController serviceUmRdp = new ServiceController(Properties.Resources.WINDOWS_USER_MODE_RDP_SERVICE_NAME);
                    serviceUmRdp.Refresh();
                    if (serviceUmRdp.Status != ServiceControllerStatus.Running)
                    {
                        serviceUmRdp.Start();
                        Thread.Sleep(500);
                        serviceUmRdp.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(10000));
                    }

                    // start service of rdp configuration
                    ServiceController serviceSessionEnv = new ServiceController(Properties.Resources.WINDOWS_SESSION_RDP_SERVICE_NAME);
                    serviceSessionEnv.Refresh();
                    if (serviceSessionEnv.Status != ServiceControllerStatus.Running)
                    {
                        serviceSessionEnv.Start();
                        Thread.Sleep(500);
                        serviceSessionEnv.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(10000));
                    }

                    bRunningRDP = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception: {0}", ex.Message));
                bRunningRDP = false;
            }

            return bRunningRDP;
#else
			return true;
#endif
		}
    }
}
