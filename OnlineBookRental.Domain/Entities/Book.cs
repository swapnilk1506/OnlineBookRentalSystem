namespace OnlineBookRental.Domain.Entities
{
    // Represents a book entity in the system.
    public class Book
    {
        // Unique identifier for the book.
        public int Id { get; set; }
        // Title of the book.
        public string Title { get; set; } = string.Empty;
        // Author of the book.
        public string Author { get; set; } = string.Empty;
        // ISBN (International Standard Book Number) of the book.
        public string ISBN { get; set; } = string.Empty;
        // Description or summary of the book.
        public string Description { get; set; } = string.Empty;
        // URL for the book's cover image.
        public string ImageUrl { get; set; } = string.Empty;
        // The price for renting the book per day.
        public decimal RentalPricePerDay { get; set; }
        // The current quantity of this book available for rental.
        public int QuantityAvailable { get; set; }
    }
}