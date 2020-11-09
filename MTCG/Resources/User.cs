using System;
using System.Threading.Tasks;

namespace MTCG.Resources
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public User(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username is null or empty.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is null or empty.");
            }

            Username = username;
            Password = password;
        }
    }
}