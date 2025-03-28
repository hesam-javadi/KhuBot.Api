using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KhuBot.Application.IRepositories;
using KhuBot.Application.IServices;
using KhuBot.Domain.DTOs;
using KhuBot.Domain.Entities;
using KhuBot.Domain.Exceptions;
using KhuBot.Domain.Utilities;
using Microsoft.AspNetCore.Identity.Data;

namespace KhuBot.Application.Services
{
    public class AuthServices(IBaseRepository<User> userRepository) : IAuthServices
    {
        public async Task<DataResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var nowDateTime = DateTime.Now;

            // get the user 
            var user = await userRepository.FindAsync(u => u.Username == request.Username);
            if (user == null)
            {
                throw new StatusCodeException([
                    new ErrorResponseDetailDto()
                    {
                        ErrorKey = null,
                        ErrorMessage = "نام کاربری یا رمز عبور اشتباهه.",
                        IsInternalError = false
                    }
                ], HttpStatusCode.BadRequest);
            }

            // Check if the password is correct
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.HashPassword))
            {
                throw new StatusCodeException([
                    new ErrorResponseDetailDto()
                    {
                        ErrorKey = null,
                        ErrorMessage = "نام کاربری یا رمز عبور اشتباهه.",
                        IsInternalError = false
                    }
                ], HttpStatusCode.BadRequest);
            }

            var ret = new LoginResponseDto
            {
                Token = Utilities.GenerateJwtToken([new Claim("userId", user.Id.ToString())], nowDateTime.AddMonths(1)),
                LoginExpireInDays = (int)(nowDateTime.AddMonths(1) - nowDateTime).TotalDays
            };

            return new DataResponseDto<LoginResponseDto>(ret);
        }
    }
}
