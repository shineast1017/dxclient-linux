using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//
// ref.
// msdn
// https://msdn.microsoft.com/ko-kr/library/system.net.httpwebrequest.begingetrequeststream(v=vs.110).aspx
//
//

namespace HCVK.HCVKSLibrary
{
    public delegate void CallbackResponse(JObject resJsonObject, Exception ex = null);


    public class HttpsClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        // declare for http header
        private const string HTTP_HEADER_CONTENTS_TYPE = "application/json; charset=utf-8";
        private const string HTTP_HEADER_ACCEPT_TYPE = "application/json";


        //  declare for http method
        public const string HTTP_PROTOCOL = "https";
        public const string HTTP_HEADER_METHOD_POST = "post";
        public const string HTTP_HEADER_METHOD_PUT = "put";


        // declare for Async Method
        private  ManualResetEvent _resetEventAllDone = new ManualResetEvent(false);
        public  HttpWebRequest _httpWebRequest = null;
        private  object _oRequestData = null;
        private  CallbackResponse _callbackResponse = null;
        private const int REQUEST_TIMEOUT = 5*1000;    // 5 sec.
		protected const int REQUEST_USBIP_TIMEOUT = 25 * 1000;    // 25 sec. usbip 를 이용한 리디렉션 타이아웃시간을 25초로 변경 



		public void StopRequest()
        {
            try
            {
                if (_httpWebRequest != null)
                {
#if DEBUG
                    _logger.Debug(string.Format("Stop Request...{0}", _httpWebRequest.RequestUri));
#endif
                    _httpWebRequest.Abort();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        public async Task<JObject> SendReqeust(string strRequestUrl, string strMethod, Dictionary<string, string> dicHeaders, object oJSonParam)
        {
#if !DEBUG
			_logger.Debug (string.Format ("Query URL: {0}, and Param Secure Hide", strRequestUrl));
#else
			_logger.Debug (string.Format ("Query URL: {0}", strRequestUrl));
			_logger.Debug (string.Format ("Query Param: {0}", JsonConvert.SerializeObject (oJSonParam, Formatting.Indented)));

#endif
			string responseJson = string.Empty;

            try
            {
                //
                // Have to resolve to support tls1.2 for secure protocol
                //
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //
                //
                //

                _httpWebRequest = (HttpWebRequest)WebRequest.Create(strRequestUrl);
                _httpWebRequest.ContentType = HTTP_HEADER_CONTENTS_TYPE;
                _httpWebRequest.Accept = HTTP_HEADER_ACCEPT_TYPE;
                _httpWebRequest.Method = strMethod;
                _httpWebRequest.ProtocolVersion = HttpVersion.Version11;
                _httpWebRequest.Timeout = REQUEST_TIMEOUT;
                _httpWebRequest.ReadWriteTimeout = REQUEST_TIMEOUT;
                foreach (KeyValuePair<string, string> header in dicHeaders)
                {
                    _httpWebRequest.Headers.Add(header.Key, header.Value);
                }

                byte[] byteJSon = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(oJSonParam, Formatting.Indented));

                _httpWebRequest.ContentLength = byteJSon.Length;

                Stream dataStream = await _httpWebRequest.GetRequestStreamAsync();
                dataStream.Write(byteJSon, 0, byteJSon.Length);

                WebResponse webResponse = await _httpWebRequest.GetResponseAsync();
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);

                responseJson = await streamReader.ReadToEndAsync();

                streamReader.Close();
                dataStream.Close();
                webResponse.Close();
            }
            catch (WebException wex)
            {
                _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }

            return JObject.Parse(responseJson);
        }

        public void SendReqeust_AsyncCallback(string strRequestUrl, string strMethod, Dictionary<string, string> dicHeaders, object oJSonParam, CallbackResponse callbackResponse, int timeout = REQUEST_TIMEOUT)
        {
            try
            {

#if !DEBUG
				_logger.Debug (string.Format ("Query URL: {0}, and Param Secure Hide", strRequestUrl));
#else
				_logger.Debug (string.Format ("Query URL: {0}", strRequestUrl));
				_logger.Debug (string.Format ("Query Param: {0}", JsonConvert.SerializeObject (oJSonParam, Formatting.Indented)));

#endif


				//
				// Have to resolve to support tls1.2 for secure protocol
				//
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.MaxServicePointIdleTime = 1000; // timeout 1sec.
                //
                //
                //

                _callbackResponse = callbackResponse;
                _oRequestData = oJSonParam;
                byte[] byteJSon = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_oRequestData, Formatting.Indented));


                _httpWebRequest = (HttpWebRequest)WebRequest.Create(strRequestUrl);
                _httpWebRequest.ContentType = HTTP_HEADER_CONTENTS_TYPE;
                _httpWebRequest.Accept = HTTP_HEADER_ACCEPT_TYPE;
                _httpWebRequest.Method = strMethod;
                _httpWebRequest.ContentLength = byteJSon.Length;
                _httpWebRequest.ProtocolVersion = HttpVersion.Version11;
                _httpWebRequest.Timeout = timeout;
                _httpWebRequest.ReadWriteTimeout = timeout;
                foreach (KeyValuePair<string, string> header in dicHeaders)
                {
                    _httpWebRequest.Headers.Add(header.Key, header.Value);
                }

                _httpWebRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), _httpWebRequest);

                //_resetEventAllDone.WaitOne();
            }
            catch (WebException wex)
            {
                _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = null;
            Stream postStream = null;

            try
            {
                request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                postStream = request.EndGetRequestStream(asynchronousResult);

                // Convert the string into a byte array.
                byte[] byteJSon = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_oRequestData, Formatting.Indented));

                // Write to the request stream.
                postStream.Write(byteJSon, 0, byteJSon.Length);
                postStream?.Close();

                // Start the asynchronous operation to get the response
                request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
            }
            catch (WebException wex)
            {
                _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));
                _callbackResponse?.Invoke(null, wex);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
                _callbackResponse?.Invoke(null, ex);
            }
            finally
            {
            }
        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream streamResponse = null;
            StreamReader streamRead = null;

            try
            {
                request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                streamResponse = response.GetResponseStream();
                streamRead = new StreamReader(streamResponse);

                string resJsonString = streamRead.ReadToEnd();
                JObject resJsonObject = JObject.Parse(resJsonString);


                _callbackResponse?.Invoke(resJsonObject, null);
            }
            catch (WebException wex)
            {
                _logger.Error(string.Format("WebException[0x{0:X8}] : {1}", wex.HResult, wex.Message.ToString()));
                _callbackResponse?.Invoke(null, wex);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
                _callbackResponse?.Invoke(null, ex);
            }
            finally
            {
                // Close the stream object
                streamResponse?.Close();
                streamRead?.Close();

                // Release the HttpWebResponse
                response?.Close();

                //_resetEventAllDone.Set();
            }
        }

        private bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
