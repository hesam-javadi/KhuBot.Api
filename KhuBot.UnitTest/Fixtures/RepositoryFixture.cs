using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Domain.Entities;
using KhuBot.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KhuBot.UnitTest.Fixtures
{
    public class RepositoryFixtures
    {
        public static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(options);
            SeedData(context);
            return context;
        }

        private static void SeedData(ApplicationDbContext context)
        {
            context.Users.Add(new User
            {
                Id = 1,
                Username = "testuser",
                HashPassword = "hashed",
                FirstName = "Test",
                LastName = "User",
                TokenLimit = 100,
                TokenUsage = 20
            });
            context.Users.Add(new User
            {
                Id = 2,
                Username = "testuser2",
                HashPassword = "hashed2",
                FirstName = "Test2",
                LastName = "User2",
                TokenLimit = 100,
                TokenUsage = 20
            });
            context.SaveChanges();
        }
    }
}
