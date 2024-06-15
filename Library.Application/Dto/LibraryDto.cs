namespace Library.Application.Dto;

public record LibraryDto
(
    Guid Id,
    string Name,
    TimeSpan OpenTime
);