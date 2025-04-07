using AlzhCareHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.AddJsonFile("secrets.json", optional: true, reloadOnChange: true); 

// Add services to the container.
builder.Services.AddControllersWithViews();

// 🔹 Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Messages");

// 🔹 Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout (30 minutes)
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".AlzhCareHub.Session"; // Custom session cookie name
});

// 🔹 Configure localization settings
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("ur")
};
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.WebHost.UseUrls("http://0.0.0.0:80"); // instead of 8081

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

// 🔹 Ensure correct middleware order
app.UseRequestLocalization(); // Apply localization settings
app.UseRouting(); // Place after localization
app.UseSession();
app.UseAuthentication(); // Required before authorization
app.UseAuthorization();
// 🔹 Configure endpoint routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=CaregiverDashboard}/{id?}");

app.Run();
