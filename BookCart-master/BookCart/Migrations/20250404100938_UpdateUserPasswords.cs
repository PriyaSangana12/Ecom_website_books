using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookCart.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE UserMaster 
                SET Password = SHA2(Password, 256)
                WHERE LENGTH(Password) < 64"); // Only hash plaintext passwords
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
