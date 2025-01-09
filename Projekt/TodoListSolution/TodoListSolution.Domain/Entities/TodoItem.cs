using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoListSolution.Domain.Entities
{
    // Klasa encji powinna zawierać tylko logikę domenową i proste właściwości
    public class TodoItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public bool IsCompleted { get; private set; }

        // Konstruktor
        public TodoItem(string title, string? description = null)
        {
            Title = title;
            Description = description;
            IsCompleted = false;
        }

        // Metody domenowe
        public void MarkAsCompleted()
        {
            IsCompleted = true;
        }

        public void Update(string title, string? description, bool iscompleted)
        {
            Title = title;
            Description = description;
            IsCompleted = iscompleted;
        }
    }
}

