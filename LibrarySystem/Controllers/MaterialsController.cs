using LibrarySystem.Data;
using LibrarySystem.Models.DTOs;
using LibrarySystem.Models.Entities;
using LibrarySystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[ApiController]
[Route("api/materials")]
public class MaterialsController(
    ILoanService loanService,
    LibraryDbContext dbContext
) : ControllerBase
{
    /// <summary>
    /// Obtiene todos los materiales bibliográficos
    /// </summary>
    /// <returns>Lista de materiales</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<MaterialDto>), 200)]
    public async Task<ActionResult<List<MaterialDto>>> GetMaterials()
    {
        var materials = await loanService.GetMaterialsAsync();
        return Ok(materials);
    }
    
    /// <summary>
    /// Crea un nuevo libro
    /// </summary>
    /// <param name="request">Datos del libro</param>
    /// <returns>Libro creado</returns>
    [HttpPost("books")]
    [ProducesResponseType(typeof(MaterialDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MaterialDto>> CreateBook([FromBody] CrateMaterialRequest request)
    {
        if (await dbContext.BibliographicMaterials.AnyAsync(m => m.ISBN == request.ISBN))
            return BadRequest("Ya existe un material con este ISBN");

        var book = new Book()
        {
            ISBN = request.ISBN,
            Name = request.Name
        };

        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync();

        var materialDto = new MaterialDto(book.ISBN, book.Name, book.Type.ToString(), book.IsAvailable);
        return CreatedAtAction(nameof(GetMaterials), materialDto);
    }
    
    /// <summary>
    /// Crea una nueva revista
    /// </summary>
    /// <param name="request">Datos de la revista</param>
    /// <returns>Revista creada</returns>
    [HttpPost("journals")]
    [ProducesResponseType(typeof(MaterialDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MaterialDto>> CreateJournal([FromBody] CrateMaterialRequest request)
    {
        if (await dbContext.BibliographicMaterials.AnyAsync(m => m.ISBN == request.ISBN))
            return BadRequest("Ya existe un material con este ISBN");

        var journal = new Journal()
        {
            ISBN = request.ISBN,
            Name = request.Name
        };

        dbContext.Journals.Add(journal);
        await dbContext.SaveChangesAsync();

        var materialDto = new MaterialDto(journal.ISBN, journal.Name, journal.Type.ToString(), journal.IsAvailable);
        return CreatedAtAction(nameof(GetMaterials), materialDto);
    }
}

public record CrateMaterialRequest(string ISBN, string Name);