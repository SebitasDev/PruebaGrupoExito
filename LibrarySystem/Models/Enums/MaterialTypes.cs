using System.ComponentModel;

namespace LibrarySystem.Models.Enums;

public enum MaterialTypes
{
    [Description("Libro")]
    Book = 1,
    
    [Description("Revista")]
    Journal = 2
}