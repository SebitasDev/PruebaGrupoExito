using LibrarySystem.Data;
using LibrarySystem.Models.Entities;
using LibrarySystem.Models.Enums;
using NSubstitute;
using LibrarySystem.Service;
using LibrarySystem.Service.Interface;
using LibrarySystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ExceptionExtensions;

namespace LibrarySystem.Tests.Services;

[TestClass]
public class LoanServiceTests
{
    private LoanService _loanService;
    private IBibliographicMaterialRepository _materialRepository;
    private ILoanRepository _loanRepository;
    private IBusinessDaysService _businessDaysService;
    private LibraryDbContext _dbContext;

    [TestInitialize]
    public void Setup()
    {
        // Crear mocks
        _materialRepository = Substitute.For<IBibliographicMaterialRepository>();
        _loanRepository = Substitute.For<ILoanRepository>();
        _businessDaysService = Substitute.For<IBusinessDaysService>();

        // Configurar DbContext en memoria
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new LibraryDbContext(options);

        // Crear instancia del servicio
        _loanService = new LoanService(
            _loanRepository,
            _materialRepository,
            _businessDaysService,
            _dbContext
        );
    }

    [TestCleanup]
    public void Cleanup()
    {
        _dbContext?.Dispose();
    }

    #region ProcessLoanAsync Tests - Material Not Found

    [TestMethod]
    public async Task ProcessLoanAsync_Material_Not_Found_Should_Return_Error()
    {
        // Arrange
        var isbn = "1234567890";
        var userId = Guid.NewGuid();
        var requestedDays = 5;

        _materialRepository.GetByIsbnAsync(isbn).Returns((BibliographicMaterial)null);

        // Act
        var result = await _loanService.ProcessLoanAsync(isbn, userId, requestedDays);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Material no encontrado", result.Message);
    }

    #endregion

    #region ProcessLoanAsync Tests - Material Already Loaned

    [TestMethod]
    public async Task ProcessLoanAsync_Material_Already_Loaned_Should_Return_Error()
    {
        // Arrange
        var isbn = "1234567890";
        var userId = Guid.NewGuid();
        var requestedDays = 5;

        var material = new Book()
        {
            ISBN = isbn,
            Name = "Test Book",
        };

        _materialRepository.GetByIsbnAsync(isbn).Returns(material);
        _loanRepository.ExistsActiveLoanAsync(isbn).Returns(true);

        // Act
        var result = await _loanService.ProcessLoanAsync(isbn, userId, requestedDays);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("El material no puede ser prestado: Ese ISBN ya está prestado", result.Message);
    }

    #endregion

    #region ProcessLoanAsync Tests - Palindrome ISBN

    [TestMethod]
    public async Task ProcessLoanAsync_Palindrome_ISBN_Should_Return_Error()
    {
        // Arrange
        var isbn = "1234554321"; // ISBN palíndromo
        var userId = Guid.NewGuid();
        var requestedDays = 5;

        var material = new Book()
        {
            ISBN = isbn,
            Name = "Test Book",
        };

        _materialRepository.GetByIsbnAsync(isbn).Returns(material);
        _loanRepository.ExistsActiveLoanAsync(isbn).Returns(false);

        // Act
        var result = await _loanService.ProcessLoanAsync(isbn, userId, requestedDays);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("El material con ISBN en palíndromo solo es para uso en la biblioteca", result.Message);
    }

    #endregion

    #region ProcessLoanAsync Tests - Cannot Be Borrowed On Date

    [TestMethod]
    public async Task ProcessLoanAsync_Journal_Cannot_Be_Borrowed_Weekend_Should_Return_Error()
    {
        // Arrange
        var isbn = "1111111111";
        var userId = Guid.NewGuid();
        var requestedDays = 2;

        var material = new Journal()
        {
            ISBN = isbn,
            Name = "Test Journal",
        };

        _materialRepository.GetByIsbnAsync(isbn).Returns(material);
        _loanRepository.ExistsActiveLoanAsync(isbn).Returns(false);
        _businessDaysService.CanBeBorrowedOnDateAsync(MaterialTypes.Journal, Arg.Any<DateTime>()).Returns(false);

        // Act
        var result = await _loanService.ProcessLoanAsync(isbn, userId, requestedDays);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("La revista no puede ser prestada para el fin de semana", result.Message);
    }

