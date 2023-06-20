using log4net;
using System;
using System.Reflection;
using System.Net;
using System.Net.Security;
//using System.Net.Http;
//using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using HCVK.HCVKSLibrary.VO;
using Newtonsoft.Json.Linq;


namespace client {
	class ServerStatusManager {

		private static readonly ILog _logger = LogManager.GetLogger (MethodBase.GetCurrentMethod ().DeclaringType);

		private string _strStatusCode = string.Empty;
		private string _url = String.Empty;
		//public HttpClient httpClient = null;
		//public HttpResponseMessage response = null;

		public ServerStatusManager()
		{

		}
		/*
		public async Task<string> GetServerStatusCodeHttpClient(VOBrokerServerNew vOBrokerServer)
		{

			_url = "https://" + vOBrokerServer.BrokerServerIP + ":" + vOBrokerServer.BrokerServerPort;

			//ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback (AcceptAllCertifications);
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			using (httpClient = new HttpClient ()) {

				httpClient.Timeout = TimeSpan.FromSeconds(2);
				response = await httpClient.GetAsync (_url);

				_strStatusCode = response.StatusCode.ToString ();


			}

			response.Dispose ();
			httpClient.Dispose ();


			return _strStatusCode;
		}
public string GetServerStatusCode (VOBrokerServerNew vOBrokerServer)
		{
			try {
				_url = "https://" + vOBrokerServer.BrokerServerIP + ":" + vOBrokerServer.BrokerServerPort;

				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback (AcceptAllCertifications);
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (_url);
				request.Method = "GET";
				request.Timeout = 2000;

				using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse ()) {


					_strStatusCode = resp.StatusCode.ToString ();

				}
				_logger.Debug (string.Format ("BrokerIP: {0} StatusCode : {1}", vOBrokerServer.BrokerServerIP, _strStatusCode));


			} catch (WebException wex) {
				_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}


			return _strStatusCode;
		}
		
		*/

		public async Task<string>  GetServerStatusCode_Async (VOBrokerServerNew vOBrokerServer)
		{
			try {
				_url = "https://" + vOBrokerServer.BrokerServerIP + ":" + vOBrokerServer.BrokerServerPort;

				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback (AcceptAllCertifications);
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (_url);
				request.Method = "GET";
				request.Timeout = 5000;

				WebResponse resp = await request.GetResponseAsync();
				StreamReader streamReader = new StreamReader (resp.GetResponseStream ());
				string Jdata = await streamReader.ReadToEndAsync ();


				streamReader.Close ();
				resp.Close ();

				// {"resultData":"OK","resultCode":200}
				_strStatusCode = JObject.Parse (Jdata) ["resultData"].ToString ();


				_logger.Debug (string.Format ("BrokerIP: {0} StatusCode : {1}", vOBrokerServer.BrokerServerIP, _strStatusCode));


			} catch (WebException wex) {
				_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}


			return _strStatusCode;
		}




		private bool AcceptAllCertifications (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public string GetServerRSACertificate (VOBrokerServerNew vOBrokerServer)
		{
			string responseText = string.Empty;

			try {

				string strPublickey_Api_Url = "https://" + vOBrokerServer.BrokerServerIP + ":" + vOBrokerServer.BrokerServerPort +"/"+ Properties.Resources.HCVKB_REQUEST_SYSTEM_PUBLICKEY;

				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback (AcceptAllCertifications);
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (strPublickey_Api_Url);
				request.Method = "GET";
				request.Timeout = 2000;


				using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse ()) {

					Stream respStream = resp.GetResponseStream ();
					using (StreamReader sr = new StreamReader (respStream)) {
						responseText = sr.ReadToEnd ();
					}
				}

				string resultData = JObject.Parse (responseText) ["resultData"].ToString ();

				responseText = JObject.Parse (resultData) ["publicKey"].ToString ();

#if !DEBUG
				_logger.Debug (string.Format ("BrokerIP: {0} Get RSA PublicKey , and Param Secure Hide", vOBrokerServer.BrokerServerIP));
#else
				_logger.Debug (string.Format ("BrokerIP: {0} Get RSA PublicKey : {1}", vOBrokerServer.BrokerServerIP, responseText));
#endif



			} catch (WebException wex) {
				_logger.Error (string.Format ("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString ()));
			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}


			return responseText;
		}

	}
}