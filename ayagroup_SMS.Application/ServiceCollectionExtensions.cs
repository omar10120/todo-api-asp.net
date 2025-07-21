using ayagroup_SMS.Application.UtilityServices;
using ayagroup_SMS.Core.Interfaces.Application.UtilityServices;
using Microsoft.Extensions.DependencyInjection;
using ayagroup_SMS.Core.Entities;
using ayagroup_SMS.Core.Interfaces.Infrastructure;
using ayagroup_SMS.Application.Entity.Services;
using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using ayagroup_SMS.Application.EntityServices;
using System;
namespace ayagroup_SMS.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //AddServices
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ITasksService, TasksService>();
            services.AddScoped<ICategoryService, CategoryService>();
          


            services.AddHttpContextAccessor();

            return services;
        }
    }
}
