using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ref.
// os version
// https://stackoverflow.com/questions/21737985/windows-version-in-c-sharp
//


namespace HCVK.HCVKSLibrary
{
	public class CommonUtils
	{



            /// <summary>
            /// extention  ConvertIpRecommendServerAddIpRuleToBrokerIp
            /// </summary>
            /// <param name=""></param>
            /// <param name="regx"></param>
            /// <param name="client_ip"></param>
            /// <param name="ip_subnet"></param>
            /// <returns></returns>
            public static string ConvertIpRecommendServerAddIpRuleToVM_Ip(string ip, string regx, string client_ip, string ip_subnet)
            {
              return ConvertIpRecommendServerAddIpRuleToBrokerIp(ip, regx, client_ip, ip_subnet);
            }

            /// <summary>
            /// Convert Ip Recommend Server AddIpRule To Broker Ip
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="regx"></param>
            /// <param name="client_ip"></param>
            /// <param name="ip_subnet"></param>
            /// <returns></returns>
            public static string ConvertIpRecommendServerAddIpRuleToBrokerIp(string ip, string regx, string client_ip, string ip_subnet)
            {
              if (ip.Length < 1 || ip_subnet.Length < 1 || client_ip.Length < 1)
                return "";

              //  apply ip Convert rule . not range
              if (CommonUtils.CheckConditionInRangeIpByRegx(client_ip, regx) == false)
              {
                return ip;
              }

              // in range
              string ret_ip = CommonUtils.Convert_IpRule_ipV4(ip, ip_subnet);
              return ret_ip;
            }

            /// <summary>
            /// target ip in ragne regx in format
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="regexFormat"></param>
            /// <returns></returns>
            public static bool CheckConditionInRangeIpByRegx(string ip, string regexFormat)
            {
              if (ip.Length < 1 || regexFormat.Length < 1)
                return false;

              //String ipstring = "10.11.3.100"; //10.11.3.100~110 
              //Regex regex = new Regex(@"^10.11.3.1((0[0-9]{1})|(10))$");

              Regex regex = new Regex(regexFormat);
              if (regex.IsMatch(ip))
              {
                Console.WriteLine(" CheckCondition Inrange ip Match");
                return true;
              }

              Console.WriteLine("not Match");
              return false;
            }


            //     public String Convert_IpRule(string ipConvertRuleRex, string srcIp, string change)
            public static string Convert_IpRule_ipV4(string srcIp, string destIpSubnet)
            {
              if (srcIp.Length < 1 || destIpSubnet.Length < 1)
                return ""; // error

              string []srcIps  = srcIp.Split('.');
              string []destsubIps = destIpSubnet.Split('.');

              if (srcIps.Count() < 4 || destsubIps.Count() < 4)
                return ""; // error


              string changeIp = "";
              for (int idx = 0; idx < destsubIps.Count(); idx++)
              {

                if (destsubIps[idx].Contains("255") == true) {  // not change
                  changeIp += srcIps[idx]; // append ori ip
                  changeIp += "."; // append . 
                } else // change
                {
                  changeIp += destsubIps[idx]; // append change ip
                  changeIp += "."; // append . 
                }
              }

              // remove last comma
              changeIp = changeIp.Remove(changeIp.Length - 1);

              Console.WriteLine("Convert_IpRule_ipV4 ret :" + changeIp);

              return changeIp;
            }




