using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;

        public int LoginExpireInDays { get; set; }
    }
}
