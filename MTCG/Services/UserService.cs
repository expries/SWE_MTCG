using System.ComponentModel.DataAnnotations;
using MTCG.Contracts;
using MTCG.Contracts.Requests;
using MTCG.Contracts.Responses;
using MTCG.Repositories;

namespace MTCG.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        
        public UserService()
        {
            _repository = new UserRepository();
        }
        
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public RegistrationResponse Register(RegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return new RegistrationResponse { Success = false, Error = "Username may not be empty." };
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return new RegistrationResponse { Success = false, Error = "Password may not be empty." };
            }

            if (_repository.GetUser(request.Username) != null)
            {
                return new RegistrationResponse { Success = false, Error = "A user with this username already exists." };
            }
            
            var user = _repository.CreateUser(request.Username, request.Password);
            return new RegistrationResponse { Success = true, Token = user.Token };
        }
        
        public LoginResponse Login(LoginRequest user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                return new LoginResponse { Success = false, Error = "Username may not be empty." };
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return new LoginResponse { Success = false, Error = "Password may not be empty." };
            }

            return _repository.CheckCredentials(user.Username, user.Password)
                ? new LoginResponse { Success = true }
                : new LoginResponse { Success = false, Error = "No user with this username and password exists." };
        }

        public bool CheckSession(string username, string token)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ValidationException("Username may not be empty.");
            }
            
            var user = _repository.GetUser(username);
            return user != null && user.Token == token;
        }
    }
}