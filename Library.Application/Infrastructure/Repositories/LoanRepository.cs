using Library.Application.Model;

namespace Library.Application.Infrastructure.Repositories;

public class LoanRepository : Repository<Loan, Guid>
{
    public LoanRepository(LibraryContext db) : base(db)
    {
    }

    public override (bool success, string message) Create(Loan loan)
    {
        return base.Create(loan);
    }

    public (bool success, string message) Create(DateTime loanDate, DateTime returnDate, Guid bookId, Guid libraryId,
        Guid userId)
    {
        var book = _db.Books.FirstOrDefault(b => b.Id == bookId);
        if (book is null)
        {
            return (false, "Book does not exist.");
        }
        var library = _db.Libraries.FirstOrDefault(l => l.Id == libraryId);
        if (library is null)
        {
            return (false, "Library does not exist.");
        }
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return (false, "User does not exist.");
        }
        return base.Create(new Loan
        (
            library: library,
            book: book,
            user: user,
            loanDate: loanDate,
            returnDate: returnDate
        ));
    }
}