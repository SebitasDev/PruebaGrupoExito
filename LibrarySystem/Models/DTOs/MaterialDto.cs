namespace LibrarySystem.Models.DTOs;

public record MaterialDto(
    string ISBN,
    string Name,
    string Type,
    bool IsAvailable
);