using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ayagroup_SMS.Core.Entities
{
    public class Tasks : BaseEntity
    {
        
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? DueDate { get; set; }

        public Guid UserId { get; set; }
        public Guid? CategoryId { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;


        public User User { get; set; }
        public Category? Category { get; set; }
    }

    public enum Priority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
}
