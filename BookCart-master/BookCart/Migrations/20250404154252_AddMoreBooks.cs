using Microsoft.EntityFrameworkCore.Migrations;
using BookCart.Models;

#nullable disable

namespace BookCart.Migrations
{
    public partial class AddMoreBooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Book",
                columns: new[] { "Title", "Author", "Category", "Price", "CoverFileName" },
                values: new object[,]
                {
                    { "Book 1", "Author 1", "Category 1", 19.99m, "book1.jpg" },
                    { "Book 2", "Author 2", "Category 2", 24.99m, "book2.jpg" },
                    { "Book 3", "Author 3", "Category 1", 15.99m, "book3.jpg" },
                    { "Book 4", "Author 4", "Category 3", 29.99m, "book4.jpg" },
                    { "Book 5", "Author 5", "Category 2", 21.99m, "book5.jpg" },
                    { "Book 6", "Author 6", "Category 3", 17.99m, "book6.jpg" },
                    { "Book 7", "Author 7", "Category 1", 22.99m, "book7.jpg" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Book WHERE Title IN ('Book 1', 'Book 2', 'Book 3', 'Book 4', 'Book 5', 'Book 6', 'Book 7')");
        }
    }
}
