using Library;
using Library.Application.Dto;
using Library.Application.Infrastructure;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Erstellen und seeden der Datenbank
var opt = new DbContextOptionsBuilder()
    .UseSqlite("Data Source=libraries.db")
    .Options;
using (var db = new LibraryContext(opt))
{
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    db.Seed(new CryptService());
}

// Add services to the container.
builder.Services.AddDbContext<LibraryContext>(opt =>
{
    opt.UseSqlite("Data Source=libraries.db");
});


// * Repositories **********************************************************************************
builder.Services.AddTransient<LibraryRepository>();
builder.Services.AddTransient<BookRepository>();
builder.Services.AddTransient<LoanRepository>();
builder.Services.AddTransient<UserRepository>();


// * Services for authentication *******************************************************************
// To access httpcontext in services
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ICryptService, CryptService>();
builder.Services.AddTransient<AuthService>(provider => new AuthService(
    isDevelopment: builder.Environment.IsDevelopment(),
    db: provider.GetRequiredService<LibraryContext>(),
    cryptService: provider.GetRequiredService<ICryptService>(),
    httpContextAccessor: provider.GetRequiredService<IHttpContextAccessor>()));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/User/Login";
        o.AccessDeniedPath = "/User/AccessDenied";
    });

    
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminOrUserRole", p => p.RequireRole(UserType.Admin.ToString(), UserType.User.ToString()));
});

// * Other Services ********************************************************************************
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddRazorPages();


// Configure the HTTP request pipeline.
// *************************************************************************************************
// MIDDLEWARE
// *************************************************************************************************
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();