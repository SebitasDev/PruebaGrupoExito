using LibrarySystem.Data;
using LibrarySystem.Models.Entities;
using LibrarySystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repository;

public sealed class BibliographicMaterialRepository(
    LibraryDbContext dbContext
) : IBibliographicMaterialRepository
{
    public async Task<BibliographicMaterial?> GetByIsbnAsync(string isbn)
    {
        return await dbContext.BibliographicMaterials
            .SingleOrDefaultAsync(x => x.ISBN == isbn);
    }
}