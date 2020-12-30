using System;
using MTCG.Database;

namespace MTCG.Entities
{
    public class UserEntity
    {
        [Column(Name="userID")]
        public Guid Id { get; set; }
        
        [Column(Name="username")]
        public string Username { get; set; }
        
        [Column(Name="password")]
        public string Password { get; set; }
        
        [Column(Name="name")]
        public string Name { get; set; }
        
        [Column(Name="bio")]
        public string Bio { get; set; }
        
        [Column(Name="image")]
        public string Image { get; set; }
        
        [Column(Name="token")]
        public string Token { get; set; }
        
        [Column(Name="coins")]
        public int Coins { get; set; }
        
        [Column(Name="wins")]
        public int Wins { get; set; }
        
        [Column(Name="draws")]
        public int Draws { get; set; }
        
        [Column(Name="loses")]
        public int Loses { get; set; }
        
        [Column(Name="elo")]
        public double Elo { get; set; }
    }
}