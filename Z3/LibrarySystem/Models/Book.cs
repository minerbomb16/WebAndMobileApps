using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Title is required.")]
        [Display(Name = "Book Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Author is required.")]
        [Display(Name = "Author Name")]
        public string Author { get; set; }

        [Display(Name = "Genre")]
        public string Genre { get; set; }

        [Range(0, 2024, ErrorMessage = "Please enter a valid year.")]
        [Display(Name = "Publication Year")]
        public int Year { get; set; }

        // New attribute to track rental status
        [Display(Name = "Is Rented")]
        public bool IsRented { get; set; }
    }
}
