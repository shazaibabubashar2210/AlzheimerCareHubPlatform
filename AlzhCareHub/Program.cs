using AlzhCareHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Globalization;

System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

var builder = WebApplication.CreateBuilder(args);

// Load configuration files
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.WebHost.UseUrls("http://*:8080");


// Stripe functionality
var stripeSection = builder.Configuration.GetSection("Stripe");
if (!stripeSection.Exists())
{
    throw new Exception("Stripe config section is missing.");
}

var stripeSettings = stripeSection.Get<StripeSettings>();
if (stripeSettings == null || string.IsNullOrEmpty(stripeSettings.SecretKey))
{
    throw new Exception("Stripe keys are missing or incomplete.");
}

builder.Services.Configure<StripeSettings>(stripeSection);
StripeConfiguration.ApiKey = stripeSettings.SecretKey;

// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Messages");

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout (30 minutes)
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".AlzhCareHub.Session"; // Custom session cookie name
});

// Configure localization settings
var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ur") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Add SignalR services
builder.Services.AddSignalR();

// Configure HttpContextAccessor for accessing HTTP context
builder.Services.AddHttpContextAccessor();
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

// Apply localization settings
app.UseRequestLocalization();

// Routing and session configuration
app.UseRouting();
app.UseSession();
app.UseAuthentication(); // Required before authorization
app.UseAuthorization();

// Configure default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=CaregiverDashboard}/{id?}");

app.Run();
