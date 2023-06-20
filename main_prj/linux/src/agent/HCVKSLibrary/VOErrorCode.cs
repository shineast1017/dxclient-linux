using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary.VO
{
    public class VOErrorCode
    {
        // declare for common error code
        public const string _E_CODE_OK = "0";
        public const string _E_CODE_X_X0009999 = "X0009999";        // unknown failed

        // declare for error of integirty
        public const string _E_CODE_X_X0000201 = "X0000201";        // integirty failure
        public const string _E_CODE_X_X0000202 = "X0000202";        // no response from agent
        public const string _E_CODE_X_X0000203 = "X0000203";        // not support node version

        // declare for error of first login
        public const string _E_CODE_C_C0000001 = "C0000001";        // first login
        public const string _E_CODE_C_C0000002 = "C0000002";        // need re-login
        public const string _E_CODE_C_C0000003 = "C0000003";        // retry timeout
        public const string _E_CODE_C_C0000004 = "C0000004";        // no info. for connecting to desktop
        public const string _E_CODE_C_C0000005 = "C0000005";        // no access previlites for connecting to desktop


        // declare for error of ready service
        public const string _E_CODE_A_A0000001 = "A0000001";        // not ready service 

        // declare for error of registred to HCVKB
        public const string _E_CODE_A_A0000101 = "A0000101";        // not registration
        public const string _E_CODE_A_A0000102 = "A0000102";        // not re-registration
        public const string _E_CODE_A_A0000103 = "A0000103";        // not running RDP Service
        public const string _E_CODE_A_A0000104 = "A0000104";        // stop service of agent
        public const string _E_CODE_A_A0000105 = "A0000105";        // failed joinning to domain


        // declare for error code from HCVKB 
        public const string _E_CODE_B_B0001001 = "B0001001";
        public const string _E_CODE_B_B0001002 = "B0001002";
        public const string _E_CODE_B_B0001003 = "B0001003";
        public const string _E_CODE_B_B0001004 = "B0001004";
        public const string _E_CODE_B_B0001005 = "B0001005";

        public const string _E_CODE_B_B0002001 = "B0002001";
        public const string _E_CODE_B_B0002002 = "B0002002";
        public const string _E_CODE_B_B0002003 = "B0002003";
        public const string _E_CODE_B_B0002004 = "B0002004";
        public const string _E_CODE_B_B0002005 = "B0002005";

        public const string _E_CODE_B_B0003001 = "B0003001";

        public const string _E_CODE_B_B0004001 = "B0004001";
        public const string _E_CODE_B_B0004002 = "B0004002";

        public const string _E_CODE_B_B0005001 = "B0005001";

        public const string _E_CODE_B_B0006001 = "B0006001";

        public const string _E_CODE_B_B0007001 = "B0007001";

        public const string _E_CODE_B_B0008001 = "B0008001";


        public const string _E_CODE_G_G0000001 = "G0000001";        // failed retrived recommend server

        public const string _E_CODE_G_G0001001 = "G0001001";        // fail to get service ip

        public const string _E_CODE_G_G0009001 = "G0009001";        // request timeout with hcVKA
        public const string _E_CODE_G_G0009999 = "G0009999";        // unknown error


        // declare error message
        public const string _E_MSG_OK = "OK";
        public const string _E_MSG_SUCCESS = "Success";

        public const string _E_MSG_FAIL = "Failed";
        public const string _E_MSG_CHECKING = "Checking";
        public const string _E_MSG_READY_SERVICE = "Not ready to service";
        public const string _E_MSG_NOT_SUPPORT_NODE_VERSION = "Not supported node version";
    }
}
