using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BookApi.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Author { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }

        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }
    }
}