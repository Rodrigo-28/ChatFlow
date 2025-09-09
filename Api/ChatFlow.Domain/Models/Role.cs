using System.ComponentModel.DataAnnotations.Schema;

namespace ChatFlow.Domain.Models
{
    [Table("role")]
    public class Role
    {
        [Column("role_id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
