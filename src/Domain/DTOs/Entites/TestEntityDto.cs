namespace Domain.DTOs.Entites;

/// <summary>
/// ДТО тестовой сущности.
/// </summary>
/// <param name="Id">Идентификатор.</param>
public record TestEntityDto(Guid Id)
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public Guid Id { get; init; } = Id;
}