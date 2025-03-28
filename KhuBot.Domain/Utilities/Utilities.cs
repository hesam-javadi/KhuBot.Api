using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.Utilities
{
    public static class Utilities
    {
        public static string? FirstCharToLower(this string? input) =>
            input switch
            {
                null or "" => null,
                _ => string.Concat(input[0].ToString().ToLower(), input.AsSpan(1))
            };

        public static string GenerateJwtToken(List<Claim> claims, DateTime tokenExpirationDateTime)
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

        public static string FormatTemplate(this string input, params string[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                input = input.Replace("{" + i + "}", items[i]);
            }

            return input;
        }
    }
}
