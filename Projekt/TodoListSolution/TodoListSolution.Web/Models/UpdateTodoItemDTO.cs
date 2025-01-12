namespace TodoListSolution.Web.Models
{
    public class UpdateTodoItemDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public string Owner { get; set; }
    }
}
