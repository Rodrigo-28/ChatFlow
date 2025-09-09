using System.ComponentModel.DataAnnotations.Schema;

namespace ChatFlow.Domain.Models
{
    [Table("notification")]
    public class Notification
    {
        [Column("notification_id")]
        public Guid Id { get; set; } // Primary key
        [Column("user_id")]

        public Guid UserId { get; set; } // The user who receives the notification

        public User User { get; set; }
        [Column("message")]

        public string Message { get; set; } // Notification content
        [Column("created_at")]

        public DateTime CreatedAt { get; set; } // When the notification was created
        [Column("is_read")]

        public bool IsRead { get; set; } // Whether the notification has been seen
    }
}
