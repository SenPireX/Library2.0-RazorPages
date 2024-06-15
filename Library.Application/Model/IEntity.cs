namespace Library.Application.Model;

public interface IEntity<Tkey> where Tkey : struct
{
    Tkey Id { get; }
}