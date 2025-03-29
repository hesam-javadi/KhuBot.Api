using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Application.IRepositories;
using KhuBot.Application.IServices;
using KhuBot.Domain.DTOs;
using KhuBot.Domain.Entities;
using KhuBot.Domain.Exceptions;
using KhuBot.Domain.Utilities;

namespace KhuBot.Application.Services
{
    public class ChatServices(
        IBaseRepository<User> userRepository,
        IBaseRepository<ChatMessage> chatMessageRepository,
        IChatBotRepository chatBotRepository) : IChatServices
    {
        public async Task<DataResponseDto<string>> SendMessageAsync(SendMessageRequestDto request, string rawDeveloperInstruction, int userId)
        {
            var beforeResponseDate = DateTime.Now;

            var user = (await userRepository.FindAsync(u => u.Id == userId))!;

            var usagePercent = 100;
            if (user.TokenLimit != 0)
            {
                usagePercent = (int)Math.Floor((double)user.TokenUsage / user.TokenLimit * 100);
                if (usagePercent > 100)
                    usagePercent = 100;
            }

            if (usagePercent == 100)
                throw new StatusCodeException([
                    new ErrorResponseDetailDto
                    {
                        ErrorId = null,
                        ErrorKey = null,
                        ErrorMessage = "متاسفانه اشتراکت تموم شد :(\nاگر از این نرم افزار راضی بودی خوشحال میشم نظرتو توی تلگرام باهام درمیون بزاری تا اشتراکتو بازم شارژ کنم :)",
                        IsInternalError = false
                    }
                ], HttpStatusCode.PaymentRequired);

            var formattedDeveloperInstruction =
                rawDeveloperInstruction.FormatTemplate(user.FirstName, user.LastName, usagePercent.ToString());

            var response = await chatBotRepository.GetResponseAsync(request.Message, formattedDeveloperInstruction);

            user.TokenUsage += response.TokenUsage;
            await userRepository.UpdateAsync(user);

            await chatMessageRepository.CreateAsync(new ChatMessage
            {
                UserId = userId,
                Message = request.Message,
                MessageTimeStamp = beforeResponseDate,
                Response = response.Content,
                ResponseTimeStamp = DateTime.Now
            });

            return new DataResponseDto<string>(response.Content);
        }

        public async Task<DataResponseDto<ChatListResponseDto>> GetChatListAsync(int userId)
        {
            var user = (await userRepository.FindAsync(u => u.Id == userId, u => u.ChatMessages))!;

            var usagePercent = 100;
            if (user.TokenLimit != 0)
            {
                usagePercent = (int)Math.Floor((double)user.TokenUsage / user.TokenLimit * 100);
                if (usagePercent > 100)
                    usagePercent = 100;
            }

            var chatMessageVms = new List<MessageDto>();

            foreach (var chatMessage in user.ChatMessages)
            {
                chatMessageVms.Add(new MessageDto
                {
                    Content = chatMessage.Message,
                    IsFromBot = false
                });

                chatMessageVms.Add(new MessageDto
                {
                    Content = chatMessage.Response,
                    IsFromBot = true
                });
            }

            return new DataResponseDto<ChatListResponseDto>(new ChatListResponseDto
            {
                Messages = chatMessageVms,
                UsagePercent = usagePercent
            });
        }
    }
}
