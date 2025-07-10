using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ayagroup_SMS.Core.conts
{
    public enum Roles
    {
        [EnumMember(Value = "Owner")]
        Owner,
        [EnumMember(Value = "Guest")]
        Guest,


    }
}
