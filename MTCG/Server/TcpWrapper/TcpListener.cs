using System.Net;

namespace MTCG.Server.TcpWrapper
{
    /// <summary>
    /// standard TCP listener wrapper
    /// </summary>
    public class TcpListener : ITcpListener
    {
        /// <summary>
        /// underlying TCP listener
        /// </summary>
        private readonly System.Net.Sockets.TcpListener _myListener;
        
        /// <summary>
        /// call TCP listener's start method
        /// </summary>
        public void Start() => _myListener.Start();
        
        /// <summary>
        /// call TCP listener's stop method
        /// </summary>
        public void Stop() => _myListener.Stop();
        
        /// <summary>
        /// start a listener that operates on a a given ip and port
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public TcpListener(string ip, int port)
        {
            var ipAddress = IPAddress.Parse(ip);
            _myListener = new System.Net.Sockets.TcpListener(ipAddress, port);
        }

        /// <summary>
        /// Accepts a pending connection request.
        /// </summary>
        /// <returns></returns>
        public ITcpClient AcceptTcpClient()
        {
            var client = _myListener.AcceptTcpClient();
            return new TcpClient(client);
        }
    }
}