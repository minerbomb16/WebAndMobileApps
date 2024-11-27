using aplikacjaMobilna.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace aplikacjaMobilna.Services
{
    public class TaskService : ITaskService
    {
        private const string FileName = "tasks.json";
        private string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

        private List<TaskItem> _tasks = new();

        public TaskService()
        {
            LoadTasksFromFile();
        }

        private void LoadTasksFromFile()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                _tasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            }
            else
            {
                _tasks = new List<TaskItem>();
            }
        }


        private void SaveTasksToFile()
        {
            var json = JsonSerializer.Serialize(_tasks);
            File.WriteAllText(FilePath, json);
        }

        public Task<List<TaskItem>> GetTasksAsync()
        {
            return Task.FromResult(_tasks);
        }

        public Task AddTaskAsync(TaskItem task)
        {
            task.Id = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
            _tasks.Add(task);
            SaveTasksToFile();
            return Task.CompletedTask;
        }

        public Task UpdateTaskAsync(TaskItem task)
        {
            var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existingTask != null)
            {
                existingTask.Title = task.Title;
                existingTask.IsCompleted = task.IsCompleted;
                SaveTasksToFile();
            }
            return Task.CompletedTask;
        }

        public Task DeleteTaskAsync(int taskId)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                _tasks.Remove(task);
                SaveTasksToFile();
            }
            return Task.CompletedTask;
        }
    }
}
