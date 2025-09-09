using System.ComponentModel.DataAnnotations.Schema;

namespace ChatFlow.Domain.Models
{
    [Table("message")]
    public class Message
    {
        [Column("message_id")]
        public Guid Id { get; set; } // Primary key

        [Column("conversation_id")]
        public Guid ConversationId { get; set; } // Reference to the conversation
        public Conversation Conversation { get; set; }


        [Column("sender_id")]
        public Guid SenderId { get; set; } // Reference to the user who sent the message
        public User Sender { get; set; }


        [Column("content")]
        public string Content { get; set; } // Message text

        [Column("sent_at")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow; // Timestamp when the message was sent

        [Column("is_read")]
        public bool IsRead { get; set; } = false; // Whether the message has been read by the recipient
    }
}
