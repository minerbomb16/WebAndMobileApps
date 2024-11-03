using System;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class Rental
    {
        public int Id { get; set; } // Unique ID for each rental

        [Required]
        public int ClientId { get; set; } // ID of the client who rented the book

        [Required]
        public int BookId { get; set; } // ID of the rented book

        [Required]
        public DateTime RentalDate { get; set; } // Date when the book was rented

        public DateTime? ReturnDate { get; set; } // Date when the book was returned (nullable if not yet returned)
    }
}
