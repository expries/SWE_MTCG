using System;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Domain
{
    public class Message
    {
        public Guid Id { get; }
        
        public User Sender { get; }
        
        public User Receiver { get; }
        
        public string Content { get; }
        
        public DateTime Timestamp { get; }

        private Message(Guid id, User sender, User receiver, string content, DateTime timestamp)
        {
            Id = id;
            Sender = sender;
            Receiver = receiver;
            Content = content;
            Timestamp = timestamp;
        }

        public static Result<Message> Create(Guid id, User sender, User receiver, string content, DateTime timestamp)
        {
            if (id.Equals(Guid.Empty))
            {
                return new MessageIdIsEmpty("Message ID may not be empty.");
            }

            if (sender is null)
            {
                return new SenderIsNull("Sender is required.");
            }

            if (receiver is null)
            {
                return new ReceiverIsNull("Receiver is required.");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return new MessageIsEmpty("A message may not be empty.");
            }

            return new Message(id, sender, receiver, content, timestamp);
        }
    }
}