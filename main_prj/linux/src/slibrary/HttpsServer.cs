using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


// ref.
// https://msdn.microsoft.com/en-us/library/system.net.httplistener(v=vs.110).aspx
// sample
// https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server
// https://stackoverflow.com/questions/20787051/web-server-httplistener-response
// certificate
// http://kagasu.hatenablog.com/entry/20160722/1469146477



namespace HCVK.HCVKSLibrary
{
	public class HttpsServer
	{
		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


		private readonly HttpListener _httpListener = null;
		private readonly Func<HttpListenerRequest, string> _callbackRequestMethod = null;
		private readonly string _strSecurePort = string.Empty;
		private X509Certificate2 _x509Certificate = null;
		public bool IsRunning
		{
			get { return _httpListener.IsListening; }
		}


		// add header field of  authentication token
		public Dictionary<string, string> _dicHeaders = new Dictionary<string, string>();
		public Dictionary<string, string> dicHeaders
		{
			get { return _dicHeaders; }
		}


		//
		// Need administrative previlige : strRequestUrls ==> to bind to mulitple host as localhost, *, local ip
		//
		public HttpsServer(Func<HttpListenerRequest, string> callbackRequestMethod, string[] strRequestUrls,
			string strKeyPath = "", string strKeyPW = "", string strSecurePort = "")
		{
			if (!HttpListener.IsSupported)
				throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

			// URI prefixes are required, for example 
			if (strRequestUrls == null || strRequestUrls.Length == 0)
				throw new ArgumentException("strRequestUrls");

			// A responder method is required
			if (callbackRequestMethod == null)
				throw new ArgumentException("callbackRequestMethod");

			if (!string.IsNullOrEmpty(strKeyPath) && !string.IsNullOrEmpty(strKeyPW) && !string.IsNullOrEmpty(strSecurePort))
			{
				if (!File.Exists(strKeyPath))
					throw new ArgumentException(strKeyPath);

				//AddCertificate(strKeyPath, strKeyPW, strSecurePort);
				_strSecurePort = strSecurePort;
			}

			try
			{
				//ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
				//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                
				_httpListener = new HttpListener();
				_callbackRequestMethod = callbackRequestMethod;

				foreach (string url in strRequestUrls)
				{
					_httpListener.Prefixes.Add(url);
					_logger.Debug(string.Format("Binding Request Urls : {0}", url));
				}
                
				_httpListener.Start();
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("HttpsServer Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}
		}

		private bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		private void AddCertificate(string strKeyPath, string strKeyPW, string strSecurePort)
		{
			_logger.Debug(string.Format("Run to secure mode using by {0}", strKeyPath));
			try
			{
				X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadWrite);
				_x509Certificate = new X509Certificate2(strKeyPath, strKeyPW, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
				if (!x509Store.Certificates.Contains(_x509Certificate))
				{
					// add certificate to store
					x509Store.Add(_x509Certificate);
				}
				_logger.Debug(string.Format("x509Store Location {0}", x509Store.Location));
				_logger.Debug(string.Format("x509Store Add success"));

				// create secure port
#if WIN32
				{
                    Assembly executeAssembly = Assembly.GetExecutingAssembly();
                    string  strGuid = (Attribute.GetCustomAttribute(executeAssembly, typeof(System.Runtime.InteropServices.GuidAttribute)) as System.Runtime.InteropServices.GuidAttribute).Value;
                    string strArguments = string.Format("http add sslcert ipport=0.0.0.0:{0} certhash={1} appid={2}{3}{4}",
                        strSecurePort, BitConverter.ToString(_x509Certificate.GetCertHash()).Replace("-", ""), "{", strGuid, "}");
                    _logger.Debug(string.Format("Run netsh arguments : {0}", strArguments));

                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = "netsh",
                        Arguments = strArguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                }
#else
				//On Linux
				//sudo httpcfg -add -port 4001 -p12 HCVKAService.pfx -pwd gkdlvj'!'12
#endif

				x509Store.Close();
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("AddCertificate Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}
		}

		public void Run()
		{
			try
			{
				ThreadPool.QueueUserWorkItem((o) =>
				{
				_logger.Debug(string.Format("HttpsServer Started.."));

				while (_httpListener.IsListening)
				{
					ThreadPool.QueueUserWorkItem((c) =>
					{
					var ctx = c as HttpListenerContext;
					try
					{
						string rstr = _callbackRequestMethod(ctx.Request);
						byte[] buf = Encoding.UTF8.GetBytes(rstr);

						ctx.Response.ContentLength64 = buf.Length;
						ctx.Response.OutputStream.Write(buf, 0, buf.Length);
					}
					catch (Exception ex2)
					{
						_logger.Error(string.Format("Exception: {0}", ex2.ToString()));

						string rstr = "Invalid Request..";
						byte[] buf = Encoding.UTF8.GetBytes(rstr);

						ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
						ctx.Response.ContentLength64 = buf.Length;
						ctx.Response.OutputStream.Write(buf, 0, buf.Length);
					}
					finally
					{
						foreach (KeyValuePair<string, string> header in _dicHeaders)
						{
							ctx.Response.Headers.Add(header.Key, header.Value);
						}

#if WIN32
								ctx.Response.ContentType = "application/json; charset=utf-8";
                                ctx.Response.ContentEncoding = Encoding.UTF8;
#else
//[TBD]???
#endif
                                ctx.Response.OutputStream.Close();
                            }
                        }, _httpListener.GetContext());
                        _logger.Debug(string.Format("HttpsServer Terminated.."));
                    }
                });
            }
            catch (HttpListenerException hlex)
            {
                _logger.Error(string.Format("HttpListenerException[0x{0:X8}] : {1}", hlex.HResult, hlex.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }

        }

        public void Stop()
        {
            try
            {
                //_httpListener.Stop();


                // delete secure port
                {
                    string strArguments = string.Format("http delete sslcert ipport=0.0.0.0:{0}", _strSecurePort);
                    _logger.Debug(string.Format("Run netsh arguments : {0}", strArguments));

                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = "netsh",
                        Arguments = strArguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                }


               //_httpListener.Close();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }
    }
}
