using APIMoodReboot.Data;
using APIMoodReboot.Helpers;
using APIMoodReboot.Repositories;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string connectionString = builder.Configuration.GetConnectionString("SqlAzure");

builder.Services.AddDbContext<MoodRebootContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddTransient<IRepositoryCenters, RepositoryCentersSql>();
builder.Services.AddTransient<IRepositoryContent, RepositoryContentSql>();
builder.Services.AddTransient<IRepositoryContentGroups, RepositoryCntGroupsSql>();
builder.Services.AddTransient<IRepositoryCourses, RepositoryCoursesSql>();
builder.Services.AddTransient<IRepositoryUsers, RepositoryUsersSql>();

HelperOAuthToken helper = new(builder.Configuration);
builder.Services.AddAuthentication(helper.GetAuthenticationOptions()).AddJwtBearer(helper.GetJwtOptions());
builder.Services.AddSingleton(helper);

builder.Services.AddTransient<HelperCourse>();
builder.Services.AddSingleton<HelperJsonSession>();
builder.Services.AddSingleton<HelperCryptography>();

// HtmlSanitizer
builder.Services.AddSingleton<HtmlSanitizer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
