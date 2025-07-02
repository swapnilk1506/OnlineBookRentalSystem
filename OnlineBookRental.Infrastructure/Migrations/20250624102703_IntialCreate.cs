using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineBookRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RentalPricePerDay = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QuantityAvailable = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ImageUrl", "QuantityAvailable", "RentalPricePerDay", "Title" },
                values: new object[,]
                {
                    { 1, "F. Scott Fitzgerald", "A novel about the roaring twenties.", "978-0743273565", "https://placehold.co/150x200/F0F0F0/000?text=Gatsby", 5, 2.50m, "The Great Gatsby" },
                    { 2, "Harper Lee", "A classic of modern American literature.", "978-0446310789", "https://placehold.co/150x200/F0F0F0/000?text=Mockingbird", 3, 3.00m, "To Kill a Mockingbird" },
                    { 3, "George Orwell", "Dystopian social science fiction novel.", "978-0451524935", "https://placehold.co/150x200/F0F0F0/000?text=1984", 8, 2.00m, "1984" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
