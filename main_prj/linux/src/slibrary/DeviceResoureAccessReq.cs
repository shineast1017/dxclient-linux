using System;
using HCVK.HCVKSLibrary;

namespace HCVK.HCVKSLibrary
{
    public class DeviceResoureAccessReq
    {

#pragma warning disable CS8321 // Local function is declared but never used


            /// <summary>
            /// Accesses the request KVM 2 daa s.
            /// </summary>
            /// <returns><c>true</c>, if request KVM 2 daa s was accessed, <c>false</c> otherwise.</returns>
            public static bool AccessRequest_KVM2DaaS()
            {
#if DEBUG
                Console.WriteLine("Run Function : " + System.Reflection.MethodBase.GetCurrentMethod().Name);

#endif
                const string DEF_DAAS2KVM_SH_CMD_SET_OFF = "echo 0 > /var/tmp/GRM-KVM-USB-REDIRECT-SWITCH";
                const string DEF_DAAS2KVM_SH_CMD_SET_ONE = "echo 1 > /var/tmp/GRM-DaaS-USB-REDIRECT-SWITCH";

                ProcessManager pm = new ProcessManager();

                try { 

                    pm.ExecuteBashCommand(DEF_DAAS2KVM_SH_CMD_SET_OFF);
                    // TODO :  some check
                    pm.ExecuteBashCommand(DEF_DAAS2KVM_SH_CMD_SET_ONE);

                } catch (Exception e)
                {
                    Console.WriteLine("execute execption, command : KVM2DaaS, more detail :" + e.ToString());
                }
                return true;
            }


            /// <summary>
            /// Accesses the request DaaS 2 KVM.
            /// </summary>
            /// <returns><c>true</c>, if request daa s2 kvm was accessed, <c>false</c> otherwise.</returns>
            public static bool AccessRequest_DaaS2KVM(bool bDAAS_Set_Off, bool bKVM_Set_On)

            {

#if DEBUG
                Console.WriteLine("Run Function : " + System.Reflection.MethodBase.GetCurrentMethod().Name);

#endif

                const string DEF_KVM2DAAS_SH_CMD_SET_OFF = "echo 0 > /var/tmp/GRM-DaaS-USB-REDIRECT-SWITCH";
                const string DEF_KVM2DAAS_SH_CMD_SET_ONE = "echo 1 > /var/tmp/GRM-KVM-USB-REDIRECT-SWITCH";

                ProcessManager pm = new ProcessManager();

                try {
                 
                    if (bDAAS_Set_Off)
                    	pm.ExecuteBashCommand(DEF_KVM2DAAS_SH_CMD_SET_OFF);
                	// TODO :  some check
                    if (bKVM_Set_On)
                    	pm.ExecuteBashCommand(DEF_KVM2DAAS_SH_CMD_SET_ONE);

                } catch (Exception e)
                {
                    Console.WriteLine("execute execption, command : DaaS2KVM, more detail :" + e.ToString());
                }
                return true;
            }

#pragma warning restore CS8321 // Local function is declared but never used
    }
}
