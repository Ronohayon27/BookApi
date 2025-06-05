using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BookApi.Models
{
    // Book model,
    public class Book
    {
        // identifier
        public int Id { get; set; }


        // Name of the book
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        // Name of the Author
        [Required]
        [StringLength(50)]
        public string Author { get; set; } = string.Empty;
        
        // Date of publication
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }
        
        // Book Price
        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }
    }
}