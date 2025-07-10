using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using ayagroup_SMS.API.Attributes;

namespace ayagroup_SMS.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Controllers and enable API Explorer
            services.AddControllers(); 
            services.AddEndpointsApiExplorer(); // Enables discovering API endpoints for Swagger/OpenAPI



            // Register custom attributes
            services.AddScoped<ApiKeyAuthorizeAttribute>(); // Register ApiKeyAuthorize attribute as Scoped

            // Configure SignalR for real-time web functionality

            // Swagger Setup
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            services.AddSwaggerGen(option =>
            {
               


                // Add API Key and Bearer Token authentication schemes to Swagger UI
                option.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key needed to access the endpoints. x-api-key: My_API_Key",
                    In = ParameterLocation.Header,
                    Name = "x-api-key",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme"
                });

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                // Require both API Key and Bearer Token for all endpoints
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        new string[] {}
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });



            // Enable ProblemDetails for standardized error responses
            services.AddProblemDetails();

            // Lowercase URLs for route mapping
            services.AddRouting(options => options.LowercaseUrls = true);

            // Configure JSON options for the controllers
            services.AddControllers();
         

            return services;
        }
    }
}
