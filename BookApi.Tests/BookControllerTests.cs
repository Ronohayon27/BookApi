using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Controllers;
using BookApi.Data;
using BookApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookApi.Tests
{
    /// <summary>
    /// Unit tests for the BooksController class
    /// </summary>
    public class BookControllerTests
    {
        // Helper method to create an in-memory database for testing
        private AppDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            var dbContext = new AppDbContext(options);
            
            // Add some test data
            if (dbContext.Books.Count() == 0)
            {
                dbContext.Books.Add(new Book
                {
                    Id = 1,
                    Title = "Test Book 1",
                    Author = "Test Author 1",
                    PublicationDate = new DateTime(2020, 1, 1),
                    Price = 19.99m
                });
                
                dbContext.Books.Add(new Book
                {
                    Id = 2,
                    Title = "Test Book 2",
                    Author = "Test Author 2",
                    PublicationDate = new DateTime(2021, 2, 2),
                    Price = 29.99m
                });
                
                dbContext.Books.Add(new Book
                {
                    Id = 3,
                    Title = "Test Book 3",
                    Author = "Test Author 3",
                    PublicationDate = new DateTime(2022, 3, 3),
                    Price = 39.99m
                });
                
                dbContext.SaveChanges();
            }
            
            return dbContext;
        }
        
        /// <summary>
        /// Tests that GetBooks returns all books with default pagination
        /// </summary>
        [Fact]
        public async Task GetBooks_ReturnsAllBooks_WhenDefaultPagination()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            
            // Act
            var result = await controller.GetBooks();
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var books = Assert.IsAssignableFrom<List<Book>>(okResult.Value);
            Assert.Equal(3, books.Count);
        }
        
        /// <summary>
        /// Tests that GetBooks returns BadRequest when page is less than 1
        /// </summary>
        [Fact]
        public async Task GetBooks_ReturnsBadRequest_WhenPageIsLessThanOne()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            
            // Act
            var result = await controller.GetBooks(page: 0);
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
        
        /// <summary>
        /// Tests that GetBooks returns NotFound when page exceeds total pages
        /// </summary>
        [Fact]
        public async Task GetBooks_ReturnsNotFound_WhenPageExceedsTotalPages()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            
            // Act
            var result = await controller.GetBooks(page: 100, pageSize: 10);
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }
        
        /// <summary>
        /// Tests that GetBook returns the correct book when it exists
        /// </summary>
        [Fact]
        public async Task GetBook_ReturnsBook_WhenBookExists()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            var bookId = 1;
            
            // Act
            var result = await controller.GetBook(bookId);
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<Book>>(result);
            var book = Assert.IsType<Book>(actionResult.Value);
            Assert.Equal(bookId, book.Id);
            Assert.Equal("Test Book 1", book.Title);
        }
        
        /// <summary>
        /// Tests that GetBook returns NotFound when the book doesn't exist
        /// </summary>
        [Fact]
        public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            var nonExistentBookId = 999;
            
            // Act
            var result = await controller.GetBook(nonExistentBookId);
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<Book>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
        
        /// <summary>
        /// Tests that CreateBook adds a new book and returns it with a 201 Created status
        /// </summary>
        [Fact]
        public async Task CreateBook_ReturnsCreatedAtAction_WithNewBook()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            var newBook = new Book
            {
                Title = "New Test Book",
                Author = "New Test Author",
                PublicationDate = new DateTime(2023, 4, 4),
                Price = 49.99m
            };
            
            // Act
            var result = await controller.CreateBook(newBook);
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<Book>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnedBook = Assert.IsType<Book>(createdAtActionResult.Value);
            
            Assert.Equal("New Test Book", returnedBook.Title);
            Assert.Equal("New Test Author", returnedBook.Author);
            Assert.Equal(new DateTime(2023, 4, 4), returnedBook.PublicationDate);
            Assert.Equal(49.99m, returnedBook.Price);
            Assert.True(returnedBook.Id > 0); // Ensure ID was assigned
            
            // Verify book was added to database
            var bookInDb = await dbContext.Books.FindAsync(returnedBook.Id);
            Assert.NotNull(bookInDb);
        }
        
        /// <summary>
        /// Tests that UpdateBook returns NoContent when successfully updating a book
        /// </summary>
        [Fact]
        public async Task UpdateBook_ReturnsNoContent_WhenBookExists()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            var bookId = 2;
            var updatedBook = new Book
            {
                Id = bookId,
                Title = "Updated Book Title",
                Author = "Updated Author",
                PublicationDate = new DateTime(2024, 5, 5),
                Price = 59.99m
            };
            
            // Act
            var result = await controller.UpdateBook(bookId, updatedBook);
            
            // Assert
            Assert.IsType<NoContentResult>(result);
            
            // Verify book was updated in database
            var bookInDb = await dbContext.Books.FindAsync(bookId);
            Assert.NotNull(bookInDb);
            Assert.Equal("Updated Book Title", bookInDb.Title);
            Assert.Equal("Updated Author", bookInDb.Author);
            Assert.Equal(new DateTime(2024, 5, 5), bookInDb.PublicationDate);
            Assert.Equal(59.99m, bookInDb.Price);
        }
        
        /// <summary>
        /// Tests that UpdateBook returns BadRequest when ID in URL doesn't match ID in body
        /// </summary>
        [Fact]
        public async Task UpdateBook_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            var bookId = 2;
            var updatedBook = new Book
            {
                Id = 999, // Different ID than in URL
                Title = "Updated Book Title",
                Author = "Updated Author",
                PublicationDate = new DateTime(2024, 5, 5),
                Price = 59.99m
            };
            
            // Act
            var result = await controller.UpdateBook(bookId, updatedBook);
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        /// <summary>
        /// Tests that UpdateBook returns NotFound when the book doesn't exist
        /// </summary>
        [Fact]
        public async Task UpdateBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            var nonExistentBookId = 999;
            var updatedBook = new Book
            {
                Id = nonExistentBookId,
                Title = "Updated Book Title",
                Author = "Updated Author",
                PublicationDate = new DateTime(2024, 5, 5),
                Price = 59.99m
            };
            
            // Act
            var result = await controller.UpdateBook(nonExistentBookId, updatedBook);
            
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        
        /// <summary>
        /// Tests that DeleteBook returns NoContent when successfully deleting a book
        /// </summary>
        [Fact]
        public async Task DeleteBook_ReturnsNoContent_WhenBookExists()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            var bookId = 3;
            
            // Act
            var result = await controller.DeleteBook(bookId);
            
            // Assert
            Assert.IsType<NoContentResult>(result);
            
            // Verify book was deleted from database
            var bookInDb = await dbContext.Books.FindAsync(bookId);
            Assert.Null(bookInDb);
        }
        
        /// <summary>
        /// Tests that DeleteBook returns NotFound when the book doesn't exist
        /// </summary>
        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            var nonExistentBookId = 999;
            
            // Act
            var result = await controller.DeleteBook(nonExistentBookId);
            
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        
        /// <summary>
        /// Tests that SearchBooks returns matching books when query is valid
        /// </summary>
        [Fact]
        public async Task SearchBooks_ReturnsMatchingBooks_WhenQueryIsValid()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            var searchQuery = "Test Author 2"; // Should match one book
            
            // Act
            var result = await controller.SearchBooks(searchQuery);
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var books = Assert.IsAssignableFrom<List<Book>>(okResult.Value);
            
            Assert.Single(books);
            Assert.Equal(2, books[0].Id);
            Assert.Equal("Test Book 2", books[0].Title);
        }
        
        /// <summary>
        /// Tests that SearchBooks returns empty list when no books match the query
        /// </summary>
        [Fact]
        public async Task SearchBooks_ReturnsEmptyList_WhenNoMatches()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            var searchQuery = "Nonexistent Book";
            
            // Act
            var result = await controller.SearchBooks(searchQuery);
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var books = Assert.IsAssignableFrom<List<Book>>(okResult.Value);
            
            Assert.Empty(books);
        }
        
        /// <summary>
        /// Tests that SearchBooks returns BadRequest when query is empty
        /// </summary>
        [Fact]
        public async Task SearchBooks_ReturnsBadRequest_WhenQueryIsEmpty()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BooksController(dbContext);
            string searchQuery = "";
            
            // Act
            var result = await controller.SearchBooks(searchQuery);
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
    }
}
