using MTCG.Results;
using MTCG.Server;
using Newtonsoft.Json;

namespace MTCG.Controllers
{
    public class ApiController
    {
        protected static Error Error(string message)
        {
            return new(message);
        }
        
        protected static ResponseContext Ok()
        {
            return Response(HttpStatus.Ok);
        }

        protected static ResponseContext Ok(object content)
        {
            return Response(HttpStatus.Ok, content);
        }

        protected static ResponseContext Ok(string content, string contentType)
        {
            return Response(HttpStatus.Ok, content, contentType);
        }
        
        protected static ResponseContext BadRequest()
        {
            return Response(HttpStatus.BadRequest);
        }
        
        protected static ResponseContext BadRequest(object content)
        {
            return Response(HttpStatus.BadRequest, content);
        }
        
        protected static ResponseContext NotFound()
        {
            return Response(HttpStatus.NotFound);
        }
        
        protected static ResponseContext NotFound(object content)
        {
            return Response(HttpStatus.NotFound, content);
        }
        
        protected static ResponseContext Conflict()
        {
            return Response(HttpStatus.Conflict);
        }
        
        protected static ResponseContext Conflict(object content)
        {
            return Response(HttpStatus.Conflict, content);
        }
        
        protected static ResponseContext Accepted()
        {
            return Response(HttpStatus.Accepted);
        }
        
        protected static ResponseContext Accepted(object content)
        {
            return Response(HttpStatus.Accepted, content);
        }
        
        protected static ResponseContext Created()
        {
            return Response(HttpStatus.Created);
        }
        
        protected static ResponseContext Created(object content)
        {
            return Response(HttpStatus.Created, content);
        }
        
        protected static ResponseContext NoContent()
        {
            return Response(HttpStatus.NoContent);
        }
        
        protected static ResponseContext NoContent(object content)
        {
            return Response(HttpStatus.NoContent, content);
        }
        
        protected static ResponseContext Forbidden()
        {
            return Response(HttpStatus.Forbidden);
        }

        protected static ResponseContext Forbidden(object content)
        {
            return Response(HttpStatus.Forbidden, content);
        }
        
        protected static ResponseContext Unauthorized()
        {
            var response = Response(HttpStatus.Unauthorized);
            response.Headers.Add("WWW-Authenticate", "Basic realm=\"Monster Trading Card Game\", charset=\"utf-8\"");
            return response;
        }

        protected static ResponseContext Unauthorized(object content)
        {
            var response = Response(HttpStatus.Unauthorized, content);
            response.Headers.Add("WWW-Authenticate", "Basic realm=\"Monster Trading Card Game\", charset=\"utf-8\"");
            return response;
        }

        protected static ResponseContext Response(HttpStatus status)
        {
            return new() {Status = status};
        }

        protected static ResponseContext Response(HttpStatus status, object content)
        {
            if (status.IsErrorCode() && content is string str)
            {
                content = Error(str);
            }
            
            string json = JsonConvert.SerializeObject(content);
            return Response(status, json, MediaType.Json);
        }
        
        protected static ResponseContext Response(HttpStatus status, string content, string contentType)
        {
            return new(status, content, contentType);
        }
    }
}