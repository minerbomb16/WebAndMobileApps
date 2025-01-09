using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using TodoListSolution.Web.Models;

namespace TodoListSolution.Web.Controllers
{
    public class TodoController : Controller
    {
        private readonly HttpClient _httpClient;

        public TodoController(IHttpClientFactory httpClientFactory)
        {
            // "ApiClient" is configured in Program.cs to point to your API
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // GET: /Todo/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Always fetch tasks from API and show them
            var tasks = await GetTasksFromApi();
            var vm = new TodoPageViewModel
            {
                Tasks = tasks
            };

            return View(vm);
        }

        // POST: /Todo/Index
        [HttpPost]
        public async Task<IActionResult> Index(TodoPageViewModel model)
        {
            // Handle Adding a New Task
            if (!string.IsNullOrWhiteSpace(model.NewTitle))
            {
                var createResponse = await _httpClient.PostAsJsonAsync("api/todo", new
                {
                    Title = model.NewTitle,
                    Description = model.NewDescription
                });
                createResponse.EnsureSuccessStatusCode();
            }

            // Handle Marking as Done
            if (model.MarkDoneId.HasValue)
            {
                var taskId = model.MarkDoneId.Value;
                var oldTaskResponse = await _httpClient.GetAsync($"api/todo/{taskId}");
                if (oldTaskResponse.IsSuccessStatusCode)
                {
                    var oldTask = await oldTaskResponse.Content.ReadFromJsonAsync<TodoItemDTO>();
                    if (oldTask != null)
                    {
                        var updatedDto = new UpdateTodoItemDTO
                        {
                            Title = oldTask.Title,
                            Description = oldTask.Description,
                            IsCompleted = true
                        };

                        var putResponse = await _httpClient.PutAsJsonAsync($"api/todo/{taskId}", updatedDto);
                        putResponse.EnsureSuccessStatusCode();
                    }
                }
            }

            // Handle Deleting a Task
            if (model.DeleteId.HasValue)
            {
                var taskId = model.DeleteId.Value;
                var deleteResponse = await _httpClient.DeleteAsync($"api/todo/{taskId}");
                deleteResponse.EnsureSuccessStatusCode();
            }

            // Handle Editing a Task
            if (model.EditId.HasValue)
            {
                if (string.IsNullOrWhiteSpace(model.EditTitle) || string.IsNullOrWhiteSpace(model.EditDescription))
                {
                    ModelState.AddModelError("", "Title and Description are required for editing a task.");
                    return View(model); // Return the same view with errors displayed
                }

                var taskId = model.EditId.Value;
                var updatedDto = new UpdateTodoItemDTO
                {
                    Title = model.EditTitle,
                    Description = model.EditDescription,
                    IsCompleted = false // Example value, adjust as needed
                };

                var putResponse = await _httpClient.PutAsJsonAsync($"api/todo/{taskId}", updatedDto);
                putResponse.EnsureSuccessStatusCode();
            }

            // Redirect to avoid reposting on refresh
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<TodoItemDTO>> GetTasksFromApi()
        {
            var response = await _httpClient.GetAsync("api/todo");
            response.EnsureSuccessStatusCode();

            var tasks = await response.Content.ReadFromJsonAsync<List<TodoItemDTO>>();
            return tasks ?? new List<TodoItemDTO>();
        }

        // GET: /Todo/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Fetch the task details using the ID
            var response = await _httpClient.GetAsync($"api/todo/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var task = await response.Content.ReadFromJsonAsync<TodoItemDTO>();

            if (task == null) return NotFound();

            // Pass the task to the view
            var editModel = new EditTodoViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description
            };

            return View(editModel);
        }

        // POST: /Todo/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(EditTodoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return view with validation errors
            }

            var updateDto = new UpdateTodoItemDTO
            {
                Title = model.Title,
                Description = model.Description,
                IsCompleted = false // Keep as is
            };

            var response = await _httpClient.PutAsJsonAsync($"api/todo/{model.Id}", updateDto);
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

            return RedirectToAction("Index");
        }
    }
}
