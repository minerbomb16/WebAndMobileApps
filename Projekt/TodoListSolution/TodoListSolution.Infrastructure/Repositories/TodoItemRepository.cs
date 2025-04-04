﻿using Microsoft.EntityFrameworkCore;
using TodoListSolution.Domain.Entities;
using TodoListSolution.Infrastructure.Data;

namespace TodoListSolution.Infrastructure.Repositories
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private readonly AppDbContext _context;

        public TodoItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TodoItem> GetByIdAsync(Guid id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task<List<TodoItem>> GetAllAsync(string owner)
        {
            if (string.IsNullOrWhiteSpace(owner))
            {
                return new List<TodoItem>(); // Return an empty list if no owner is provided
            }

            return await _context.TodoItems
                .Where(item => item.Owner == owner) // Filter tasks by owner
                .ToListAsync();
        }


        public async Task AddAsync(TodoItem item)
        {
            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TodoItem item)
        {
            _context.TodoItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TodoItem item)
        {
            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}

