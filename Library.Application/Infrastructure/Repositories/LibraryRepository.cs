using Library.Application.Model;

namespace Library.Application.Infrastructure.Repositories;

public class LibraryRepository : Repository<Model.Library, Guid>
{
    public record LibraryWithBooksCount(
        Guid Guid,
        string Name,
        TimeSpan OpenTime,
        TimeSpan CloseTime,
        User? User,
        int? BookCount,
        int? LoanCount
    );

    public LibraryRepository(LibraryContext db) : base(db)
    {
    }

    public IReadOnlyList<LibraryWithBooksCount> GetLibraryWithBooksCounts()
    {
        return _db.Libraries
            .Select(l => new LibraryWithBooksCount(l.Id, l.Name, l.OpenTime, l.CloseTime, l.User, l.Books.Count, l.Loans.Count))
            .ToList();
    }

    public override (bool success, string message) Delete(Model.Library library)
    {
        if (library.Loans.Count != 0)
        {
            return (false, $"Library {library.Name} has open loans");
        }

        return base.Delete(library);
    }
}