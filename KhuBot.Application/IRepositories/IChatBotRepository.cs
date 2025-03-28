using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Domain.Entities;

namespace KhuBot.Application.IRepositories
{
    public interface IChatBotRepository
    {
        Task<BotResponse> GetResponseAsync(string message, string developerInstruction);
    }
}
