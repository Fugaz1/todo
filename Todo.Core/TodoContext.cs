using Microsoft.EntityFrameworkCore;
using Todo.Core.Entities;

namespace Todo.Core
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}