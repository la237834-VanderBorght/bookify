using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Bookify.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [JsonIgnore]
        public Book? Book { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        [Required]
        [Range(0, 5)]
        public int Note { get; set; }

        [MaxLength(100)]
        public string Titre { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Avis { get; set; }
    }
}