using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Application.Model;

public enum UserType
{
    Admin, User
}

[Table("User")]
public class User : IEntity<Guid>
{
    public Guid Id { get; private set; }
    [MaxLength(16)] public string Username { get; set; }
    [MaxLength(44)] public string Salt { get; set; }
    [MaxLength(88)] public string PasswordHash { get; set; }
    public UserType UserType { get; set; }
    public ICollection<Library> Libraries { get; } = new List<Library>();

    public User(string username, string salt, string passwordHash, UserType userType)
    {
        Id = Guid.NewGuid();
        Username = username;
        Salt = salt;
        PasswordHash = passwordHash;
        UserType = userType;
    }
    
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected User() {}
}