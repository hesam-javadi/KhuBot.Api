using FluentAssertions;
using KhuBot.Infrastructure.Repositories;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Domain.Entities;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;
using System.Reflection;
using Microsoft.SemanticKernel;
using KhuBot.Domain.Exceptions;
using System.ClientModel;
using System.ClientModel.Primitives;

namespace KhuBot.UnitTest.Repositories
{
    public class DeepSeekChatBotRepositoryTests
    {
        private readonly Mock<IChatCompletionService> _chatServiceMock;
        private readonly DeepSeekChatBotRepository _repository;

        public DeepSeekChatBotRepositoryTests()
        {
            _chatServiceMock = new Mock<IChatCompletionService>(MockBehavior.Strict);
            _repository = new DeepSeekChatBotRepository(_chatServiceMock.Object);
        }

        [Fact]
        public async Task GetResponseAsync_ReturnsBotResponse_WhenChatServiceSucceeds()
        {
            // Arrange
            var message = "What is 2+2?";
            var expectedContent = "4";
            var expectedTokenUsage = 500;
            Type chatTokenUsageType = typeof(ChatTokenUsage);
            ConstructorInfo chatTokenUsageConstructor = chatTokenUsageType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                [typeof(int), typeof(int), typeof(int)], null);
            ChatTokenUsage chatTokenUsage = (ChatTokenUsage)chatTokenUsageConstructor.Invoke([200, 300, expectedTokenUsage]);

            var chatResponse = new ChatMessageContent(AuthorRole.Assistant, expectedContent)
            {
                Metadata = new Dictionary<string, object>
                {
                    { "Usage", chatTokenUsage }
                }
            };
            _chatServiceMock.Setup(s =>
                    s.GetChatMessageContentsAsync(It.IsAny<ChatHistory>(), It.IsAny<PromptExecutionSettings>(), null,
                        default))
                .ReturnsAsync(new List<ChatMessageContent> { chatResponse });

            // Act
            var result = await _repository.GetResponseAsync(message, "You are a good assistant!");

            // Assert
            result.Should().NotBeNull();
            result.Content.Should().Be(expectedContent);
            result.TokenUsage.Should().Be(expectedTokenUsage);
        }

        [Fact]
        public async Task GetResponseAsync_ThrowsPaymentRequiredException_WhenChatServiceReturns402()
        {
            // Arrange
            var message = "What is 2+2?";
            var developerInstruction = "You are a good assistant!";
            var innerException = CreateClientResultException((int)HttpStatusCode.PaymentRequired);
            var exception = new HttpOperationException("Payment required", innerException);
            _chatServiceMock.Setup(s =>
                    s.GetChatMessageContentsAsync(It.IsAny<ChatHistory>(), It.IsAny<PromptExecutionSettings>(), null, default))
                .ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _repository.GetResponseAsync(message, developerInstruction);

            // Assert
            var thrownException = await act.Should().ThrowAsync<StatusCodeException>();
            thrownException.Which.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            thrownException.Which.ErrorResponse.ErrorMessages.Should().ContainSingle();
        }

        [Fact]
        public async Task GetResponseAsync_ThrowsTooManyRequestsException_WhenChatServiceReturns429()
        {
            // Arrange
            var message = "What is 2+2?";
            var developerInstruction = "You are a good assistant!";
            var innerException = CreateClientResultException((int)HttpStatusCode.TooManyRequests);
            var exception = new HttpOperationException("Too many requests", innerException);
            _chatServiceMock.Setup(s =>
                    s.GetChatMessageContentsAsync(It.IsAny<ChatHistory>(), It.IsAny<PromptExecutionSettings>(), null, default))
                .ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _repository.GetResponseAsync(message, developerInstruction);

            // Assert
            var thrownException = await act.Should().ThrowAsync<StatusCodeException>();
            thrownException.Which.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
            thrownException.Which.ErrorResponse.ErrorMessages.Should().ContainSingle();
        }

        [Fact]
        public async Task GetResponseAsync_ThrowsServiceUnavailableException_WhenChatServiceReturns503()
        {
            // Arrange
            var message = "What is 2+2?";
            var developerInstruction = "You are a good assistant!";
            var innerException = CreateClientResultException((int)HttpStatusCode.ServiceUnavailable);
            var exception = new HttpOperationException("Service unavailable", innerException);
            _chatServiceMock.Setup(s =>
                    s.GetChatMessageContentsAsync(It.IsAny<ChatHistory>(), It.IsAny<PromptExecutionSettings>(), null, default))
                .ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _repository.GetResponseAsync(message, developerInstruction);

            // Assert
            var thrownException = await act.Should().ThrowAsync<StatusCodeException>();
            thrownException.Which.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            thrownException.Which.ErrorResponse.ErrorMessages.Should().ContainSingle();
        }

        [Fact]
        public async Task GetResponseAsync_ThrowsArgumentException_WhenMessageIsEmpty()
        {
            // Arrange
            var message = " ";
            var developerInstruction = "You are a good assistant!";

            // Act
            Func<Task> act = async () => await _repository.GetResponseAsync(message, developerInstruction);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .Where(e => e.ParamName == "message");
        }

        private static ClientResultException CreateClientResultException(int statusCode)
        {
            var responseMock = new Mock<PipelineResponse>();
            responseMock.SetupGet(r => r.Status).Returns(statusCode);

            return new ClientResultException("", responseMock.Object);
        }
    }
}

