using System;
using System.Collections.Generic;
using System.Reflection;
using HCVK.HCVKSLibrary;
using HCVK.HCVKSLibrary.VO;
using Newtonsoft.Json.Linq;
using client.Request;
using log4net;

namespace client {

	public static class BrokerServerDefinded {
		public const int DEF_NEW_GET_MESSAGE_INTERVAL_STOP = 1000 * 5;
		//const Definded
		public const string URL_GETNEWMSG = "/control/msg/connect";
		public const string URL_CHECKREADEDMSG = "/control/msg/connect/apply";

	}


	public class MSG {
		public string content_ { set; get; }
		public string sendTime_ { set; get; }
		public string seq_ { set; get; }
		// callbackUrl information
		public JArray CallbackUrl_arrObj { set; get; }
	}


	public class BrokerNoticeMsgManager : HttpsClient {
		public class JSonRequest {
			public string requestNodeName = Properties.Resources.REQUEST_NODE_NAME;
			public string requestNodeVersion = Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			public object requestNodeData;
		}

		List<MSG> _getMsgs = new List<MSG> ();

		static Queue<string> _receive_respon = new Queue<string> ();
		private static readonly ILog _logger = LogManager.GetLogger (MethodBase.GetCurrentMethod ().DeclaringType);
		private string _serverUrl = "";
		private string _proto = "https";
		private string _userID = "";
		private string _clientIP = "";

		private VOBrokerServerNew _vOBrokerServer = null;
		public JArray _Broker_callbackUrl_Array = null;

		public BrokerNoticeMsgManager ()
		{
			_receive_respon.Clear ();
			_getMsgs.Clear ();
		}

		~BrokerNoticeMsgManager()
		{
			_receive_respon.Clear ();
			_getMsgs.Clear ();
		}

		public void InitServerInfoAndUserInfo (VOBrokerServerNew vOBrokerServer, string proto, string userID, string clientID)
		{
			if (vOBrokerServer == null || vOBrokerServer.BrokerServerIP.Length == 0 ||
			vOBrokerServer.BrokerServerPort.Length == 0 || proto.Length == 0 ||
				userID.Length == 0 || clientID.Length == 0)
				return;

			_userID = userID;
			_clientIP = clientID;

			if (_vOBrokerServer == null) {
				_vOBrokerServer = new VOBrokerServerNew ();
			}
			_vOBrokerServer.AuthToken = vOBrokerServer.AuthToken;
			_vOBrokerServer.BrokerServerIP = vOBrokerServer.BrokerServerIP;
			_vOBrokerServer.BrokerServerPort = vOBrokerServer.BrokerServerPort;
			_proto = proto;

			_serverUrl = string.Format ("{0}://{1}:{2}", _proto, _vOBrokerServer.BrokerServerIP, _vOBrokerServer.BrokerServerPort);

			return;
		}

		public void RequestSystemMessage ()
		{
			const int MSG_LEN = 2;
			string [,] inputData = new string [MSG_LEN, 2] {
				{"userId",_userID},
				{"clientIP",_clientIP}};
			Object jdata = MakeMsg (inputData, MSG_LEN);
			if (jdata == null)
				return;

			SendRequestMsg (jdata, BrokerServerDefinded.URL_GETNEWMSG, _vOBrokerServer.AuthToken);

		}

		//request broker server readed system message
		public void RequestCheckMessage (string _seq_id)
		{
			const int MSG_LEN = 3;
			string [,] inputData = new string [MSG_LEN, 2] {
				{"userId", _userID},
				{"clientIP", _clientIP},
				{"seq", _seq_id}
			};

			Object jdata = MakeMsg (inputData, MSG_LEN);
			if (jdata == null)
				return;

			SendRequestMsg (jdata,BrokerServerDefinded.URL_CHECKREADEDMSG, _vOBrokerServer.AuthToken);

		}


		object MakeMsg (string [,] jsondata, int len)
		{
			JObject data = new JObject ();

			for(int idx =0; idx<len; idx++) {
				if (jsondata [idx, 0] == null || jsondata [idx, 0].Length == 0)
					break;
				data.Add (jsondata [idx, 0], jsondata [idx, 1]);
			}

			JSonRequest ret = new JSonRequest {
				requestNodeData = data
			};

			return ret;
		}

