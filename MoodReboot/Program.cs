using Amazon.S3;
using Ganss.Xss;
using Microsoft.AspNetCore.Authentication.Cookies;
using MoodReboot.Helpers;
using MoodReboot.Hubs;
using MoodReboot.Services;
using MvcCoreAWSS3.Services;
using NugetMoodReboot.Helpers;

var builder = WebApplication.CreateBuilder(args);

//string signalrCnn = builder.Configuration.GetConnectionString("SignalR");
//string azureStorageKeys = builder.Configuration.GetValue<string>("AzureKeys:StorageAccount");

// IHttpClientFactory
builder.Services.AddHttpClient();

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// HtmlSanitizer
builder.Services.AddSingleton<HtmlSanitizer>();

// SignalR
builder.Services.AddSignalR();

// AWS S3
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddTransient<ServiceStorageS3>();

//AWS IVS
builder.Services.AddTransient<ServiceIVS>();

// Api Services
builder.Services.AddTransient<ServiceApiCenters>();
builder.Services.AddTransient<ServiceApiContents>();
builder.Services.AddTransient<ServiceApiContentGroups>();
builder.Services.AddTransient<ServiceApiCourses>();
builder.Services.AddTransient<ServiceApiUsers>();
builder.Services.AddTransient<ServiceMail>();

// Helpers
builder.Services.AddSingleton<HelperApi>();
builder.Services.AddSingleton<HelperCryptography>();
builder.Services.AddTransient<HelperFileAWS>();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IOTimeout = TimeSpan.FromMinutes(120);
});

// Seguridad
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADMIN_ONLY", policy => policy.RequireRole("ADMIN"));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme,
        config =>
        {
            config.AccessDeniedPath = "/Managed/AccessError";
        }
);

// Indicamos que queremos utilizar nuestras propias rutas
builder.Services.AddControllersWithViews(
    options => options.EnableEndpointRouting = false
).AddSessionStateTempDataProvider();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseDeveloperExceptionPage();

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/Managed/AccessError";
        await next();
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapHub<ChatHub>("/chatHub");

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}"
        );
});

app.Run();
