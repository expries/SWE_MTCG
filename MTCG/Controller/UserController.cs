using System;
using System.Collections.Generic;
using MTCG.Requests;
using MTCG.Results.Errors;
using MTCG.Server;
using MTCG.Services;

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

            if (!registration.Success)
            {
                return BadRequest(registration.Error);
            }

            var user = registration.Value;
            return Ok(user.Token);
        }

        public ResponseContext UpdateUser(string username, UserUpdateRequest request, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new {Error = "Authorization is required."});
            }

            var result = _userService.UpdateUser(username, request, token);

            if (result.Success)
            {
                var user = result.Value;
                return Ok(user);
            }

            if (result.HasError<UserNotFound>())
            {
                return NotFound(result.Error);
            }
            
            if (result.HasError<NotPermitted>())
            {
                return Forbidden(result.Error);
            }

            return BadRequest(result.Error);
        }

        public ResponseContext Get(string username)
        {
            var result = _userService.GetUser(username);

            if (!result.Success)
            {
                return NotFound(result.Error);
            }

            var user = result.Value;
            return Ok(user);
        }

        public ResponseContext GetAll()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        public ResponseContext Login(LoginRequest request)
        {
            bool valid = _userService.CheckCredentials(request.Username, request.Password);
            return valid ? Ok() : BadRequest("No user with this username and password exists.") ;
        }

        public ResponseContext GetCards(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new {Error = "Authorization is required."});
            }

            var result = _userService.GetUser(token);

            if (!result.Success)
            {
                return NotFound(result.Error);
            }
            
            var user = result.Value;
            return Ok(user.Stack);
        }

        public ResponseContext AcquirePackage(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new {Error = "Authorization is required."});
            }

            var purchase = _userService.AcquirePackage(token);

            if (purchase.Success)
            {
                var package = purchase.Value;
                return Ok(package);
            }

            if (purchase.HasError<UserNotFound>())
            {
                return NotFound(purchase.Error);
            }

            return Conflict(purchase.Error);
        }

        public ResponseContext GetDeck(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new {Error = "Authorization is required."});
            }
            
            var result = _userService.GetUserByAuthToken(token);

            if (!result.Success)
            {
                return NotFound(result.Error);
            }

            var user = result.Value;
            return Ok(user.Deck);
        }

        public ResponseContext UpdateDeck(List<Guid> cardIds, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new {Error = "Authorization is required."});
            }

            var result = _userService.UpdateDeck(token, cardIds);

            if (result.Success)
            {
                var deck = result.Value;
                return Ok(deck);
            }

            if (result.HasError<UserNotFound, CardNotFound>())
            {
                return NotFound(result.Error);
            }
            
            return Conflict(result.Error);
        }
    }
}