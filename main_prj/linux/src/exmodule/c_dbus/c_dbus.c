/*
 *    c_dbus.c  
 *    crossent support dbus interface(IPC) module for mono or other project
 *    
 *    feature 1   dbus client request msg
 *    bustype(system, session) 
 *    
 */

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h> 
#include <string.h>
#include <stdbool.h>
#include <ctype.h>

#include <dbus/dbus.h>

/*
const char *const INTERFACE_NAME = "kr.gooroom.ssohelper";
const char *const SERVER_BUS_NAME = "kr.gooroom.ssohelper";
const char *const CLIENT_BUS_NAME = "kr.gooroom.ssohelper";
const char *const SERVER_OBJECT_PATH_NAME = "/kr/gooroom/ssohelper";
const char *const METHOD_NAME = "do_task";
*/

DBusError dbus_error;
void print_dbus_error (char *str);


// for test
char _tbuf[1024] = {0, };
char* Get_Dbus_MsgRet(char *name, int lang) {


        if (name == NULL || lang == 0)
                return "";


        memset(_tbuf, 0, 1024);
        strncpy(_tbuf, name, lang);

        printf(_tbuf);

        return _tbuf;
}



// for test
char* Get_Dbus_Msg(char *name, int age) {

  return "Talk Hello";
}


// for test
char* Get_Dbus_Msg1() {

  return "Talk Hello";
}


// for test
int  Get_Dbus_Msg2() {

  return 100;
}



/////
// Check Vaild BusName
// 
// input bus type, bus name, bus name lang
// ret int check type
int RequestCheckValidBusName(int bustype, char* bus_name, int busn_lang) {


        // check safe

        // safe check
        if (bus_name == NULL || busn_lang == 0) {

                // check fail ret -1  nothing
                return -1;
        }

        // set value

        char b_name[256] = {0, };
        strncpy(b_name, bus_name,  busn_lang);


        int ret = 0;
        DBusConnection *conn;
        dbus_error_init (&dbus_error);

        if (bustype == 0 )  { 
                // bustype 0 is SESSION type
                //
                conn = dbus_bus_get (DBUS_BUS_SESSION, &dbus_error);
        } else {  

                // bustype 1 is SYSTEM type
                //
                conn = dbus_bus_get (DBUS_BUS_SYSTEM, &dbus_error);
        }


        if (dbus_error_is_set (&dbus_error))
                print_dbus_error ("dbus_bus_get");

        if (!conn) 
                return -1;


        /*
         *       DBUS_REQUEST_NAME_REPLY_PRIMARY_OWNER 
         *       DBUS_REQUEST_NAME_REPLY_IN_QUEUE
         *
         * */

        ret = dbus_bus_request_name (conn, b_name, 0, &dbus_error);

        return ret;
}


/////
// Get Request D Bus Send And Response 
// work Method
//
// input bus type, bus name, bus name lang,
// bus obj path, bus obj lang,
// bus interface name, businterlang
// bus method name, methodlang
// send data, send data lang
// 
// ret data chars

char* RequestDbusMsg(int bustype, char* bus_name, int busn_lang,
                char* bus_objp, int bus_objp_lang,
                char* bus_if_name, int bus_if_lang,
                char* method, int method_lang,
                char* send_data, int send_data_lang) {

        // safe check
        if (bus_name == NULL || busn_lang == 0 ||
            bus_objp == NULL || bus_objp_lang == 0 ||
            bus_if_name == NULL || bus_if_lang == 0 ||
            method == NULL || method_lang == 0 ||
            send_data == NULL || send_data_lang == 0 || 
            send_data_lang >= 256 *2 ) {

                // check fail ret ""  nothing
                return "";
        }

        static char ret_buf[1024]= {0, };  // ret buf
        char b_name[256] = {0, };  // bus name
        char b_ifname[256] = {0, };  // bus interface  name
        char bo_name[256] = {0, }; // bus object name path
        char b_mname[256] = {0, }; // bus method name
        char b_sendd[256 * 2] = {0, }; // bus send data
        strncpy(b_name, bus_name,  busn_lang);      // SERVER_BUS_NAME
        strncpy(bo_name, bus_objp,  bus_objp_lang); // SERVER_OBJECT_PATH_NAME
        strncpy(b_ifname, bus_if_name,  bus_if_lang); //  SERVER BUS INTEFACE NAME
        strncpy(b_mname, method,  method_lang);       // METHOD_NAME
        strncpy(b_sendd, send_data,  send_data_lang); // SEND_DATANAME


        memset(ret_buf, 0, sizeof(ret_buf));

        DBusConnection *conn;

        dbus_error_init (&dbus_error);

        if (bustype == 0 )  { 
                // bustype 0 is SESSION type
                //
                conn = dbus_bus_get (DBUS_BUS_SESSION, &dbus_error);
        } else {  

                // bustype 1 is SYSTEM type
                //
                conn = dbus_bus_get (DBUS_BUS_SYSTEM, &dbus_error);
        }

        if (dbus_error_is_set (&dbus_error)) {

                print_dbus_error ("dbus_bus_get");
        }

        if (!conn)  {
                // TODO :Error Handle
                return "";
        }


        
        DBusMessage *request;

        if ((request = dbus_message_new_method_call (
                                        b_name,               // bus
                                        bo_name,              // bus object
                                        b_ifname,            // bus interface 
                                        b_mname)) == NULL) {  // bus method

            fprintf (stderr, "Error in dbus_message_new_method_call\n");
            return ""; // ret ""   is error
        }

        DBusMessageIter iter;
        dbus_message_iter_init_append (request, &iter);
        char *ptr = b_sendd;   //  bus send data pointer, addr set 
        if (!dbus_message_iter_append_basic (&iter, DBUS_TYPE_STRING, &ptr)) {

            fprintf (stderr, "Error in dbus_message_iter_append_basic\n");
            return ""; // ret "" is error  message iter append error
        }


        DBusPendingCall *pending_return;
        if (!dbus_connection_send_with_reply (conn, request, &pending_return, -1)) {
            fprintf (stderr, "Error in dbus_connection_send_with_reply\n");
                
            return ""; // ret "" is error message send with reply 
        }

        if (pending_return == NULL) {
            fprintf (stderr, "pending return is NULL");

            return ""; // ret "" is error pending return is NULL
        }

        dbus_connection_flush (conn);
                
        dbus_message_unref (request);	

        dbus_pending_call_block (pending_return);

        DBusMessage *reply;
        if ((reply = dbus_pending_call_steal_reply (pending_return)) == NULL) {
            fprintf (stderr, "Error in dbus_pending_call_steal_reply");

            return "";  // ret "" is error pendding call steal reply
        }

        dbus_pending_call_unref	(pending_return);

        char *s = NULL;
        if (dbus_message_get_args (reply, &dbus_error, DBUS_TYPE_STRING, &s, DBUS_TYPE_INVALID)) {

                printf ("DEBUG: %s\n", s);

                strncpy(ret_buf, s, strlen(s));

        } else {
             fprintf (stderr, "Did not get arguments in reply\n");

             return ""; // ret "" is error arguments in reply
        }

        dbus_message_unref (reply);	

        if (dbus_bus_release_name (conn, b_name, &dbus_error) == -1) {

                fprintf (stderr, "Error in dbus_bus_release_name\n");
                return ""; // ret "' is error in dbus bus release name
        }

        // printf ("close connect!!! done"); 

        return ret_buf;
}

void print_dbus_error (char *str) 
{
        fprintf (stderr, "%s: %s\n", str, dbus_error.message);
        dbus_error_free (&dbus_error);
}


// not use
int main ()
{
        return 0;
}

