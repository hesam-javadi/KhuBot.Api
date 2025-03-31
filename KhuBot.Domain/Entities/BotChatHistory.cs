using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.Entities
{
    public class BotChatHistory
    {
        public string Message { get; set; } = null!;

        public bool IsFromBot { get; set; }
    }
}
