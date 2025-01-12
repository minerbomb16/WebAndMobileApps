using Microsoft.AspNetCore.Mvc;
using TodoListSolution.Domain.Entities;
using TodoListSolution.Infrastructure.Repositories;
using TodoListSolution.API.DTOs;

namespace TodoListSolution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoItemRepository _todoItemRepository;

        public TodoController(ITodoItemRepository todoItemRepository)
        {
            _todoItemRepository = todoItemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetAll(string? owner)
        {
            if (string.IsNullOrWhiteSpace(owner))
            {
                return Ok(new List<TodoItemDTO>()); // Return empty list if no owner is set
            }

            var items = await _todoItemRepository.GetAllAsync(owner);

            var dtoList = items.Select(i => new TodoItemDTO
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                IsCompleted = i.IsCompleted,
                Owner = i.Owner
            }).ToList();

            return Ok(dtoList);
        }


        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TodoItemDTO>> Get(Guid id)
        {
            var item = await _todoItemRepository.GetByIdAsync(id);
            if (item == null) return NotFound();

            return new TodoItemDTO
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted,
                Owner = item.Owner
            };
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateTodoItemDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Owner))
            {
                return BadRequest("Title and Owner are required.");
            }

            var newItem = new TodoItem(
                title: model.Title,
                description: model.Description,
                owner: model.Owner
            );

            await _todoItemRepository.AddAsync(newItem);

            return CreatedAtAction(nameof(Get), new { id = newItem.Id }, null);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateTodoItemDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Description))
            {
                Console.WriteLine($"Validation failed: Title={model.Title}, Description={model.Description}");
                return BadRequest("Title and Description are required.");
            }

            var item = await _todoItemRepository.GetByIdAsync(id);
            if (item == null)
            {
                Console.WriteLine($"Task not found: Id={id}");
                return NotFound("Task not found.");
            }

            item.Update(model.Title, model.Description, model.IsCompleted);
            await _todoItemRepository.UpdateAsync(item);

            return NoContent();
        }



        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var item = await _todoItemRepository.GetByIdAsync(id);
            if (item == null) return NotFound();

            await _todoItemRepository.DeleteAsync(item);
            return NoContent();
        }
    }
}
