using System.ComponentModel.DataAnnotations;
using Library.Application.Infrastructure;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Library.Webapp.Pages.User;

public class RegisterModel : PageModel
{
    private readonly AuthService _authService;
    private readonly UserRepository _userRepository;
    private readonly ICryptService _cryptService;

    public RegisterModel(AuthService authService, UserRepository userRepository, ICryptService cryptService)
    {
        _authService = authService;
        _userRepository = userRepository;
        _cryptService = cryptService;
    }

    [BindProperty]
    [Required, StringLength(16, MinimumLength = 3)]
    public string Username { get; set; }

    [BindProperty]
    [Required, StringLength(255, MinimumLength = 6)]
    public string Password { get; set; }

    [BindProperty]
    [Required, Compare("Password")]
    public string ConfirmPassword { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var salt = _cryptService.GenerateSecret();
        var passwordHash = _cryptService.GenerateHash(salt, Password);

        var user = new Library.Application.Model.User(Username, salt, passwordHash, UserType.User);

        await _userRepository.AddUserAsync(user);
        
        var (success, message) = await _authService.TryLoginAsync(Username, Password);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return Page();
        }

        await _authService.TryLoginAsync(Username, Password);

        return RedirectToPage("/Index");
    }
}