using ayagroup_SMS.Core.Entities;

namespace ayagroup_SMS.Core.DTOs.Requestes
{
    public class TaskCreateDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid userid { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;
    }
}
