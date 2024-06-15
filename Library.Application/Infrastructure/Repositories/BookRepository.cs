using Library.Application.Model;

namespace Library.Application.Infrastructure.Repositories;

public class BookRepository : Repository<Book, Guid>
{
    public BookRepository(LibraryContext db) : base(db)
    {
    }
    
}