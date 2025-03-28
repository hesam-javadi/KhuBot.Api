using FluentAssertions;
using KhuBot.Domain.Entities;
using KhuBot.Infrastructure.Repositories;
using KhuBot.UnitTest.Fixtures;

namespace KhuBot.UnitTest.Repositories
{
    public class BaseRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsAllUsers_WhenNoConditionProvided()
        {
            // Arrange
            var context = RepositoryFixtures.GetInMemoryDbContext();
            var repository = new BaseRepository<User>(context);

            // Act
            var users = await repository.GetAllAsync();

            // Assert
            users.Should().HaveCount(2);
            users[0].Username.Should().Be("testuser");
            users[1].Username.Should().Be("testuser2");
        }

        [Theory]
        [InlineData("testuser")]
        [InlineData("testuser2")]
        public async Task GetAllAsync_ReturnsAllUsers_WithCondition(string username)
        {
            // Arrange
            var context = RepositoryFixtures.GetInMemoryDbContext();
            var repository = new BaseRepository<User>(context);

            // Act
            var users = await repository.GetAllAsync(u => u.Username == username);

            // Assert
            users.Should().HaveCount(1);
            users[0].Username.Should().Be(username);
        }

        [Fact]
        public async Task CreateAsync_AddsUser_WhenValidModelProvided()
        {
            // Arrange
            var context = RepositoryFixtures.GetInMemoryDbContext();
            var repository = new BaseRepository<User>(context);
            var newUser = new User
            {
                Username = "newuser",
                FirstName = "New",
                LastName = "User",
                HashPassword = "hashed",
                TokenLimit = 50
            };

            // Act
            var createdUser = await repository.CreateAsync(newUser);

            // Assert
            context.Users.Should().HaveCount(3);
            context.Users.Last().Should().BeEquivalentTo(newUser);
            createdUser.Should().BeEquivalentTo(newUser);
        }

        [Theory]
        [InlineData("testuser")]
        [InlineData("testuser2")]
        public async Task FindAsync_ReturnUser_WhenUserFound(string username)
        {
            // Arrange
            var context = RepositoryFixtures.GetInMemoryDbContext();
            var repository = new BaseRepository<User>(context);

            // Act
            var user = await repository.FindAsync(u => u.Username == username);

            // Assert
            user.Should().NotBeNull();
            user.Username.Should().Be(username);
        }

        [Fact]
        public async Task FindAsync_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            var context = RepositoryFixtures.GetInMemoryDbContext();
            var repository = new BaseRepository<User>(context);

            // Act
            var user = await repository.FindAsync(u => u.Username == "notfound");

            // Assert
            user.Should().BeNull();
        }

        [Theory]
        [InlineData("testuser", true)]
        [InlineData("testuser2", true)]
        [InlineData("notfound", false)]
        public async Task IsExistsAsync(string username, bool isExist)
        {
            // Arrange
            var context = RepositoryFixtures.GetInMemoryDbContext();
            var repository = new BaseRepository<User>(context);

            // Act
            var result = await repository.IsExistsAsync(u => u.Username == username);

            // Assert
            result.Should().Be(isExist);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesUser_WhenValidModelProvided()
        {
            // Arrange
            var context = RepositoryFixtures.GetInMemoryDbContext();
            var repository = new BaseRepository<User>(context);
            var user = context.Users.First();
            user.Username = "updateduser";

            // Act
            await repository.UpdateAsync(user);

            // Assert
            context.Users.Should().HaveCount(2);
            context.Users.First().Username.Should().Be("updateduser");
        }

        [Fact]
        public async Task DeleteAsync_RemovesUser_WhenValidModelProvided()
        {
            // Arrange
            var context = RepositoryFixtures.GetInMemoryDbContext();
            var repository = new BaseRepository<User>(context);
            var user = context.Users.First();

            // Act
            await repository.DeleteAsync(user);

            // Assert
            context.Users.Should().HaveCount(1);
            context.Users.Should().NotContain(user);
        }
    }
}
