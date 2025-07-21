using ayagroup_SMS.Core.Entities;

namespace ayagroup_SMS.Core.DTOs.Requestes
{
    public class TaskCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(256, ErrorMessage = "Title cannot exceed 256 characters")]
        public string Title { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Invalid due date format")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public Guid? CategoryId { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Range(0, 2, ErrorMessage = "Priority must be between 0 (Low) and 2 (High)")]
        public Priority Priority { get; set; } = Priority.Medium;
    }
}
