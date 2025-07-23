using LibrarySystem.Data;
using LibrarySystem.Extensions;
using LibrarySystem.Models.DTOs;
using LibrarySystem.Models.Entities;
using LibrarySystem.Models.Enums;
using LibrarySystem.Repository.Interface;
using LibrarySystem.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Service;

public sealed class LoanService(
    ILoanRepository loanRepository,
    IBibliographicMaterialRepository bibliographicMaterialRepository,
    IBusinessDaysService businessDaysService,
    LibraryDbContext dbContext
) : ILoanService
{
    public async Task<LoanResultDto> ProcessLoanAsync(string isbn, Guid userId, int requestedDays)
    {
        try
        {
            var material = await bibliographicMaterialRepository.GetByIsbnAsync(isbn);
            
            if (material == null) 
                return new LoanResultDto(false, "Material no encontrado");
            
            var isLoan = await loanRepository.ExistsActiveLoanAsync(isbn);
            
            if (isLoan) 
                return new LoanResultDto(false, "El material no puede ser prestado: Ese ISBN ya está prestado");
            
            if (isbn.IsPalindrome())
                return new LoanResultDto(false, "El material con ISBN en palíndromo solo es para uso en la biblioteca");
            
            var actualDate = DateTime.Now;
            
            var canBeBorrowed = await businessDaysService.CanBeBorrowedOnDateAsync(material.Type, actualDate);
            if (!canBeBorrowed)
            {
                var message = material.Type == MaterialTypes.Journal 
                    ? "La revista no puede ser prestada para el fin de semana"
                    : "El material no puede ser prestado en la fecha solicitada";
                
                return new LoanResultDto(false, message);
            }
            
            // Validar los días solicitados según el tipo de material
            var validationResult = await businessDaysService.ValidateRequestedDaysAsync(material, requestedDays, actualDate);
            if (!validationResult.IsValid)
            {
                return new LoanResultDto(false, validationResult.ErrorMessage);
            }
            
            var returnDate = await businessDaysService.CalculateReturnDateAsync(material, actualDate, requestedDays);

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                ISBN = isbn,
                UserId = userId,
                RequestDate = actualDate,
                ReturnDate = returnDate,
                Status = "Activo"
            };

            await dbContext.Loans.AddAsync(loan);
            await dbContext.SaveChangesAsync();
            
            var loanDto = new LoanDto(
                loan.Id,
                loan.ISBN,
                loan.RequestDate,
                loan.ReturnDate,
                loan.UserId,
                loan.Status,
                material.Name,
                material.Type.ToString()
            );
            
            return new LoanResultDto(true, "Préstamo procesado exitosamente", loanDto);
        }
        catch (Exception)
        {
            return new LoanResultDto(false, "Error interno del servidor");
        }
    }

    public async Task<List<LoanDto>> GetLoansAsync()
    {
        var loans = await dbContext.Loans
            .Include(l => l.Material)  // Una línea mágica!
            .OrderByDescending(l => l.RequestDate)
            .ToListAsync();

        return loans.Select(l => new LoanDto(
            l.Id,
            l.ISBN,
            l.RequestDate,
            l.ReturnDate,
            l.UserId,
            l.Status,
            l.Material.Name,
            l.Material.Type.ToString()
        )).ToList();
    }

    public async Task<LoanDto?> GetLoanByIdAsync(Guid id)
    {
        var loan = await dbContext.Loans
            .Include(l => l.Material)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null) 
            return null;

        return new LoanDto(
            loan.Id,
            loan.ISBN,
            loan.RequestDate,
            loan.ReturnDate,
            loan.UserId,
            loan.Status,
            loan.Material.Name,
            loan.Material.Type.ToString()
        );
    }

    public async Task<List<MaterialDto>> GetMaterialsAsync()
    {
        var materials = await dbContext.BibliographicMaterials
            .OrderBy(m => m.Name)
            .ToListAsync();

        return materials.Select(m => new MaterialDto(
            m.ISBN,
            m.Name,
            m.Type.ToString(),
            m.IsAvailable
        )).ToList();
    }

}