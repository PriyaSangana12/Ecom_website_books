using BookCart.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCart.Data
{
    public static class BookSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BookDBContext(
                serviceProvider.GetRequiredService<DbContextOptions<BookDBContext>>()))
            {
                if (context.Book.Any())
                {
                    return; // DB already seeded
                }

                context.Book.AddRange(
                    new Book
                    {
                        Title = "Clean Code",
                        Author = "Robert C. Martin",
                        Category = "Programming",
                        Price = 45.99m,
                        CoverFileName = "clean_code.jpg"
                    },
                    new Book
                    {
                        Title = "Design Patterns",
                        Author = "Erich Gamma",
                        Category = "Programming",
                        Price = 54.99m,
                        CoverFileName = "design_patterns.jpg"
                    },
                    new Book
                    {
                        Title = "The Pragmatic Programmer",
                        Author = "Andrew Hunt",
                        Category = "Programming",
                        Price = 39.99m,
                        CoverFileName = "pragmatic_programmer.jpg"
                    },
                    new Book
                    {
                        Title = "Code Complete",
                        Author = "Steve McConnell",
                        Category = "Programming",
                        Price = 49.99m,
                        CoverFileName = "code_complete.jpg"
                    },
                    new Book
                    {
                        Title = "Refactoring",
                        Author = "Martin Fowler",
                        Category = "Programming",
                        Price = 44.99m,
                        CoverFileName = "refactoring.jpg"
                    },
                    new Book
                    {
                        Title = "Head First Design Patterns",
                        Author = "Eric Freeman",
                        Category = "Programming",
                        Price = 52.99m,
                        CoverFileName = "head_first_dp.jpg"
                    },
                    new Book
                    {
                        Title = "Domain-Driven Design",
                        Author = "Eric Evans",
                        Category = "Programming",
                        Price = 47.99m,
                        CoverFileName = "ddd.jpg"
                    },
                    new Book
                    {
                        Title = "The Clean Coder",
                        Author = "Robert C. Martin",
                        Category = "Programming",
                        Price = 38.99m,
                        CoverFileName = "clean_coder.jpg"
                    },
                    new Book
                    {
                        Title = "Working Effectively with Legacy Code",
                        Author = "Michael Feathers",
                        Category = "Programming",
                        Price = 42.99m,
                        CoverFileName = "legacy_code.jpg"
                    },
                    new Book
                    {
                        Title = "Test Driven Development",
                        Author = "Kent Beck",
                        Category = "Programming",
                        Price = 36.99m,
                        CoverFileName = "tdd.jpg"
                    },
                    new Book
                    {
                        Title = "Book 1",
                        Author = "Author 1",
                        Category = "Category 1",
                        Price = 19.99m,
                        CoverFileName = "book1.jpg"
                    },
                    new Book
                    {
                        Title = "Book 2",
                        Author = "Author 2",
                        Category = "Category 2",
                        Price = 24.99m,
                        CoverFileName = "book2.jpg"
                    },
                    new Book
                    {
                        Title = "Book 3",
                        Author = "Author 3",
                        Category = "Category 1",
                        Price = 15.99m,
                        CoverFileName = "book3.jpg"
                    },
                    new Book
                    {
                        Title = "Book 4",
                        Author = "Author 4",
                        Category = "Category 3",
                        Price = 29.99m,
                        CoverFileName = "book4.jpg"
                    },
                    new Book
                    {
                        Title = "Book 5",
                        Author = "Author 5",
                        Category = "Category 2",
                        Price = 21.99m,
                        CoverFileName = "book5.jpg"
                    },
                    new Book
                    {
                        Title = "Book 6",
                        Author = "Author 6",
                        Category = "Category 3",
                        Price = 17.99m,
                        CoverFileName = "book6.jpg"
                    },
                    new Book
                    {
                        Title = "Book 7",
                        Author = "Author 7",
                        Category = "Category 1",
                        Price = 22.99m,
                        CoverFileName = "book7.jpg"
                    }
                );

                var count = context.SaveChanges();
                var logger = serviceProvider.GetService<ILogger<BookDBContext>>();
                logger?.LogInformation($"Seeded {count} books to database");
            }
        }
    }
}
