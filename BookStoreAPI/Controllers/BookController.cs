using Microsoft.AspNetCore.Mvc;
using BookStoreAPI.Data;
using BookStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BookController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        return await _context.Books.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Book>> AddBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
    }
}
