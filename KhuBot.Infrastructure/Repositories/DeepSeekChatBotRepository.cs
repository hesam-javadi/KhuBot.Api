using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Application.IRepositories;

namespace KhuBot.Infrastructure.Repositories
{
    public class DeepSeekChatBotRepository : IChatBotRepository
    {
        private readonly string _apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ??
                                          throw new KeyNotFoundException(
                                              "DeepSeek API Key is not found in environment variables.");

        public Task<string> GetResponseAsync(string message)
        {
            throw new NotImplementedException();
        }
    }
}
