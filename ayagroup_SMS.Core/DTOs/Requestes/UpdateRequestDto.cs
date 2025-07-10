using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ayagroup_SMS.Core.DTOs.Requestes
{
    public class UpdateRequestDto
    {
        [Required(ErrorMessage = "Username name is required.")]
        [MaxLength(256, ErrorMessage = "Username name cannot exceed 256 characters.")]
        public string Username { get; set; }

    }
}
