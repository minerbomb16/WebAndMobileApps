using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index(string? owner)
        {
            // Fetch tasks for the provided owner
            var tasks = string.IsNullOrWhiteSpace(owner)
                ? new List<TodoItemDTO>() // Show empty list if no owner is set
                : await GetTasksFromApi(owner);

            var vm = new TodoPageViewModel
            {
                Tasks = tasks,
                CurrentOwner = owner
            };

            return View(vm);
        }

        // POST: /Todo/Index
        [HttpPost]
        public async Task<IActionResult> Index(TodoPageViewModel model)
        {
            // Handle Updating the Current Owner
            if (!string.IsNullOrWhiteSpace(model.NewOwner))
            {
                return RedirectToAction(nameof(Index), new { owner = model.NewOwner });
            }

            // Handle Adding a New Task
            if (!string.IsNullOrWhiteSpace(model.NewTitle))
            {
                if (string.IsNullOrWhiteSpace(model.CurrentOwner))
                {
                    ModelState.AddModelError("", "Owner is required to add a task.");
                    return RedirectToAction(nameof(Index), new { owner = model.CurrentOwner });
                }

                var createResponse = await _httpClient.PostAsJsonAsync("api/todo", new
                {
                    Title = model.NewTitle,
                    Description = model.NewDescription,
                    Owner = model.CurrentOwner
                });

                createResponse.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index), new { owner = model.CurrentOwner });
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
                            IsCompleted = true,
                            Owner = oldTask.Owner
                        };

                        var putResponse = await _httpClient.PutAsJsonAsync($"api/todo/{taskId}", updatedDto);
                        putResponse.EnsureSuccessStatusCode();
                    }
                }

                return RedirectToAction(nameof(Index), new { owner = model.CurrentOwner });
            }

            // Handle Deleting a Task
            if (model.DeleteId.HasValue)
            {
                var taskId = model.DeleteId.Value;
                var deleteResponse = await _httpClient.DeleteAsync($"api/todo/{taskId}");
                deleteResponse.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index), new { owner = model.CurrentOwner });
            }

            // Redirect to avoid resubmitting data on refresh
            return RedirectToAction(nameof(Index), new { owner = model.CurrentOwner });
        }

        private async Task<List<TodoItemDTO>> GetTasksFromApi(string owner)
        {
            var response = await _httpClient.GetAsync($"api/todo?owner={Uri.EscapeDataString(owner)}");
            response.EnsureSuccessStatusCode();

            var tasks = await response.Content.ReadFromJsonAsync<List<TodoItemDTO>>();
            return tasks ?? new List<TodoItemDTO>();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, string owner)
        {
            // Fetch the task details by ID
            var response = await _httpClient.GetAsync($"api/todo/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var task = await response.Content.ReadFromJsonAsync<TodoItemDTO>();
            if (task == null) return NotFound();

            // Pass the task and current owner to the view
            var editModel = new EditTodoViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CurrentOwner = owner
            };

            return View(editModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditTodoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return view with validation errors
            }

            // Prepare the UpdateTodoItemDTO
            var updateDto = new UpdateTodoItemDTO
            {
                Title = model.Title,
                Description = model.Description,
                IsCompleted = false, // Keep the task as not completed
                Owner = model.CurrentOwner // Include the Owner
            };

            // Send the PUT request to the API
            var response = await _httpClient.PutAsJsonAsync($"api/todo/{model.Id}", updateDto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update the task. Please try again.");
                return View(model);
            }

            // Redirect back to the main page with the current owner
            return RedirectToAction("Index", new { owner = model.CurrentOwner });
        }

    }
}
