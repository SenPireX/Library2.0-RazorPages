using AutoMapper;
using AutoMapper.QueryableExtensions;
using Library.Application.Dto;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.Webapp.Pages.Libraries;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly LibraryRepository _library;
    private readonly AuthService _authService;

    public DetailsModel(LibraryRepository library, AuthService authService)
    {
        _library = library;
        _authService = authService;
    }
    
    [FromRoute] public Guid Guid { get; set; }
    public Application.Model.Library? Library { get; set; }

    public IActionResult OnGet(Guid guid)
    {
        return Page();
    }

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var library = _library.Set
            .Include(l => l.User)
            .Include(l => l.Loans)
            .ThenInclude(l => l.Book)
            .Include(l => l.Loans)
            .ThenInclude(l => l.Borrower)
            .FirstOrDefault(l => l.Id == Guid);

        if (library is null)
        {
            context.Result = RedirectToPage("/Libraries/Index");
            return;
        }
        
        var username = _authService.Username;
        if (!_authService.HasRole("Admin") && username != library.User?.Username) 
        {
            context.Result = new ForbidResult();
        }
        
        Library = library;
    }
}