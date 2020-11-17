using System;
using MTCG.Exceptions;

namespace MTCG.Resources
{
    public class User
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public Guid Token { get; set; }
        public int Coins { get; private set; }

        public User(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new BadRequestException("Username may not be empty.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new BadRequestException("Password may not be empty.");
            }

            Username = username;
            Password = password;
            Coins = 0;
        }

        public void AddCoins(int coins)
        {
            Coins = Math.Max(0, Coins + coins);
        }
    }
}