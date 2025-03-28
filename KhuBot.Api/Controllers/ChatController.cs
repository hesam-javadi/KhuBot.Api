using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using KhuBot.Application.Attributes;
using KhuBot.Application.IServices;
using KhuBot.Application.Services;
using KhuBot.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KhuBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(ErrorResponseDto), 400)]
    [ProducesResponseType(typeof(ErrorResponseDto), 500)]
    public class ChatController(IChatServices chatServices) : ControllerBase
    {
        // POST: api/Chat/SendMessage
        [HttpPost("SendMessage")]
        [ProducesResponseType(typeof(DataResponseDto<string>), 200)]
        [UserAuthorize]
        public async Task<IActionResult> SendMessage(
            [FromBody] SendMessageRequestDto request)
        {
            var userId = Convert.ToInt32(User.FindFirstValue("userId")!);
            var rawDeveloperInstruction = await System.IO.File.ReadAllTextAsync(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeveloperInstruction.txt"));
            return Ok(await chatServices.SendMessageAsync(request, rawDeveloperInstruction, userId));
        }

        // GET: api/Chat/GetChatList
        [HttpGet("GetChatList")]
        [ProducesResponseType(typeof(DataResponseDto<ChatListResponseDto>), 200)]
        [UserAuthorize]
        public async Task<IActionResult> GetChatList()
        {
            var userId = Convert.ToInt32(User.FindFirstValue("userId")!);
            return Ok(await chatServices.GetChatListAsync(userId));
        }
    }
}
