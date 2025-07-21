
using System.ComponentModel.DataAnnotations;

namespace ayagroup_SMS.Core.DTOs.Requestes
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Username name is required.")]
        [MaxLength(256, ErrorMessage = "Username name cannot exceed 256 characters.")]
        public string Name { get; set; }
    }
}