using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HCVK.HCVKSLibrary
{
    public class ProcessManager
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SECURITY_ATTRIBUTES
        {
            public uint nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public uint cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        internal enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        internal enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx", SetLastError = true)]
        private static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            uint dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            Int32 ImpersonationLevel,
            Int32 dwTokenType,
            ref IntPtr phNewToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(
            IntPtr ProcessHandle,
            UInt32 DesiredAccess,
            ref IntPtr TokenHandle);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool CreateEnvironmentBlock(
            ref IntPtr lpEnvironment,
            IntPtr hToken,
            bool bInherit);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool DestroyEnvironmentBlock(
            IntPtr lpEnvironment);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(
            IntPtr hObject);


        private const short SW_SHOW = 5;
        private const uint TOKEN_QUERY = 0x0008;
        private const uint TOKEN_DUPLICATE = 0x0002;
        private const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        private const int GENERIC_ALL_ACCESS = 0x10000000;
        private const int STARTF_USESHOWWINDOW = 0x00000001;
        private const int STARTF_FORCEONFEEDBACK = 0x00000040;
        private const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;
        private const int MAXSIZE = 16384; // size _does_ matter


        private bool LaunchProcessAsUser(string cmdLine, IntPtr token, IntPtr envBlock)
        {
            bool result = false;

            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
            SECURITY_ATTRIBUTES saProcess = new SECURITY_ATTRIBUTES();
            SECURITY_ATTRIBUTES saThread = new SECURITY_ATTRIBUTES();
            saProcess.nLength = (uint)Marshal.SizeOf(saProcess);
            saThread.nLength = (uint)Marshal.SizeOf(saThread);

            STARTUPINFO si = new STARTUPINFO();
            si.cb = (uint)Marshal.SizeOf(si);


            //if this member is NULL, the new process inherits the desktop 
            //and window station of its parent process. If this member is 
            //an empty string, the process does not inherit the desktop and 
            //window station of its parent process; instead, the system 
            //determines if a new desktop and window station need to be created. 
            //If the impersonated user already has a desktop, the system uses the 
            //existing desktop. 

            si.lpDesktop = @"WinSta0\Default"; //Modify as needed 
            si.dwFlags = STARTF_USESHOWWINDOW | STARTF_FORCEONFEEDBACK;
            si.wShowWindow = SW_SHOW;
            //Set other si properties as required. 

            result = CreateProcessAsUser(
                token,
                null,
                cmdLine,
                ref saProcess,
                ref saThread,
                false,
                CREATE_UNICODE_ENVIRONMENT,
                envBlock,
                null,
                ref si,
                out pi);


            if (result == false)
            {
                _logger.Debug(string.Format("CreateProcessAsUser Error: {0}", Marshal.GetLastWin32Error()));
            }

            return result;
        }


        private IntPtr GetPrimaryToken(int processId)
        {
            IntPtr token = IntPtr.Zero;
            IntPtr primaryToken = IntPtr.Zero;
            bool retVal = false;
            Process p = null;

            try
            {
                p = Process.GetProcessById(processId);
            }
            catch (ArgumentException)
            {
                _logger.Error(string.Format("ProcessID {0} Not Available", processId));
                throw;
            }


            //Gets impersonation token 
            retVal = OpenProcessToken(p.Handle, TOKEN_DUPLICATE, ref token);
            if (retVal == true)
            {
                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
                sa.nLength = (uint)Marshal.SizeOf(sa);

                //Convert the impersonation token into Primary token 
                retVal = DuplicateTokenEx(
                    token,
                    TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_QUERY,
                    ref sa,
                    (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                    (int)TOKEN_TYPE.TokenPrimary,
                    ref primaryToken);

                //Close the Token that was previously opened. 
                CloseHandle(token);
                if (retVal == false)
                {
                    _logger.Debug(string.Format("DuplicateTokenEx Error: {0}", Marshal.GetLastWin32Error()));
                }
            }
            else
            {
                _logger.Debug(string.Format("OpenProcessToken Error: {0}", Marshal.GetLastWin32Error()));
            }

            //We'll Close this token after it is used. 
            return primaryToken;
        }

        private IntPtr GetEnvironmentBlock(IntPtr token)
        {

            IntPtr envBlock = IntPtr.Zero;
            bool retVal = CreateEnvironmentBlock(ref envBlock, token, false);
            if (retVal == false)
            {
                //Environment Block, things like common paths to My Documents etc. 
                //Will not be created if "false" 
                //It should not adversley affect CreateProcessAsUser. 
                _logger.Debug(string.Format("CreateEnvironmentBlock Error: {0}", Marshal.GetLastWin32Error()));

            }
            return envBlock;
        }

        private string GetProcessInfoByPID(int PID, out string User, out string Domain)//, out string OwnerSID)
        {
            User = string.Empty;
            Domain = string.Empty;
            string OwnerSID = string.Empty;
            string processname = string.Empty;
            try
            {
                ObjectQuery sq = new ObjectQuery("Select * from Win32_Process Where ProcessID = '" + PID + "'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);
                if (searcher.Get().Count == 0)
                    return "Unknown";

                foreach (ManagementObject oReturn in searcher.Get())
                {
                    //Name of process
                    //arg to send with method invoke to return user and domain - below is link to SDK doc on it

                    string[] o = new String[2];
                    //Invoke the method and populate the o var with the user name and domain
                    oReturn.InvokeMethod("GetOwner", (object[])o);

                    //int pid = (int)oReturn["ProcessID"];
                    processname = (string)oReturn["Name"];
                    //dr[2] = oReturn["Description"];
                    User = o[0];
                    if (User == null)
                        User = string.Empty;

                    Domain = o[1];
                    if (Domain == null)
                        Domain = string.Empty;

                    string[] sid = new String[1];
                    oReturn.InvokeMethod("GetOwnerSid", (object[])sid);
                    OwnerSID = sid[0];

                    return OwnerSID;
                }
            }
            catch
            {
                return OwnerSID;
            }
            return OwnerSID;
        }

        public bool LaunchProcessAsCurrentSession(string appCmdLine /*,int processId*/)
        {
            bool ret = false;

            try
            {
                //Either specify the processID explicitly 
                //Or try to get it from a process owned by the user. 
                //In this case assuming there is only one explorer.exe 

                Process[] ps = Process.GetProcessesByName("explorer");
                int processId = -1;//=processId 
                if (ps.Length > 0)
                {
                    WindowsIdentity wiUser = WindowsIdentity.GetCurrent();
                    _logger.Debug(string.Format("Current User[{0}], SID[{1}]", wiUser.Name, wiUser.User.Value));


                    foreach (Process process in ps)
                    {
                        string strUser = string.Empty;
                        string strDomain = string.Empty;
                        string strOwnerSID = string.Empty;

                        strOwnerSID = GetProcessInfoByPID(process.Id, out strUser, out strDomain);
                        _logger.Debug(string.Format("Process[{0}, {1}], User[{2}], Domain[{3}], SID[{4}]",
                            process.ProcessName, process.Id, strUser, strDomain, strOwnerSID));

                        if (strOwnerSID.Equals(wiUser.User.Value, StringComparison.CurrentCultureIgnoreCase))
                        {
                            processId = process.Id;
                            break;
                        }
                    }

                    processId = ps[0].Id;
                }

                if (processId > 1)
                {
                    IntPtr token = GetPrimaryToken(processId);

                    if (token != IntPtr.Zero)
                    {
                        IntPtr envBlock = GetEnvironmentBlock(token);
                        ret = LaunchProcessAsUser(appCmdLine, token, envBlock);
                        if (envBlock != IntPtr.Zero)
                            DestroyEnvironmentBlock(envBlock);

                        CloseHandle(token);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception:{0}", ex.Message));
            }
            return ret;
        }

        public bool CheckProcessAlive(string strProcessName)
        {
            bool ret = false;

            try
            {
                Process[] ps = Process.GetProcessesByName(strProcessName);
                if (ps.Length > 0)
                    ret = true;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception:{0}", ex.Message));
            }
            return ret;
        }

        public bool KillAllProcessByName(string strProcessName)
        {
            bool ret = false;

            try
            {
                Process[] ps = Process.GetProcessesByName(strProcessName);
                if (ps.Length > 0)
                {
                    _logger.Debug(string.Format("Found process name: {0}", strProcessName));
                    foreach (Process process in ps)
                    {
                        process.Kill();
                    }

                    ret = true;
                }
                else
                {
                    _logger.Debug(string.Format("Not found process name: {0}", strProcessName));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception:{0}", ex.Message));
            }
            return ret;
        }


        /// <summary>
        /// Run the bash command.
        /// </summary>
        /// <returns>The bash command.</returns>
        /// <param name="cmd">Cmd.</param>
       public string ExecuteBashCommand(string cmd)
        {
            cmd = cmd.Replace("\"", "\"\"");

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + cmd + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }
    }
}
