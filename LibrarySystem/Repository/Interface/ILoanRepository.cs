namespace LibrarySystem.Repository.Interface;

public interface ILoanRepository
{
    Task<bool> ExistsActiveLoanAsync(string isbn);
}