using APIMoodReboot.Data;
using APIMoodReboot.Helpers;
using APIMoodReboot.Models;
using APIMoodReboot.Repositories;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
SecretAWS secretos = await HelperSecret.GetSecret();

string connectionString = secretos.RDSConnectionString;

builder.Services.AddDbContext<MoodRebootContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", X => X.AllowAnyHeader().AllowCredentials().AllowAnyMethod().WithOrigins("http://ec2-52-45-33-3.compute-1.amazonaws.com", "https://localhost:7196"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