		void SendRequestMsg (object jsondata, string urisubPath, string token)
		{
			//Make uriFullPash 
			string urifullpath = _serverUrl + urisubPath;

			RequestPost_AsyncCallback (urifullpath, jsondata, Callback_GetMessage, token);
		}

		// ret receive msg empty is null
		public string GetResponseMsg ()
		{
			string data = "";

			lock (_receive_respon) {
				if (_receive_respon.Count == 0)
					return "";

				data = _receive_respon.Dequeue ();
			}
			return data;
		}

		private void RequestPost_AsyncCallback (string uriFullPath, object sendJsonData, CallbackResponse resPonse, string token = "TAuthToken")
		{
			try {
				//add header field of authentication token
				Dictionary<string, string> dicHeader = new Dictionary<string, string> ();
				//add AuthToken
				dicHeader.Add (Properties.Resources.HCVK_HEADER_AUTH_TOKEN, token);

				SendReqeust_AsyncCallback (uriFullPath, "POST", dicHeader, sendJsonData, resPonse);

			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}
		}

		static void Callback_GetMessage (JObject resJsonObject, Exception e)
		{
			try 
			{
				if (resJsonObject == null) {
					return;
				}

				if (resJsonObject != null && resJsonObject [RequestJSONParam.RESPONSE_RESULT_CODE].ToString ().Equals ("0")) {
					//success response, fetch and update auth token
					_receive_respon.Enqueue (resJsonObject.ToString ());
				}

			}catch(Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}
		}

		public List<MSG> ParserGetmessageFromJsonData (string Jdata)
		{
			_getMsgs.Clear ();

			string resultCode = JObject.Parse (Jdata) ["resultCode"].ToString ();

			if (resultCode.Equals ("0")) {
				string resultData = JObject.Parse (Jdata) ["resultData"].ToString ();

				if (resultData.Equals ("") || resultData.Equals ("[]"))
					return null;

				JArray jarr = JArray.Parse (resultData);

				foreach (JObject msg in jarr) {
					string content = msg ["contents"].ToString ();
					string sendTime = msg ["sendTime"].ToString ();
					string seq = msg ["seq"].ToString ();
					JArray CallbackUrl = null;

					try {
						// if it contains a [callbackUrl] field, Queue it in, otherwise show SystemMessagePopup Dialog.
						if (msg.ContainsKey("callbackUrl") == true) {

							//_Broker_callbackUrl_Array = (JArray)msg ["callbackUrl"];
							CallbackUrl = (JArray)msg ["callbackUrl"];
														}
						} catch (Exception ex) {
						_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
					}

					_getMsgs.Add (new MSG { content_ = content, sendTime_ = sendTime, seq_ = seq, CallbackUrl_arrObj = CallbackUrl });
				}

			}
			return _getMsgs;
		}


		/*
		//Send broker server readed system message
		public void SendConfirmMessage (string _seq_id)
		{
			// Search to match seq_id in Queue data
			MSG outMSG = null;
			bool bIsConfirmMsg = false;
			for(int i = 0; i < _ConfirmMsgs.Count; i++) 
			{
				outMSG = _ConfirmMsgs [i];

				if(outMSG.seq_ == _seq_id) 
				{
					bIsConfirmMsg = true;
					_ConfirmMsgs.Remove (outMSG);
					break;
				}
			}

			if (bIsConfirmMsg == true) {

				const int SendMSG_LEN = 1;
				string [,] inputData = new string [SendMSG_LEN, 2] {{"userId", _userID}};

				Object jdata = MakeMsg (inputData, SendMSG_LEN);
				if (jdata == null)
					return;

				SendRequestMsg (jdata, outMSG.url_, _vOBrokerServer.AuthToken);

			}
	
		}
		*/
		public void SendConfirmMessage (object jdata, string _url)
		{
			var refineJdata = JObject.Parse (jdata.ToString ());
			var data = refineJdata.GetValue ("requestNodeData");
			JSonRequest JsendData = new JSonRequest {
				requestNodeData = data
			};


			SendRequestMsg (JsendData, _url, _vOBrokerServer.AuthToken);
		}

		public void SendConfirmMessage (string _url)
		{
			const int MSG_LEN = 1;
			string [,] inputData = new string [MSG_LEN, 2] {
				{"userId",_userID}};
			Object jdata = MakeMsg (inputData, MSG_LEN);
			if (jdata == null)
				return;

			SendRequestMsg (jdata, _url, _vOBrokerServer.AuthToken);

		}
	}
}