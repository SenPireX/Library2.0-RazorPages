using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.Application.Dto;
using Library.Application.Infrastructure;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.Webapp.Pages.Libraries;

public class AddModel : PageModel
{
    private readonly LibraryRepository _library;
    private readonly LoanRepository _loan;
    private readonly BookRepository _book;
    private readonly AuthService _authService;
    private readonly IMapper _mapper;

    public AddModel(IMapper mapper, LibraryRepository library, LoanRepository loan, BookRepository book,
        AuthService authService)
    {
        _mapper = mapper;
        _library = library;
        _loan = loan;
        _book = book;
        _authService = authService;
    }

    [FromRoute] public Guid Guid { get; private set; }
    [BindProperty] public Loan NewLoan { get; set; } = default!;
    public Application.Model.Library Library { get; private set; } = default!;
    public Dictionary<Guid, LoanDto> EditLoans { get; set; } = new();
    public Dictionary<Guid, bool> LoansToDelete { get; set; } = new();
    public IReadOnlyList<Loan> Loans { get; set; } = new List<Loan>();

    public IEnumerable<SelectListItem> BookSelectList =>
        _book.Set.OrderBy(b => b.Title).Select(b => new SelectListItem(b.Title, b.Id.ToString()));

    public IActionResult OnPostNewLoan(Guid guid, LoanDto newLoan)
    {
        if (!ModelState.IsValid) { return Page(); }
        
        var (success, message) = _loan.Create
        (
            loanDate: newLoan.LoanDate,
            returnDate: newLoan.ReturnDate,
            bookId: newLoan.BookId,
            libraryId: newLoan.LibraryId,
            userId: newLoan.UserId
        );

        if (!success)
        {
            ModelState.AddModelError("", message!);
            return Page();
        }
        return RedirectToPage();
    }

    public IActionResult OnGet(Guid guid)
    {
        return Page();
    }

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var library = _library.Set
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

        Library = library;
        LoansToDelete = library.Loans.ToDictionary(l => l.Id, l => false);
        var loans = _loan.Set
            .Where(l => l.Library.Id == Guid)
            .Select(l => new LoanDto(
                l.Id,
                l.LoanDate,
                l.ReturnDate,
                l.BookId,
                l.LibraryId,
                l.UserId
            ))
            .ToList();
        EditLoans = loans.ToDictionary(l => l.Id, l => l);
    }
}