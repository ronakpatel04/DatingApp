using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{

    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IHubContext<PresenceHub> presenceHub;

        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.presenceHub = presenceHub;
        }



        public async override Task OnConnectedAsync()
        {

            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);
            var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);


            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {

            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }



        public async Task SendMessage(CreateMessageDto createMessageDto)
        {

            var username = Context.User.GetUsername();

            if (username == createMessageDto.ReceiverUserName?.ToLower())
            {
                throw new HubException("You can not send message to yourself !!");
            }





            var sender = await userRepository.GetUserByUsernameAsync(username);
            var receiver = await userRepository.GetUserByUsernameAsync(createMessageDto.ReceiverUserName);

            if (receiver == null) throw new HubException("not found ! ");

            var message = new Message
            {
                Sender = sender,
                Receiver = receiver,
                SenderUsername = sender.UserName,
                ReceiverUsername = receiver.UserName,
                Content = createMessageDto.Content
            };


            var groupName = GetGroupName(sender.UserName, receiver.UserName);

            var group = await messageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(x => x.Username == receiver.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(receiver.UserName);
                if (connections != null)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }


            messageRepository.AddMessage(message);




            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())

            {
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
            }

        }



        private string GetGroupName(string caller, string other)
        {

            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
        private async Task<bool> AddToGroup(string groupName)
        {
            var group = await messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {

                group = new Group(groupName);
                messageRepository.AddGroup(group);

            }
            group.Connections.Add(connection);
            return await messageRepository.SaveAllAsync();
        }


        private async Task RemoveFromMessageGroup()
        {
            var connection = await messageRepository.GetConnection(Context.ConnectionId);
            messageRepository.RemoveConnection(connection);
            await messageRepository.SaveAllAsync();

        }

    }




}