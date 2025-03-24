using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Domain.Entities
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(this.User))]
        public int UserId { get; set; }

        public string Content { get; set; } = null!;

        public DateTime TimeStamp { get; set; }

        public bool IsFromBot { get; set; }

        #region Relations

        public virtual User User { get; set; } = null!;

        #endregion
    }
}
