using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using BookApi.Data;
using BookApi.Models;

namespace BookApi.Controllers
{
    /// <summary>
    /// API controller for managing book resources
    /// </summary>
    [Route("api/[controller]")]
    // for model validation
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a paginated list of books
        /// </summary>
        /// <param name="page">Page number (starts at 1)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>A collection of books for the requested page</returns>
        /// <response code="200">Returns the list of books</response>
        /// <response code="400">If page or pageSize parameters are invalid</response>
        /// <response code="404">If the requested page exceeds total pages</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(int page = 1, int pageSize = 10)
        {
            // Validate page and pageSize parameters
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be greater than 0.");
            }
            // Calculate total number of books and total pages
            var totalBooks = await _context.Books.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

            // If the requested page exceeds total pages, return NotFound
            if (page > totalPages)
            {
                return NotFound("Page number exceeds total pages.");
            }
            // Fetch the books for the requested page
            // Using Skip and Take for pagination
            var books = await _context.Books
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return Ok(books);
        }


        /// <summary>
        /// Gets a specific book by ID
        /// </summary>
        /// <param name="id">The ID of the book to retrieve</param>
        /// <returns>The requested book</returns>
        /// <response code="200">Returns the requested book</response>
        /// <response code="404">If the book with the specified ID is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            // Find the book with the specified ID
            var book = await _context.Books.FindAsync(id);
            // If book is not found, return NotFound
            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <param name="book">The book data to create</param>
        /// <returns>The created book with its assigned ID</returns>
        /// <response code="201">Returns the newly created book</response>
        /// <response code="400">If the book data is invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="id">The ID of the book to update</param>
        /// <param name="updatedBook">The updated book data</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the book was successfully updated</response>
        /// <response code="400">If the ID in the URL doesn't match the ID in the body</response>
        /// <response code="404">If the book with the specified ID is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateBook(int id, Book updatedBook)
        {
            // Check if the ID in the URL matches the ID in the body
            if (id != updatedBook.Id)
            {
                return BadRequest("ID in URL and body must match.");
            }
            // Find the existing book
            var existingBook = await _context.Books.FindAsync(id);

            // If the book does not exist, return NotFound
            if (existingBook == null)
            {
                return NotFound();
            }

            // Update the properties
            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;
            existingBook.PublicationDate = updatedBook.PublicationDate;
            existingBook.Price = updatedBook.Price;

            await _context.SaveChangesAsync();

            return NoContent(); // 204 success, no body
        }

        /// <summary>
        /// Deletes a specific book
        /// </summary>
        /// <param name="id">The ID of the book to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the book was successfully deleted</response>
        /// <response code="404">If the book with the specified ID is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteBook(int id)
        {
            // Find the book with the specified ID
            var book = await _context.Books.FindAsync(id);

            // If book is not found, return NotFound
            if (book == null)
            {
                return NotFound();
            }

            // Remove the book from the context
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 success
        }

        /// <summary>
        /// Searches for books by title or author
        /// </summary>
        /// <param name="query">The search query string</param>
        /// <returns>A collection of books matching the search criteria</returns>
        /// <response code="200">Returns the list of matching books</response>
        /// <response code="400">If the query string is empty</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks(string query)
        {
            // Validate the query string
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query string is required.");
            }

            // Normalize the query to lower case for case-insensitive search, looks more user-friendly
            var results = await _context.Books
                .Where(b =>
                    b.Title.ToLower().Contains(query.ToLower()) ||
                    b.Author.ToLower().Contains(query.ToLower()))
                .ToListAsync();

            return Ok(results);
        }
    }
}
