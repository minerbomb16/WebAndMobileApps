using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages
{
    public class FormModel : PageModel
    {
        [BindProperty]
        public ContactFormModel ContactData { get; set; } = new ContactFormModel();

        public bool Submitted { get; set; } = false;

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            if (Request.Form["action"] == "reset")
            {
                ContactData = new ContactFormModel();
                Submitted = false;
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Submitted = true;
            return Page();
        }
    }
}
