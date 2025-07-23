using LibrarySystem.Models.Entities;
using LibrarySystem.Models.Enums;

namespace LibrarySystem.Service.Interface;

public interface IBusinessDaysService
{
    Task<DateTime> AddBusinessDaysAsync(DateTime initialDate, int availableDays);
    Task<DateTime> GetNextBusinessDayAsync(DateTime date);
    bool IsAvailableDay(DateTime date);
    
    Task<DateTime> CalculateReturnDateAsync(BibliographicMaterial material, DateTime requestDate, int requestedDays);
    Task<(bool IsValid, string ErrorMessage)> ValidateRequestedDaysAsync(BibliographicMaterial material, int requestedDays, DateTime requestDate);
    Task<bool> CanBeBorrowedOnDateAsync(MaterialTypes materialType, DateTime requestDate);
}