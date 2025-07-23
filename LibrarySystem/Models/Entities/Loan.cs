namespace LibrarySystem.Models.Entities;

public class Loan
{
    public Guid Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; } = "Activo";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public BibliographicMaterial Material { get; set; } = null!;
}