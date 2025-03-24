using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Application.IRepositories
{
    public interface IChatBotRepository
    {
        Task<string> GetResponseAsync(string message);
    }
}
