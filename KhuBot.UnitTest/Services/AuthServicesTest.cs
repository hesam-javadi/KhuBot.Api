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

namespace KhuBot.UnitTest.Services
{
    public class AuthServicesTests
    {
        private readonly Mock<IBaseRepository<User>> _userRepositoryMock;
        private readonly AuthServices _authServices;

        public AuthServicesTests()
        {
            _userRepositoryMock = new Mock<IBaseRepository<User>>();
            _authServices = new AuthServices(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequestDto { Username = "testuser", Password = "TestUser123" };
            Environment.SetEnvironmentVariable("JWT_KEY", Guid.NewGuid().ToString());
            _userRepositoryMock.Setup(r => r.FindAsync(u => u.Username == request.Username))
                .ReturnsAsync(new User
                {
                    Id = 1,
                    Username = "testuser",
                    HashPassword = BCrypt.Net.BCrypt.HashPassword("TestUser123"),
                    FirstName = "Test",
                    LastName = "User"
                });

            // Act
            DataResponseDto<LoginResponseDto>? response = null;
            Func<Task> act = async () => response = await _authServices.LoginAsync(request);

            // Assert
            await act.Should().NotThrowAsync();
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(response.Data.Token);
            var userId = Convert.ToInt32(jwtSecurityToken.Claims.First(claim => claim.Type == "userId").Value);
            userId.Should().Be(1);
        }

        [Fact]
        public async Task LoginAsync_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var request = new LoginRequestDto { Username = "testuser", Password = "TestUser123" };
            _userRepositoryMock.Setup(r => r.FindAsync(u => u.Username == request.Username))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _authServices.LoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<StatusCodeException>()
                .Where(e => e.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task LoginAsync_ThrowsException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new LoginRequestDto { Username = "testuser", Password = "TestUser123" };
            _userRepositoryMock.Setup(r => r.FindAsync(u => u.Username == request.Username))
                .ReturnsAsync(new User
                {
                    Id = 1,
                    Username = "testuser",
                    HashPassword = BCrypt.Net.BCrypt.HashPassword("TestUser1234"),
                    FirstName = "Test",
                    LastName = "User"
                });

            // Act
            Func<Task> act = async () => await _authServices.LoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<StatusCodeException>()
                .Where(e => e.StatusCode == HttpStatusCode.BadRequest);
        }
    }
}
