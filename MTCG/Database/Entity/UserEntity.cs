using System;

namespace MTCG.Database.Entity
{
    public class UserEntity
    {
        [Column(Name="userID")]
        public Guid Id { get; set; }
        
        [Column(Name="username")]
        public string Username { get; set; }
        
        [Column(Name="password")]
        public string Password { get; set; }
        
        [Column(Name="coins")]
        public int Coins { get; set; }
    }
}