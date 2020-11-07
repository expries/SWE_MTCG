using System;

namespace MTCG.Server.TcpWrapper
{
    // standard TCP client wrapper
    public class TcpClient : ITcpClient
    {
        private readonly System.Net.Sockets.TcpClient _client;
        public void Close() => _client.Close();

        public TcpClient(System.Net.Sockets.TcpClient client)
        {
            _client = client;
        }
        
        // fetch client request
        public RequestContext GetRequest()
        {
            var request = new RequestContext();
            var stream = _client.GetStream();
            var buffer = new byte[4096];

            do  // read request from stream
            {
                int bytesReceived = stream.Read(buffer, 0, buffer.Length);
                string receivedString = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                request.Read(receivedString);

            } while (stream.DataAvailable);
            
            return request;
        }
        
        // send response to client
        public void SendResponse(ResponseContext response)
        {
            response.Headers["Date"] = DateTime.Now.ToString("R");
            string responseString = response.ToString();
            var payload = System.Text.Encoding.UTF8.GetBytes(responseString);

            var stream = _client.GetStream();
            stream.Write(payload, 0, payload.Length);
        }
    }
}