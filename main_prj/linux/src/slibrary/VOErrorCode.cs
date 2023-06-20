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
		public const string _E_CODE_B_B0002006 = "B0002006"; // 계정잠금 상태
		public const string _E_CODE_B_B0002007 = "B0002007"; // 마지막 로그인 후 30일 초과, 비밀번호 재설정 필요
		public const string _E_CODE_B_B0002008 = "B0002008"; // 비밀번호 유효기간 초과, 비밀번호 재설정 필요
		public const string _E_CODE_B_B0002010 = "B0002010"; // 최초로그인 또는 비밀번호 초기화 상태, 비밀번호 재설정 필요
		public const string _E_CODE_B_B0003001 = "B0003001";

        public const string _E_CODE_B_B0004001 = "B0004001"; //현재 비밀번호를 정확하게 입력해 주세요
        public const string _E_CODE_B_B0004002 = "B0004002"; // 비밀번호 재작성이 필요합니다. (9자 이상 영문대소문자,숫자 및 특수문자 사용)
		public const string _E_CODE_B_B0004003 = "B0004003"; // 비밀번호 재작성이 필요합니다.(동일한 문자,숫자의 연속적인  반복입력)
		public const string _E_CODE_B_B0004004 = "B0004004"; // 비밀번호 재작성이 필요합니다.(키보드상의연속된 문자 또는 숫자의 순차적 입력)
		public const string _E_CODE_B_B0004005 = "B0004005"; // 현재 비밀번호와 동일합니다.
		public const string _E_CODE_B_B0004006 = "B0004006"; // 사용자ID와 동일합니다
		public const string _E_CODE_B_B0004007 = "B0004007"; // 초기 비밀번호와 동일합니다
		public const string _E_CODE_B_B0004008 = "B0004008"; // 최근 사용된 비밀번호입니다. 재작성이 필요합니다.
		public const string _E_CODE_B_B0004009 = "B0004009"; // 비밀번호 변경 처리시 오류가 발생되었습니다.

		public const string _E_CODE_B_B0005001 = "B0005001";

        public const string _E_CODE_B_B0006001 = "B0006001";

        public const string _E_CODE_B_B0007001 = "B0007001";

        public const string _E_CODE_B_B0008001 = "B0008001";


        public const string _E_CODE_G_G0000001 = "G0000001";        // failed retrived recommend server

        public const string _E_CODE_G_G0001001 = "G0001001";        // fail to get service ip

        public const string _E_CODE_G_G0009001 = "G0009001";        // request timeout with hcVKA
        public const string _E_CODE_G_G0009999 = "G0009999";        // unknown error

		// declare Error codes for GPMS Login Process
		public const string _E_CODE_GPMS_GRTP100 = "GRTP100";
		public const string _E_CODE_GPMS_GRTP120 = "GRTP120";
		public const string _E_CODE_GPMS_GRTP121 = "GRTP121";
		public const string _E_CODE_GPMS_GRTP401 = "GRTP401";
		public const string _E_CODE_GPMS_GRTP112 = "GRTP112";
		public const string _E_CODE_GPMS_GRTP500 = "GRTP500";

		// declare Error codes for USBIP MODULE
		public const string _E_CODE_AU_000001 = "AU000001";
		public const string _E_CODE_AU_000002 = "AU000002";
		public const string _E_CODE_AU_000003 = "AU000003";
		public const string _E_CODE_AU_000101 = "AU000101";
		public const string _E_CODE_AU_000102 = "AU000102";
		public const string _E_CODE_AU_000103 = "AU000103";
		public const string _E_CODE_AU_000201 = "AU000201";
		public const string _E_CODE_D_USBE001 = "USBE001";
		public const string _E_CODE_D_USBE002 = "USBE002";
		public const string _E_CODE_D_USBE003 = "USBE003";
		public const string _E_CODE_C_0000011 = "C0000011";
		public const string _E_CODE_C_0000012 = "C0000012";
		public const string _E_CODE_C_0000013 = "C0000013";

		// declare error message
		public const string _E_MSG_OK = "OK";
        public const string _E_MSG_SUCCESS = "Success";

        public const string _E_MSG_FAIL = "Failed";
        public const string _E_MSG_CHECKING = "Checking";
        public const string _E_MSG_READY_SERVICE = "Not ready to service";
        public const string _E_MSG_NOT_SUPPORT_NODE_VERSION = "Not supported node version";

        public const string _E_MSG_A_A0000101 = "not registration";
        public const string _E_MSG_A_A0000102 = "not re-registration";
        public const string _E_MSG_A_A0000103 = "not running RDP Service";
        public const string _E_MSG_A_A0000104 = "stop service of agent";
        public const string _E_MSG_A_A0000105 = "failed joinning to domain";
        public const string _E_MSG_X_X0000201 = "integirty failure";




	}
}
