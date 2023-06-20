using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using static HCVK.Request.RequestJSONParam;

namespace HCVK.Request {
	class RequestJSONParam {
		//declare for const of response header
		public const string RESPONSE_RESULT_CODE = "resultCode";
		public const string RESPONSE_RESULT_MSG = "resultMessageCode";
		public const string RESPONSE_RESULT_DATA = "resultData";


		// ------------------------------------------------------------------------------------------
		// request header
		public class JSonRequest {
			public string requestNodeName = "HCVKC";//Properties.Resources.REQUEST_NODE_NAME;
			public string requestNodeVersion=  Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			public object requestNodeData;
		}

		// ------------------------------------------------------------------------------------------

		//------------------------------------------------------------------------------------------
		// request body for HCVKD
		public class JSonDeviceInfo {
			public string busId;
		}
	}
}



namespace HCVK.HCVKSLibrary
{
    public class QuickDbusMessageClient
    {

        string _ret_data = "";
        private string sLoginId = "";
        private string sEncMsg = "";

        public string GetReponseDataString()
        {
            return _ret_data;
        }
        public string GetLoginId()
        {
            return sLoginId;
        }


        public string GetEncMsg()
        {
            return sEncMsg;
        }
        public QuickDbusMessageClient()
        { }


        /*  preset list
            // gooroom(han)
            BUS, kr.gooroom.ssohelper
            IF, kr.gooroom.ssohelper
            OBJ PATH, /kr/gooroom/ssohelper
            METHOD, do_task
                   
        */


        ////////////////////////
        //////////   Interface Defined  Section
        ////////////////////////

        /// <summary>
        /// Rule
        /// Method define  {method_name}Async
        /// Request Target Method  do_task  + Async = do_taskAsync 
        /// </summary>
        const string DEF_DBUS_IF_GOOROOM_SSOHELPER ="kr.gooroom.ssohelper";
        [DBusInterface(DEF_DBUS_IF_GOOROOM_SSOHELPER)]  // interface defined
        public interface ISSOHelper : IDBusObject
        {
            Task<string> do_taskAsync(string message);  //  dbus method name rule,Tmds.Dbus Struct
        }

        public void GetLoginInfoAsync()
        {
            string sendMsg = "{ \"task\": \"request_authinfo\"}";

            try
            {
                Task.Run(async () =>
                {
                    using (var connection = new Connection(Address.System))
                    {
                        await connection.ConnectAsync();
                        var objectPath = new ObjectPath("/kr/gooroom/ssohelper");
                        var service = "kr.gooroom.ssohelper";
                        var ISSOHelper = connection.CreateProxy<ISSOHelper>(service, objectPath);


                        var retdata = await ISSOHelper.do_taskAsync(sendMsg);

                        sEncMsg = JObject.Parse(retdata)["encMsg"].ToString();
                        //System.Diagnostics.Debug.Write(sEncMsg);
                        sLoginId = JObject.Parse(retdata)["loginId"].ToString();
                        //System.Diagnostics.Debug.Write(sLoginId);

                        _ret_data = (string)retdata;

                        Console.WriteLine("login: " + sLoginId);
                        Console.WriteLine("encMsg: " + sEncMsg);

                    }

                }).Wait();


            }
            catch
            {
                System.Diagnostics.Debug.Write("Dbus send message error ");

            }

        }
        ////////////////////////
        //////////   For Test Only
        ////////////////////////
        public void Test1Async()
        {
            Task.Run(async () =>
            {
                using (var connection = new Connection(Address.Session))
                {
                    await connection.ConnectAsync();
                    var objectPath = new ObjectPath("/kr/gooroom/ssohelper");
                    var service = "kr.gooroom.ssohelper";
                    var ISSOHelper = connection.CreateProxy<ISSOHelper>(service, objectPath);


                    var retdata = await ISSOHelper.do_taskAsync("1 2");
                    // var retdata = await ISSOHelper.do_taskAsync("Hello!!");

                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Console.WriteLine(retdata); // expected   ret  3 = 1 + 2
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                
                }

            }).Wait();
        }




        ////////////////////////
        //////////   QuickDbusMessage Client Support Function Section 
        ////////////////////////

        // example code
        /*
            
            QuickDbusMessageClient test = new QuickDbusMessageClient();

            //test.Test1Async();

            test.MethodRequestMessageAsync(
            "kr.gooroom.ssohelper",
            "/kr/gooroom/ssohelper",
            "kr.gooroom.ssohelper",
            "do_task", false, "1 2");

        */


        /// <summary>
        /// Methods the request message.
        /// </summary>
        /// <returns>The request message.</returns>
        /// <param name="system_bus">If set to <c>true</c> system bus.</param>
        /// <param name="bus_name">Bus name.</param>
        /// <param name="bus_objname">Bus objname.</param>
        /// <param name="bus_ifname">Bus ifname.</param>
        /// <param name="method">Method.</param>
        /// <param name="send_data">Send data.</param>

