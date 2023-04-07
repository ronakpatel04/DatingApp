using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interface
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        
        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);

        Task<PagedList<MessageDto>>GetMessagesForUser();

        Task<IEnumerable<MessageDto>>GetMessageThread(int currentuserId , int reciverId);
    
        Task<bool>SaveAllAsync();

    }
}