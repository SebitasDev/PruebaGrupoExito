using LibrarySystem.Models.Entities;
using LibrarySystem.Models.Enums;
using LibrarySystem.Service;

namespace LibrarySystem.Tests.Services;

[TestClass]
public class BusinessDaysServiceTests
{
    private BusinessDaysService _service;

    [TestInitialize]
    public void Setup()
    {
        _service = new BusinessDaysService();
    }

    #region AddBusinessDaysAsync Tests

    [TestMethod]
    public async Task AddBusinessDaysAsync_Should_Skip_Sundays()
    {
        // Arrange
        var initialDate = new DateTime(2025, 1, 17); // Viernes
        var businessDays = 1;

        // Act
        var result = await _service.AddBusinessDaysAsync(initialDate, businessDays);

        // Assert
        var expectedDate = new DateTime(2025, 1, 20); // Lunes
        Assert.AreEqual(expectedDate, result);
    }

    [TestMethod]
    public async Task AddBusinessDaysAsync_Should_Count_Saturdays_As_Business_Days()
    {
        // Arrange
        var initialDate = new DateTime(2025, 1, 17); // Viernes
        var businessDays = 2;

        // Act
        var result = await _service.AddBusinessDaysAsync(initialDate, businessDays);

        // Assert
        var expectedDate = new DateTime(2025, 1, 21); // Martes (salta domingo)
        Assert.AreEqual(expectedDate, result);
    }

    [TestMethod]
    public async Task AddBusinessDaysAsync_Multiple_Sundays_Should_Be_Skipped()
    {
        // Arrange
        var initialDate = new DateTime(2025, 1, 17); // Viernes
        var businessDays = 8; // Más de una semana

        // Act
        var result = await _service.AddBusinessDaysAsync(initialDate, businessDays);

        // Assert
        // Debería saltar 2 domingos (19 y 26 de enero)
        var expectedDate = new DateTime(2025, 1, 27); // Lunes
        Assert.AreEqual(expectedDate, result);
    }

    #endregion

    #region GetNextBusinessDayAsync Tests

    [TestMethod]
    public async Task GetNextBusinessDayAsync_From_Saturday_Should_Return_Monday()
    {
        // Arrange
        var saturday = new DateTime(2025, 1, 18); // Sábado

        // Act
        var result = await _service.GetNextBusinessDayAsync(saturday);

        // Assert
        var expectedDate = new DateTime(2025, 1, 20); // Lunes
        Assert.AreEqual(expectedDate, result);
    }

    [TestMethod]
    public async Task GetNextBusinessDayAsync_From_Sunday_Should_Return_Monday()
    {
        // Arrange
        var sunday = new DateTime(2025, 1, 19); // Domingo

        // Act
        var result = await _service.GetNextBusinessDayAsync(sunday);

        // Assert
        var expectedDate = new DateTime(2025, 1, 20); // Lunes
        Assert.AreEqual(expectedDate, result);
    }

    #endregion

    #region IsAvailableDay Tests

    [TestMethod]
    [DataRow(DayOfWeek.Monday, true)]
    [DataRow(DayOfWeek.Tuesday, true)]
    [DataRow(DayOfWeek.Wednesday, true)]
    [DataRow(DayOfWeek.Thursday, true)]
    [DataRow(DayOfWeek.Friday, true)]
    [DataRow(DayOfWeek.Saturday, true)]
    [DataRow(DayOfWeek.Sunday, false)]
    public void IsAvailableDay_Should_Return_Correct_Values(DayOfWeek dayOfWeek, bool expected)
    {
        // Arrange
        var date = GetDateForDayOfWeek(dayOfWeek);

        // Act
        var result = _service.IsAvailableDay(date);

        // Assert
        Assert.AreEqual(expected, result);
    }

    #endregion

    #region CalculateReturnDateAsync Tests

    [TestMethod]
    public async Task CalculateReturnDateAsync_For_Book_Should_Add_Business_Days()
    {
        // Arrange
        var book = new Book()
        {
            ISBN = "1234567890", // suma = 45 > 30, entonces 15 días
            Name = "Test Book",
        };
        var requestDate = new DateTime(2025, 1, 20); // Lunes
        var requestedDays = 3;

        // Act
        var result = await _service.CalculateReturnDateAsync(book, requestDate, requestedDays);

        // Assert
        var expectedDate = new DateTime(2025, 1, 23); // Jueves
        Assert.AreEqual(expectedDate, result);
    }

    [TestMethod]
    public async Task CalculateReturnDateAsync_For_Journal_Should_Add_Calendar_Days()
    {
        // Arrange
        var journal = new Journal()
        {
            ISBN = "1111111111",
            Name = "Test Journal"
        };
        var requestDate = new DateTime(2025, 1, 20); // Lunes
        var requestedDays = 2;

        // Act
        var result = await _service.CalculateReturnDateAsync(journal, requestDate, requestedDays);

        // Assert
        var expectedDate = new DateTime(2025, 1, 22); // Miércoles
        Assert.AreEqual(expectedDate, result);
    }

