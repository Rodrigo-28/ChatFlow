using System.ComponentModel.DataAnnotations.Schema;

namespace ChatFlow.Domain.Models
{
    [Table("profile")]
    public class Profile
    {
        [Column("user_id")]
        public Guid UserId { get; set; } // Reference to the user (also the primary key)
        //preguntar...
        public User User { get; set; }
        [Column("bio")]

        public string Bio { get; set; } // Optional user bio

        [Column("profile_picture_url")]

        public string ProfilePictureUrl { get; set; } // Optional profile picture
    }
}
