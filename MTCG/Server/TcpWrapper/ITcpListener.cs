namespace MTCG.Server.TcpWrapper
{
    public interface ITcpListener
    {
        public ITcpClient AcceptTcpClient();
        public void Start();
    }
}