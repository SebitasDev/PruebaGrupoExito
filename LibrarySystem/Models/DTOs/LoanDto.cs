namespace LibrarySystem.Models.DTOs;

public record LoanDto(
    Guid Id,
    string ISBN,
    DateTime RequestDate,
    DateTime ReturnDate,
    Guid UserId,
    string Status,
    string MaterialName,
    string MaterialType
);