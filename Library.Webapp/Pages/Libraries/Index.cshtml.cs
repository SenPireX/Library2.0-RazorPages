using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Library.Webapp.Pages.Libraries;

public class IndexModel : PageModel
{
    private readonly LibraryRepository _library;
    private readonly AuthService _authService;

    public IndexModel(LibraryRepository library, AuthService authService)
    {
        _library = library;
        _authService = authService;
    }

    [TempData] public string? Message { get; set; }

    public IReadOnlyList<LibraryRepository.LibraryWithBooksCount> Libraries { get; private set; } =
        new List<LibraryRepository.LibraryWithBooksCount>();
    
    public void OnGet()
    {
        Libraries = _library.GetLibraryWithBooksCounts();
    }

    public bool CanEditLibrary(Guid libraryGuid) => _authService.IsAdmin; 
    //|| Libraries.FirstOrDefault(l => l.Guid == libraryGuid)?.User?.Username == _authService.Username;
}