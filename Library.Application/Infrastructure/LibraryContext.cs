using Bogus;
using Library.Application.Model;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Infrastructure;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions opt) : base(opt)
    {
    }

    public DbSet<Model.Library> Libraries => Set<Model.Library>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Model.Library>().HasKey(library => library.Id);
        modelBuilder.Entity<Model.Library>()
            .HasOne(l => l.User)
            .WithMany(u => u.Libraries)
            .HasForeignKey(l => l.UserId)
            .IsRequired();

        modelBuilder.Entity<Model.Library>()
            .HasMany(l => l.Loans)
            .WithOne(l => l.Library)
            .HasForeignKey(l => l.LibraryId)
            .IsRequired();

        modelBuilder.Entity<Model.Library>()
            .HasMany(l => l.Books)
            .WithOne(b => b.Library)
            .HasForeignKey(b => b.LibraryId)
            .IsRequired();

        modelBuilder.Entity<Book>().HasKey(b => b.Id);

        modelBuilder.Entity<Loan>().HasKey(l => l.Id);
        modelBuilder.Entity<Loan>()
            .HasOne(e => e.Book)
            .WithMany(l => l.Loans)
            .HasForeignKey(l => l.BookId)
            .IsRequired();

        modelBuilder.Entity<Loan>()
            .HasOne(e => e.Borrower)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .IsRequired();
    }

    public void Seed(ICryptService cryptService)
    {
        Randomizer.Seed = new Random(1342);

        //One admin user
        var adminSalt = cryptService.GenerateSecret(256);
        var admin = new User
        (
            username: "admin",
            salt: adminSalt,
            passwordHash: cryptService.GenerateHash(adminSalt, "1234"),
            userType: UserType.Admin
        );
        Users.Add(admin);
        SaveChanges();

        //Libraries
        var i = 0;
        var libraries = new Faker<Model.Library>("en").CustomInstantiator(f =>
            {
                var name = f.Address.City() + " City Library";
                var openTime = TimeSpan.FromHours(f.Random.Number(7, 10));
                var closeTime = TimeSpan.FromHours(f.Random.Number(17, 20));
                var salt = cryptService.GenerateSecret(256);
                var username = $"library{++i:000}";

                return new Model.Library
                (
                    name: name,
                    openTime: openTime,
                    closeTime: closeTime,
                    user: new User
                    (
                        username: username,
                        salt: salt,
                        passwordHash: cryptService.GenerateHash(salt, "1234"),
                        userType: UserType.User
                    )
                );
            })
            .Generate(5)
            .GroupBy(l => l.Name).Select(g => g.First())
            .ToList();
        Libraries.AddRange(libraries);
        SaveChanges();

        //Books
        var books = new Faker<Book>("en").CustomInstantiator(f =>
                new Book
                (
                    title: f.Random.Words(2),
                    author: f.Name.FullName(),
                    genre: f.PickRandom<BookGenre>(),
                    publishDate: f.Date.Past(20)
                )
                {
                    Library = f.Random.ListItem(libraries)
                }
            )
            .Generate(50)
            .GroupBy(b => b.Title).Select(g => g.First())
            .ToList();
        Books.AddRange(books);
        SaveChanges();
        
        //Loans
        var loans = new Faker<Loan>("en").CustomInstantiator(f =>
            {
                var library = f.Random.ListItem(libraries);
                var book = f.Random.ListItem(books);
                book.IsLoaned = true;

                return new Loan
                (
                    library: library,
                    book: book,
                    user: f.Random.ListItem(Users.ToList()),
                    loanDate: f.Date.Past(),
                    returnDate: f.Date.Future()
                );
            })
            .Generate(20)
            .GroupBy(l => new { l.LibraryId, l.BookId }).Select(g => g.First())
            .ToList();
        Loans.AddRange(loans);
        SaveChanges();
    }
}