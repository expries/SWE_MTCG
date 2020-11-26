using System.Runtime.InteropServices.ComTypes;
using MTCG.Server;

namespace MTCG.Controllers
{
    public class ApiController
    {
        private static ResponseContext Response(HttpStatus status, string content, string contentType)
        {
            return new ResponseContext
            {
                Status = status,
                Content = content,
                ContentType = contentType
            };
        }

        private static ResponseContext Response(HttpStatus status)
        {
            return new ResponseContext {Status = status};
        }
        
        protected static ResponseContext Ok()
        {
            return Response(HttpStatus.Ok);
        }

        protected static ResponseContext Ok(string content, string contentType = MediaType.Plaintext)
        {
            return Response(HttpStatus.Ok, content, contentType);
        }
        
        protected static ResponseContext BadRequest()
        {
            return Response(HttpStatus.BadRequest);
        }
        
        protected static ResponseContext BadRequest(string content, string contentType = MediaType.Plaintext)
        {
            return Response(HttpStatus.BadRequest, content, contentType);
        }
        
        protected static ResponseContext NotFound()
        {
            return Response(HttpStatus.NotFound);
        }
        
        protected static ResponseContext NotFound(string content, string contentType = MediaType.Plaintext)
        {
            return Response(HttpStatus.NotFound, content, contentType);
        }
        
        protected static ResponseContext Conflict()
        {
            return Response(HttpStatus.Conflict);
        }
        
        protected static ResponseContext Conflict(string content, string contentType = MediaType.Plaintext)
        {
            return Response(HttpStatus.Conflict, content, contentType);
        }
        
        protected static ResponseContext Accepted()
        {
            return Response(HttpStatus.Accepted);
        }
        
        protected static ResponseContext Accepted(string content, string contentType = MediaType.Plaintext)
        {
            return Response(HttpStatus.Accepted, content, contentType);
        }
        
        protected static ResponseContext Created()
        {
            return Response(HttpStatus.Created);
        }
        
        protected static ResponseContext Created(string content, string contentType = MediaType.Plaintext)
        {
            return Response(HttpStatus.Created, content, contentType);
        }
        
        protected static ResponseContext NoContent()
        {
            return Response(HttpStatus.NoContent);
        }
        
        protected static ResponseContext NoContent(string content, string contentType = MediaType.Plaintext)
        {
            return Response(HttpStatus.NoContent);
        }
    }
}