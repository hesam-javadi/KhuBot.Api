using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.DTOs
{
    public class SendMessageRequestDto
    {
        [DisplayName("پیام")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Message { get; set; } = null!;
    }
}
