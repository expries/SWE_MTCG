using System;
using System.Collections.Generic;
using MTCG.Resources.Cards;

namespace MTCG.Resources
{
    public class User
    {
        public Guid Id { get; }

        public string Username { get; }
        
        public string Password { get; }
        
        public int Coins { get; private set; }

        public string Token => GetToken();

        public readonly List<Card> Stack;

        public User(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username may not be empty.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password may not be empty.");
            }

            Username = username;
            Password = password;
            Stack = new List<Card>();
            Coins = 0;
        }

        public User(string username, string password, Guid id) : this(username, password)
        {
            if (id.Equals(Guid.Empty))
            {
                throw new ArgumentException("User ID may not be empty.");
            }
            
            Id = id;
        }

        public void AddCoins(int coins)
        {
            Coins = Math.Max(0, Coins + coins);
        }

        public void AddCard(Card card)
        {
            if (card.Id.Equals(Guid.Empty))
            {
                throw new ArgumentException("Card ID may not be empty.");
            }
            
            Stack.Add(card);
        }
        
        private string GetToken()
        {
            return $"{Username}-mtcgToken";
        }
    }
}