using System;

namespace MTCG.Server.TcpWrapper
{
    // standard TCP client wrapper
    public class TcpClient : ITcpClient
    {
        private readonly System.Net.Sockets.TcpClient _client;
        
        public void Close() => _client.Close();

        private RequestContext Request { get; set; }

        public TcpClient(System.Net.Sockets.TcpClient client)
        {
            _client = client;
        }

        public void ReadRequest()
        {
            Request = new RequestContext();
            var stream = _client.GetStream();
            var buffer = new byte[4096];

            do  // read request from stream
            {
                int bytesReceived = stream.Read(buffer, 0, buffer.Length);
                string receivedString = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Request.Read(receivedString);

            } while (stream.DataAvailable);
        }

        public RequestContext GetRequest()
        {
            return Request;
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