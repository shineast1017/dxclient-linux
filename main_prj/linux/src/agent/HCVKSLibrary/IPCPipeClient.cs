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
    public class IPCPipeClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public void Send(string strSendData, string strPipeName, int nTimeOut = 1000)
        {
            try
            {
                NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", strPipeName, PipeDirection.Out, PipeOptions.Asynchronous);

                // The connect function will indefinitely wait for the pipe to become available
                // If that is not acceptable specify a maximum waiting time (in ms)
                pipeStream.Connect(nTimeOut);

                byte[] _buffer = Encoding.UTF8.GetBytes(strSendData);
                pipeStream.BeginWrite(_buffer, 0, _buffer.Length, AsyncSend, pipeStream);
            }
            catch (TimeoutException tex)
            {
                _logger.Error(string.Format("Timeout Exception : {0}", tex.Message.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }

        private void AsyncSend(IAsyncResult iar)
        {
            try
            {
                // Get the pipe
                NamedPipeClientStream pipeStream = (NamedPipeClientStream)iar.AsyncState;

                // End the write
                pipeStream.EndWrite(iar);
                pipeStream.Flush();
                pipeStream.Close();
                pipeStream.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
            }
        }
    }
}
