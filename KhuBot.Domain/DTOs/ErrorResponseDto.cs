using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.DTOs
{
    public class ErrorResponseDto : BaseResponseDto
    {
        public ErrorResponseDto(List<ErrorResponseDetailDto> errorMessages)
        {
            ErrorMessages = errorMessages;
            IsSuccess = false;
        }

        public List<ErrorResponseDetailDto> ErrorMessages { get; set; }
    }
}
