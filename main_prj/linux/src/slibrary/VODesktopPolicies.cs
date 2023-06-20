using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
    public class VODesktopPolicies
    {
        private string  _strPolicyId = string.Empty;
        private string _strPolicyName = string.Empty;
        private string _strPolicyType = string.Empty;
        private bool _bApply = false;

        public string PolicyId
        {
            set { _strPolicyId = value; } get { return _strPolicyId; }
        }
        public string PolicyName
        {
            set { _strPolicyName = value;  } get { return _strPolicyName; }
        }
        public string PolicyType
        {
            set { _strPolicyType = value;  } get { return _strPolicyType; }
        }
        public bool Apply
        {
            set { _bApply = value; } get { return _bApply; }
        }
    }
}
