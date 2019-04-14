using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.Models
{
    public class ToDoTask
    {
        public int Id { get; set; }
        public bool Completed { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public ApplicationUser TaskOwner { get; set; }
    }
}
