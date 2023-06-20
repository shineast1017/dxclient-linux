using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ref.
// os version
// https://stackoverflow.com/questions/21737985/windows-version-in-c-sharp
//


namespace HCVK.HCVKSLibrary
{
	public class CommonUtils
	{
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
