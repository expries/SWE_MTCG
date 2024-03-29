using System.Net;

namespace MTCG.Server
{
    /// <summary>
    /// HTTP status codes
    /// </summary>
    public enum HttpStatus
    {
        Ok                  = 200,
        Created             = 201,
        Accepted            = 202,
        NoContent           = 204,
        BadRequest          = 400,
        Unauthorized        = 401,
        Forbidden           = 403,
        NotFound            = 404,
        MethodNotAllowed    = 405,
        Conflict            = 409,
        InternalServerError = 500,
        NotImplemented      = 501,
    };
    
    /// <summary>
    /// extension methods as described in
    /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/how-to-implement-and-call-a-custom-extension-method
    /// </summary>
    internal static class HttpStatusMethods
    {
        // get status code for given HTTP status
        public static int GetCode(this HttpStatus status)
        {
            return (int) status;
        }

        public static bool IsErrorCode(this HttpStatus status)
        {
            int code = status.GetCode();
            return 400 <= code && code <= 599;
        }
        
        // get status phrase for a given HTTP status
        public static string GetPhrase(this HttpStatus status)
        {
            return status switch
            {
                HttpStatus.Ok                  => "OK",
                HttpStatus.Accepted            => "Accepted",
                HttpStatus.Created             => "Created",
                HttpStatus.Unauthorized        => "Unauthorized",
                HttpStatus.Forbidden           => "Forbidden",
                HttpStatus.BadRequest          => "Bad Request",
                HttpStatus.NoContent           => "No Content",
                HttpStatus.NotFound            => "Not Found",
                HttpStatus.Conflict            => "Conflict",
                HttpStatus.NotImplemented      => "Not Implemented",
                HttpStatus.InternalServerError => "Internal Server Error",
                _                              => null
            };
        }
    }
}