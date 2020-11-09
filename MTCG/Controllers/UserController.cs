using System;
using System.ComponentModel.DataAnnotations;
using MTCG.Contracts;
using MTCG.Contracts.Requests;
using MTCG.Exceptions;
using MTCG.Resources;
using MTCG.Server;
using MTCG.Services;

namespace MTCG.Controllers
{
    public class UserController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public UserController()
        {
            _userService = new UserService();
        }

        public ResponseContext Register(RegistrationRequest request)
        {
            var registrationResponse = _userService.Register(request);

            if (!registrationResponse.Success)
            {
                throw new BadRequestException(registrationResponse.Error);
            }

            return new ResponseContext
            {
                Status = HttpStatus.Created,
                ContentType = MediaType.Plaintext,
                Content = registrationResponse.Token
            };
        }
        
        public ResponseContext Login(LoginRequest request)
        {
            var response = _userService.Login(request);

            if (!response.Success)
            {
                throw new BadRequestException(response.Error);
            }
            
            return new ResponseContext { Status = HttpStatus.Ok };
        }
    }
}