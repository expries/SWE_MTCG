using System;

namespace MTCG.Server.TcpWrapper
{
    /// <summary>
    /// standard TCP client wrapper
    /// </summary>
    public class TcpClient : ITcpClient
    {
        /// <summary>
        /// underlying TCP client
        /// </summary>
        private readonly System.Net.Sockets.TcpClient _client;

        /// <summary>
        /// last request that was read
        /// </summary>
        private RequestContext Request { get; set; }
        
        /// <summary>
        /// call TCP client's close method
        /// </summary>
        public void Close() => _client.Close();

        /// <summary>
        /// initialise wrapper with a given TCP client
        /// </summary>
        /// <param name="client"></param>
        public TcpClient(System.Net.Sockets.TcpClient client)
        {
            _client = client;
            Request = null;
        }

        /// <summary>
        /// read request context from network stream
        /// </summary>
        public void ReadRequest()
        {
            Request = new RequestContext();
            var stream = _client.GetStream();
            byte[] buffer = new byte[4056];

            // read request from stream
            do
            {
                int receivedBytes = stream.Read(buffer);
                string receivedString = System.Text.Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                Request.Read(receivedString); 
            } 
            while (stream.DataAvailable);
        }

        /// <summary>
        /// get last request that was read
        /// </summary>
        /// <returns></returns>
        public RequestContext GetRequest()
        {
            return Request;
        }

        /// <summary>
        /// send response to client
        /// </summary>
        /// <param name="response"></param>
        public void SendResponse(ResponseContext response)
        {
            response.Headers["Date"] = DateTime.Now.ToString("R");
            string responseString = response.ToString();
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(responseString);

            var stream = _client.GetStream();
            stream.Write(payload, 0, payload.Length);
        }
    }
}