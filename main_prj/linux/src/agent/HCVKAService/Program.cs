using System;
using System.ServiceProcess;

namespace HCVK.HCVKAService
{
	class Program
	{
#if (DEBUG != true)
		public static void Main(string[] args)
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] { new HCVKAgentService() };
			ServiceBase.Run(ServicesToRun);
		}
#endif
	}
}
