namespace WebApi.Controllers;

using Domain.DTOs.Entites;
using Domain.Models;

using Infrastructure.Services.Interfaces;

using Mapster;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Контроллер тестовых сущностей.
/// </summary>
[Route("[controller]")]
[ApiController]
public class TestEntitiesController : ControllerBase
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<TestEntitiesController> _logger;

    /// <inheritdoc cref="ITestEntitesService"/>
    private readonly ITestEntitesService _testEntitesService;

    /// <inheritdoc />
    /// <param name="logger">Логгер.</param>
    /// <param name="testEntitesService">Сервис тестовых сущностей.</param>
    public TestEntitiesController(ILogger<TestEntitiesController> logger,
        ITestEntitesService testEntitesService)
    {
        _logger = logger;
        _logger.LogDebug($"Инициализация: {nameof(TestEntitiesController)}.");

        _testEntitesService = testEntitesService;

        _logger.LogDebug($"{nameof(TestEntitiesController)}: инициализирован.");
    }

    /// <summary>
    /// Получить тестовую сущность.
    /// </summary>
    /// <param name="id">Идентификатор тестовой сущности.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>>
    /// <returns>Тестовая сущность в формате JSON.</returns>
    /// <response code="200">Когда тестовая сущность успешно получена.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(TestEntityDto))]
    public async Task<OkObjectResult> GetTestEntityAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation(
            "Получен запрос на получение тестовой сущности по id.");
        _logger.LogDebug("Идентификатор тестовой сущности: {id}", id);

        var testEntity =
            await _testEntitesService.GetEntityAsync(id, cancellationToken);

        var testEntityDto = testEntity.Adapt<TestEntityDto>();

        _logger.LogInformation(
            "Запрос на получение тестовой сущности по id - успешно обработан.");
        _logger.LogDebug("Дто полученной тестовой сущности: {testEntityDto}.",
            testEntityDto);

        return Ok(testEntityDto);
    }

    /// <summary>
    /// Добавить тестовую сущность.
    /// </summary>
    /// <param name="testEntityDto">Идентификатор тестовой сущности.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>>
    /// <response code="203">Когда тестовая сущность успешно добавлена.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<NoContentResult> AddTestEntityAsync(
        TestEntityDto testEntityDto,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation(
            "Поступил запрос на добавление тестовой сущности.");
        _logger.LogDebug("Дто тестовой сущности: {testEntityDto}",
            testEntityDto);

        var testEntity = testEntityDto.Adapt<TestEntity>();

        await _testEntitesService.AddTestEntityAsync(testEntity,
            cancellationToken);

        _logger.LogInformation(
            "Запрос на добавление тестовой сущности - успешно обработан.");
        _logger.LogDebug("Модель добавленной тестовой сущности: {testEntity}.",
            testEntity);

        return NoContent();
    }
}