using McKIESales.WEB.Models;
using McKIESales.WEB.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//  Configures the application to read the MongoDbSettings from the configuration
//  file and bind them to the MongoDbSettings class.
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

//  Registers MongoDbContext as a singleton service in the dependency injection container,
//  ensuring that only one instance of MongoDbContext is used throughout the application's
//  lifetime.
builder.Services.AddSingleton<MongoDbContext>();

//   adds services required for MVC (Model-View-Controller) pattern in the application.
//   It enables the use of both controllers and views in the ASP.NET Core app, allowing
//   for web pages to be rendered using controllers.
builder.Services.AddControllersWithViews();

//  This line sets up cookie-based authentication for the app, specifying paths
//  for login, logout, and access denial actions.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()){
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();

app.Run();
