using APIMoodRebootAWS.Data;
using APIMoodRebootAWS.Helpers;
using APIMoodRebootAWS.Models;
using APIMoodRebootAWS.Repositories;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Interfaces;

namespace APIMoodRebootAWSAWS;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        SecretAWS secretos = HelperSecret.GetSecret().Result;

        string connectionString = secretos.RDSConnectionString;

        services.AddDbContext<MoodRebootContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        services.AddTransient<IRepositoryCenters, RepositoryCentersSql>();
        services.AddTransient<IRepositoryContent, RepositoryContentSql>();
        services.AddTransient<IRepositoryContentGroups, RepositoryCntGroupsSql>();
        services.AddTransient<IRepositoryCourses, RepositoryCoursesSql>();
        services.AddTransient<IRepositoryUsers, RepositoryUsersSql>();

        HelperOAuthToken helper = new(Configuration);
        services.AddAuthentication(helper.GetAuthenticationOptions()).AddJwtBearer(helper.GetJwtOptions());
        services.AddSingleton(helper);

        services.AddTransient<HelperCourse>();
        services.AddSingleton<HelperJsonSession>();
        services.AddSingleton<HelperCryptography>();

        // HtmlSanitizer
        services.AddSingleton<HtmlSanitizer>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowOrigin", X => X.AllowAnyHeader().AllowCredentials().AllowAnyMethod().WithOrigins("http://ec2-52-45-33-3.compute-1.amazonaws.com", "https://localhost:7196"));
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Api CRUD para la api Amazon",
                Description = "Amazon",
            });
        });

        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(url: "swagger/v1/swagger.json", name: "Api CRUD Amazon Swagger");
                options.RoutePrefix = "";
            });
        }

        app.UseCors("AllowOrigin");

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}