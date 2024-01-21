namespace Infrastructure.Services.Implementations;

using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

using Domain.DTOs;
using Domain.Settings;

using Infrastructure.Extensions.Settings;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <inheritdoc />
public class GamesHubService : BackgroundService
{
    /// <inheritdoc cref="GamesHubSettings"/>
    private readonly GamesHubSettings? _gamesHubSettings;

    /// <inheritdoc cref="RegistrarSettings"/>
    private readonly RegistrarSettings? _registrarSettings;

    /// <summary>
    /// Фабрика http клиентов.
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<GamesHubService> _logger;

    /// <inheritdoc cref="IGamesHubService"/>
    /// <param name="gamesHubSettings">Настройки узла игр.</param>
    /// <param name="registrarSettings">Настройки регистратора.</param>
    /// <param name="httpClientFactory">Фабрика http клиентов.</param>
    /// <param name="logger">Логгер.</param>
    public GamesHubService(IOptions<GamesHubSettings> gamesHubSettings,
        IOptions<RegistrarSettings> registrarSettings,
        IHttpClientFactory httpClientFactory,
        ILogger<GamesHubService> logger)
    {
        _logger = logger;
        _logger.LogDebug($"Инициализация: {nameof(GamesHubSettings)}.");

        _gamesHubSettings = gamesHubSettings.Value;
        _registrarSettings = registrarSettings.Value;
        _httpClientFactory = httpClientFactory;

        _logger.LogDebug($"{nameof(GamesHubSettings)}: инициализирован.");
    }

    /// <inheritdoc />
    public async Task<bool> RegisterGameAsync(HttpClient httpClient,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation("Регистрация игры в узле игр.");

        if (!AreSettingsCorrect())
        {
            _logger.LogError(
                "Не удалось зарегистрировать игру в узле т.к. настройки были не корректными.");

            return false;
        }

        var registrationUrl =
            new Uri(
                $"{_gamesHubSettings!.Host}{_gamesHubSettings.RegisterEndpoint}");

        var registerGameDto = new RegisterGameDto
        {
            Name = _registrarSettings!.GameName!
        };

        var registerGameDtoJson = JsonSerializer.Serialize(registerGameDto) ??
                                  throw new SerializationException(
                                      "Не удалось сериализовать ДТО регистрации игры.");

        var jsonContent = new StringContent(registerGameDtoJson, Encoding.UTF8,
            "application/json");

        _logger.LogInformation("Отправка запроса регистрации игры.");
        _logger.LogDebug(
            "Адрес запроса: {registrationUrl}", registrationUrl);
        _logger.LogDebug(
            "ДТО регистрации игры в формате json: {registerGameDtoJson}",
            registerGameDtoJson);

        var registrationResponse =
            await httpClient.PostAsync(registrationUrl, jsonContent,
                cancellationToken);

        _logger.LogInformation("Получен ответ на запрос регистрации игры.");
        _logger.LogDebug(
            "Ответ на запрос регистрации игры: {registrationResponse}",
            registrationResponse);

        if (!registrationResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Ответ от узла игр на запрос регистрации игры оказался не успешным.");

            return false;
        }

        _logger.LogInformation(
            "Регистрация игры в узле игр - выполнено успешно.");

        return true;
    }

    /// <inheritdoc />
    public bool AreSettingsCorrect()
    {
        _logger.LogInformation(
            "Проверка корректности настроек регистратора и узла игр.");

        var areCorrect = _registrarSettings.AreValid() &&
                         _gamesHubSettings.AreValid();

        _logger.LogInformation(
            "Проверка корректности настроек регистратора и узла игр - завершено.");
        _logger.LogDebug($"Результат проверки: {areCorrect}.");

        return areCorrect;
    }

    /// <summary>
    /// Запустить фоновую задачу.
    /// </summary>
    /// <param name="stoppingToken">Токен отмены выполнения задачи.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Подготовка периодической регистрации в узле игр.");

        if (!_registrarSettings.AreValid())
        {
            _logger.LogError(
                "Не удалось запустить периодическую регистрацию в узле игр т.к. настройки регистратора были не корректными.");

            // StopAsync не подходит т.к. он может отменить остановку регистрации. см. реализацию StopAsync.
            Dispose();

            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug(
                "Выполнение периодической регистрации в узле игр.");

            using var httpClient = _httpClientFactory.CreateClient("GamesHub");

            // Не ждём, т.к. эта задача должна выполняется в фоне.
            RegisterGameAsync(httpClient, stoppingToken);

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(_registrarSettings!
                        .RegistrationTimeSeconds),
                    stoppingToken);
            }
            catch (TaskCanceledException exception)
            {
                _logger.LogInformation(
                    "При ожидании следующей иттерации регистрации произошла ошибка - задача была отменена.");
                _logger.LogDebug("Ошибка: {exception}", exception);
            }
        }

        _logger.LogInformation(
            "Периодическая регистрация в узле игр - отменена.");
    }
}