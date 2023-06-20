using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary
{
    public class ConfigManager
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string GetAppConfig(string strKey)
        {
            string strReturn = string.Empty;
            try
            {
                if (strKey == null || strKey.Trim().Length == 0)
                {
                    throw new ArgumentNullException("strKey");
                }

                strReturn = ConfigurationManager.AppSettings[strKey].ToString();
            }
            catch (ArgumentNullException exArgNull)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", exArgNull.HResult, exArgNull.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
            return strReturn;
        }
    }
}
