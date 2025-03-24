using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using KhuBot.Application.IRepositories;
using KhuBot.Domain.DTOs;
using KhuBot.Domain.Entities;
using KhuBot.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KhuBot.Application.Attributes
{
    public class UserAuthorizeAttribute : TypeFilterAttribute
    {
        public UserAuthorizeAttribute() : base(typeof(UserAuthorizeImpl))
        {
            Arguments = new object[] { };
        }

        private class UserAuthorizeImpl(
            IBaseRepository<User> userRepository)
            : IAsyncActionFilter
        {
            public async Task OnActionExecutionAsync(
                ActionExecutingContext context, ActionExecutionDelegate next)
            {
                SecurityToken validatedToken;

                var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
                    .Last();
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtIssuer = "KhuBot";
                var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
                var byteKey = Encoding.ASCII.GetBytes(jwtKey);
                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(byteKey)
                    }, out validatedToken);
                }
                catch (Exception e)
                {
                    if (e is ArgumentNullException or ArgumentException or SecurityTokenMalformedException
                        or SecurityTokenDecryptionFailedException or SecurityTokenEncryptionKeyNotFoundException
                        or SecurityTokenException or SecurityTokenExpiredException
                        or SecurityTokenInvalidAudienceException or SecurityTokenInvalidLifetimeException
                        or SecurityTokenInvalidSignatureException or SecurityTokenNoExpirationException
                        or SecurityTokenNotYetValidException or SecurityTokenReplayAddFailedException
                        or SecurityTokenReplayDetectedException)
                    {
                        throw new StatusCodeException([
                            new ErrorResponseDetailDto()
                            {
                                ErrorId = null,
                                ErrorKey = "login",
                                IsInternalError = false,
                                ErrorMessage = "توکن معتبر نمی‌باشد."
                            }
                        ], HttpStatusCode.Unauthorized);
                    }

                    throw;
                }

                await next();
            }
        }
    }
}