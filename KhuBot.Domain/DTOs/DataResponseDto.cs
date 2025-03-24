using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.DTOs
{
    public class DataResponseDto<T> : BaseResponseDto
    {
        public DataResponseDto(T data)
        {
            Data = data;
            IsSuccess = true;
        }

        public T Data { get; set; }
    }
}
