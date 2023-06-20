using System;
using System.Net;
using System.Text.RegularExpressions;

namespace HCVK.HCVKSLibrary
{
    public class ValidationUtils
    {
        public ValidationUtils()
        {
        }

        public static bool IsIPAddress(string strIP)
		{
			IPAddress ip;
			bool valid = IPAddress.TryParse(strIP, out ip);

			return valid;
		}

		public static bool IsIPPort(string strPort)
        {
			string IPPORT_REGULAR_PATTERN =
				@"^([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9]|[1-5][0-9][0-9][0-9][0-9]|6[0-4][0-9][0-9][0-9]|65[0-4][0-9][0-9]|655[0-2][0-9]|6553[0-5])$";
			
			Regex regex = new Regex(IPPORT_REGULAR_PATTERN);
			Match match = regex.Match(strPort);

			return match.Success;
        }
	}
}
