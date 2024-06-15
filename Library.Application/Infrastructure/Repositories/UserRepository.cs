using Library.Application.Model;

namespace Library.Application.Infrastructure.Repositories;

public class UserRepository : Repository<User, Guid>
{
    private readonly ICryptService _cryptService;
    
    public UserRepository(LibraryContext db, ICryptService cryptService) : base(db)
    {
        _cryptService = cryptService;
    }

    public async Task AddUserAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}