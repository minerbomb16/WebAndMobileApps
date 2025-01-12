using System.ComponentModel.DataAnnotations;

namespace TodoListSolution.Web.Models
{
    public class EditTodoViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string CurrentOwner { get; set; } // To persist the owner
    }
}
