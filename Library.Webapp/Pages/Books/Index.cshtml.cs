using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Library.Webapp.Pages.Books;

[Authorize(Roles = "Admin, User")]
public class IndexModel : PageModel
{
    private readonly BookRepository _books;

    public IndexModel(BookRepository books)
    {
        _books = books;
    }

    public IEnumerable<Book> Books => _books.Set.OrderBy(b => b.Title);
        
    public void OnGet() {}
}