using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Application.Model;

public enum BookGenre {
    Adventure, Biography, Comedy,
    Drama, Fantasy, Historical, Horror, 
    Crime, Romance, FairyTale, 
    Mystery, Philosophy, Poetry,
    Politics, Psychology, Religion,
    Satire, ScienceFiction, Thriller, Tragedy, 
    Utopia, Western, Dystopia,
    Memoir, Epic, Essay,
    Fable, Myth, Travelogue
}

[Table("Book")]
public class Book : IEntity<Guid>
{
    public Guid Id { get; private set; }
    [MaxLength(32)] public string Title { get; set; }
    [MaxLength(32)] public string Author { get; set; }
    public bool IsLoaned { get; set; }
    public BookGenre Genre { get; set; }
    public DateTime PublishDate { get; set; }
    
    public Guid LibraryId { get; set; }
    [ForeignKey("LibraryId")] public Library Library { get; set; }
    public ICollection<Loan> Loans { get; } = new List<Loan>();

    public Book(string title, string author, BookGenre genre, DateTime publishDate, bool isLoaned = false)
    {
        Id = Guid.NewGuid();
        Title = title;
        Author = author;
        Genre = genre;
        PublishDate = publishDate;
        IsLoaned = isLoaned;
    }
    
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Book() {}
}