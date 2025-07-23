using LibrarySystem.Models.DTOs;
using LibrarySystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers;

[ApiController]
[Route("api/loans")]
public class LoansController(ILoanService loanService) : ControllerBase
{
    /// <summary>
    /// Procesa una solicitud de préstamo
    /// </summary>
    /// <param name="solicitud">Datos de la solicitud</param>
    /// <returns>Resultado del procesamiento</returns>
    [HttpPost]
    [ProducesResponseType(typeof(LoanResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(LoanResultDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoanResultDto>> ProcessLoan([FromBody] LoanRequestDto request)
    {
        var result = await loanService.ProcessLoanAsync(request.ISBN, request.UserId, request.DaysToLoan);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    
    /// <summary>
    /// Obtiene todos los préstamos
    /// </summary>
    /// <returns>Lista de préstamos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<LoanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LoanDto>>> GetLoans()
    {
        var results = await loanService.GetLoansAsync();
        return Ok(results);
    }
    
    /// <summary>
    /// Obtiene un préstamo por ID
    /// </summary>
    /// <param name="id">ID del préstamo</param>
    /// <returns>Datos del préstamo</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanDto>> GetLoan(Guid id)
    {
        var loan = await loanService.GetLoanByIdAsync(id);
        return loan != null ? Ok(loan) : NotFound();
    }
}