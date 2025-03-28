using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Domain.DTOs;

namespace KhuBot.Application.IServices
{
    public interface IChatServices
    {
        Task<DataResponseDto<string>> SendMessageAsync(SendMessageRequestDto request, string rawDeveloperInstruction, int userId);

        Task<DataResponseDto<ChatListResponseDto>> GetChatListAsync(int userId);
    }
}
