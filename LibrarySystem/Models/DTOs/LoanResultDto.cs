namespace LibrarySystem.Models.DTOs;

public record LoanResultDto(
    bool IsSuccess,
    string Message,
    LoanDto? Loan = null
);