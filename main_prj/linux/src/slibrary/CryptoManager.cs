﻿using log4net;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HCVK.HCVKSLibrary
{
    public class CryptoManager
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public static string MakeHashFromStringBySHA256(string strPlaneText)
        {
            string strReturn = string.Empty;
            try
            {
                SHA256Managed sha256Managed = new SHA256Managed();
                strReturn = Convert.ToBase64String(sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(strPlaneText)));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
            return strReturn;
        }


        public static string MakeHashFromFileBySHA256(string strTargetFullPath)
        {
            string strReturn = string.Empty;
            try
            {
                StringBuilder strSHA2 = new StringBuilder();
                FileStream fileStream = new FileStream(strTargetFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] byteResult = (new SHA256CryptoServiceProvider()).ComputeHash(fileStream);
                fileStream.Close();

                for (int i = 0; i < byteResult.Length; i++)
                {
                    strSHA2.Append(byteResult[i].ToString("X2"));
                }

                strReturn = MakeHashFromStringBySHA256(strSHA2.ToString());
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
            return strReturn;
        }

        public static string EncodingBase64(string strPlainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(strPlainText));
        }
        public static string DecodingBase64(string strEncodedText)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(strEncodedText));
        }

		public static string EncryptRSA_CertPublicKey_FromFile (string certpath, string plaintext)
		{
			string encryptText = string.Empty;

			try {

				var x509 = new X509Certificate2 (File.ReadAllBytes (certpath));

				RSACryptoServiceProvider publicKeyProvider = (RSACryptoServiceProvider)x509.PublicKey.Key;

				// 암호화할 문자열을 utf8인코딩
				byte [] inbuf = (new UTF8Encoding ()).GetBytes (plaintext);
				// 암호화, padding 추가
				byte [] encbuf = publicKeyProvider.Encrypt (inbuf, true);

				// 암호화된 문자열 Base64인코딩
				encryptText = System.Convert.ToBase64String (encbuf);

			} catch (Exception ex) {
				_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
			}

			return encryptText;

		}

		static bool CompareBytearrays (byte [] a, byte [] b)
		{
			if (a.Length != b.Length)
				return false;
			int i = 0;
			foreach (byte c in a) {
				if (c != b [i])
					return false;
				i++;
			}
			return true;
		}
		/// <summary>
		/// decode x509 pem format to publickey to byte array
		/// </summary>
		/// <param name="instr"></param>
		/// <returns></returns>
		static byte [] DecodeOpenSSLPublicKey (String instr)
		{
			const String pempubheader = "-----BEGIN PUBLIC KEY-----";
			const String pempubfooter = "-----END PUBLIC KEY-----";
			String pemstr = instr.Trim ();
			byte [] binkey;
			if (!pemstr.StartsWith (pempubheader) || !pemstr.EndsWith (pempubfooter))
				return null;
			StringBuilder sb = new StringBuilder (pemstr);
			sb.Replace (pempubheader, "");  //remove headers/footers, if present
			sb.Replace (pempubfooter, "");

			String pubstr = sb.ToString ().Trim ();   //get string after removing leading/trailing whitespace

			try {
				binkey = Convert.FromBase64String (pubstr);
			} catch (System.FormatException) {       //if can't b64 decode, data is not valid
				return null;
			}
			return binkey;
		}


		/// <summary>
		/// for .net   low pem x509 publickey  using some decode 
		/// setting RSA parameter converto and setup  RSA data RSAParameters
		/// </summary>
		/// <param name="x509key"></param>
		/// <returns> RSACryptoServiceProvider </returns>
		public static RSACryptoServiceProvider DecodeX509PublicKey (byte [] x509key)
		{
			// encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
			byte [] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
			byte [] seq = new byte [15];
			// ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
			MemoryStream mem = new MemoryStream (x509key);
			BinaryReader binr = new BinaryReader (mem);    //wrap Memory Stream with BinaryReader for easy reading
			byte bt = 0;
			ushort twobytes = 0;

			try {

				twobytes = binr.ReadUInt16 ();
				if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
					binr.ReadByte ();    //advance 1 byte
				else if (twobytes == 0x8230)
					binr.ReadInt16 ();   //advance 2 bytes
				else
					return null;

				seq = binr.ReadBytes (15);       //read the Sequence OID
				if (!CompareBytearrays (seq, SeqOID))    //make sure Sequence for OID is correct
					return null;

				twobytes = binr.ReadUInt16 ();
				if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
					binr.ReadByte ();    //advance 1 byte
				else if (twobytes == 0x8203)
					binr.ReadInt16 ();   //advance 2 bytes
				else
					return null;

				bt = binr.ReadByte ();
				if (bt != 0x00)     //expect null byte next
					return null;

				twobytes = binr.ReadUInt16 ();
				if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
					binr.ReadByte ();    //advance 1 byte
				else if (twobytes == 0x8230)
					binr.ReadInt16 ();   //advance 2 bytes
				else
					return null;

				twobytes = binr.ReadUInt16 ();
				byte lowbyte = 0x00;
				byte highbyte = 0x00;

				if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
					lowbyte = binr.ReadByte ();  // read next bytes which is bytes in modulus
				else if (twobytes == 0x8202) {
					highbyte = binr.ReadByte (); //advance 2 bytes
					lowbyte = binr.ReadByte ();
				} else
					return null;
				byte [] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
				int modsize = BitConverter.ToInt32 (modint, 0);

				byte firstbyte = binr.ReadByte ();
				binr.BaseStream.Seek (-1, SeekOrigin.Current);

				if (firstbyte == 0x00) {   //if first byte (highest order) of modulus is zero, don't include it
					binr.ReadByte ();    //skip this null byte
					modsize -= 1;   //reduce modulus buffer size by 1
				}

				byte [] modulus = binr.ReadBytes (modsize);   //read the modulus bytes

				if (binr.ReadByte () != 0x02)            //expect an Integer for the exponent data
					return null;
				int expbytes = (int)binr.ReadByte ();        // should only need one byte for actual exponent data (for all useful values)
				byte [] exponent = binr.ReadBytes (expbytes);

				// ------- create RSACryptoServiceProvider instance and initialize with public key -----
				RSACryptoServiceProvider RSA = new RSACryptoServiceProvider ();
				RSAParameters RSAKeyInfo = new RSAParameters ();
				RSAKeyInfo.Modulus = modulus;
				RSAKeyInfo.Exponent = exponent;
				RSA.ImportParameters (RSAKeyInfo);
				return RSA;
			} catch (Exception) {
				return null;
			} finally { binr.Close (); }

		}

		/// <summary>
		/// publickey(x509).pem with base64
		/// </summary>
		/// <param name="publickey_x509Base64"></param>
		/// <param name="plaintext"></param>
		/// <returns></returns>
		public static string EncryptRSA_CertPublicKey_FromPublicKey (string publickey_x509Base64, string plaintext, bool on_padding)
		{
			string encryptText = "";

			try {
				// setting PEM format for publickey
				string pub_pem = "-----BEGIN PUBLIC KEY-----";
				pub_pem += publickey_x509Base64;
				pub_pem += "-----END PUBLIC KEY-----";


				var x509 = new X509Certificate2 ();

				// convert and decode publickey.pem to RSACryptoSserviceProvider 
				// ready to crypto service
				RSACryptoServiceProvider publicKeyProvider =
					DecodeX509PublicKey (DecodeOpenSSLPublicKey (pub_pem));


				//암호화할 문자열을 UFT8인코딩
				byte [] inbuf = (new UTF8Encoding ()).GetBytes (plaintext);

				if (publicKeyProvider.PublicOnly) {
					Console.WriteLine ("PublicOnly");
				}

				//암호화
				byte [] encbuf = publicKeyProvider.Encrypt (inbuf, on_padding);

				//암호화된 문자열 Base64인코딩
				encryptText = System.Convert.ToBase64String (encbuf);

				Console.WriteLine ("DEBUG: encrypted Text : {0}", encryptText);
				Console.WriteLine ("DEBUG: encrypted Text Total Langth: {0}", encryptText.Length);

			} catch (Exception e) {
				Console.WriteLine ("DEBUG: error " + e.ToString ());
			}

			return encryptText;
		}




	}
}
