using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }


        [HttpPost]

        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.ReceiverUserName?.ToLower())
            {
                return BadRequest("You can not send message to yourself !!");
            }


            var sender = await userRepository.GetUserByUsernameAsync(username);
            var receiver = await userRepository.GetUserByUsernameAsync(createMessageDto.ReceiverUserName);

            if (receiver == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Receiver = receiver,
                SenderUsername = sender.UserName,
                ReceiverUsername = receiver.UserName,
                Content = createMessageDto.Content
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));


            return BadRequest("Failed to send Message");

        }

        [HttpGet]

        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)

        {
            messageParams.Username = User.GetUsername();

            var messages = await messageRepository.GetMessagesForUser(messageParams);


            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;

        }


        [HttpGet("thread/{username}")]

        public async Task<ActionResult<IEnumerable<MessageDto>>>GetMessageThread(string username)
        {

            var currentUsername = User.GetUsername();
            return Ok(await messageRepository.GetMessageThread(currentUsername, username));
        }


        [HttpDelete("{id}")]

        public async Task<ActionResult>DeleteMessage(int id)
        {

            var username = User.GetUsername();
            var message =  await messageRepository.GetMessage(id);

            if(message.SenderUsername != username && message.ReceiverUsername != username) return Unauthorized();

            if(message.ReceiverUsername == username) message.ReceiverDeleted = true;
            if(message.SenderUsername == username) message.SenderDeleted = true;


            if(message.SenderDeleted && message.ReceiverDeleted)
            {
                messageRepository.DeleteMessage(message);
            }

            if(await messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("problem deleting the message!");

        }


    }
}