using System.Collections.Generic;
using MTCG.Exception;
using MTCG.Request;
using MTCG.Resource;
using MTCG.Resource.Cards;
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
            var action = _userService.CreateUser(request.Username, request.Password);

            if (!action.Success)
            {
                return BadRequest(action.Error.Message);
            }

            var user = action.Item;
            return Created(user.Token);
        }

        public ResponseContext Get(string username)
        {
            var user = _userService.GetUser(username);
            return user is null ? NotFound("User not found.") : Ok(user);
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
            var user = _userService.GetUser(username);
            return user is null ? NotFound("User not found.") : Ok(user.Stack);
        }

        public ResponseContext AcquirePackage(string username)
        {
            var purchase = _userService.AcquirePackage(username);

            if (!purchase.Success)
            {
                return BadRequest(purchase.Error.Message);
            }

            var package = purchase.Item;
            return Ok(package);
        }
    }
}