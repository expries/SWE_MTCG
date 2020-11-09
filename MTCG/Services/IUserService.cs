using MTCG.Contracts;
using MTCG.Contracts.Requests;
using MTCG.Contracts.Responses;
using MTCG.Resources;

namespace MTCG.Services
{
    public interface IUserService
    {
        public RegistrationResponse Register(RegistrationRequest user);
        public LoginResponse Login(LoginRequest user);
        public bool CheckSession(string username, string token);
    }
}