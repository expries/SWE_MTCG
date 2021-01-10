using System;
using System.Collections.Generic;
using MTCG.Domain;

namespace MTCG.Repositories
{
    public interface IMessageRepository
    {
        public Message GetById(Guid messageId);
        
        public List<Message> GetByReceiver(User receiver);
        
        public List<Message> GetConversation(User sender, User receiver);

        public Message Create(Message message);

        public void Delete(Message message);
    }
}