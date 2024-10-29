using Microsoft.AspNetCore.Mvc;
using Lab2task.Models;

namespace Lab2task.Controllers
{
    public class CalculatorController : Controller
    {
        public IActionResult Index()
        {
            var model = new CalculatorModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(CalculatorModel model)
        {
            if (ModelState.IsValid)
            {
                model.Result = model.Number1 + model.Number2;
            }

            return View(model);
        }
    }
}