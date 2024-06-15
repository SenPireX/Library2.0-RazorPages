using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Application.Model;

[Table("Loan")]
public class Loan : IEntity<Guid>
{
    public Guid Id { get; private set; }
    
    public Guid BookId { get; set; }
    [ForeignKey("BookId")] public Book Book { get; set; }
    
    public Guid LibraryId { get; set; }
    [ForeignKey("LibraryId")] public Library Library { get; set; }
    
    public Guid UserId { get; set; }
    [ForeignKey("UserId")] public User Borrower { get; set; }
    
    public DateTime LoanDate { get; set; }
    public DateTime ReturnDate { get; set; }

    public Loan(Library library, Book book, User user, DateTime loanDate, DateTime returnDate)
    {
        Id = Guid.NewGuid();
        LibraryId = library.Id;
        Library = library;
        BookId = book.Id;
        Book = book;
        UserId = user.Id;
        Borrower = user;
        LoanDate = loanDate;
        ReturnDate = returnDate;
    }
    
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Loan() {}
}