using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.DTOs
{
    public class ChatListResponseDto
    {
        public List<MessageDto> Messages { get; set; } = [];

        public int UsagePercent { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
    }
}
