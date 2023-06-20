using System;

namespace HCVK.HCVKSLibrary.VO
{
	[Serializable()]
    public class VOBookmark
    {
		public string ServerIP { get; set; }

		public string ServerName { get; set; }

		public string ServerPort { get; set; }

		public string OSCode { get; set; }

		public string DesktopID { get; set; }

		public string DesktopName { get; set; }

		public string DesktopIP { get; set; }
    }
}
