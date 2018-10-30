using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Todo.Application;
using Todo.Core.Entities;

namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public ActionResult<List<TodoItem>> Get()
        {
            return _todoService.GetAll();
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public ActionResult<TodoItem> Get(int id)
        {
            var item = _todoService.GetById(id);

            if (item == null)
                return NotFound();

            return item;
        }

        [HttpPost]
        public IActionResult Create([FromBody]
            TodoItem item)
        {
            var createdItem = _todoService.Create(item);

            return CreatedAtRoute("GetTodo", new {id = createdItem.Id}, createdItem);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]
            TodoItem item)
        {
            item.Id = id;

            var updatedItem = _todoService.Update(item);
            if (updatedItem == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deletedItem = _todoService.Delete(id);
            if (deletedItem == null)
                return NotFound();

            return NoContent();
        }
    }
}