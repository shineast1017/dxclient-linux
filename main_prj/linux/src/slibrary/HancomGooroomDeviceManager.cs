using System;
using System.Threading.Tasks;
using Tmds.DBus;
using log4net;
using System.Reflection;

namespace HCVK.HCVKSLibrary
{
	public sealed class HancomGooroomDeviceManager
	{
		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public const bool OWNER_KVM = true;
		public const bool OWNER_DAAS = false;

		private static readonly object _sync = new Object ();

		// USB 장치 제어권 변경 시그널 interface name
		private const String INTERFACE_NAME = "kr.gooroom.VirtViewer1";
		// USB 장치 제어권 변경 시그널 path
		private const String PATH_NAME = "/kr/gooroom/VirtViewer1";

		private static HancomGooroomDeviceManager _instance = null;

		// 제어권을 가져가야하는 시그널 Value
		private Boolean _deviceOwnerCondition = OWNER_DAAS;
		private Boolean currentOwnerCondition = OWNER_DAAS;

		private Connection connection = null;
		private IVirtViewer virtViewer = null;

		public delegate void TakeUSBDeviceCallback(bool isTake);

		public TakeUSBDeviceCallback takeAction;


		private HancomGooroomDeviceManager()
		{

		}

		~HancomGooroomDeviceManager()
		{
			if(_instance != null)
			{
				_instance.connection.Dispose();
			}
			Console.WriteLine ("~HancomGooroomDeviceManager");
		}


		public static HancomGooroomDeviceManager GetInstance()
		{
			if (_instance == null)
			{
				lock (_sync)
				{
					if (_instance == null)
					{
						_instance = new HancomGooroomDeviceManager();
					}
				}
			}
			return _instance;
		}




		public void SetUSBOwnerCondition(Boolean isKVM)
		{
			lock (_sync)
			{
				this._deviceOwnerCondition = isKVM;
				this.currentOwnerCondition = OWNER_KVM;
			}
		}


		[DBusInterface (INTERFACE_NAME)]
		public interface IVirtViewer : IDBusObject {
			Task<IDisposable> WatchUSBOwnerChangeAsync (Action<Boolean> handler);
		}

		public void Setup()
		{
			lock (_sync)
			{
				this.connection = new Connection(Address.Session);
				this.connection.ConnectAsync();
				this.virtViewer = connection.CreateProxy<IVirtViewer>(INTERFACE_NAME, PATH_NAME);
				this.virtViewer.WatchUSBOwnerChangeAsync(OnUSBOwnerChange);
			}
		}

		private void OnUSBOwnerChange (Boolean isKVM)
		{
			_logger.Info(string.Format("USBOwnerChange : {0}", isKVM));
			lock (_sync) {

				if ( isKVM == this.currentOwnerCondition)
				{
					_logger.Info(string.Format("USB Owner is not changed : current( {0} ), received( {1} ), ", this.currentOwnerCondition, isKVM));
					return;
				}

				// 현재 제어권 가진다.
				if (isKVM == this._deviceOwnerCondition)
				{
					_logger.Info(string.Format("Try take USB Device : owner condition( {0} )", this._deviceOwnerCondition));
					takeAction(true);
					_logger.Info(string.Format("Take USB Device : owner condition( {0} )", this._deviceOwnerCondition));
				}
				else
				{
					_logger.Info(string.Format("Try release USB Device : owner condition( {0} )", this._deviceOwnerCondition));
					takeAction(false);
					_logger.Info(string.Format("Release USB Device : owner condition( {0} )", this._deviceOwnerCondition));
				}
				this.currentOwnerCondition = isKVM;
			}


		}

		public Boolean isUSBOwner()
		{
			if(this.currentOwnerCondition == this._deviceOwnerCondition)
			{
				return true;
			}
			return false;
		}

	}

}
