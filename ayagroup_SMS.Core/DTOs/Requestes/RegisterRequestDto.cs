using ayagroup_SMS.Core.conts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace ayagroup_SMS.Core.DTOs.Requestes
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Username name is required.")]
        [MaxLength(256, ErrorMessage = "Username name cannot exceed 256 characters.")]
        public string Username { get; set; }

 

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public Roles Roles { get; set; }
        



    }
    public static class RolesExtensions
    {
        public static string GetDescription(this Roles role)
        {
            var field = role.GetType().GetField(role.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? role.ToString() : attribute.Description;
        }
    }
}
