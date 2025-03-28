using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.Entities
{
    public class BotResponse
    {
        public string Content { get; set; } = null!;

        public int TokenUsage { get; set; }
    }
}
