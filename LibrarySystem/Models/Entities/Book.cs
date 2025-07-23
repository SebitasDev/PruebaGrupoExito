using LibrarySystem.Models.Enums;

namespace LibrarySystem.Models.Entities;

public class Book : BibliographicMaterial
{
    public Book() => Type = MaterialTypes.Book;

    public override int CalculateLoanDays()
    {
        var sumDigits = ISBN.Where(char.IsDigit)
            .Select(c => int.Parse(c.ToString()))
            .Sum();

        return sumDigits > 30 ? 15 : 10;
    }
    
    public override bool CanBeBorrowedOnDate(DateTime requestDate) => true;
}