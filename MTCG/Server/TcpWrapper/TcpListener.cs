using System.Net;

namespace MTCG.Server.TcpWrapper
{
    // standard TCP listener wrapper
    public class TcpListener : ITcpListener
    {
        private readonly System.Net.Sockets.TcpListener _myListener;
        
        public void Start() => _myListener.Start();
        
        public void Stop() => _myListener.Stop();
        
        public TcpListener(string ip, int port)
        {
            var ipAddress = IPAddress.Parse(ip);
            _myListener = new System.Net.Sockets.TcpListener(ipAddress, port);
        }

        public ITcpClient AcceptTcpClient()
        {
            var client = _myListener.AcceptTcpClient();
            return new TcpClient(client);
        }
    }
}