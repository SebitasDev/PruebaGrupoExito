using LibrarySystem.Models.Entities;

namespace LibrarySystem.Repository.Interface;

public interface IBibliographicMaterialRepository
{
    Task<BibliographicMaterial?> GetByIsbnAsync(string isbn);
}