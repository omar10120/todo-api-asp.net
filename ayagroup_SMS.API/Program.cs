using ayagroup_SMS.API;
using ayagroup_SMS.Application;
using ayagroup_SMS.Core;
using Microsoft.AspNetCore.Identity;
using ayagroup_SMS.Infrastructure;
using ayagroup_SMS.Infrastructure.Repositories;
using ayagroup_SMS.Core.Entities;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ayagroup_SMS.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddCoreServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins("*") 
            .AllowAnyMethod()
            .AllowAnyHeader());

});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
var app = builder.Build();

app.UseCors("AllowSpecificOrigins"); 

// call Seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var dbContext = services.GetRequiredService<ApplicationDbContext>();



    await Seeder.SeedRolesAndAdminAsync(userManager, roleManager, dbContext);
}


app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});
app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
