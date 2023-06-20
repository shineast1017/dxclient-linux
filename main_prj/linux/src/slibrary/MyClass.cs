using System;
using log4net;
using System.Reflection;

namespace test1111
{
    public class MyClass
    {
		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public MyClass()
        {
			
        }

		public void SetLog(string strLog)
		{
			Console.WriteLine("SetLog");
			_logger.Debug(strLog);
		}
    }
}
