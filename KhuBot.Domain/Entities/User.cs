using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string HashPassword { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int TokenLimit { get; set; }

        public int TokenUsage { get; set; }

        #region Relations

        public List<ChatMessage> ChatMessages { get; set; } = [];

        #endregion
    }
}
