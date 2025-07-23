using LibrarySystem.Models.DTOs;
using LibrarySystem.Models.Entities;

namespace LibrarySystem.Service.Interface;

public interface ILoanService
{
    Task<LoanResultDto> ProcessLoanAsync(string isbn, Guid userId, int requestedDays);
    Task<List<LoanDto>> GetLoansAsync();
    Task<LoanDto?> GetLoanByIdAsync(Guid id);
    Task<List<MaterialDto>> GetMaterialsAsync();
}