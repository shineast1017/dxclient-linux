using log4net;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace HCVK.HCVKSLibrary
{
    public class CryptoManager
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public static string MakeHashFromStringBySHA256(string strPlaneText)
        {
            string strReturn = string.Empty;
            try
            {
                SHA256Managed sha256Managed = new SHA256Managed();
                strReturn = Convert.ToBase64String(sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(strPlaneText)));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
            return strReturn;
        }


        public static string MakeHashFromFileBySHA256(string strTargetFullPath)
        {
            string strReturn = string.Empty;
            try
            {
                StringBuilder strSHA2 = new StringBuilder();
                FileStream fileStream = new FileStream(strTargetFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] byteResult = (new SHA256CryptoServiceProvider()).ComputeHash(fileStream);
                fileStream.Close();

                for (int i = 0; i < byteResult.Length; i++)
                {
                    strSHA2.Append(byteResult[i].ToString("X2"));
                }

                strReturn = MakeHashFromStringBySHA256(strSHA2.ToString());
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
            return strReturn;
        }

        public static string EncodingBase64(string strPlainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(strPlainText));
        }
        public static string DecodingBase64(string strEncodedText)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(strEncodedText));
        }
    }
}
