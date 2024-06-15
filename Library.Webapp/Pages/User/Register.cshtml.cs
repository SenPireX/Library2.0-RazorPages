using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Library.Application.Dto;
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
    private readonly IMapper _mapper;
    private readonly ICryptService _cryptService;

    public RegisterModel(AuthService authService, UserRepository userRepository, IMapper mapper, ICryptService cryptService)
    {
        _authService = authService;
        _userRepository = userRepository;
        _mapper = mapper;
        _cryptService = cryptService;
    }

    [BindProperty] public UserDto Input { get; set; } = default!;

    public void OnGet() {}
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = _mapper.Map<Application.Model.User>(Input);
        user.Salt = _cryptService.GenerateSecret(256);
        user.PasswordHash = _cryptService.GenerateHash(user.Salt, Input.Password);

        await _userRepository.CreateUser(user);

        var (success, message)= await _authService.TryLoginAsync(user.Username, Input.Password);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return Page();
        }
        
        return RedirectToPage("/Index");
    }
}