using KhuBot.Application.IRepositories;
using KhuBot.Application.Services;
using KhuBot.Domain.DTOs;
using KhuBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using KhuBot.Domain.Exceptions;
using Moq;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.SqlServer.Server;

namespace KhuBot.UnitTest.Services
{
    public class ChatServicesTests
    {
        private readonly Mock<IBaseRepository<User>> _userRepositoryMock;
        private readonly Mock<IBaseRepository<ChatMessage>> _chatMessageRepositoryMock;
        private readonly Mock<IChatBotRepository> _chatBotRepsoitory;
        private readonly ChatServices _chatServices;

        public ChatServicesTests()
        {
            _userRepositoryMock = new Mock<IBaseRepository<User>>();
            _chatMessageRepositoryMock = new Mock<IBaseRepository<ChatMessage>>();
            _chatBotRepsoitory = new Mock<IChatBotRepository>();
            _chatServices = new ChatServices(_userRepositoryMock.Object, _chatMessageRepositoryMock.Object,
                _chatBotRepsoitory.Object);
        }

        [Fact]
        public async Task GetChatListAsync_ShouldReturnCorrectChatList()
        {
            // Arrange
            int userId = 1;
            var user = new User
            {
                Id = userId,
                TokenUsage = 75,
                TokenLimit = 100,
                ChatMessages =
                [
                    new ChatMessage { Id = 1, UserId = userId, Message = "Hello", Response = "Hi there!" }
                ]
            };

            _userRepositoryMock.Setup(repo => repo.FindAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<User, dynamic>>[]>()))
                .ReturnsAsync(user);

            // Act
            var result = await _chatServices.GetChatListAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Messages.Count.Should().Be(2);
            result.Data.UsagePercent.Should().Be(75);
            result.Data.Messages[0].Content.Should().Be("Hello");
            result.Data.Messages[0].IsFromBot.Should().BeFalse();
            result.Data.Messages[1].Content.Should().Be("Hi there!");
            result.Data.Messages[1].IsFromBot.Should().BeTrue();
        }

        [Fact]
        public async Task GetChatListAsync_WithZeroTokenLimit_ShouldReturnOneHundredPercent()
        {
            // Arrange
            int userId = 1;
            var user = new User
            {
                Id = userId,
                TokenUsage = 75,
                TokenLimit = 0,
                ChatMessages = []
            };

            _userRepositoryMock.Setup(repo => repo.FindAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<User, dynamic>>[]>()))
                .ReturnsAsync(user);

            // Act
            var result = await _chatServices.GetChatListAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.UsagePercent.Should().Be(100);
        }

        [Fact]
        public async Task GetChatListAsync_WithExceededTokenLimit_ShouldCap100Percent()
        {
            // Arrange
            int userId = 1;
            var user = new User
            {
                Id = userId,
                TokenUsage = 150,
                TokenLimit = 100,
                ChatMessages = []
            };

            _userRepositoryMock.Setup(repo => repo.FindAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<User, dynamic>>[]>()))
                .ReturnsAsync(user);

            // Act
            var result = await _chatServices.GetChatListAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.UsagePercent.Should().Be(100);
        }

        [Fact]
        public async Task SendMessageAsync_GetCorrectResponse_WhenUserHasRemainingToken()
        {
            // Arrange
            var userId = 1;
            var message = "Hello";
            var expectedResponse = new BotResponse
            {
                Content = "Hi there!",
                TokenUsage = 20
            };
            var user = new User
            {
                Id = userId,
                TokenUsage = 0,
                TokenLimit = 100,
                ChatMessages = []
            };

            _userRepositoryMock.Setup(repo => repo.FindAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<User, dynamic>>[]>()))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask)
                .Callback<User>(u =>
                {
                    _userRepositoryMock.Setup(repo => repo.FindAsync(
                            It.Is<System.Linq.Expressions.Expression<Func<User, bool>>>(expr => true),
                            It.IsAny<System.Linq.Expressions.Expression<Func<User, dynamic>>[]>()))
                        .ReturnsAsync(u);
                });

            _chatBotRepsoitory.Setup(repo => repo.GetResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _chatServices.SendMessageAsync(new SendMessageRequestDto { Message = message },
                "Something", userId);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("Hi there!");
            var updatedUser = await _userRepositoryMock.Object.FindAsync(u => u.Id == userId);
            updatedUser.Should().NotBeNull();
            updatedUser.TokenUsage.Should().Be(20);
        }

        [Fact]
        public async Task SendMessageAsync_GetCorrectResponse_WhenUserHasExceededToken()
        {
            // Arrange
            var userId = 1;
            var message = "Hello";
            var user = new User
            {
                Id = userId,
                TokenUsage = 100,
                TokenLimit = 100,
                ChatMessages = []
            };
            _userRepositoryMock.Setup(repo => repo.FindAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<User, dynamic>>[]>()))
                .ReturnsAsync(user);

            // Act
            Func<Task> act = async () =>
                await _chatServices.SendMessageAsync(new SendMessageRequestDto { Message = message }, "Something",
                    userId);

            // Assert
            var thrownException = await act.Should().ThrowAsync<StatusCodeException>();
            thrownException.Which.StatusCode.Should().Be(HttpStatusCode.PaymentRequired);
            thrownException.Which.ErrorResponse.ErrorMessages.Should().ContainSingle();
        }
    }
}