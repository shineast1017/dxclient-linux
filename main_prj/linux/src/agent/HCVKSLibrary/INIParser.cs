using System;
using log4net;
using System.Reflection;
using IniParser;
using IniParser.Model;

namespace HCVK.HCVKSLibrary
{
    public class INIParser
    {
		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
 
		public INIParser()
        {
        }

		static public string GetValue(string filePath, string strSection, string strKey)
		{
			if (string.IsNullOrEmpty(filePath))
				return "";

			try
			{
				FileIniDataParser parser = new FileIniDataParser();

				IniData data = parser.ReadFile(filePath);

				string strValue = data[strSection][strKey];

				if (string.IsNullOrEmpty(strValue))
					return "";
				return strValue;
			}
			catch(Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
				return "";
			}
        }
    }
}
