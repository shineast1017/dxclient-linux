using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
    public class VODesktopTemplate
    {
        private string _strTemplateName = string.Empty;
        private string _strTemplateDescription = string.Empty;
        private string _strOSName = string.Empty;
        private string _strOSCode = string.Empty;

        public string TemplateName
        {
            set { _strTemplateName = value; }
            get { return _strTemplateName; }
        }
        public string TemplateDescription
        {
            set { _strTemplateDescription = value; }
            get { return _strTemplateDescription; }
        }
        public string OSName
        {
            set { _strOSName = value; }
            get { return _strOSName; }
        }
        public string OSCode
        {
            set { _strOSCode = value; }
            get { return _strOSCode; }
        }
    }
}
