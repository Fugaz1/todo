using System.Collections.Generic;
using System.Linq;
using Todo.Core;
using Todo.Core.Entities;

namespace Todo.Application
{
    public class TodoService : ITodoService
    {
        private readonly TodoContext _ctx;

        public TodoService(TodoContext ctx)
        {
            _ctx = ctx;
        }

        public TodoItem GetById(int id)
        {
            var item = _ctx.TodoItems.Find(id);
            return item;
        }

        public List<TodoItem> GetAll()
        {
            return _ctx.TodoItems.ToList();
        }
    }

    public interface ITodoService
    {
        TodoItem GetById(int id);
        List<TodoItem> GetAll();
    }
}