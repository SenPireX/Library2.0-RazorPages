using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Library.Application.Dto;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.Webapp.Pages.Libraries;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly LibraryRepository _library;
    private readonly BookRepository _book;
    private readonly LoanRepository _loan;
    private readonly IMapper _mapper;
    private readonly AuthService _authService;

    public DetailsModel(LibraryRepository library, BookRepository book, LoanRepository loan, IMapper mapper, AuthService authService)
    {
        _library = library;
        _book = book;
        _loan = loan;
        _mapper = mapper;
        _authService = authService;
    }
    
    [FromRoute] public Guid Guid { get; set; }
    public Application.Model.Library? Library { get; set; }
    public LoanDto NewLoan { get; set; }
    public IReadOnlyList<Loan> Loans { get; private set; } = new List<Loan>();
    public Dictionary<Guid, LoanDto> EditLoans { get; set; } = new();
    public Dictionary<Guid, bool> LoansToDelete { get; set; } = new();
    public IEnumerable<SelectListItem> BookSelectList =>
        _book.Set.OrderBy(b => b.Title).Select(b => new SelectListItem(b.Title, b.Id.ToString()));
    
    public IActionResult OnPostEditLoan(Guid guid, Guid loanId, Dictionary<Guid, LoanDto> editLoans)
    {
        if (!ModelState.IsValid) { return Page(); }
        
        var loan = _loan.FindById(loanId);
        if (loan is null) { return RedirectToPage(); }
        
        _mapper.Map(editLoans[loanId], loan);
        var (success, message) = _loan.Update(loan);
        if (!success)
        {
            ModelState.AddModelError("", message!);
            return Page();
        }
        return RedirectToPage();
    }
    
    public IActionResult OnPostNewLoan(Guid guid, LoanDto newLoan)
    {
        if (!User.Identity.IsAuthenticated)
        {
            ModelState.AddModelError("", "User is not authenticated.");
            return Page();
        }

        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }
        
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine($"UserIdString: {userIdString}");
        if (userIdString is null)
        {
            ModelState.AddModelError("","User does not exist.");
            return Page();
        }
        
        if (!ModelState.IsValid) { return Page(); }

        if (!Guid.TryParse(userIdString, out var userId))
        {
            ModelState.AddModelError("", "Invalid UserId format.");
            return Page();
        }
        
        var (success, message) = _loan.Create
        (
            libraryId: guid,
            loanDate: newLoan.LoanDate,
            returnDate: newLoan.ReturnDate,
            bookId: newLoan.BookId,
            userId: userId
        );

        if (!success)
        {
            ModelState.AddModelError("", message!);
            return Page();
        }
        return RedirectToPage();
    }

    public IActionResult OnPostDelete(Guid guid, Dictionary<Guid, bool> loansToDelete)
    {
        if (!User.Identity.IsAuthenticated)
        {
            ModelState.AddModelError("", "User is not authenticated.");
            return Page();
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString is null)
        {
            ModelState.AddModelError("", "User does not exist.");
            return Page();
        }

        if (!Guid.TryParse(userIdString, out var userId))
        {
            ModelState.AddModelError("", "Invalid UserId format.");
            return Page();
        }

        var userIsAdmin = User.IsInRole("Admin");

        foreach (var loanId in loansToDelete.Where(kv => kv.Value).Select(kv => kv.Key))
        {
            var loan = _loan.FindById(loanId);
            if (loan is null)
            {
                ModelState.AddModelError("", $"Loan with ID {loanId} not found.");
                continue;
            }

            if (!userIsAdmin && loan.UserId != userId)
            {
                ModelState.AddModelError("", "You are not authorized to delete this loan.");
                continue;
            }

            var (success, message) = _loan.Delete(loan);
            if (!success)
            {
                ModelState.AddModelError("", message!);
            }
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
        
        /*var username = _authService.Username;
        if (!_authService.HasRole("Admin") && username != library.User?.Username) 
        {
            context.Result = new ForbidResult();
            return;
        }*/
        
        Library = library;
        Loans = library.Loans.ToList();
        LoansToDelete = Loans.ToDictionary(l => l.Id, l => false);
        EditLoans = _loan.Set
            .Where(l => l.Library.Id == Guid)
            .ProjectTo<LoanDto>(_mapper.ConfigurationProvider)
            .ToDictionary(l => l.Id, l => l);

    }
}