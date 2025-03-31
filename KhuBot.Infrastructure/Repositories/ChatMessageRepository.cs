using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Application.IRepositories;
using KhuBot.Domain.Entities;
using KhuBot.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KhuBot.Infrastructure.Repositories
{
    public class ChatMessageRepository(ApplicationDbContext db)
        : BaseRepository<ChatMessage>(db), IChatMessageRepository
    {
        public async Task<List<ChatMessage>> GetLastNMessagesAsync(int userId, int n)
        {
            return await db.ChatMessages.Where(x => x.UserId == userId)
                .OrderByDescending(x => x.MessageTimeStamp)
                .Take(n)
                .ToListAsync();
        }
    }
}
