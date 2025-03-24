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
            [FromBody, Required(ErrorMessage = "پیام الزامی است.")] string message)
        {
            var userId = Convert.ToInt32(User.FindFirstValue("userId")!);
            return Ok(await chatServices.SendMessageAsync(message, userId));
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
