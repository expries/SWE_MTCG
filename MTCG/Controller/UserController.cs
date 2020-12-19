using MTCG.Request;
using MTCG.Server;
using MTCG.Service;

namespace MTCG.Controller
{
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public ResponseContext Register(RegistrationRequest request)
        {
            var registration = _userService.CreateUser(request.Username, request.Password);
            return registration.Match(user => Ok(user.Token), BadRequest);
        }

        public ResponseContext Get(string username)
        {
            var result = _userService.GetUser(username);
            return result.Match(Ok, NotFound);
        }

        public ResponseContext GetAll()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }
        
        public ResponseContext Login(LoginRequest request)
        {
            bool valid = _userService.VerifyLogin(request.Username, request.Password);
            return valid ? Ok() : BadRequest("No user with this username and password exists.") ;
        }

        public ResponseContext GetCards(string username)
        {
            var result = _userService.GetUser(username);
            return result.Match(user => Ok(user.Stack), NotFound);
        }

        public ResponseContext AcquirePackage(string username)
        {
            var purchase = _userService.AcquirePackage(username);
            return purchase.Match(Ok, NotFound, BadRequest);
        }
    }
}