using Library.Application.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Library.Webapp.Pages.Libraries;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly LibraryRepository _libraries;

    public DeleteModel(LibraryRepository libraries)
    {
        _libraries = libraries;
    }
    
    [TempData] public string? Message { get; set; }
    public Application.Model.Library Library { get; set; } = default!;
    
    public IActionResult OnPostCancel() => RedirectToPage("/Libraries/Index");
    
    
    public IActionResult OnPostDelete(Guid guid)
    {
        var library = _libraries.FindById(guid);
        if (library is null) { return RedirectToPage("/Libraries/Index"); }
        
        var (success, message) = _libraries.Delete(library);
        if (!success) { Message = message; }
        
        return RedirectToPage("/Libraries/Index");
    }
    
    public IActionResult OnGet(Guid guid)
    {
        var library = _libraries.FindById(guid);
        if (library is null) { return RedirectToPage("/Libraries/Index"); }
        
        Library = library;
        return Page();
    }

}