using LibrarySystem.Models.Enums;

namespace LibrarySystem.Models.Entities;

public abstract class BibliographicMaterial
{
    public string ISBN { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public MaterialTypes Type { get; protected set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public abstract int CalculateLoanDays();
    public abstract bool CanBeBorrowedOnDate(DateTime requestDate);

}