using ElLibrary.Data;
using ElLibrary.Domain.Entities;
using ElLibrary.Domain.Services;
using ElLibrary.Infrastructure;
using ElLibrary.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .WriteTo.File("log.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();//

Log.Information("Start app");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ELibraryContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("express")));
builder.Services.AddScoped<IRepository<User>, EFRepository<User>>();
builder.Services.AddScoped<IRepository<Role>, EFRepository<Role>>();
builder.Services.AddScoped<IRepository<Book>,EFRepository<Book>>();
builder.Services.AddScoped<IRepository<Category>, EFRepository<Category>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookReader, BookReader>();
builder.Services.AddScoped<IBooksService, BooksService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.ExpireTimeSpan = TimeSpan.FromHours(1);
        opt.Cookie.Name = "library_session";
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SameSite = SameSiteMode.Strict;
        opt.LoginPath = "/User/Login";
        opt.AccessDeniedPath = "/User/AccessDenied";
    }

    );
builder.Logging.ClearProviders();//
builder.Logging.AddSerilog(dispose: true);//
DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("express"));
using (var context = new ELibraryContext(optionsBuilder.Options))
{
    EFInitialSeed.Seed(context);
}

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseBooksProtection();
app.MapControllerRoute("default", "{Controller=Books}/{Action=Index}");
//app.MapGet("/", () => "Hello World!");

app.Run();
