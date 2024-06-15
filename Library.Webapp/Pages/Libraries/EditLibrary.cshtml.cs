using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Library.Application.Dto;
using Library.Application.Infrastructure;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using System;
using System.Linq;

namespace Library.Webapp.Pages.Libraries;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly LibraryRepository _libraries;
    private readonly IMapper _mapper;

    public EditModel(IMapper mapper, LibraryRepository libraries)
    {
        _mapper = mapper;
        _libraries = libraries;
    }

    [BindProperty] public LibraryDto Library { get; set; }

    public IActionResult OnPost(Guid guid)
    {
        if (!ModelState.IsValid) { return Page(); }

        var library = _libraries.FindById(guid);
        if (library is null)
        {
            return RedirectToPage("/Libraries/Index");
        }

        _mapper.Map(Library, library);
        var (success, message) = _libraries.Update(library);
        if (!success)
        {
            ModelState.AddModelError("", message);
            return Page(); 
        }
        return RedirectToPage("/Libraries/Index");
    }

    public IActionResult OnGet(Guid guid)
    {
        var library = _libraries.FindById(guid);
        if (library is null)
        {
            return RedirectToPage("/Libraries/Index");
        }

        Library = _mapper.Map<LibraryDto>(library);
        return Page();
    }
}