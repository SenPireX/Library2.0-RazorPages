namespace Library.Application.Dto;

public record LoanDto
(
    Guid Id,
    DateTime LoanDate,
    DateTime ReturnDate,
    Guid BookId,
    Guid LibraryId,
    Guid UserId
);