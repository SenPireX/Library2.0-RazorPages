using Library.Application.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Library.Test;

[Collection("Sequential")]
public class LibraryContextTest
{
    private LibraryContext GetDatabase(bool deleteDb = false)
    {
        var db = new LibraryContext(new DbContextOptionsBuilder()
            .UseSqlite("Data Source=libraries.db")
            .Options);
        if (deleteDb)
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        return db;
    }

    [Fact]
    public void CreateDatabaseSuccessTest()
    {
        using var db = GetDatabase(deleteDb: true);
    }

    [Fact]
    public void SeedDatabaseTest()
    {
        using var db = GetDatabase(deleteDb: true);
        db.Seed(new CryptService());
        
        Assert.True(db.Libraries.Count() == 5);
        Assert.True(db.Books.Count() == 50);
        Assert.True(db.Loans.Count() == 20);
    }
}