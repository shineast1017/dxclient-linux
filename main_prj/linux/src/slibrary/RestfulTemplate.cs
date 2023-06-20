using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


// Restful
// https://msdn.microsoft.com/en-us/library/jj819168.aspx
// https://stackoverflow.com/questions/10017564/url-mapping-with-c-sharp-httplistener
// https://stackoverflow.com/questions/8637856/httplistener-with-post-data
//


namespace HCVK.HCVKSLibrary
{
    public class RestfulTemplate
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        // declare class of mapping with attribute
        public class Mapping : Attribute
        {
            public string Map;
            public Mapping(string s)
            {
                Map = s;
            }
        }


        // declare response data object
        public class JSonResult
        {
            public string resultCode;
            public string resultMessage;
            public Object resultData;
        }
        public class SampleData
        {
            public string message;
            public string currentTime;
        }


        private object _oParent = null;


        private HttpsServer _httpsServer = null;
        private string _strSSLKeyPath = string.Empty;
        private string _strEncodedSSLKeyPass = string.Empty;
        private string _strInterfacePort = string.Empty;


        public RestfulTemplate()
        {
        }
        public RestfulTemplate(object oParent)
        {
            _oParent = oParent;
        }

        public string SSLKeyPath
        {
            set { _strSSLKeyPath = value; }
            get { return _strSSLKeyPath; }
        }
        public string EncodedSSLKeyPass
        {
            set { _strEncodedSSLKeyPass = value; }
            get { return _strEncodedSSLKeyPass; }
        }
        public string InterfacePort
        {
            set { _strInterfacePort = value; }
            get { return _strInterfacePort; }
        }

        public bool IsRunning
        {
            get { return _httpsServer.IsRunning; }
        }
        public Dictionary<string, string> dicHeaders
        {
            get { return _httpsServer._dicHeaders; }
        }


        public void InitializeRestfulTemplate(bool bIsEnableTest = false)
        {
            try
            {
                string strCertPass = Encoding.UTF8.GetString(Convert.FromBase64String(_strEncodedSSLKeyPass));

                List<string> requestUrls = new List<string>();
				requestUrls.Add(string.Format("https://*:{0}/", _strInterfacePort));
				//requestUrls.Add(string.Format("https://hostname.com:{0}/", _strInterfacePort));
                if (bIsEnableTest)
                    requestUrls.Add(string.Format("http://*:{0}/", int.Parse(_strInterfacePort) + 1));


                _httpsServer = new HttpsServer(CallbackRequest, requestUrls.ToArray(), _strSSLKeyPath, strCertPass, _strInterfacePort);
                _httpsServer.Run();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        public void FinalizeRestfulTemplate()
        {
            try
            {
                _httpsServer.Stop();
                _httpsServer = null;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }


        private string CallbackRequest(HttpListenerRequest request)
        {
            _logger.Debug(string.Format("Request Command : [{0}] {1}", request.HttpMethod, request.Url));
            string strReturn = string.Empty;
            try
            {
                string methodName = string.Empty;
                for (int nLoop = 0; nLoop < request.Url.Segments.Length - 1; nLoop++)
                {
                    methodName += request.Url.Segments[nLoop];
                    //_logger.Debug(string.Format("  Add method name : {0}", methodName));
                }
                methodName += request.Url.Segments[request.Url.Segments.Length - 1].Replace("/", "");
                _logger.Debug(string.Format("  Full method name : {0}", methodName));

                string strRequestBody = string.Empty;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    strRequestBody = reader.ReadToEnd();
                }
                strRequestBody = string.IsNullOrEmpty(strRequestBody) ? "{}" : strRequestBody;
                object[] @oRequestBody = { strRequestBody };

                MethodInfo method = null;
                try
                {
                    method = GetType()
                             .GetMethods()
                             .Where(mi => mi.GetCustomAttributes(true).Any(attr => attr is Mapping && ((Mapping)attr).Map == methodName))
                             .First();

                    if (method == null)
                        throw new ArgumentOutOfRangeException("methodName", "Unsupported interface..!!");
                }
                catch (Exception iex)
                {
                    _logger.Error(string.Format("Inner Exception[0x{0:X8}] : {1}", iex.HResult, iex.ToString()));

                    throw new ArgumentOutOfRangeException("methodName", "Unsupported interface..!!");
                }


                object retObject = method.Invoke(this, @oRequestBody);

                strReturn =  JsonConvert.SerializeObject(retObject, Formatting.Indented).ToString();

                // sample of getting param in request url
                //string[] strParams = request.Url
                //                        .Segments
                //                        .Skip(2)
                //                        .Select(s => s.Replace("/", ""))
                //                        .ToArray();
                //object[] @params = method.GetParameters()
                //                    .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                //                    .ToArray();
                //
                //object ret = method.Invoke(this, @params);

            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }

            return strReturn;
        }


        // sample mapping of request url
        [Mapping("HyperCloud")]
        public object Method_HyperCloud(string strRequestBody)
        {
            try
            {
                JObject jsonObject = JObject.Parse(strRequestBody);
                _logger.Debug(string.Format("RequestBody ======>\n{0}", jsonObject.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }


            JSonResult jsonResult = new JSonResult
            {
                resultCode = "0",
                resultMessage = "OK",
                resultData = new SampleData
                {
                    message = "Welcome HyperCloud..!!",
                    currentTime = string.Format("{0}", DateTime.Now)
                }
            };

            return jsonResult;
        }
    }
}
