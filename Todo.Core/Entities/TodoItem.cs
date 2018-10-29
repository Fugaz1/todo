using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Core.Entities
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
