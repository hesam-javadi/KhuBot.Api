using KhuBot.Application.IServices;
using KhuBot.Application.Services;
using KhuBot.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace KhuBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(ErrorResponseDto), 400)]
    [ProducesResponseType(typeof(ErrorResponseDto), 500)]
    public class AuthController(IAuthServices authServices) : ControllerBase
    {
        // POST: api/Auth/Login
        [HttpPost("Login")]
        [ProducesResponseType(typeof(DataResponseDto<LoginResponseDto>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            return Ok(await authServices.LoginAsync(request));
        }
    }
}
