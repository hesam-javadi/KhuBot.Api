using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Domain.DTOs;

namespace KhuBot.Application.IServices
{
    public interface IAuthServices
    {
        Task<DataResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    }
}
