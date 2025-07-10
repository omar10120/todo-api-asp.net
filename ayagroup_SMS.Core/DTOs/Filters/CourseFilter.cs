using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ayagroup_SMS.Core.DTOs.Filters
{
    public class CourseFilter
    {
        public string? Name { get; set; }
        public Guid? TeacherId { get; set; }
       
    }
}
