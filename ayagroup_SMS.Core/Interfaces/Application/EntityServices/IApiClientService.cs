using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ayagroup_SMS.Core.Interfaces.Application.EntityServices
{
    public interface IApiClientService
    {
        Task<bool> IsValidApiKey(string apiKey);
        Task<string> GenerateApiKey(string appName);
    }
}
