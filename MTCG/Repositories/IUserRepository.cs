using MTCG.Resources;

namespace MTCG.Repositories
{
    public interface IUserRepository
    {
        public User CreateUser(string username, string password);
        public bool CheckCredentials(string username, string password);
        public User GetUser(string username);
    }
}