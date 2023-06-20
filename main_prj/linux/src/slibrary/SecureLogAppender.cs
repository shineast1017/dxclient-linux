using System;
using System.Linq;
using System.Reflection;
using log4net.Appender;
using log4net.Core;

namespace HCVK.HCVKSLibrary.Secure {
	public class SecureLogAppender : RollingFileAppender {
		private FieldInfo _loggingEventm_dataFieldInfo = typeof (LoggingEvent).GetField ("m_data", BindingFlags.Instance | BindingFlags.NonPublic);

		private const string SECURE_MAKSING_SEGMENT = "<------------- SECURE LOG ---------------->";
		private string [] SECURE_TARGETS = { "userId", "password", "/p:", "Id", "Pw", "token", "enc" };


		public SecureLogAppender ()
		{

		}


		protected override void Append (LoggingEvent loggingEvent)
		{
			string loggingMessage = loggingEvent.RenderedMessage;

			if (!string.IsNullOrEmpty (loggingMessage)) {
				if (SECURE_TARGETS.Any (data => loggingMessage.IndexOf (data, StringComparison.OrdinalIgnoreCase) >= 0)) {
					string secureMessage = Encrpyt (loggingMessage);

					LoggingEventData loggingEventData = (LoggingEventData)_loggingEventm_dataFieldInfo.GetValue (loggingEvent);
					loggingEventData.Message = secureMessage;
					_loggingEventm_dataFieldInfo.SetValue (loggingEvent, loggingEventData);
				}
			}

			base.Append (loggingEvent);
		}

		private string Encrpyt (string str)
		{
			string resultString = string.Empty;

			if (!string.IsNullOrEmpty (str)) {
				string [] splitStringArray = str.Split ('\n');

				for (int i = 0; i < splitStringArray.Length; i++) {
					string st = splitStringArray [i];

#if DEBUG
					resultString += st;
#else
					if (SECURE_TARGETS.Any (data => st.IndexOf (data, StringComparison.OrdinalIgnoreCase) >= 0)) {
						resultString += SECURE_MAKSING_SEGMENT;
					} else {
						resultString += st;
					}
#endif


					if (i < splitStringArray.Length - 1)
						resultString += "\n";
				}
			}
			return resultString;

		}


	}

}