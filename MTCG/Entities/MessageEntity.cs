using System;
using MTCG.Database;

namespace MTCG.Entities
{
    public class MessageEntity
    {
        [Column(Name="messageID")]
        public Guid Id { get; set; }
        
        [Column(Name="sender")]
        public string Sender { get; set; }
        
        [Column(Name="receiver")]
        public string Receiver { get; set; }
        
        [Column(Name="message")]
        public string Message { get; set; }
        
        [Column(Name="time")]
        public DateTime Timestamp { get; set; }
    }
}