        public bool MethodRequestMessageSync(string bus_name,
             string bus_objname, string bus_ifname, string method, bool system_bus = false, string send_data = "")
        {


            if (bus_ifname.Length < 1 || bus_objname.Length < 1 || method.Length < 1 || bus_name.Length < 1)
                return false;


            // clear class member
            _ret_data = "";

            string ret_data = "";

            Task.Run(async () =>
            {
               
                string select_sessiontype;

                if (system_bus == true)
                    select_sessiontype = Address.System;
                else
                    select_sessiontype = Address.Session;

                try {
                 
                    using (var connection = new Connection(select_sessiontype))
                    {
                        await connection.ConnectAsync();
                        var objectPath = new ObjectPath(bus_objname);
                        var service = bus_name;
              

                        switch (bus_ifname)
                        {
                            case DEF_DBUS_IF_GOOROOM_SSOHELPER:
                                var func_poxy = connection.CreateProxy<ISSOHelper>(service, objectPath);

                                // method
                                if (method.Contains("do_task") == true)
                                {
                                    ret_data = await func_poxy.do_taskAsync(send_data);
                                }

                                // set data
                                _ret_data = ret_data;

                                break;
                            default:
                                // // some error  TODO : ERROR notice
                                Console.WriteLine("warnning: not deined interface ");
                                break;
                        }

                        Console.WriteLine(ret_data);

                    }

                    // TODO : some shared ? or queue

                    //return ret_data;

                }
                catch ( Exception e)
                {
                    Console.WriteLine("Connect Dbus Service Somerror Exepction more details :"
                     + e.ToString());
                    // some error 
                }

            }).Wait(); // task all done wait

            return true;
        }


		//-----------------------------------------------------------------------------------------------
		// DBUS
		////////////////////////
		//////////   Interface Defined  Section
		////////////////////////
		/// <summary>
		/// Rule
		/// Method define  {method_name}Async
		/// Request Target Method  bind_device  + Async = bind_deviceAsync
		// DBUS Type :  System Bus
		// DBUS Name(IF) : com.nhncrossent.usbip
		// DBUS Path : /com/nhncrossent/usbip
		/// </summary>
		[DBusInterface ("com.nhncrossent.usbip")]  // interface defined
		public interface IDeviceBind : IDBusObject {
			Task<string> bind_deviceAsync (string message);  //  dbus method name rule,Tmds.Dbus Struct
		}

		public bool SetDeviceBindAsync (string strBusId)
		{
			bool bRetVal = false;

		  JSonRequest oParam = new JSonRequest {
				requestNodeData = new JSonDeviceInfo {
					busId = strBusId
				}
			};

			string sendMsg = string.Format ("{0}", JsonConvert.SerializeObject (oParam, Formatting.Indented));

			try {
				Task.Run (async () => {
					using (var connection = new Connection (Address.System)) {
						await connection.ConnectAsync ();
						var objectPath = new ObjectPath ("/com/nhncrossent/usbip");
						var service = "com.nhncrossent.usbip";
						var DevBindProxy = connection.CreateProxy<IDeviceBind> (service, objectPath);


						var retdata = await DevBindProxy.bind_deviceAsync (sendMsg);

						if (retdata != null) {
							//_logger.Debug (string.Format ("response : {0}", retdata));

							string resultCode = ParserGetResultCodeFromJsonData (retdata);

							if (resultCode.Equals ("0") != true) {
								//_logger.Debug (string.Format ("SetDeviceBindAsync failure response : {0}", resultCode));
							} else {
								bRetVal = true;
							}

						}
					}

				}).Wait ();

			} catch {
				System.Diagnostics.Debug.Write ("Dbus send message error ");
			}

			return bRetVal;

		}
		/// <summary>
		/// Rule
		/// Method define  {method_name}Async
		/// Request Target Method  bind_device  + Async = bind_deviceAsync
		// DBUS Type :  System Bus
		// DBUS Name(IF) : com.nhncrossent.usbip
		// DBUS Path : /com/nhncrossent/usbip
		/// </summary>
		[DBusInterface ("com.nhncrossent.usbip")]  // interface defined
		public interface IDeviceUnBind : IDBusObject {
			Task<string> unbind_deviceAsync (string message);  //  dbus method name rule,Tmds.Dbus Struct
		}

		public bool SetDeviceUnbindAsync (string strBusId)
		{
			bool bRetVal = false;

			JSonRequest oParam = new JSonRequest {
				requestNodeData = new JSonDeviceInfo {
					busId = strBusId
				}
			};

			string sendMsg = string.Format ("{0}", JsonConvert.SerializeObject (oParam, Formatting.Indented));

			try {
				Task.Run (async () => {
					using (var connection = new Connection (Address.System)) {
						await connection.ConnectAsync ();
						var objectPath = new ObjectPath ("/com/nhncrossent/usbip");
						var service = "com.nhncrossent.usbip";
						var DevUnbindProxy = connection.CreateProxy<IDeviceUnBind> (service, objectPath);

						var retdata = await DevUnbindProxy.unbind_deviceAsync (sendMsg);

						if (retdata != null) {
							//_logger.Debug (string.Format ("response : {0}", retdata));

							string resultCode = ParserGetResultCodeFromJsonData (retdata);

							if (resultCode.Equals ("0") != true) {
								//_logger.Debug (string.Format ("SetDeviceUnbindAsync failure response : {0}", resultCode));
							} else {
								bRetVal = true;
							}

						}
					}

				}).Wait ();

			} catch {
				System.Diagnostics.Debug.Write ("Dbus send message error ");
			}

			return bRetVal;

		}

		public string ParserGetResultCodeFromJsonData (string Jdata)
		{
			string resultCode = "";

			try {
				resultCode = JObject.Parse (Jdata) ["resultCode"].ToString ();
			} catch {
				//_logger.Error (string.Format ("ParserGetResultCodeFromJsonData Error"));
			}

			return resultCode;
		}


	}
}
