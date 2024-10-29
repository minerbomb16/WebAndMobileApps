using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages
{
    [IgnoreAntiforgeryToken]
    public class Calculate : PageModel
    {
        [BindProperty]
        public decimal Number1 { get; set; }

        [BindProperty]
        public decimal Number2 { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (Number1 > 999999999999)
            {
                return Content("First number is too large");
            } else if (Number2 > 999999999999)
            {
                return Content("Second number is too large");
            }
            var sum = Number1 + Number2;
            return Content(sum.ToString());
        }
    }
}