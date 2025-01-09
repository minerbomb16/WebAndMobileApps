using TodoListSolution.Domain.Entities;

namespace TodoListSolution.Infrastructure.Repositories
{
    public interface ITodoItemRepository
    {
        Task<TodoItem> GetByIdAsync(Guid id);
        Task<List<TodoItem>> GetAllAsync();
        Task AddAsync(TodoItem item);
        Task UpdateAsync(TodoItem item);
        Task DeleteAsync(TodoItem item);
    }
}

