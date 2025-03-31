using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Application.IRepositories;
using KhuBot.Domain.DTOs;
using KhuBot.Domain.Entities;
using KhuBot.Domain.Exceptions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

namespace KhuBot.Infrastructure.Repositories
{
    public class DeepSeekChatBotRepository(IChatCompletionService chatCompletionService) : IChatBotRepository
    {
        public async Task<BotResponse> GetResponseAsync(string message, string developerInstruction,
            List<BotChatHistory> histories)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.", nameof(message));

            var chatHistory = new ChatHistory();
            foreach (var botChatHistory in histories)
            {
                if (botChatHistory.IsFromBot)
                    chatHistory.AddAssistantMessage(botChatHistory.Message);
                else
                    chatHistory.AddUserMessage(botChatHistory.Message);
            }
            chatHistory.AddSystemMessage(developerInstruction);
            chatHistory.AddUserMessage(message);
            IReadOnlyList<ChatMessageContent> response;
            try
            {
                response = await chatCompletionService.GetChatMessageContentsAsync(chatHistory,
                    new OpenAIPromptExecutionSettings()
                    {
                        Temperature = 0,
                    });
            }
            catch (HttpOperationException e)
            {
                if (e.InnerException is ClientResultException { Status: 402 })
                {
                    throw new StatusCodeException([
                        new ErrorResponseDetailDto
                        {
                            ErrorId = null,
                            ErrorKey = null,
                            ErrorMessage =
                                "متاسفانه موجودیمون برای استفاده از سرویس‌های هوش مصنوعی تموم شده :(\nاگر از این نرم افزار راضی بودید خوشحال میشم نظرتونو بگید تا اگر استقبال‌ها زیاد بود دوباره شارژش کنیم براتون."
                        }
                    ], HttpStatusCode.ServiceUnavailable);
                }

                if (e.InnerException is ClientResultException { Status: 429 })
                {
                    throw new StatusCodeException([
                        new ErrorResponseDetailDto
                        {
                            ErrorId = null,
                            ErrorKey = null,
                            ErrorMessage = "خیلی سریع پیام میفرستی :> یه نفسی بگیر و دوباره تلاش کن."
                        }
                    ], HttpStatusCode.TooManyRequests);
                }

                if (e.InnerException is ClientResultException { Status: 503 })
                {
                    throw new StatusCodeException([
                        new ErrorResponseDetailDto
                        {
                            ErrorId = null,
                            ErrorKey = null,
                            ErrorMessage = "متاسفانه سرویس‌دهندمون یکم شلوغه، بعد از چند دقیقه دوباره تلاش کن."
                        }
                    ], HttpStatusCode.ServiceUnavailable);
                }

                throw;
            }

            var content = response[0].Content ?? throw new InvalidOperationException("No response content received.");
            var tokenUsage = 0;
            var metadata = response[0].Metadata ??
                           new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>());
            metadata.TryGetValue("Usage", out var usageObj);
            if (usageObj is ChatTokenUsage convertedUsageObj)
            {
                tokenUsage = convertedUsageObj.TotalTokenCount;
            }

            return new BotResponse
            {
                Content = content,
                TokenUsage = tokenUsage
            };
        }
    }
}