		public static long DateTimeToUnixTime(DateTime dateTime)
		{
			DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

			return (long)(dateTime.ToUniversalTime() - sTime).TotalMilliseconds;
		}
		public static DateTime UnixTimeStampToDateTime(long lUnixTimeStamp)
		{
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return dtDateTime.AddMilliseconds(lUnixTimeStamp).ToLocalTime();
		}
        public static string GetPrinterNames()
        {
            string strReturn = "";
            string strResultCmd = "";

            ProcessManager pm = new ProcessManager();

            try
            {
                strResultCmd = pm.ExecuteBashCommand("lpstat -a");
                string[] strPriterStats = strResultCmd.Split('\n');
                string strTemp = "";
                for (int i = 0; i < strPriterStats.Length-1; i++)
                {
                    strTemp = strPriterStats[i];
                    string[] strPrinterInfos = strTemp.Split(' ');
                    strReturn += strPrinterInfos[0];

                    if (i < strPriterStats.Length - 2)
                    {
                        strReturn += ",";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("execute execption, command : lpstat, more detail :" + e.ToString());
            }


            return strReturn;
        }
		public static string GetWebCamUsbDeviceForAllDevice()
		{
			string strReturn = "";
			string strResultCmd = "";

			ProcessManager pm = new ProcessManager ();

			try {
				strResultCmd = pm.ExecuteBashCommand ("lsusb | cut -d' ' -f6");
				string [] strUsbDeviceStats = strResultCmd.Split ('\n');
				string strTemp = "";
				for (int i = 0; i < strUsbDeviceStats.Length - 1; i++) {
					strTemp = strUsbDeviceStats [i];
					strResultCmd = pm.ExecuteBashCommand ("lsusb -d " + strTemp + " -v");
					if(strResultCmd.Contains("Video Streaming")) {
						strReturn = strTemp;
						break;
					}

				}
			} catch (Exception e) {
				Console.WriteLine ("execute execption, command : lpusb -d -v, more detail :" + e.ToString ());
			}

			return strReturn;
		}
		// 프로세스 pid를 이용하여, 윈도우의 windowID를 가져옵니다. (동일 프로세스에 2개이상의 윈도우가 발생할 수 있어서 특정 윈도우의 windowID를 가져오게 필터를 추가합니다.
		public static string GetWindowIDFromPID (int pid, string ExcludeWord = "")
		{
			string strReturn = "";
			string strResultCmd = "";

			ProcessManager pm = new ProcessManager ();

			try {
				strResultCmd = pm.ExecuteBashCommand ("wmctrl -l -p");
				string [] strWindowAllInfos = strResultCmd.Split ('\n');
				for (int i = 0; i < strWindowAllInfos.Length - 1; i++) {
					string [] strSplitWindowInfo = strWindowAllInfos [i].Split (' ');
					/// 4번째 항목이 pid 입니다. 
					if (strSplitWindowInfo[3].Contains (pid.ToString ()) == true) {
						// pid 가 맞고, 1) excludeWord가 없거나, 있다면 excludeWord가 제외된 pid의 윈도우id를 리턴합니다
						if (ExcludeWord == "" || (ExcludeWord != "" && strWindowAllInfos [i].Contains (ExcludeWord) == false)) {
							strReturn = strSplitWindowInfo [0];
							break;
						} else {
							Console.WriteLine ("Exclude Word Catch!! " + ExcludeWord);

						}
					}
				}
			} catch (Exception e) {
				Console.WriteLine ("execute execption, command : wmctrl, more detail :" + e.ToString ());
			}

			return strReturn;
		}

		// 창의 윈도우 id를 가져옵니다.
		public static string GetWindowID(int pid)
		{
			string strWindowID = "";
			string strShCmd = string.Format ("xdotool search --pid {0}",pid);
			ProcessManager pm = new ProcessManager ();
			try {
				strWindowID = pm.ExecuteBashCommand (strShCmd);
				strWindowID = strWindowID.TrimEnd ('\n', '\r');
			} catch (Exception e) {
				Console.WriteLine ("execute execption, command : xdotool, more detail :" + e.ToString ());
			}
			return strWindowID;
		}
		// 프로세스 pid를 이용하여, 창의 이름을 변경합니다.
		public static void SetWindowTitleWithWMCTRL (int pid, string sTitle)
		{
			string sWindowID = CommonUtils.GetWindowID (pid);
			string strShCmd = string.Format ("wmctrl -i -r {0} -T \'{1}\'", sWindowID, sTitle);
			ProcessManager pm = new ProcessManager ();
			try {
				pm.ExecuteBashCommand (strShCmd);
			} catch (Exception e) {
				Console.WriteLine ("execute execption, command : wmctrl, more detail :" + e.ToString ());
			}

		}
		// 윈도우 id를 이용하여, 창의 이름을 변경합니다. wmctrl 패키지를 이용합니다
		public static void SetWindowTitleWithWMCTRL (string sWindowID, string sTitle)
		{
			string strShCmd = string.Format ("wmctrl -i -r {0} -T \'{1}\'", sWindowID, sTitle);
			ProcessManager pm = new ProcessManager ();
			try {
				pm.ExecuteBashCommand (strShCmd);
			} catch (Exception e) {
				Console.WriteLine ("execute execption, command : wmctrl, more detail :" + e.ToString ());
			}

		}

		public static string GetWebCamUsbDeviceId()
        {
            string strWebCamDeviceID = "";
            string strShCmd = "";
            string strshFilePath = "/usr/lib/DaaSXpertClient/" +System.Configuration.ConfigurationManager.AppSettings["GetWebCamDeviceIdFileName"];

			strShCmd = ReadTextFromFile (strshFilePath);
			if (strShCmd == "") {
				strShCmd = "lsusb | grep Camera | cut -d' ' -f6";
			}


            ProcessManager pm = new ProcessManager();

            try
            {
                strWebCamDeviceID = pm.ExecuteBashCommand(strShCmd);
                strWebCamDeviceID = strWebCamDeviceID.TrimEnd('\n', '\r');
            }
            catch (Exception e)
            {
                Console.WriteLine("execute execption, command : lsusb, more detail :" + e.ToString());
            }
			return strWebCamDeviceID;
        }
		/*
		usbip list -l 실행후 결과는 아래와 같습니다. devid와 일치하는 장치의 busid를 추출합니다.
		- busid 2-1 (80ee:0021)
		VirtualBox : USB Tablet (80ee:0021)
		*/
		public static string GetWebCamDeviceBusId (string strDeviceId, string strUsbIpPath)
		{
			string strWebCamDeviceBusId = "";

			if (strDeviceId == "")
				return strWebCamDeviceBusId;

			ProcessManager pm = new ProcessManager ();
			string sTemp = "";
			string strWebCamDeviceID = strDeviceId;

			try {
				if(strUsbIpPath == "") {
					sTemp = pm.ExecuteBashCommand ("usbip list -l");
				} else {
					sTemp = pm.ExecuteBashCommand (strUsbIpPath + "/usbip list -l");
				}

			} catch (Exception e) {
				Console.WriteLine ("execute execption, command : usbip, more detail :" + e.ToString ());
				return "Error :" + e.ToString();
			}

			string [] strUsbipListStats = sTemp.Split ('\n');
			string strTemp = "";
			bool bRequestStop = false;

			try {
				for (int i = 0; i < strUsbipListStats.Length; i++) {
					strTemp = strUsbipListStats [i];

					if (strTemp.Contains (strWebCamDeviceID) && strTemp.Contains ("busid")) {

						string [] strUsbInfos = strTemp.Split (' '); // busid

						for (int j = 0; j < strUsbInfos.Length; j++) {
							if (strUsbInfos [j].Contains ("busid") == true) {
								strWebCamDeviceBusId = strUsbInfos [j + 1];
								strWebCamDeviceBusId = strWebCamDeviceBusId.TrimEnd ();
								strWebCamDeviceBusId = strWebCamDeviceBusId.TrimStart ();
								bRequestStop = true;
								break;
							}
						}
					}

					if (bRequestStop == true)
						break;
				}

			} catch (Exception e) {
				Console.WriteLine ("execute execption, GetWebCamDeviceBusId(), more detail :" + e.ToString ());
			}

			return strWebCamDeviceBusId;
		}

		public static string ReadTextFromFile(string strPath, bool bRefine = false)
		{
			string strReturn = "";

			try {
				if (System.IO.File.Exists (strPath) == true) {
					strReturn = System.IO.File.ReadAllText (strPath);

					if(bRefine) {
						strReturn = strReturn.Replace ("\n", "");
						strReturn = strReturn.Replace ("\r", "");
						strReturn = strReturn.Trim ();
					}

				}

			} catch (Exception e) {
				Console.WriteLine ("execute execption, ReadTextFromFile(), more detail :" + e.ToString ());
			}

			return strReturn;

		}
		// process로 실행한 flatpak 프로그램을 pid로 검색해서, 프로그램을 종료합니다.
		// 	flatpak ps ex) "1210192721\t32744\tcom.thincast.client\torg.kde.Platform\n"
		public static void KillflatpakApplication(int pid)
		{
			ProcessManager pm = new ProcessManager ();
			string sTemp = "";

			try {
				sTemp = pm.ExecuteBashCommand ("flatpak ps --columns=instance,pid ");
				// \n 구분.. \t 구분 pid 확인-> 종료 
				string [] arrSplitStdout = sTemp.Split ('\n');
				for (int i = 0; i < arrSplitStdout.Length; i++) {
					if (arrSplitStdout [i] == "") continue;

					string [] arrSplitColumns = arrSplitStdout [i].Split ('\t');

					if (arrSplitColumns [1].Equals (pid.ToString ())) {
						sTemp = pm.ExecuteBashCommand (string.Format ("flatpak kill {0} ", arrSplitColumns [0]));
						break;
					}
				}

			} catch (Exception e) {
				Console.WriteLine ("execute execption, KillflatpakApplication(), more detail :" + e.ToString ());
			}


		}


		public static string GetPlatfromUUID ()
		{
			string strReturn = "";

			ProcessManager pm = new ProcessManager ();

			try {
				strReturn = pm.ExecuteBashCommand ("gooroom-system-uuid-helper");

				strReturn = strReturn.Replace ("\n", "");
				strReturn = strReturn.Replace ("\r", "");
				strReturn = strReturn.Trim ();

			} catch (Exception e) {
				Console.WriteLine ("execute execption, command : gooroom-system-uuid-helper, more detail :" + e.ToString ());
			}

			return strReturn;
		}


		public static string GetUserName()
		{
			string strReturn = "";

			ProcessManager pm = new ProcessManager ();

			try {
				strReturn = pm.ExecuteBashCommand ("whoami");

				strReturn = strReturn.Replace ("\n", "");
				strReturn = strReturn.Replace ("\r", "");
				strReturn = strReturn.Trim ();

			} catch (Exception e) {
				Console.WriteLine ("execute execption, command : whoami, more detail :" + e.ToString ());
			}

			if(strReturn == "") {
				strReturn = (System.Environment.UserName != "") ?
System.Environment.UserName : (Environment.GetEnvironmentVariable ("USER") != "") ? Environment.GetEnvironmentVariable ("USER") : Environment.GetEnvironmentVariable ("USERNAME");

			}

			return strReturn;
		}




#if WIN32
		public static string GetOSVersion()
		{
			//+--------------------------------------------------------------------------------------------------------------------------------------------------------------------------+
			//|            | Windows      | Windows      | Windows      | Windows NT | Windows | Windows | Windows | Windows | Windows | Windows | Windows | Windows | Windows | Windows |
			//|            | 95           | 98           | Me           | 4.0        | 2000    | XP      | 2003    | Vista   | 2008    | 7       | 2008 R2 | 8       | 8.1     | 10      |
			//+--------------------------------------------------------------------------------------------------------------------------------------------------------------------------+
			//| PlatformID | Win32Windows | Win32Windows | Win32Windows | Win32NT    | Win32NT | Win32NT | Win32NT | Win32NT | Win32NT | Win32NT | Win32NT | Win32NT | Win32NT | Win32NT |
			//+--------------------------------------------------------------------------------------------------------------------------------------------------------------------------+
			//| Major      |              |              |              |            |         |         |         |         |         |         |         |         |         |         |
			//| version    | 4            | 4            | 4            | 4          | 5       | 5       | 5       | 6       | 6       | 6       | 6       | 6       | 6       | 10      |
			//+--------------------------------------------------------------------------------------------------------------------------------------------------------------------------+
			//| Minor      |              |              |              |            |         |         |         |         |         |         |         |         |         |         |
			//| version    | 0            | 10           | 90           | 0          | 0       | 1       | 2       | 0       | 0       | 1       | 1       | 2       | 3       | 0       |
			//+--------------------------------------------------------------------------------------------------------------------------------------------------------------------------+


			return string.Format("{0}.{1}.{2}", ComputerInfo.WinMajorVersion, ComputerInfo.WinMinorVersion, ComputerInfo.IsServer.Equals("1") ? "Server" : "Desktop");
		}

		public static class ComputerInfo
		{
			/// <summary>
			///     Returns the Windows major version number for this computer.
			/// </summary>
			public static UInt32 WinMajorVersion
			{
				get
				{
					dynamic major;
					// The 'CurrentMajorVersionNumber' string value in the CurrentVersion key is new for Windows 10, 
					// and will most likely (hopefully) be there for some time before MS decides to change this - again...
					if (TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", out major))
					{
						return (UInt32)major;
					}

					// When the 'CurrentMajorVersionNumber' value is not present we fallback to reading the previous key used for this: 'CurrentVersion'
					dynamic version;
					if (!TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out version))
						return 0;

					var versionParts = ((string)version).Split('.');
					if (versionParts.Length != 2) return 0;
					UInt32 majorAsUInt;
					return UInt32.TryParse(versionParts[0], out majorAsUInt) ? majorAsUInt : 0;
				}
			}

			/// <summary>
			///     Returns the Windows minor version number for this computer.
			/// </summary>
			public static UInt32 WinMinorVersion
			{
				get
				{
					dynamic minor;
					// The 'CurrentMinorVersionNumber' string value in the CurrentVersion key is new for Windows 10, 
					// and will most likely (hopefully) be there for some time before MS decides to change this - again...
					if (TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMinorVersionNumber",
						out minor))
					{
						return (UInt32)minor;
					}

					// When the 'CurrentMinorVersionNumber' value is not present we fallback to reading the previous key used for this: 'CurrentVersion'
					dynamic version;
					if (!TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out version))
						return 0;

					var versionParts = ((string)version).Split('.');
					if (versionParts.Length != 2) return 0;
					uint minorAsUInt;
					return uint.TryParse(versionParts[1], out minorAsUInt) ? minorAsUInt : 0;
				}
			}

			/// <summary>
			///     Returns whether or not the current computer is a server or not.
			/// </summary>
			public static UInt32 IsServer
			{
				get
				{
					dynamic installationType;
					if (TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "InstallationType",
						out installationType))
					{
						return (UInt32)(installationType.Equals("Client") ? 0 : 1);
					}

					return 0;
				}
			}

            private static bool TryGeRegistryKey(string path, string key, out dynamic value)
            {
                value = null;
                try
                {
                    var rk = Registry.LocalMachine.OpenSubKey(path);
                    if (rk == null) return false;
                    value = rk.GetValue(key);
                    return value != null;
                }
                catch
                {
                    return false;
                }
            }
        }
#endif
	}


}
