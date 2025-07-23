namespace LibrarySystem.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Método de extensión que verifica si un ISBN es palíndromo
    /// </summary>
    /// <param name="isbn">El ISBN a verificar</param>
    /// <returns>True si es palíndromo, False en caso contrario</returns>
    public static bool IsPalindrome(this string isbn)
    {
        if (string.IsNullOrEmpty(isbn)) return false;
        
        var digits = isbn.Where(char.IsDigit).ToArray();
        return digits.SequenceEqual(digits.Reverse());
    }
}