    #endregion

    #region ProcessLoanAsync Tests - Invalid Requested Days

    [TestMethod]
    public async Task ProcessLoanAsync_Invalid_Requested_Days_Should_Return_Error()
    {
        // Arrange
        var isbn = "1111111111";
        var userId = Guid.NewGuid();
        var requestedDays = 5; // Más de 2 días para revista

        var material = new Journal()
        {
            ISBN = isbn,
            Name = "Test Journal",
        };

        var validationResult = (IsValid: false, ErrorMessage: "Las revistas solo pueden ser prestadas por máximo 2 días");

        _materialRepository.GetByIsbnAsync(isbn).Returns(material);
        _loanRepository.ExistsActiveLoanAsync(isbn).Returns(false);
        _businessDaysService.CanBeBorrowedOnDateAsync(MaterialTypes.Journal, Arg.Any<DateTime>()).Returns(true);
        _businessDaysService.ValidateRequestedDaysAsync(material, requestedDays, Arg.Any<DateTime>()).Returns(validationResult);

        // Act
        var result = await _loanService.ProcessLoanAsync(isbn, userId, requestedDays);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Las revistas solo pueden ser prestadas por máximo 2 días", result.Message);
    }

    #endregion

    #region ProcessLoanAsync Tests - Successful Loan

    [TestMethod]
    public async Task ProcessLoanAsync_Valid_Request_Should_Create_Loan_Successfully()
    {
        // Arrange
        var isbn = "9876543210"; // No es palíndromo
        var userId = Guid.NewGuid();
        var requestedDays = 5;
        var expectedReturnDate = DateTime.Now.AddDays(7);

        var material = new Book()
        {
            ISBN = isbn,
            Name = "Test Book",
        };

        var validationResult = (IsValid: true, ErrorMessage: string.Empty);

        _materialRepository.GetByIsbnAsync(isbn).Returns(material);
        _loanRepository.ExistsActiveLoanAsync(isbn).Returns(false);
        _businessDaysService.CanBeBorrowedOnDateAsync(MaterialTypes.Book, Arg.Any<DateTime>()).Returns(true);
        _businessDaysService.ValidateRequestedDaysAsync(material, requestedDays, Arg.Any<DateTime>()).Returns(validationResult);
        _businessDaysService.CalculateReturnDateAsync(material, Arg.Any<DateTime>(), requestedDays).Returns(expectedReturnDate);

        // Act
        var result = await _loanService.ProcessLoanAsync(isbn, userId, requestedDays);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Préstamo procesado exitosamente", result.Message);
        Assert.IsNotNull(result.Loan);
        Assert.AreEqual(isbn, result.Loan.ISBN);
        Assert.AreEqual(userId, result.Loan.UserId);
        Assert.AreEqual(expectedReturnDate, result.Loan.ReturnDate);
        Assert.AreEqual("Activo", result.Loan.Status);

        // Verificar que se guardó en la base de datos
        var savedLoan = await _dbContext.Loans.FirstOrDefaultAsync(l => l.ISBN == isbn);
        Assert.IsNotNull(savedLoan);
        Assert.AreEqual("Activo", savedLoan.Status);
    }

    #endregion

    #region ProcessLoanAsync Tests - Book with High ISBN Sum

