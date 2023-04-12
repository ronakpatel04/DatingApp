using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddGroup(Group group)
        {


            context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);

        }

        public async Task<Connection> GetConnection(string connectionId)
        {

            return await context.Connections.FindAsync(connectionId);


        }

        public async Task<Message> GetMessage(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);


        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.ReceiverUsername == messageParams.Username && u.ReceiverDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.ReceiverUsername == messageParams.Username && u.ReceiverDeleted == false && u.DateRead == null)
            };

            var message = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(message, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string receiverUserName)
        {

            var messages = await context.Messages.Include(u => u.Sender).ThenInclude(p => p.Photos).Include(u => u.Receiver).ThenInclude(p => p.Photos).Where(
                m => m.ReceiverUsername == currentUserName && m.ReceiverDeleted == false && m.SenderUsername == receiverUserName ||
                         m.ReceiverUsername == receiverUserName && m.SenderDeleted == false && m.SenderUsername == currentUserName
            ).OrderBy(m => m.MessageSent).ToListAsync();


            var unreadMessages = messages.Where(m => m.DateRead == null && m.ReceiverUsername == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                await context.SaveChangesAsync();
            }

            return mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public void RemoveConnection(Connection connection)
        {

            context.Connections.Remove(connection);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}