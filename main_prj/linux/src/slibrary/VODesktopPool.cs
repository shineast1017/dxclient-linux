using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
	public class VODesktopPoolEx
    {
        private VODesktopPool _voDesktopPool = new VODesktopPool();
        private string _strProtocol = string.Empty;
        private int _nResoutionIndex = 3; //default
        private int _nSpeedIndex = 0; //default
        private bool _bAudio = false;

        public VODesktopPool DesktopPool
        {
            set { _voDesktopPool = value; }
            get { return _voDesktopPool; }
        }
        public string Protocol
        {
            set { _strProtocol = value; }
            get { return _strProtocol; }
        }
        public int ResolutionIndex
        {
            set { _nResoutionIndex = value; }
            get { return _nResoutionIndex; }
        }
        public int SpeedIndex
        {
            set { _nSpeedIndex = value; }
            get { return _nSpeedIndex; }
        }
        public bool Audio
        {
            set { _bAudio = value; }
            get { return _bAudio; }
        }
    }

	public class VODesktopPool
    {
        private string _strPoolID = string.Empty;
        private string _strPoolName = string.Empty;
        private string _strPoolDesc = string.Empty;
        private string _strAccessDiv = string.Empty;
        private string _strVMAvailableCount = string.Empty;
        private string _strVMTotalDesktopCount = string.Empty;



        private string _strSupportProtocols = string.Empty;
        private VODesktop _voDesktop = new VODesktop();


        private string _strTag = string.Empty;
        private bool _bIsEnabled = false;


        public string PoolID
        {
            set { _strPoolID = value; }
            get { return _strPoolID; }
        }
        public string PoolName
        {
            set { _strPoolName = value; }
            get { return _strPoolName; }
        }
        public string PoolDesc
        {
            set { _strPoolDesc = value; }
            get { return _strPoolDesc; }
        }
        public string AccessDiv
        {
            set { _strAccessDiv = value; }
            get { return _strAccessDiv; }
        }
        public string SupportProtocols
        {
            set { _strSupportProtocols = value; }
            get { return _strSupportProtocols; }
        }
        public VODesktop Desktop
        {
            set { _voDesktop = value; }
            get { return _voDesktop; }
        }
        public string Tag
        {
            set { _strTag = value; }
            get { return _strTag; }
        }
        public bool IsEnabled
        {
            set { _bIsEnabled = value; }
            get { return _bIsEnabled; }
        }
        public string VmAvailableCount //  vm available count 
        {
            set { _strVMAvailableCount = value; }
            get { return _strVMAvailableCount; }
        }
        public string VmTotalCount //  vm Total Count
        {
            set { _strVMTotalDesktopCount = value; }
            get { return _strVMTotalDesktopCount; }
        }


    }
}
