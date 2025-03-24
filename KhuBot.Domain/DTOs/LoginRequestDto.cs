using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.DTOs
{
    public class LoginRequestDto
    {
        [DisplayName("نام کاربری")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Username { get; set; } = null!;

        [DisplayName("رمز عبور")]
        [Required(ErrorMessage = "{0} الزامی است.")]
        public string Password { get; set; } = null!;
    }
}