    [TestMethod]
    public async Task CalculateReturnDateAsync_Book_Return_On_Sunday_Should_Move_To_Monday()
    {
        // Arrange
        var book = new Book()
        {
            ISBN = "1234567890",
            Name = "Test Book",
        };
        // Configurar fecha que resulte en domingo como fecha de retorno
        var requestDate = new DateTime(2025, 1, 16); // Jueves
        var requestedDays = 3; // Debería caer en domingo, moverse a lunes

        // Act
        var result = await _service.CalculateReturnDateAsync(book, requestDate, requestedDays);

        // Assert
        Assert.AreNotEqual(DayOfWeek.Sunday, result.DayOfWeek);
    }

    #endregion

    #region ValidateRequestedDaysAsync Tests

    [TestMethod]
    public async Task ValidateRequestedDaysAsync_Journal_More_Than_2_Days_Should_Fail()
    {
        // Arrange
        var journal = new Journal()
        {
            ISBN = "1111111111",
            Name = "Test Journal",
        };
        var requestedDays = 3;
        var requestDate = new DateTime(2025, 1, 20); // Lunes

        // Act
        var result = await _service.ValidateRequestedDaysAsync(journal, requestedDays, requestDate);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("Las revistas solo pueden ser prestadas por máximo 2 días", result.ErrorMessage);
    }

    [TestMethod]
    public async Task ValidateRequestedDaysAsync_Journal_For_Weekend_Should_Fail()
    {
        // Arrange
        var journal = new Journal()
        {
            ISBN = "1111111111",
            Name = "Test Journal",
        };
        var requestedDays = 2;
        var requestDate = new DateTime(2025, 1, 17); // Viernes

        // Act
        var result = await _service.ValidateRequestedDaysAsync(journal, requestedDays, requestDate);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("La revista no puede ser prestada para el fin de semana", result.ErrorMessage);
    }

    [TestMethod]
    public async Task ValidateRequestedDaysAsync_Book_More_Than_Max_Days_Should_Fail()
    {
        // Arrange - ISBN con suma <= 30 (máximo 10 días)
        var book = new Book()
        {
            ISBN = "1111111111", // suma = 10 <= 30, entonces máximo 10 días
            Name = "Test Book",
        };
        var requestedDays = 12; // Más que el máximo

        // Act
        var result = await _service.ValidateRequestedDaysAsync(book, requestedDays, DateTime.Now);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("Los libros de este tipo solo pueden ser prestados por máximo 10 días hábiles", result.ErrorMessage);
    }

    [TestMethod]
    public async Task ValidateRequestedDaysAsync_Valid_Book_Request_Should_Pass()
    {
        // Arrange
        var book = new Book()
        {
            ISBN = "9876543210", // suma = 45 > 30, entonces máximo 15 días
            Name = "Test Book",
        };
        var requestedDays = 10; // Dentro del límite

        // Act
        var result = await _service.ValidateRequestedDaysAsync(book, requestedDays, DateTime.Now);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(string.Empty, result.ErrorMessage);
    }

    [TestMethod]
    public async Task ValidateRequestedDaysAsync_Valid_Journal_Request_Should_Pass()
    {
        // Arrange
        var journal = new Journal()
        {
            ISBN = "1111111111",
            Name = "Test Journal",
        };
        var requestedDays = 2;
        var requestDate = new DateTime(2025, 1, 20); // Lunes (no cae en fin de semana)

        // Act
        var result = await _service.ValidateRequestedDaysAsync(journal, requestedDays, requestDate);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(string.Empty, result.ErrorMessage);
    }

    #endregion

    #region CanBeBorrowedOnDateAsync Tests

    [TestMethod]
    public async Task CanBeBorrowedOnDateAsync_Book_Any_Day_Should_Return_True()
    {
        // Arrange
        var requestDate = new DateTime(2025, 1, 19); // Domingo

        // Act
        var result = await _service.CanBeBorrowedOnDateAsync(MaterialTypes.Book, requestDate);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow("2025-01-17", false)] // Viernes
    [DataRow("2025-01-18", false)] // Sábado  
    [DataRow("2025-01-19", false)] // Domingo
    [DataRow("2025-01-20", true)]  // Lunes
    [DataRow("2025-01-21", true)]  // Martes
    public async Task CanBeBorrowedOnDateAsync_Journal_Should_Validate_Weekdays(string dateString, bool expected)
    {
        // Arrange
        var requestDate = DateTime.Parse(dateString);

        // Act
        var result = await _service.CanBeBorrowedOnDateAsync(MaterialTypes.Journal, requestDate);

        // Assert
        Assert.AreEqual(expected, result);
    }

    #endregion

    #region Helper Methods

    private static DateTime GetDateForDayOfWeek(DayOfWeek dayOfWeek)
    {
        var baseDate = new DateTime(2025, 1, 20); // Lunes
        var daysToAdd = ((int)dayOfWeek - (int)baseDate.DayOfWeek + 7) % 7;
        return baseDate.AddDays(daysToAdd);
    }

    #endregion
}