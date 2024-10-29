using System.ComponentModel.DataAnnotations;

public class ContactFormModel
{
    [Required(ErrorMessage = "Imi� jest wymagane.")]
    [StringLength(50, ErrorMessage = "Imi� nie mo�e by� d�u�sze ni� 50 znak�w.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email jest wymagany.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Wiadomo�� jest wymagana.")]
    [StringLength(500, ErrorMessage = "Wiadomo�� nie mo�e by� d�u�sza ni� 500 znak�w.")]
    public string Message { get; set; }
}
