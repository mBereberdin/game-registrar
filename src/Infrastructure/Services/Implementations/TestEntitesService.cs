namespace Infrastructure.Services.Implementations;

using Domain.Models;

using Infrastructure.Services.Interfaces;

using Microsoft.Extensions.Logging;

/// <inheritdoc />
public class TestEntitesService : ITestEntitesService
{
    /// <summary>
    /// Тестовые сущности.
    /// </summary>
    private readonly IList<TestEntity> _testEntities;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<ITestEntitesService> _logger;

    /// <inheritdoc cref="TestEntitesService"/>
    /// <param name="logger">Логгер.</param>
    public TestEntitesService(ILogger<ITestEntitesService> logger)
    {
        _logger = logger;
        _logger.LogDebug($"Инициализация: {nameof(TestEntitesService)}.");

        _testEntities = new List<TestEntity>();
        _logger.LogDebug($"{nameof(TestEntitesService)}: инициализирован.");
    }

    /// <summary>
    /// Тестовые сущности.
    /// </summary>
    public IList<TestEntity> TestEntities
    {
        get
        {
            _logger.LogInformation("Получение тестовых сущностей.");
            _logger.LogDebug(
                $"Кол-во тестовых сущностей: {_testEntities.Count}.");
            _logger.LogDebug(
                "Идентификаторы тестовых сущностей: {testEntities}",
                _testEntities.Select(testEntity => testEntity.Id));

            return _testEntities;
        }
    }

    /// <inheritdoc />
    public async Task<TestEntity> GetEntityAsync(Guid id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation("Получение тестовой сущности по id.");
        _logger.LogDebug("Id тестовой сущности: {id}.", id);

        var testEntity = await Task.Run(
            () => TestEntities.First(entity => entity.Id.Equals(id)),
            cancellationToken);

        _logger.LogInformation(
            "Получение тестовой сущности по id - успешно завершено.");
        _logger.LogDebug("Тестовая сущность: {testEntity}.", testEntity);

        return testEntity;
    }

    /// <inheritdoc />
    public async Task AddTestEntityAsync(TestEntity testEntity,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation("Добавление тестовой сущности.");
        _logger.LogDebug("Тестовая сущность: {testEntity}.", testEntity);

        await Task.Run(() => _testEntities.Add(testEntity), cancellationToken);

        _logger.LogInformation(
            "Добавление тестовой сущности - успешно завершено.");
        _logger.LogDebug("Тестовая сущность: {testEntity}.", testEntity);
    }
}