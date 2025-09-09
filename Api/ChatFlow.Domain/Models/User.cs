using System.ComponentModel.DataAnnotations.Schema;

namespace ChatFlow.Domain.Models
{
    [Table("users")]
    public class User
    {
        [Column("user_id")]
        public Guid Id { get; set; } // Primary key

        [Column("username")]
        public string Username { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; } // Hashed password for security

        //[Column("profile_url")]
        //public string ProfilePictureUrl { get; set; } // Optional

        [Column("is_online")]
        public bool IsOnline { get; set; } = false;// Online/Offline status

        [Column("last_online")]
        public DateTime LastOnline { get; set; } = DateTime.UtcNow; // Timestamp of last online status

        [Column("role_id")]
        public int RoleId { get; set; }
        public Role Role { get; set; } // Role (e.g., User, Admin)
        public Profile Profile { get; set; }



        public ICollection<Conversation> Conversations { get; set; } // List of conversations

        public ICollection<Message> Messages { get; set; } // List of messages sent by the user
    }
}
