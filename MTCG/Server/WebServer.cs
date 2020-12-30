using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MTCG.Exceptions;
using MTCG.Server.TcpWrapper;

namespace MTCG.Server
{
    /// <summary>
    /// listen for clients and respond
    /// </summary>
    public class WebServer
    {
        private bool _running;
        private readonly Dictionary<string, Dictionary<string, Func<RequestContext, ResponseContext>>> _routes;

        /// <summary>
        /// Initialise webserver
        /// </summary>
        public WebServer()
        {
            _routes = new Dictionary<string, Dictionary<string, Func<RequestContext, ResponseContext>>>();
            _running = false;
        }

        /// <summary>
        /// launch webserver; required for listening for clients
        /// </summary>
        public void Start()
        {
            _running = true;
        }

        /// <summary>
        /// stop webserver; stops listening for clients
        /// </summary>
        public void Stop()
        {
            _running = false;
        }

        /// <summary>
        /// register an endpoint handler
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="handler"></param>
        public void RegisterRoute(string method, string endpoint, Func<RequestContext, ResponseContext> handler)
        {
            if (!_routes.ContainsKey(endpoint))
            {
                _routes[endpoint] = new Dictionary<string, Func<RequestContext, ResponseContext>>();
            }
            
            _routes[endpoint][method] = handler;
            Console.WriteLine("Registered Route: " + endpoint + " (" + method + ")");
        }

        /// <summary>
        /// listen for clients given on a given ip and port and serve their requests
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Listen(string ip, int port)
        {
            var listener = new TcpListener(ip, port);
            Listen(listener);
        }

        /// <summary>
        /// listen for clients given a ITcpListener and serve their requests
        /// </summary>
        /// <param name="listener"></param>
        public async void Listen(ITcpListener listener)
        {
            var clientTasks = new List<Task>();
            listener.Start();
            
            while (_running)
            {
                if (clientTasks.Count > 9)
                {
                    var taskArray = clientTasks.ToArray();
                    int taskIndex = Task.WaitAny(taskArray);
                    await taskArray[taskIndex];
                    clientTasks.Remove(taskArray[taskIndex]);
                }
                
                var client = listener.AcceptTcpClient();
                var clientTask = Task.Run(() => ServeClient(client));
                clientTasks.Add(clientTask);
            }
            
            listener.Stop();
        }

        /// <summary>
        /// interpret client request and send appropriate response
        /// </summary>
        /// <param name="tcpClient"></param>
        public void ServeClient(ITcpClient tcpClient)
        {
            try
            {
                tcpClient.ReadRequest();
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
            /*
            catch
            {
                Console.WriteLine("Failed to send response to client.");
            }
            */
        }

        /// <summary>
        /// find endpoint that matches request and invoke it
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
            /*
            catch
            {
                response.Status = HttpStatus.InternalServerError;
                response.Content = "An error has occurred.";
                response.ContentType = MediaType.Plaintext;
            }
            */

            return response;
        }
        
        /// <summary>
        /// invoke handler for given endpoint and method
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private ResponseContext InvokeRoute(string endpoint, RequestContext request)
        {
            if (!_routes.ContainsKey(endpoint))
            {
                return new ResponseContext(
                    HttpStatus.NotFound, "The requested endpoint does not exist.");
            }
            
            if (!_routes[endpoint].ContainsKey(request.Method))
            {
                return new ResponseContext(
                    HttpStatus.MethodNotAllowed, "Endpoint does not support requested method.");
            }
            
            Console.WriteLine("Invoked route: " + endpoint + " (" + request.Method + ")");
            return _routes[endpoint][request.Method].Invoke(request);
        }

        /// <summary>
        /// returns matching endpoint
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

            return string.Empty;
        }

        /// <summary>
        /// get values of placeholders that are set in uri pattern
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="uriPattern"></param>
        /// <returns></returns>
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

        /// <summary>
        /// get regex match of uri against pattern
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="uriPattern"></param>
        /// <returns></returns>
        private static Match MatchAgainstPattern(string uri, string uriPattern)
        {
            var placeholderNames = GetUriParameters(uriPattern);
            
            foreach (string name in placeholderNames)
            {
                string placeholder = "{" + name + "}";
                uriPattern = uriPattern.Replace(placeholder, "([a-zA-Z0-9-]+)");
            }
            
            uriPattern = "^" + uriPattern + "$";
            return Regex.Match(uri, uriPattern);
        }

        /// <summary>
        /// get all the placeholders in a uri pattern
        /// </summary>
        /// <param name="uriPattern"></param>
        /// <returns></returns>
        private static List<string> GetUriParameters(string uriPattern)
        {
            var parameters = new List<string>();
            
            string parameter;
            while ((parameter = GetStringBetween(uriPattern, "{", "}")) != string.Empty)
            {
                uriPattern = uriPattern.Replace("{" + parameter + "}", "");
                parameters.Add(parameter);
            }
            
            return parameters;
        }

        /// <summary>
        /// get string between two substrings
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strStart"></param>
        /// <param name="strEnd"></param>
        /// <returns></returns>
        private static string GetStringBetween(string str, string strStart, string strEnd)
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