using LibrarySystem.Models.Enums;

namespace LibrarySystem.Models.Entities;

public class Journal : BibliographicMaterial
{
    public Journal() => Type = MaterialTypes.Journal;

    public override int CalculateLoanDays() => 2;

    public override bool CanBeBorrowedOnDate(DateTime requestDate)
    {
        var returnDate = requestDate.AddDays(CalculateLoanDays());
        return returnDate.DayOfWeek != DayOfWeek.Saturday && 
               returnDate.DayOfWeek != DayOfWeek.Sunday;
    }
}