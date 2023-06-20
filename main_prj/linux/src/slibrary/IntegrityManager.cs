using log4net;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary
{
    public delegate bool CallbackCheckIntegrity(object sender);


    public class IntegrityManager
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private CallbackCheckIntegrity _callbackCheckIntegrity = null;
        

        private bool TargetCheckIntegrity()
        {
            bool bReturn = false;
            try
            {
                _logger.Debug(string.Format("Call to callback function of checkIntegrity"));
                bReturn = _callbackCheckIntegrity(this);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
            return bReturn;
        }

        private bool CheckIntegrity()
        {
            bool bReturn = false;
            try
            {
                bReturn = TargetCheckIntegrity();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
            return bReturn;
        }

        public bool CheckIntegrity(CallbackCheckIntegrity callbackCheckIntegrity)
        {
            bool bReturn = false;
            try
            {
                // check valication of parameter
                if (callbackCheckIntegrity == null)
                    throw new ArgumentNullException("undefined callback functions for integrity");

                // assign parameters to members
                _callbackCheckIntegrity = callbackCheckIntegrity;


                // check to start integrity
                bReturn = CheckIntegrity();
            }
            catch (ArgumentNullException exArgNull)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", exArgNull.HResult, exArgNull.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
            return bReturn;
        }
    }
}
