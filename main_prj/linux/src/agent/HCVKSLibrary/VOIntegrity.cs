using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
    public class VOIntegrity
    {
        private string _strTargetName = string.Empty;
        private string _strExpectedHash = string.Empty;
        private string _strCurrentHash = string.Empty;
        private string _strIsHomogeneous = string.Empty;

        public string targetName
        {
            set { _strTargetName = value; }
            get { return _strTargetName; }
        }
        public string expectedHash
        {
            set { _strExpectedHash = value; }
            get { return _strExpectedHash; }
        }
        public string currentHash
        {
            set { _strCurrentHash = value; }
            get { return _strCurrentHash; }
        }
        public string isHomogeneous
        {
            set { _strIsHomogeneous = value; }
            get { return _strIsHomogeneous; }
        }
    }
}
