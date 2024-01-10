namespace Domain.Models;

/// <summary>
/// Тестовая сущность.
/// </summary>
public class TestEntity
{
    /// <inheritdoc cref="TestEntity"/>
    /// <param name="id">Идентификатор.</param>
    public TestEntity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Идентификатор.
    /// </summary>
    public Guid Id { get; set; }
}