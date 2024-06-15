using System.Security.Claims;
using Library.Application.Infrastructure;
using Library.Application.Model;
using Microsoft.AspNetCore.Authentication;

namespace Library.Webapp.Services;

public class AuthService
{
    private readonly bool _isDevelopment;
    private readonly LibraryContext _db;
    private readonly ICryptService _cryptService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(bool isDevelopment, LibraryContext db, ICryptService cryptService,
        IHttpContextAccessor httpContextAccessor)
    {
        _isDevelopment = isDevelopment;
        _db = db;
        _cryptService = cryptService;
        _httpContextAccessor = httpContextAccessor;
    }

    public HttpContext HttpContext => _httpContextAccessor?.HttpContext
                                      ?? throw new NotSupportedException();

    public async Task<(bool success, string message)> TryLoginAsync(string username, string password)
    {
        var dbUser = _db.Users.FirstOrDefault(u => u.Username == username);
        if (dbUser is null)
        {
            return (false, "Unkown Username or wrong password.");
        }

        var passwordHash = _cryptService.GenerateHash(dbUser.Salt, password);
        if (!_isDevelopment && passwordHash != dbUser.PasswordHash)
        {
            return (false, "Unknown username or wrong password.");
        }

        var role = dbUser.UserType.ToString();
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, dbUser.Id.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
        };
        var claimsIdentity = new ClaimsIdentity
        (
            claims,
            Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme
        );

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(4),
        };

        await HttpContext.SignInAsync
        (
            Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );
        return (true, string.Empty);
    }

    public async Task<(bool success, string message)> RegisterAsync(string username, string password)
    {
        var existingUser = _db.Users.FirstOrDefault(u => u.Username == username);
        if (existingUser != null)
        {
            return (false, "Username already exists.");
        }

        var salt = _cryptService.GenerateSecret();
        var passwordHash = _cryptService.GenerateHash(salt, password);
        var user = new User(username, salt, passwordHash, UserType.User);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await TryLoginAsync(username, password);

        return (true, "Registration successful.");
    }
    
    public bool IsAuthenticated => HttpContext.User.Identity?.Name != null;
    public string? Username => HttpContext.User.Identity?.Name;
    public bool HasRole(string role) => HttpContext.User.IsInRole(role);
    public bool IsAdmin => HttpContext.User.IsInRole(UserType.Admin.ToString());
    public Task LogoutAsync() => HttpContext.SignOutAsync();
}