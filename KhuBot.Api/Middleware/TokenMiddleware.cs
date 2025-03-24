using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KhuBot.Application.IRepositories;
using KhuBot.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KhuBot.Api.Middleware
{
    public class TokenMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context, IBaseRepository<User> userRepository)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, userRepository, token);

            await next(context);
        }
        private async Task AttachUserToContext(HttpContext context, IBaseRepository<User> userRepository, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtIssuer = "KhuBot";
                var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
                var byteKey = Encoding.ASCII.GetBytes(jwtKey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(byteKey)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Convert.ToInt32(jwtToken.Claims.First(x => x.Type == "userId").Value);

                var isUserExist = userId != 0 && await userRepository.IsExistsAsync(u => u.Id == userId);
                var claims = new List<Claim>();
                if (!isUserExist)
                {
                    claims =
                    [
                        new Claim("userId", "0")
                    ];
                    var newToken = GenerateJwtToken(claims, DateTime.Now.AddDays(-1));
                    context.Request.Headers["Authorization"] = "Bearer " + newToken;
                }
                else
                {
                    claims =
                    [
                        new Claim("userId", userId.ToString())
                    ];
                    var newToken = GenerateJwtToken(claims, jwtToken.ValidTo);
                    context.Request.Headers["Authorization"] = "Bearer " + newToken;
                }
                var identity = new ClaimsIdentity(claims, "Bearer");
                context.User = new ClaimsPrincipal(identity);
            }
            catch
            {
                var claims = new List<Claim>()
                {
                    new Claim("userId", "0")
                };
                var newToken = GenerateJwtToken(claims, DateTime.Now.AddDays(-1));
                context.Request.Headers["Authorization"] = "Bearer " + newToken;
                var identity = new ClaimsIdentity(claims, "Bearer");
                context.User = new ClaimsPrincipal(identity);
            }
        }

        private string GenerateJwtToken(List<Claim> claims, DateTime tokenExpirationDateTime)
        {
            var jwtIssuer = "KhuBot";
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var sectoken = new JwtSecurityToken(jwtIssuer,
                jwtIssuer,
                claims,
                expires: tokenExpirationDateTime,
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(sectoken);
            return token;
        }
    }
}
