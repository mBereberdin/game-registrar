namespace Domain.DTOs.Entites;

/// <summary>
/// ДТО создания тестовой сущности.
/// </summary>
/// <param name="Id">Идентифиактор.</param>
public record CreateTestEntityDto(Guid Id)
{
    /// <summary>
    /// Идентифиактор.
    /// </summary>
    public Guid Id { get; set; } = Id;
}