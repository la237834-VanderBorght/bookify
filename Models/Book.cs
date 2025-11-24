using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Bookify.Models
{
    /// <summary>
    /// Classe Book reéprésentant un livre dans l'application
    /// </summary>
    public class Book
    {
        // Attributs de la classe Book
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "L'auteur est obligatoire.")]
        public string Author { get; set; }
        [Required(ErrorMessage = "ISBN obligatoire.")]
        public string ISBN { get; set; }
        [Required(ErrorMessage = "Le prix est obligatoire.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "La description est obligatoire.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "L'éditeur est obligatoire.")]
        public string Publisher { get; set; }

        // Foreign key for Gender
        [ForeignKey("Gender")]
        public int GenderId { get; set; }
        [JsonIgnore]
        public Gender Gender { get; set; }

        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
