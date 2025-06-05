using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BookApi.Models
{
    /// <summary>
    /// Represents a book in the library management system
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Unique identifier for the book (auto-generated)
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The title of the book
        /// </summary>
        /// <example>The Great Gatsby</example>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [DisplayName("Book Title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The author of the book
        /// </summary>
        /// <example>F. Scott Fitzgerald</example>
        [Required(ErrorMessage = "Author is required")]
        [StringLength(50, ErrorMessage = "Author name cannot exceed 50 characters")]
        [DisplayName("Author Name")]
        public string Author { get; set; } = string.Empty;
        
        /// <summary>
        /// The publication date of the book
        /// </summary>
        /// <example>1925-04-10</example>
        [DataType(DataType.Date)]
        [DisplayName("Publication Date")]
        public DateTime PublicationDate { get; set; }
        
        /// <summary>
        /// The price of the book in USD
        /// </summary>
        /// <example>15.99</example>
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between $0.01 and $10,000.00")]
        [DataType(DataType.Currency)]
        [DisplayName("Price (USD)")]
        public decimal Price { get; set; }
    }
}