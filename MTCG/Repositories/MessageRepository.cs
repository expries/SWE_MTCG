using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Database;
using MTCG.Domain;
using MTCG.Entities;

namespace MTCG.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DatabaseManager _db;
        private readonly IUserRepository _userRepository;
        
        public MessageRepository(DatabaseManager db, IUserRepository userRepository)
        {
            _db = db;
            _userRepository = userRepository;
        }
        
        public Message GetById(Guid messageId)
        {
            const string sql = "SELECT messageID, sender, receiver, message, time " +
                               "FROM chat WHERE messageID = @MessageId";

            var entity = _db.QueryFirstOrDefault<MessageEntity>(sql, new {MessageId = messageId});
            var message = MapEntity(entity);
            return message;
        }

        public List<Message> GetConversation(User sender, User receiver)
        {
            const string sql = "SELECT messageID, sender, receiver, message, time " +
                               "FROM chat WHERE sender = @SenderUsername AND receiver = @ReceiverUsername";

            var entities = _db.Query<MessageEntity>(sql, new {
                SenderUsername = sender.Username, 
                ReceiverUsername = receiver.Username
            });
            
            var messages = entities.Select(MapEntity).ToList();
            return messages;
        }

        public List<Message> GetByReceiver(User receiver)
        {
            const string sql = "SELECT messageID, sender, receiver, message, time " +
                               "FROM chat WHERE receiver = @Username";

            var entities = _db.Query<MessageEntity>(sql, new {Username = receiver.Username});
            var messages = entities.Select(MapEntity).ToList();
            return messages;
        }

        public Message Create(Message message)
        {
            const string sql = "INSERT INTO message (messageID, sender, receiver, message, time) " +
                               "VALUES (@MessageId, @SenderId, @ReceiverId, @Message, @Timestamp)";

            _db.Execute(sql, new {
                MessageId = message.Id,
                SenderId = message.Sender.Id, 
                ReceiverId = message.Receiver.Id, 
                Message = message.Content,
                Timestamp = message.Timestamp
            });

            return GetById(message.Id);
        }

        public void Delete(Message message)
        {
            const string sql = "DELETE FROM message WHERE messageID = @MessageId";
            _db.Execute(sql, new {MessageId = message.Id});
        }

        private Message MapEntity(MessageEntity entity)
        {
            if (entity is null)
            {
                return null;
            }
            
            var sender = _userRepository.GetByUsername(entity.Sender);
            var receiver = _userRepository.GetByUsername(entity.Receiver);
            var message = Message.Create(entity.Id, sender, receiver, entity.Message, entity.Timestamp);
            return message.Value;
        }
    }
}