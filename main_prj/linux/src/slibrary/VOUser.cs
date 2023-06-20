using HCVK.HCVKSLibrary;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
    public class VOUser
    {
        private string _strClientIP = string.Empty;
        private string _strClientMAC = string.Empty;
        private string _strEncodedUserID = string.Empty;
        private string _strEncodedPassword = string.Empty;
        private string _strEncodedNewPassword = string.Empty;
        private string _strDomain = string.Empty;
        private string _strClientDeviceType = string.Empty;
        private string _strDeviceOSVersion = string.Empty;
        private string _strClientReleaseVersion = string.Empty;
        private string _strClientReleaseDt = string.Empty;

        private string _strDomainUserID = string.Empty;
        private string _strUserDescription = string.Empty;
        private string _strUserName = string.Empty;
        private string _strOuName = string.Empty;
        private string _strTenantName = string.Empty;
        private string _strTenantID = string.Empty;
        private string _strEmail = string.Empty;
        private string _strPhone = string.Empty;
        private bool _bIsFirstLogin = false;
        private bool _bIsReLogin = false;
        private bool _bIsEnabled = false;
        private string _strVmConnectPw = string.Empty;
        private string _strEngMsg = string.Empty;// Encryption Message received from DBUS

        public string ClientIP
        {
            set { _strClientIP = value; }
            get { return _strClientIP; }
        }
        public string ClientMAC
        {
            set { _strClientMAC = value; }
            get { return _strClientMAC; }
        }
        public string UserID
        {
            set { _strEncodedUserID = CryptoManager.EncodingBase64(value); }
            get { return CryptoManager.DecodingBase64(_strEncodedUserID); }
        }
        public string EncodedUserID
        {
            set { _strEncodedUserID = value; }
            get { return _strEncodedUserID; }
        }
        public string Password
        {
            set { _strEncodedPassword = CryptoManager.EncodingBase64(value); }
            get { return CryptoManager.DecodingBase64(_strEncodedPassword); }
        }
        public string EncodedPassword
        {
            get { return _strEncodedPassword; }
        }
        public string NewPassword
        {
            set { _strEncodedNewPassword = CryptoManager.EncodingBase64(value); }
            get { return CryptoManager.DecodingBase64(_strEncodedNewPassword); }
        }
        public string EncodedNewPassword
        {
            get { return _strEncodedNewPassword; }
        }
        public string Domain
        {
            set { _strDomain = value; }
            get { return _strDomain; }
        }
        public string DeviceType
        {
            set { _strClientDeviceType = value; }
            get { return _strClientDeviceType; }
        }
        public string DeviceOSVersion
        {
            set { _strDeviceOSVersion = value; }
            get { return _strDeviceOSVersion; }
        }

        public string ClientReleaseVersion
        {
            set { _strClientReleaseVersion = value; }
            get { return _strClientReleaseVersion; }
        }
        public string ClientReleaseDt
        {
            set { _strClientReleaseDt = value; }
            get { return _strClientReleaseDt; }
        }


        public string DomainUserID
        {
            set { _strDomainUserID = CryptoManager.EncodingBase64(value); }
            get { return CryptoManager.DecodingBase64(_strDomainUserID); }
        }
        public string EncodedDomainUserID
        {
            set { _strDomainUserID = value; }
            get { return _strDomainUserID; }
        }
        public string UserDescription
        {
            set { _strUserDescription = value; }
            get { return _strUserDescription; }
        }
        public string UserName
        {
            set { _strUserName = value; }
            get { return _strUserName; }
        }
        public string OuName
        {
            set { _strOuName = value; }
            get { return _strOuName; }
        }
        public string TenantName
        {
            set { _strTenantName = value; }
            get { return _strTenantName; }
        }
        public string TenantID
        {
            set { _strTenantID = value; }
            get { return _strTenantID; }
        }
        public string Email
        {
            set { _strEmail = value; }
            get { return _strEmail; }
        }
        public string Phone
        {
            set { _strPhone = value; }
            get { return _strPhone; }
        }
        public bool IsFirstLogin
        {
            set { _bIsFirstLogin = value; }
            get { return _bIsFirstLogin; }
        }
        public bool IsReLogin
        {
            set { _bIsReLogin = value; }
            get { return _bIsReLogin; }
        }
        public bool IsEnabled
        {
            set { _bIsEnabled = value; }
            get { return _bIsEnabled; }
        }

        public string VmConnectPw
        {
            set { _strVmConnectPw = CryptoManager.EncodingBase64(value); }
            get { return CryptoManager.DecodingBase64(_strVmConnectPw); }
        }
        public string engMsg
        {
            set { _strEngMsg = value; }
            get { return _strEngMsg; }
        }
    }
}
