using System.ComponentModel.DataAnnotations;

namespace Library.Application.Dto;

public class UserDto
{
    [Required, StringLength(16, MinimumLength = 3)]
    public string Username { get; set; }
    
    [Required, StringLength(255, MinimumLength = 4)]
    public string Password { get; set; }
    
    [Required, Compare("Password")]
    public string ConfirmPassword { get; set; }
}