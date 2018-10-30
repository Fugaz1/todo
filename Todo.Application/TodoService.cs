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

        public TodoItem Create(TodoItem entity)
        {
            _ctx.TodoItems.Add(entity);
            _ctx.SaveChanges();

            return entity;
        }

        public TodoItem Update(TodoItem item)
        {
            var existingItem = this.GetById(item.Id);
            if (existingItem == null)
                return null;

            existingItem.IsComplete = item.IsComplete;
            existingItem.Name = item.Name;

            _ctx.SaveChanges();

            return existingItem;
        }

        public TodoItem Delete(int id)
        {
            var existingItem = this.GetById(id);
            if (existingItem == null)
                return null;

            _ctx.TodoItems.Remove(existingItem);
            _ctx.SaveChanges();

            return existingItem;
        }
    }

    public interface ITodoService
    {
        TodoItem GetById(int id);
        List<TodoItem> GetAll();
        TodoItem Create(TodoItem item);
        TodoItem Update(TodoItem item);
        TodoItem Delete(int id);
    }
}