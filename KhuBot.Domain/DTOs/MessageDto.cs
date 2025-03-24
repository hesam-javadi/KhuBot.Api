using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.DTOs
{
    public class MessageDto
    {
        public string Content { get; set; } = null!;

        public bool IsFromBot { get; set; }
    }
}
