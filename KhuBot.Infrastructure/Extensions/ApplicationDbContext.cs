using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhuBot.Infrastructure.Extensions
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
    }
}