    [TestMethod]
    public async Task ProcessLoanAsync_Book_High_ISBN_Sum_Should_Allow_15_Days()
    {
        // Arrange
        var isbn = "9876543210"; // suma = 45 > 30, permite hasta 15 días
        var userId = Guid.NewGuid();
        var requestedDays = 15;
        var expectedReturnDate = DateTime.Now.AddDays(15);

        var material = new Book()
        {
            ISBN = isbn,
            Name = "Test Book",
        };

        var validationResult = (IsValid: true, ErrorMessage: string.Empty);

        _materialRepository.GetByIsbnAsync(isbn).Returns(material);
        _loanRepository.ExistsActiveLoanAsync(isbn).Returns(false);
        _businessDaysService.CanBeBorrowedOnDateAsync(MaterialTypes.Book, Arg.Any<DateTime>()).Returns(true);
        _businessDaysService.ValidateRequestedDaysAsync(material, requestedDays, Arg.Any<DateTime>()).Returns(validationResult);
        _businessDaysService.CalculateReturnDateAsync(material, Arg.Any<DateTime>(), requestedDays).Returns(expectedReturnDate);

        // Act
        var result = await _loanService.ProcessLoanAsync(isbn, userId, requestedDays);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Préstamo procesado exitosamente", result.Message);
    }

    #endregion

    #region ProcessLoanAsync Tests - Book with Low ISBN Sum

    [TestMethod]
    public async Task ProcessLoanAsync_Book_Low_ISBN_Sum_Should_Limit_10_Days()
    {
        // Arrange
        var isbn = "1111111111"; // suma = 10 <= 30, límite de 10 días
        var userId = Guid.NewGuid();
        var requestedDays = 12; // Más del límite

        var material = new Book()
        {
            ISBN = isbn,
            Name = "Test Book",
        };

        var validationResult = (IsValid: false, ErrorMessage: "Los libros de este tipo solo pueden ser prestados por máximo 10 días hábiles");

        _materialRepository.GetByIsbnAsync(isbn).Returns(material);
        _loanRepository.ExistsActiveLoanAsync(isbn).Returns(false);
        _businessDaysService.CanBeBorrowedOnDateAsync(MaterialTypes.Book, Arg.Any<DateTime>()).Returns(true);
        _businessDaysService.ValidateRequestedDaysAsync(material, requestedDays, Arg.Any<DateTime>()).Returns(validationResult);

        // Act
        var result = await _loanService.ProcessLoanAsync(isbn, userId, requestedDays);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Los libros de este tipo solo pueden ser prestados por máximo 10 días hábiles", result.Message);
    }

    #endregion

    #region ProcessLoanAsync Tests - Exception Handling

    [TestMethod]
    public async Task ProcessLoanAsync_Repository_Exception_Should_Return_Error()
    {
        // Arrange
        var isbn = "1234567890";
        var userId = Guid.NewGuid();
        var requestedDays = 5;

        _materialRepository.GetByIsbnAsync(isbn).Throws(new Exception("Database error"));

        // Act
        var result = await _loanService.ProcessLoanAsync(isbn, userId, requestedDays);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Error interno del servidor", result.Message);
    }

    #endregion

    #region GetActiveLoansAsync Tests

    [TestMethod]
    public async Task GetActiveLoansAsync_Should_Return_Active_Loans()
    {
        // Arrange
        var activeLoans = new List<Loan>
        {
            new Loan
            {
                Id = Guid.NewGuid(),
                ISBN = "1234567890",
                UserId = Guid.NewGuid(),
                RequestDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(7),
                Status = "Activo"
            }
        };

        // Act
        var result = await _loanService.GetLoansAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("1234567890", result.First().ISBN);
    }

    #endregion

    #region GetLoanByIdAsync Tests

    [TestMethod]
    public async Task GetLoanByIdAsync_Existing_Loan_Should_Return_LoanDto()
    {
        // Arrange
        var loanId = Guid.NewGuid();
        var loan = new Loan
        {
            Id = loanId,
            ISBN = "1234567890",
            UserId = Guid.NewGuid(),
            RequestDate = DateTime.Now,
            ReturnDate = DateTime.Now.AddDays(7),
            Status = "Activo"
        };

        // Act
        var result = await _loanService.GetLoanByIdAsync(loanId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(loanId, result.Id);
        Assert.AreEqual("1234567890", result.ISBN);
    }

    [TestMethod]
    public async Task GetLoanByIdAsync_Non_Existing_Loan_Should_Return_Null()
    {
        // Arrange
        var loanId = Guid.NewGuid();

        // Act
        var result = await _loanService.GetLoanByIdAsync(loanId);

        // Assert
        Assert.IsNull(result);
    }

    #endregion
}