using aplikacjaMobilna.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aplikacjaMobilna.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetTasksAsync();
        Task AddTaskAsync(TaskItem task);
        Task UpdateTaskAsync(TaskItem task);
        Task DeleteTaskAsync(int taskId);
    }
}
