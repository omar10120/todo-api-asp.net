using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
namespace ayagroup_SMS.API.Attributes
{
    public class ApiKeyAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "x-api-key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var apiClientService = context.HttpContext.RequestServices.GetService<IApiClientService>();

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var isValidApiKey = await apiClientService.IsValidApiKey(potentialApiKey);
            if (!isValidApiKey)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }

}
