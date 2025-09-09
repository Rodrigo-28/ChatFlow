using System.ComponentModel.DataAnnotations.Schema;

namespace ChatFlow.Domain.Models
{
    [Table("conversation")]
    public class Conversation
    {
        [Column("conversation_id")]
        public Guid Id { get; set; } // Primary key


        [Column("user1_id")]
        public Guid User1Id { get; set; } // Reference to one participant (User)
        public User User1 { get; set; }


        [Column("user2_id")]
        public Guid User2Id { get; set; } // Reference to the second participant (User)
        public User User2 { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;// When the conversation started


        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Last update (i.e., last message timestamp)
        public ICollection<Message> Messages { get; set; } // Messages in this conversation
    }
}
