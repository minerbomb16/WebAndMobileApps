using Microsoft.AspNetCore.Mvc;
using Lab2task.Extensions;

namespace Lab2task.Controllers
{
    public class TODOController : Controller
    {
        private const string SessionKey = "Tasks";

        private List<Models.TODOModel> GetTasksFromSession()
        {
            var tasks = HttpContext.Session.GetObjectFromJson<List<Models.TODOModel>>(SessionKey);
            if (tasks == null)
            {
                tasks = new List<Models.TODOModel>();
                HttpContext.Session.SetObjectAsJson(SessionKey, tasks);
            }
            return tasks;
        }

        public IActionResult Index()
        {
            var tasks = GetTasksFromSession();
            return View(tasks);
        }

        [HttpPost]
        public IActionResult Add(string description)
        {
            var tasks = GetTasksFromSession();
            int newId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;

            tasks.Add(new Models.TODOModel
            {
                Id = newId,
                Description = description,
                IsCompleted = false
            });

            HttpContext.Session.SetObjectAsJson(SessionKey, tasks);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var tasks = GetTasksFromSession();
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                tasks.Remove(task);
                HttpContext.Session.SetObjectAsJson(SessionKey, tasks);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Complete(int id)
        {
            var tasks = GetTasksFromSession();
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.IsCompleted = true;
                HttpContext.Session.SetObjectAsJson(SessionKey, tasks);
            }
            return RedirectToAction("Index");
        }
    }
}
