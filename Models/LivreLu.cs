using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Bookify.Models
{
    public class LivreLu
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        [JsonIgnore]
        public Book Book { get; set; } = null!;

        [ForeignKey("User")]
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; } = null!;

        public DateTime DateLu { get; set; } = DateTime.Now;
    }
}