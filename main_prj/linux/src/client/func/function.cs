using System;
using System.Linq;
using log4net;
using HCVK.HCVKSLibrary.VO;
using System.Reflection;
using System.Collections.Generic;
using client.Request;
using System.Net;
using Newtonsoft.Json.Linq;
using HCVK.HCVKSLibrary;

namespace client
{
    public delegate void CallbackShowEditServerPage(int nEditMode, int nIdx);
	public delegate void CallbackShowServerListPage();
	public delegate void CallbackEditServerItem(int nEditMode, int nIdx, string strSeverName, string strSeverIP, string strServerPort);
	public delegate void CallbackShowServerInfoDetail(int nIdx);
	public delegate void CallbackShowDesktopInfoDetail(string strWidgetName);
	public delegate void CallbackShowLoginPage(VOBrokerServerNew vrokerServer);
	public delegate void CallbackShowDesktopListPage(bool bRefresh);
	public delegate void CallbackShowDesktopPolicyPage(List<VODesktopPolicies> listPolicies);
	public delegate void CallbackShowEditDesktopPage(VODesktopPoolEx vODesktopPoolEx);
	public delegate void CallbackEditDesktopItem(VODesktopPoolEx vODesktopPoolEx);
	public delegate void CallbackChangeLoginStatus();
	public delegate void CallbackChangeConnectStatus(string strPoolID, bool bConnect);
	public delegate void CallbackEnableBookmark(bool bEnable);
	public delegate void CallbackLoadBookmarkInfo();
	public delegate bool CallbackExcuteBookmarkServer(string strServerIP);
	public delegate void CallbackExcuteBookmarkDesktop(string strDesktopID);
	public delegate void CallbackSelectedLeftItem(int nIdx);
	public delegate void CallbackRefreshDesktopInfo(string strDesktopPoolID);

	public delegate void CallbackSetDesktopInfo (string vODesktop);

	public delegate string CallbackConnectedDesktopID ();
	public delegate void CallbackCheckPasswordRule (string strResultCode, string strResultMessage);
	public delegate void CallbackServerStatusCode (string strPassword);
	public delegate void RequestCheckServerState ();


	public class MainFunc
	{
		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static CallbackShowEditServerPage callbackShowEditServerPage = null;
		public static CallbackShowServerListPage callbackShowServerListPage = null;
		public static CallbackEditServerItem callbackEditServerItem = null;
		public static CallbackShowServerInfoDetail callbackShowServerInfoDetail = null;
		public static CallbackShowDesktopInfoDetail callbackShowDesktopInfoDetail = null;
		public static CallbackShowLoginPage callbackShowLoginPage = null;
		public static CallbackShowDesktopListPage callbackShowDesktopListPage = null;
		public static CallbackShowDesktopPolicyPage callbackShowDesktopPolicyPage = null;
		public static CallbackShowEditDesktopPage callbackShowEditDesktopPage = null;
		public static CallbackChangeLoginStatus callbackChangeLoginStatus = null;
		public static CallbackChangeConnectStatus callbackChangeConnectStatus = null;
		public static CallbackEditDesktopItem callbackEditDesktopItem = null;
		public static CallbackEnableBookmark callbackEnableBookmark = null;
		public static CallbackLoadBookmarkInfo callbackLoadBookmarkInfo = null;
		public static CallbackExcuteBookmarkServer callbackExcuteBookmarkServer = null;
		public static CallbackExcuteBookmarkDesktop callbackExcuteBookmarkDesktop = null;
		public static CallbackSelectedLeftItem callbackSelectedLeftItem = null;
		public static CallbackRefreshDesktopInfo callbackRefreshDesktopInfo = null;
		public static CallbackSetDesktopInfo callbackSetDesktopInfo = null;

		public static CallbackConnectedDesktopID CallbackGetConnectedDesktopID = null;
		public static CallbackCheckPasswordRule callbackCheckPasswordRule = null;
		public static CallbackServerStatusCode callbackServerStatus = null;
		public static RequestCheckServerState requestCheckServerState = null;


		public static bool AddBrokerServer(VOBrokerServerNew broker)
		{
			if (broker != null)
			{
				MainWindow.mainWindow.environment.BrokerServers.Add(broker);
				return true;
			}
			return false;
		}
		public static bool EditBrokerServer(int nIdx, VOBrokerServerNew broker)
		{
			int nCnt = MainWindow.mainWindow.environment.BrokerServers.Count;
			if (nIdx < 0 || nIdx > nCnt)
				return false;
			if (broker != null)
			{
				VOBrokerServerNew oldbroker = MainWindow.mainWindow.environment.BrokerServers[nIdx];

				oldbroker.BrokerServerDesc = broker.BrokerServerDesc;
				oldbroker.BrokerServerIP = broker.BrokerServerIP;
				oldbroker.BrokerServerPort = broker.BrokerServerPort;
				oldbroker.ConfigIP = broker.ConfigIP;

				return true;
			}
			return false;
		}
		public static int RemoveBrokerServer(int nIdx, VOBrokerServerNew broker)
		{
			int nCnt = MainWindow.mainWindow.environment.BrokerServers.Count;
			if (nIdx < 0 || nIdx > nCnt)
			{
				if (broker != null)
				{
					VOBrokerServerNew oldbroker = MainWindow.mainWindow.environment.BrokerServers.First(
						b => b.BrokerServerDesc == broker.BrokerServerDesc &&
						b.BrokerServerIP == broker.BrokerServerIP &&
						b.BrokerServerPort == broker.BrokerServerPort);
					if (oldbroker != null)
					{
						int nRes = MainWindow.mainWindow.environment.BrokerServers.IndexOf(oldbroker);
						MainWindow.mainWindow.environment.BrokerServers.Remove(oldbroker);
						return nRes;
					}
				}
				return -1;
			}
			MainWindow.mainWindow.environment.BrokerServers.RemoveAt(nIdx);
			return nIdx;
		}
		public static VOBrokerServerNew GetBrokerServer(int nIdx)
		{
			if (nIdx < 0 || nIdx > MainWindow.mainWindow.environment.BrokerServers.Count)
				return null;

			return MainWindow.mainWindow.environment.BrokerServers[nIdx];
		}

		public static List<VOBrokerServerNew> GetBrokerServerAll()
		{
			if(MainWindow.mainWindow.environment.BrokerServers != null)
			    return MainWindow.mainWindow.environment.BrokerServers;
			return new List<VOBrokerServerNew>();
		}

		public static int GetBrokerServerIdx(VOBrokerServerNew broker)
		{
			return MainWindow.mainWindow.environment.BrokerServers.IndexOf(broker);
		}

		public static List<VODesktopPool> GetDesktopListAll()
		{
			return MainWindow.mainWindow.GetListDesktopPool();
		}

	}
}
