using Microsoft.AspNetCore.Mvc;
using BookCart.API.Data;
using BookCart.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookCart.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BooksController> _logger;

        public BooksController(ApplicationDbContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            _logger.LogInformation("Fetching all books from the database");

            var books = await _context.Books.ToListAsync();

            _logger.LogInformation($"Fetched {books.Count} books from the database");

            return Ok(books);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            _logger.LogInformation("Creating a new book entry");

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Book with ID {book.Id} was created successfully");

            return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
        }
    }
}
