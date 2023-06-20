
using System;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography.X509Certificates;


namespace HCVK.HCVKSLibrary {
	public class StoredCertManager {


		public StoredCertManager()
		{

		}


		// download save certificate from url
		public void SaveCertificateFile(string url, string path)
		{
			try {
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
				request.AllowAutoRedirect = false;
				request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

				HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
				response.Close ();

				X509Certificate cert = new X509Certificate (request.ServicePoint.Certificate);

				if (path.Length > 1) {
					StreamWriter sw = File.CreateText (path);
					sw.Write (ExportToPEM (cert));
					sw.Flush ();
					sw.Close ();
				}

			} catch {

			}


		}

		// Export a certificate to a PEM format sstring
		public string	ExportToPEM(X509Certificate _cert)
		{
			StringBuilder builder = new StringBuilder ();

			try {
				builder.AppendLine ("-----BEGIN CERTIFICATE-----");
				builder.AppendLine (Convert.ToBase64String(_cert.Export(X509ContentType.Cert),Base64FormattingOptions.InsertLineBreaks));
				builder.AppendLine ("-----END CERTIFICATE-----");


			} catch {

			}

			return builder.ToString ();
		}

	}
}