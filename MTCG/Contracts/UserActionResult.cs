using System.Security.Cryptography.X509Certificates;
using MTCG.Resources;

namespace MTCG.Contracts
{
    public enum UserError
    {
        NotFound,
        UsernameTaken,
        BadFormat
    }
    
    public class UserActionResult : ActionResult<UserError>
    {
        public User User { get; }
        
        public UserActionResult(UserError error) : base(error) {}

        public UserActionResult(User user)
        {
            User = user;
        }
    }
}