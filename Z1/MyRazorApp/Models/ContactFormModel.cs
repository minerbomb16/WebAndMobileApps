using System.ComponentModel.DataAnnotations;

public class ContactFormModel
{
    [Required(ErrorMessage = "Imiê jest wymagane.")]
    [StringLength(50, ErrorMessage = "Imiê nie mo¿e byæ d³u¿sze ni¿ 50 znaków.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email jest wymagany.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Wiadomoœæ jest wymagana.")]
    [StringLength(500, ErrorMessage = "Wiadomoœæ nie mo¿e byæ d³u¿sza ni¿ 500 znaków.")]
    public string Message { get; set; }
}
