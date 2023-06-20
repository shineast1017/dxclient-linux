using log4net;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

// ref
// IPC using by Pipe
// https://www.codeproject.com/Tips/492231/Csharp-Async-Named-Pipes
//

namespace HCVK.HCVKSLibrary
{
    public delegate void DelegateMessage(string strReply);


    public class IPCPipeServer
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event DelegateMessage PipeMessage;
        private string _strPipeName;


        public void Listen(string strPipeName)
        {
            try
            {
                // Set to class level var so we can re-use in the async callback method
                _strPipeName = strPipeName;
                // Create the new async pipe 
                NamedPipeServerStream pipeServer = new NamedPipeServerStream(strPipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                // Wait for a connection
                pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        private void WaitForConnectionCallBack(IAsyncResult iar)
        {
            try
            {
                // Get the pipe
                NamedPipeServerStream pipeServer = (NamedPipeServerStream)iar.AsyncState;
                // End waiting for the connection
                pipeServer.EndWaitForConnection(iar);

                byte[] buffer = new byte[255];

                // Read the incoming message
                int nReadBytes = pipeServer.Read(buffer, 0, 255);

                // Convert byte buffer to string
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, nReadBytes)];
                decoder.GetChars(buffer, 0, nReadBytes, chars, 0);
                string strReceivedData = new string(chars);
                //_logger.Debug(string.Format("Received Data : {0}", strReceivedData));

                // Pass message back to calling form
                PipeMessage.Invoke(strReceivedData);

                // Kill original sever and create new wait server
                pipeServer.Close();
                pipeServer = null;
                pipeServer = new NamedPipeServerStream(_strPipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                // Recursively wait for the connection again and again....
                pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }
    }
}
