using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Application.IRepositories;
using KhuBot.Application.IServices;
using KhuBot.Domain.DTOs;
using KhuBot.Domain.Entities;

namespace KhuBot.Application.Services
{
    public class AuthServices(IBaseRepository<User> userRepository) : IAuthServices
    {
        public Task<DataResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
