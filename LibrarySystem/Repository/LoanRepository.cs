using LibrarySystem.Data;
using LibrarySystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repository;

public class LoanRepository(LibraryDbContext dbContext ) : ILoanRepository
{
    public async Task<bool> ExistsActiveLoanAsync(string isbn)
    {
        return await dbContext.Loans
            .AnyAsync(p => p.ISBN == isbn && p.Status == "Activo");
    }
}