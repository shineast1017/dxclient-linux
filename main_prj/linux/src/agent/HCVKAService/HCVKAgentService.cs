using HCVK.HCVKAService.Resources;
using HCVK.HCVKSLibrary;
using log4net;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;


// ref.
// receive event for system shutdown
// https://social.msdn.microsoft.com/Forums/vstudio/en-US/5b7d82c3-e7d6-471f-b085-d46e7de9c737/service-not-doing-onstop-or-onshutdown-when-windows-shuts-down?forum=csharpgeneral
//


namespace HCVK.HCVKAService
{
	public partial class HCVKAgentService : ServiceBase
	{
		private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private HCVKAMain _hcVKAMain = null;


		private bool useFallback = false;
		private object originalControlCallbackEx = null;

		#region Reflection-Variables
		// Type of the ServiceBase and the .NET wrapper of native Methods
		private static readonly Type serviceBaseType = typeof(ServiceBase);
		private static readonly Type targetDelegate = typeof(ServiceBase).Assembly.GetType("System.ServiceProcess.NativeMethods+ServiceControlCallbackEx");

		// Fields and Methods of the ServiceBase using reflection
		private static readonly FieldInfo acceptedCommands = serviceBaseType.GetField("acceptedCommands", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly MethodInfo initializeServiceBase = serviceBaseType.GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo commandCallbackEx = serviceBaseType.GetField("commandCallbackEx", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly MethodInfo baseCallbackEx = serviceBaseType.GetMethod("ServiceCommandCallbackEx", BindingFlags.Instance | BindingFlags.NonPublic);

		// Native service constants
		private const int SERVICE_CONTROL_PRESHUTDOWN = 0x0000000F;
		private const int SERVICE_ACCEPT_PRESHUTDOWN = 0x00000100;
		private const int SERVICE_CONTROL_SHUTDOWN = 0x00000005;
		#endregion Reflection-Variables

        #region Reflection-Methods
		private void SetPreShutdownControlCallbackEx()
		{
			if (originalControlCallbackEx == null)
			{
				// Save original ServiceControlCallbackEx
				originalControlCallbackEx = commandCallbackEx.GetValue(this);

				// Register our ServiceControlCallbackEx to the .NET wrapped ServiceControlCallbackEx
				commandCallbackEx.SetValue(this, Delegate.CreateDelegate(targetDelegate, this, "PreShutdownServiceControlCallbackEx"));
			}
		}

		private void SetOriginalControlCallbackEx()
		{
			if (originalControlCallbackEx != null)
			{
				// Restore original ServiceControlCallbackEx && reset store
				commandCallbackEx.SetValue(this, originalControlCallbackEx);
				originalControlCallbackEx = null;
			}
		}

		private int AddAcceptedCommands(int serviceAcceptFlag)
		{
			// Get accepted commands variable using reflection
			int acceptedCommands = (int)HCVKAgentService.acceptedCommands.GetValue(this);

			// Check set accept flags
			if (serviceAcceptFlag > 0)
			{
				// Add flags to accepted commands
				acceptedCommands |= serviceAcceptFlag;

				// OnPreShutdown accepted? Set PreShutdownControlCallbackEx
				if ((acceptedCommands & SERVICE_ACCEPT_PRESHUTDOWN) != 0)
				{ SetPreShutdownControlCallbackEx(); }

				// Add accepted commands to variable and set variable using reflection
				HCVKAgentService.acceptedCommands.SetValue(this, acceptedCommands);
			}

			// Accepted commands
			return acceptedCommands;
		}

		private int RemoveAcceptedCommands(int serviceAcceptFlag)
		{
			// Get accepted commands variable using reflection
			int acceptedCommands = (int)HCVKAgentService.acceptedCommands.GetValue(this);

			// Check set accept flags
			if (serviceAcceptFlag > 0)
			{
				// Remove flags from accepted commands
				acceptedCommands ^= serviceAcceptFlag;

				// OnPreShutdown removed? Reset ServiceControlCallbackEx
				if ((acceptedCommands & SERVICE_ACCEPT_PRESHUTDOWN) == 0)
				{ SetOriginalControlCallbackEx(); }

				// Add accepted commands to variable and set variable using reflection
				HCVKAgentService.acceptedCommands.SetValue(this, acceptedCommands);
			}

			// Accepted commands
			return acceptedCommands;
		}

		private int PreShutdownServiceControlCallbackEx(int control, int eventType, IntPtr eventData, IntPtr eventContext)
		{
			// Check control for PreShutdown
			if (control == SERVICE_CONTROL_PRESHUTDOWN)
			{ OnPreShutdown(); }

			// Call original ServiceCommandCallbackEx
			return (int)baseCallbackEx.Invoke(this, new object[] { control, eventType, eventData, eventContext });
		}
		#endregion Reflection-Methods

		public bool CanPreShutdown
		{
			get
			{
				// Check if OnPreShutdown is an accepted command
				int acceptedCommands = AddAcceptedCommands(0);
				return ((acceptedCommands & SERVICE_ACCEPT_PRESHUTDOWN) != 0);
			}
			set
			{
				try
				{
					// Add or remove?
					if (value == true)
					{
						// Add OnPreShutdown to accepted commands
						AddAcceptedCommands(SERVICE_ACCEPT_PRESHUTDOWN);
					}
					else
					{
						// Remove OnPreShutdown from accepted commands
						RemoveAcceptedCommands(SERVICE_ACCEPT_PRESHUTDOWN);
					}
				}
				catch (Exception)
				{
					// Set use fallback
					useFallback = value;

					// Fallback use shutdown
					if (useFallback == true)
					{
						CanShutdown = true;
					}
				}
			}
		}

		// Set Language
		public void SetLangText()
		{
			CultureInfo cultureInfo = CultureInfo.CurrentCulture;

			try
			{
				string settingLang = new ConfigManager() { }.GetAppConfig("Language");
				_logger.Debug(string.Format("Get language config : {0}", settingLang.Equals(string.Empty) ? "Empty" : settingLang));

				if (!settingLang.Equals(string.Empty))
				{
					cultureInfo = CultureInfo.CreateSpecificCulture(settingLang);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}

			_logger.Debug(string.Format("Set cultureInfo : {0}", cultureInfo.ToString()));
			MultiLang.Culture = cultureInfo;
		}


		public HCVKAgentService()
		{
			_logger.Debug(string.Format(" "));
			_logger.Debug(string.Format(" "));
			_logger.Debug(string.Format(" "));
			_logger.Debug(string.Format(" "));
			_logger.Debug(string.Format("************************************"));
			_logger.Debug(string.Format("*     HCVKAService (v.{0}) ", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
			//_logger.Debug(string.Format("*     TimeStamp ({0})      ", DateTime.Now.ToLocalTime()));
#if WIN32
			_logger.Debug(string.Format("*     OSVersion ({0})      ", CommonUtils.GetOSVersion()));
#else
			_logger.Debug(string.Format("*     OSVersion ({0})      ", "16.04"));
#endif
			_logger.Debug(string.Format("************************************"));
            _logger.Debug(string.Format(" "));

            InitializeComponent();
            SetLangText();
        }

#if DEBUG
		public static void Main(String[] args)
        {
            (new HCVKAgentService()).OnStart(new string[1]);
			//ServiceBase.Run(new HCVKAgentService());

			Thread.Sleep(Timeout.Infinite);
        }
#endif

		protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry(Properties.Resources.EVENT_LOG_NAME, MultiLang.HCVKA_SERVICE_MSG_0001);
            _logger.Debug(string.Format("{0}", MultiLang.HCVKA_SERVICE_MSG_0001));


            if (_hcVKAMain == null)
                _hcVKAMain = new HCVKAMain();

            _hcVKAMain.InitializeHCVKAMain();
        }

        protected override void OnStop()
        {
            _hcVKAMain.FinalizeHCVKAMain();

            // wait for restarting rdp service
            System.Threading.Thread.Sleep(5000);


            _hcVKAMain = null;


            EventLog.WriteEntry(Properties.Resources.EVENT_LOG_NAME, MultiLang.HCVKA_SERVICE_MSG_0002);
            _logger.Debug(string.Format("{0} - OnStop", MultiLang.HCVKA_SERVICE_MSG_0002));
        }

        protected override void OnShutdown()
        {
            if (useFallback)
            {
                OnPreShutdown();
            }


            //your code here
            {
                _hcVKAMain.FinalizeHCVKAMain();

                // wait for restarting rdp service
                System.Threading.Thread.Sleep(5000);


                _hcVKAMain = null;
            }


            //EventLog.WriteEntry(Properties.Resources.EVENT_LOG_NAME, MultiLang.HCVKA_SERVICE_MSG_0002);
            _logger.Debug(string.Format("{0} - OnShutdown", MultiLang.HCVKA_SERVICE_MSG_0002));


            // call base
            base.OnShutdown();
        }

        protected virtual void OnPreShutdown()
        {
            //your code here
            //EventLog.WriteEntry(Properties.Resources.EVENT_LOG_NAME, MultiLang.HCVKA_SERVICE_MSG_0002);
            _logger.Debug(string.Format("{0} - OnPreShutdown", MultiLang.HCVKA_SERVICE_MSG_0002));
        }
    }
}
