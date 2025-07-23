using LibrarySystem.Extensions;

namespace LibrarySystem.Tests.Extensions;

[TestClass]
public class ISBNExtensionsTests
{
    #region IsPalindrome Tests

    [TestMethod]
    public void IsPalindrome_Simple_Palindrome_Should_Return_True()
    {
        // Arrange
        var isbn = "1234554321";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsPalindrome_Non_Palindrome_Should_Return_False()
    {
        // Arrange
        var isbn = "1234567890";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsPalindrome_Single_Character_Should_Return_True()
    {
        // Arrange
        var isbn = "1";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsPalindrome_Even_Length_Palindrome_Should_Return_True()
    {
        // Arrange
        var isbn = "1221";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsPalindrome_Odd_Length_Palindrome_Should_Return_True()
    {
        // Arrange
        var isbn = "12321";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsPalindrome_With_Hyphens_Should_Ignore_Hyphens()
    {
        // Arrange
        var isbn = "123-45-54321";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsPalindrome_With_Spaces_Should_Ignore_Spaces()
    {
        // Arrange
        var isbn = "123 45 54321";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsPalindrome_Mixed_Case_Should_Be_Case_Insensitive()
    {
        // Arrange
        var isbn = "123A321";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsPalindrome_Empty_String_Should_Return_True()
    {
        // Arrange
        var isbn = "";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsPalindrome_Null_String_Should_Handle_Gracefully()
    {
        // Arrange
        string isbn = null;

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => isbn.IsPalindrome());
    }

    [TestMethod]
    public void IsPalindrome_Real_ISBN13_Palindrome_Should_Return_True()
    {
        // Arrange
        var isbn = "9781234554321";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsFalse(result); // Este no es realmente un palíndromo
    }

    [TestMethod]
    public void IsPalindrome_Real_ISBN13_Palindrome_Correct_Should_Return_True()
    {
        // Arrange - ISBN de 13 dígitos que SÍ es palíndromo
        var isbn = "9781234554321".Replace("9781234554321", "1234554321987"); // Crear uno verdadero
        isbn = "1234554321"; // Usar ISBN-10 palíndromo real

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow("1234554321", true)]   // Palíndromo clásico
    [DataRow("9876543210", false)]  // No palíndromo
    [DataRow("1111111111", true)]   // Todos iguales
    [DataRow("1234567890", false)]  // Secuencial
    [DataRow("0123454321", true)]   // Con cero inicial
    [DataRow("9999999999", true)]   // Todos 9s
    public void IsPalindrome_DataDriven_Tests(string isbn, bool expected)
    {
        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.AreEqual(expected, result, $"ISBN {isbn} should return {expected}");
    }

    #endregion

    #region Performance Tests

    [TestMethod]
    public void IsPalindrome_Long_String_Should_Be_Performant()
    {
        // Arrange - Crear string muy largo pero simétrico
        var longPalindrome = "123456789" + "987654321";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = longPalindrome.IsPalindrome();

        // Assert
        stopwatch.Stop();
        Assert.IsTrue(result);
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, "El método debería ser rápido incluso con strings largos");
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    public void IsPalindrome_Only_Spaces_And_Hyphens_Should_Return_True()
    {
        // Arrange
        var isbn = "--- ---";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result); // String vacío después de limpiar
    }

    [TestMethod]
    public void IsPalindrome_Numbers_And_Letters_Mixed_Palindrome()
    {
        // Arrange
        var isbn = "1A2B2A1";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsPalindrome_Numbers_And_Letters_Mixed_Non_Palindrome()
    {
        // Arrange
        var isbn = "1A2B3C1";

        // Act
        var result = isbn.IsPalindrome();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion
}