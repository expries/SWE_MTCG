using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MTCG.Exception;
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

        public void Stop()
        {
            _running = false;
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
            while (_running)
            {
                var client = listener.AcceptTcpClient();
                ServeClient(client);
            }
            listener.Stop();
        }

        // interpret client request and send appropriate response
        public void ServeClient(ITcpClient tcpClient)
        {
            try
            {
                var request = tcpClient.GetRequest();
                request.PrintProperties();
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
                request.PathParam = GetParameters(request.Path, endpoint);
                response = InvokeRoute(endpoint, request);
            }
            catch (HttpException error)  // request format is invalid
            {
                response.Status = error.Status;
                response.Content = error.Message;
                response.ContentType = MediaType.Plaintext;
            }
            catch
            {
                response.Status = HttpStatus.InternalServerError;
                response.Content = "An error has occurred.";
                response.ContentType = MediaType.Plaintext;
            }

            return response;
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
                throw new MethodNotAllowedException("Endpoint does not support requested method.");
            }
            Console.WriteLine("Invoked route: " + endpoint + " (" + request.Method + ")");
            return _routes[endpoint][request.Method].Invoke(request);
        }

        // returns matching endpoint
        private string GetEndpoint(RequestContext request)
        {
            foreach ((string endpoint, _) in _routes)
            {
                var match = MatchAgainstPattern(request.Path, endpoint);
                if (match.Success)
                {
                    return endpoint;
                }
            }
            throw new NotFoundException("Requested endpoint does not exist.");
        }

        // get values of placeholders that are set in uri pattern
        private static Dictionary<string, string> GetParameters(string uri, string uriPattern)
        {
            var uriParameters = new Dictionary<string, string>();
            var placeholders = GetUriParameters(uriPattern);
            var match = MatchAgainstPattern(uri, uriPattern);

            // map every regex group to the correct parameter in the uri pattern
            for (int j = 0; j < Math.Max(placeholders.Count, match.Groups.Count - 1); j++)
            {
                string name = placeholders[j];
                string value = match.Groups.Values.ElementAt(j + 1).Value;
                uriParameters[name] = value;
            }

            return uriParameters;
        }

        // get regex match of uri against pattern
        private static Match MatchAgainstPattern(string uri, string uriPattern)
        {
            var placeholderNames = GetUriParameters(uriPattern);
            foreach (string name in placeholderNames)
            {
                string placeholder = "{" + name + "}";
                uriPattern = uriPattern.Replace(placeholder, "([a-zA-Z0-9]+)");
            }
            uriPattern = "^" + uriPattern + "$";
            return Regex.Match(uri, uriPattern);
        }

        // get all the placeholders in a uri pattern
        private static List<string> GetUriParameters(string uriPattern)
        {
            var parameters = new List<string>();
            string parameter;
            while ((parameter = GetStrBetween(uriPattern, "{", "}")) != string.Empty)
            {
                uriPattern = uriPattern.Replace("{" + parameter + "}", "");
                parameters.Add(parameter);
            }
            return parameters;
        }

        // get next placeholder in uri pattern
        private static string GetStrBetween(string str, string strStart, string strEnd)
        {
            int i = str.IndexOf(strStart, StringComparison.Ordinal);
            
            if (i < 0)
            {
                return string.Empty;
            }
            
            string fromParameter = str.Substring(i + 1);
            int j = fromParameter.IndexOf(strEnd, StringComparison.Ordinal);
            
            if (j < 0)
            {
                return string.Empty;
            }
            
            return fromParameter.Substring(0, j);
        }
    }
}