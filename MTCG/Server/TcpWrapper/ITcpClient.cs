namespace MTCG.Server.TcpWrapper
{
    public interface ITcpClient
    {
        public RequestContext GetRequest();
        public void SendResponse(ResponseContext response);
        public void Close();
    }
}