using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Application.Model;

[Table("Library")]
public class Library : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
    public Guid? UserId { get; set; }
    [ForeignKey("UserId")] public User? User { get; set; }
    
    public ICollection<Book> Books { get; set; } = new List<Book>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public Library(string name, TimeSpan openTime, TimeSpan closeTime, User? user = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        OpenTime = openTime;
        CloseTime = closeTime;
        User = user;
    }
    
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Library() {}
}