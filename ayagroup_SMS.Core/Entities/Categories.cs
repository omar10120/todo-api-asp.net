using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ayagroup_SMS.Core.Entities
{
    public class Category :BaseEntity
    {
        
        public string Name { get; set; }

        public ICollection<Tasks> Tasks { get; set; }
    }
}
