using LibrarySystem.Service.Interface;
using LibrarySystem.Models.Entities;
using LibrarySystem.Models.Enums;

namespace LibrarySystem.Service;

public class BusinessDaysService : IBusinessDaysService
{
    public async Task<DateTime> AddBusinessDaysAsync(DateTime initialDate, int availableDays)
    {
        var actualDate = initialDate;
        var dayAdds = 0;

        while (dayAdds < availableDays)
        {
            actualDate = actualDate.AddDays(1);
        
            if (IsAvailableDay(actualDate)) 
            {
                dayAdds++;
            }
        }

        return await Task.FromResult(actualDate);
    }

    public async Task<DateTime> GetNextBusinessDayAsync(DateTime date)
    {
        var nextDate = date.AddDays(1);
        
        while (!IsAvailableDay(nextDate)) nextDate = nextDate.AddDays(1);
        
        return await Task.FromResult(nextDate);
    }

    public bool IsAvailableDay(DateTime date) =>
        date.DayOfWeek != DayOfWeek.Sunday;

    public async Task<DateTime> CalculateReturnDateAsync(BibliographicMaterial material, DateTime requestDate, int requestedDays)
    {
        return material.Type switch
        {
            MaterialTypes.Journal => await CalculateJournalReturnDateAsync(requestDate, requestedDays),
            MaterialTypes.Book => await CalculateBookReturnDateAsync(requestDate, requestedDays),
            _ => throw new InvalidOperationException($"Tipo de material no soportado: {material.Type}")
        };
    }

    public async Task<(bool IsValid, string ErrorMessage)> ValidateRequestedDaysAsync(BibliographicMaterial material, int requestedDays, DateTime requestDate)
    {
        return material.Type switch
        {
            MaterialTypes.Journal => await ValidateJournalDaysAsync(requestedDays, requestDate),
            MaterialTypes.Book => await ValidateBookDaysAsync(material.ISBN, requestedDays),
            _ => (false, $"Tipo de material no soportado: {material.Type}")
        };
    }

    private static async Task<DateTime> CalculateJournalReturnDateAsync(DateTime requestDate, int requestedDays)
    {
        var returnDate = requestDate;
    
        // Para revistas, simplemente agregamos los días solicitados (incluyendo fines de semana)
        returnDate = returnDate.AddDays(requestedDays);
    
        return await Task.FromResult(returnDate);
    }

    private async Task<DateTime> CalculateBookReturnDateAsync(DateTime requestDate, int requestedDays)
    {
        // Calcular la fecha agregando solo días hábiles
        var returnDate = await AddBusinessDaysAsync(requestDate, requestedDays);
    
        // Si la fecha de entrega cae domingo, mover al siguiente día hábil (lunes)
        if (returnDate.DayOfWeek == DayOfWeek.Sunday)
        {
            returnDate = returnDate.AddDays(1); // Mover a lunes
        }
    
        return returnDate;
    }

    private static async Task<(bool IsValid, string ErrorMessage)> ValidateJournalDaysAsync(int requestedDays, DateTime requestDate)
    {
        // Las revistas solo se pueden prestar por máximo 2 días
        if (requestedDays > 2)
        {
            return (false, "Las revistas solo pueden ser prestadas por máximo 2 días");
        }
        
        // Verificar que no caiga en fin de semana
        var journalReturnDate = await CalculateJournalReturnDateAsync(requestDate, requestedDays);
        if (journalReturnDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return (false, "La revista no puede ser prestada para el fin de semana");
        }
        
        return (true, string.Empty);
    }

    private static async Task<(bool IsValid, string ErrorMessage)> ValidateBookDaysAsync(string isbn, int requestedDays)
    {
        var maxDays = GetMaxBookLoanDays(isbn);
        if (requestedDays > maxDays)
        {
            return (false, $"Los libros de este tipo solo pueden ser prestados por máximo {maxDays} días hábiles");
        }
        
        return await Task.FromResult((true, string.Empty));
    }

    private static int GetMaxBookLoanDays(string isbn)
    {
        var digitSum = isbn.Where(char.IsDigit).Sum(c => int.Parse(c.ToString()));
        return digitSum > 30 ? 15 : 10;
    }

    public async Task<bool> CanBeBorrowedOnDateAsync(MaterialTypes materialType, DateTime requestDate)
    {
        return materialType switch
        {
            MaterialTypes.Journal => await CanJournalBeBorrowedAsync(requestDate),
            MaterialTypes.Book => await Task.FromResult(true),
            _ => await Task.FromResult(false)
        };
    }

    private static async Task<bool> CanJournalBeBorrowedAsync(DateTime requestDate)
    {
        // Las revistas no se pueden prestar si la fecha de solicitud es viernes
        // (porque podrían incluir el fin de semana en el período de préstamo)
        return await Task.FromResult(
            requestDate.DayOfWeek != DayOfWeek.Friday &&
            requestDate.DayOfWeek != DayOfWeek.Saturday &&
            requestDate.DayOfWeek != DayOfWeek.Sunday
        );
    }
}