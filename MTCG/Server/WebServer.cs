using System;
using System.Collections.Generic;
using MTCG.Exceptions;
using MTCG.Server.TcpWrapper;

namespace MTCG.Server
{
    // listen and serve clients - REST style
    public class WebServer
    {
        private bool _running;
        private readonly Dictionary<string, Dictionary<string, Func<RequestContext, ResponseContext>>> _routes;

        public WebServer()
        {
            _routes = new Dictionary<string, Dictionary<string, Func<RequestContext, ResponseContext>>>();
            _running = false;
        }

        public void Start()
        {
            _running = true;
        }

        // register an endpoint handler
        public void RegisterRoute(string method, string endpoint, Func<RequestContext, ResponseContext> handler)
        {
            if (!_routes.ContainsKey(endpoint))
            {
                _routes[endpoint] = new Dictionary<string, Func<RequestContext, ResponseContext>>();
            }
            _routes[endpoint][method] = handler;
            Console.WriteLine("Registered Route: " + endpoint + " (" + method + ")");
        }

        // listen with self-made TcpListener
        public void Listen(string ip, int port)
        {
            var listener = new TcpListener(ip, port);
            Listen(listener);
        }

        // listen with given TcpListener and serve client requests
        public void Listen(ITcpListener listener)
        {
            listener.Start();
            do
            {
                var client = listener.AcceptTcpClient();
                ServeClient(client);

            } while (_running);
        }

        // interpret client request and send appropriate response
        private void ServeClient(ITcpClient tcpClient)
        {
            try
            {
                var request = tcpClient.GetRequest();
                var response = RouteRequest(request);
                response.Headers["Connection"] = "close";
                tcpClient.SendResponse(response);
                tcpClient.Close();
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("The client connection has been closed.");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("The client is not connected to a remote host.");
            }
            catch
            {
                Console.WriteLine("Failed to send response to client.");
            }
        }

        // find and invoke requested route endpoints
        private ResponseContext RouteRequest(RequestContext request)
        {
            var response = new ResponseContext();

            try
            {
                string endpoint = GetEndpoint(request);
                request.PathParam = UriMatcher.GetParameters(request.Path, endpoint);
                response = InvokeRoute(endpoint, request);
            }
            catch (HttpException error)
            {
                response.Status = error.Status;
                response.Content = error.Message;
                response.ContentType = MediaType.Plaintext;
            }
            catch
            {
                response.Status = HttpStatus.InternalServerError;
                response.Content = "An error has occurred";
                response.ContentType = MediaType.Plaintext;
            }
            
            return response;
        }

        // returns matching endpoint
        private string GetEndpoint(RequestContext request)
        {
            foreach ((string endpoint, _) in _routes)
            {
                var match = UriMatcher.MatchAgainstPattern(request.Path, endpoint);
                if (match.Success)
                {
                    return endpoint;
                }
            }
            throw new NotFoundException("Requested endpoint does not exist.");
        }
        
        // invoke handler for given endpoint and method
        private ResponseContext InvokeRoute(string endpoint, RequestContext request)
        {
            if (!_routes.ContainsKey(endpoint))
            {
                throw new NotFoundException("Requested endpoint does not exist.");
            }
            if (!_routes[endpoint].ContainsKey(request.Method))
            {
                throw new MethodNotImplementedException("Endpoint does not support requested method.");
            }
            Console.WriteLine("Invoked route: " + endpoint + " (" + request.Method + ")");
            return _routes[endpoint][request.Method].Invoke(request);
        }
    }
}