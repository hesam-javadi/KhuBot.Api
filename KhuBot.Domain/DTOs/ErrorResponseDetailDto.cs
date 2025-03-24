using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.DTOs
{
    public class ErrorResponseDetailDto
    {
        public string ErrorMessage { get; set; } = null!;
        public string? ErrorKey { get; set; }
        public string? ErrorId { get; set; }
        public bool IsInternalError { get; set; }
    }
}
