using Microsoft.EntityFrameworkCore;
using BookStoreAPI.Models;

namespace BookStoreAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Book> Books { get; set; }
}
