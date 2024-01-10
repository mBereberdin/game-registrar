namespace Infrastructure.Services.Interfaces;

using Domain.Models;

/// <summary>
/// Сервис тестовых сущностей.
/// </summary>
public interface ITestEntitesService
{
    /// <summary>
    /// Получить тестовую сущность.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу, результатом которой является модель тестовой сущности.</returns>
    public Task<TestEntity> GetEntityAsync(Guid id,
        CancellationToken cancellationToken);

    /// <summary>
    /// Добвить тестовую сущность.
    /// </summary>
    /// <param name="testEntity">Тестовая сущность.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу.</returns>
    public Task AddTestEntityAsync(TestEntity testEntity,
        CancellationToken cancellationToken);
}