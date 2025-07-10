
using Microsoft.AspNetCore.Identity;

namespace ayagroup_SMS.Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

     
  

    }
}
