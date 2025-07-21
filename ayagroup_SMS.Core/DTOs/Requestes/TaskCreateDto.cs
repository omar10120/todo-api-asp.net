using ayagroup_SMS.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace ayagroup_SMS.Core.DTOs.Requestes
{
    public class TaskCreateDto
    {
     
        public string Title { get; set; }


        public string? Description { get; set; }

      
        public DateTime? DueDate { get; set; }


        public Guid? CategoryId { get; set; }

        public Priority Priority { get; set; } = Priority.Medium;
    }
}
