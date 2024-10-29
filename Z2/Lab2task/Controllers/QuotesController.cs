using Microsoft.AspNetCore.Mvc;
using Lab2task.Models;

namespace Lab2task.Controllers
{
    public class QuotesController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new QuotesModel();
            model.QuotesList = GetQuotesFromLocalStorage();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(QuotesModel model)
        {
            if (!string.IsNullOrEmpty(model.NewQuote))
            {
                model.QuotesList.Add(model.NewQuote);
            }

            return View(model);
        }

        private List<string> GetQuotesFromLocalStorage()
        {
            return new List<string> { "Default quote 1", "Default quote 2" };
        }
    }
}
