namespace LibrarySystem.Models.DTOs;

public sealed record LoanRequestDto(string ISBN, Guid UserId, int DaysToLoan = 1);