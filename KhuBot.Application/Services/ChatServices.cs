using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Application.IRepositories;
using KhuBot.Application.IServices;
using KhuBot.Domain.DTOs;
using KhuBot.Domain.Entities;

namespace KhuBot.Application.Services
{
    public class ChatServices(IBaseRepository<User> userRepository,
        IBaseRepository<ChatMessage> chatMessageRepository,
        IChatBotRepository chatBotRepository) : IChatServices
    {
        public Task<DataResponseDto<string>> SendMessageAsync(string message, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponseDto<ChatListResponseDto>> GetChatListAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
