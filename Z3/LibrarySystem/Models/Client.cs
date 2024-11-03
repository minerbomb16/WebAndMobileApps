using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name is required.")]
        [Display(Name = "Client Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Phone number is required.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
    }
}
