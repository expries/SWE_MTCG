using MTCG.Server;
using Newtonsoft.Json;

namespace MTCG.Controller
{
    public class ApiController
    {
        protected static ResponseContext Ok()
        {
            return Response(HttpStatus.Ok);
        }

        protected static ResponseContext Ok(object obj)
        {
            return Response(HttpStatus.Ok, obj);
        }
        
        protected static ResponseContext BadRequest()
        {
            return Response(HttpStatus.BadRequest);
        }
        
        protected static ResponseContext BadRequest(object obj)
        {
            return Response(HttpStatus.BadRequest, obj);
        }
        
        protected static ResponseContext NotFound()
        {
            return Response(HttpStatus.NotFound);
        }
        
        protected static ResponseContext NotFound(object obj)
        {
            return Response(HttpStatus.NotFound, obj);
        }
        
        protected static ResponseContext Conflict()
        {
            return Response(HttpStatus.Conflict);
        }
        
        protected static ResponseContext Conflict(object obj)
        {
            return Response(HttpStatus.Conflict, obj);
        }
        
        protected static ResponseContext Accepted()
        {
            return Response(HttpStatus.Accepted);
        }
        
        protected static ResponseContext Accepted(object obj)
        {
            return Response(HttpStatus.Accepted, obj);
        }
        
        protected static ResponseContext Created()
        {
            return Response(HttpStatus.Created);
        }
        
        protected static ResponseContext Created(object obj)
        {
            return Response(HttpStatus.Created, obj);
        }
        
        protected static ResponseContext NoContent()
        {
            return Response(HttpStatus.NoContent);
        }
        
        protected static ResponseContext NoContent(object obj)
        {
            return Response(HttpStatus.NoContent, obj);
        }
        
        protected static ResponseContext Forbidden()
        {
            return Response(HttpStatus.Forbidden);
        }

        protected static ResponseContext Forbidden(object obj)
        {
            return Response(HttpStatus.Forbidden, obj);
        }

        protected static ResponseContext Response(HttpStatus status)
        {
            return new ResponseContext {Status = status};
        }

        protected static ResponseContext Response(HttpStatus status, object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return Response(status, json, MediaType.Json);
        }
        
        protected static ResponseContext Response(HttpStatus status, string content, string contentType)
        {
            return new ResponseContext
            {
                Status = status,
                Content = content,
                ContentType = contentType
            };
        }
    }
}