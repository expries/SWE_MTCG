using System;
using MTCG.Domain;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Server;

namespace MTCG.Controllers
{
    public class MessageController : ApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public MessageController(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }
        
        public ResponseContext SendMessage(string token, MessageCreationRequest request)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication with the provided token failed.");
            }

            var receiver = _userRepository.GetByUsername(request.Receiver);

            if (receiver is null)
            {
                return NotFound("Found no user with username " + request.Receiver + ".");
            }

            if (receiver.Id == user.Id)
            {
                return Conflict("Users may not send messages to themselves");
            }
            
            if (_messageRepository.GetById(request.Id) != null)
            {
                return Conflict("A message with this ID already exists.");
            }

            var createMessage = 
                Message.Create(request.Id,user, receiver, request.Message, DateTime.Now);

            if (!createMessage.Success)
            {
                return BadRequest(createMessage.Error);
            }

            var message = createMessage.Value;
            _messageRepository.Create(message);
            return Created();
        }

        public ResponseContext GetInbox(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication with the provided token failed.");
            }

            var message = _messageRepository.GetByReceiver(user);
            return Ok(message);
        }
        
        public ResponseContext ReadConversation(string token, string chatPartnerUsername)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication with the provided token failed.");
            }

            var chatPartner = _userRepository.GetByUsername(chatPartnerUsername);

            if (chatPartner is null)
            {
                return NotFound("Found no user with username " + chatPartnerUsername + ".");
            }

            var messages  = _messageRepository.GetConversation(user, chatPartner);
            return Ok(messages);
        }

        public ResponseContext DeleteMessage(string token, Guid messageId)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication with the provided token failed.");
            }

            var message = _messageRepository.GetById(messageId);

            if (message is null)
            {
                return NotFound("Found no message with id " + messageId + ".");
            }

            if (message.Sender.Id != user.Id)
            {
                return Forbidden("You can't delete messages that you haven't sent yourself.");
            }
            
            _messageRepository.Delete(message);
            return NoContent();
        }

        public ResponseContext GetMessage(string token, Guid messageId)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication with the provided token failed.");
            }

            var message = _messageRepository.GetById(messageId);
            
            if (message is null)
            {
                return NotFound("Found no message with id " + messageId + ".");
            }

            return Ok(message);
        }
    }
}