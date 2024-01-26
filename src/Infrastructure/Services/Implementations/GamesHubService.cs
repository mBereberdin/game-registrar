namespace Infrastructure.Services.Implementations;

using Domain.Constants;
using Domain.DTOs;
using Domain.Settings;

using Infrastructure.Extensions;
using Infrastructure.Extensions.Settings;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Сервис для взаимодействия с Games Hub.
/// </summary>
public class GamesHubService : BackgroundService
{
    /// <inheritdoc cref="GamesHubSettings"/>
    private readonly GamesHubSettings? _gamesHubSettings;

    /// <inheritdoc cref="RegistrarSettings"/>
    private readonly RegistrarSettings? _registrarSettings;

    /// <summary>
    /// URL регистрации игры.
    /// </summary>
    private readonly Uri _gameRegistrationUrl;

    /// <summary>
    /// Строковое содержимое регистрации игры в формате JSON.
    /// </summary>
    private readonly StringContent _registerGameStringContent;

    /// <summary>
    /// Фабрика http клиентов.
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<GamesHubService> _logger;

    /// <inheritdoc cref="GamesHubService"/>
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
        _logger.LogDebug($"Инициализация: {nameof(GamesHubService)}.");

        _gamesHubSettings = gamesHubSettings.Value;
        _registrarSettings = registrarSettings.Value;
        _httpClientFactory = httpClientFactory;

        if (!AreSettingsCorrect())
        {
            _logger.LogDebug(
                $"Не удалось проинициализировать: {nameof(GamesHubService)}, т.к. настройки были не корректными.");

            throw new ArgumentException(
                "Не корректные настройки для инициализации сервиса.");
        }

        _gameRegistrationUrl = new Uri(
            $"{_gamesHubSettings!.Host}{_gamesHubSettings.RegisterEndpoint}");

        _registerGameStringContent = new RegisterGameDto
        {
            Name = _registrarSettings!.GameName!
        }.ToJsonStringContent();

        _logger.LogDebug($"{nameof(GamesHubService)}: инициализирован.");
    }

    /// <summary>
    /// Регистрация игры.
    /// </summary>
    /// <param name="httpClient">Http клиент.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    private async Task RegisterGameAsync(HttpClient httpClient,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation("Регистрация игры в узле игр.");

        _logger.LogInformation("Отправка запроса регистрации игры.");
        _logger.LogDebug(
            "Адрес запроса: {gameRegistrationUrl}", _gameRegistrationUrl);

        var registrationResponse =
            await httpClient.PostAsync(_gameRegistrationUrl,
                _registerGameStringContent, cancellationToken);

        _logger.LogInformation("Получен ответ на запрос регистрации игры.");
        _logger.LogDebug(
            "Ответ на запрос регистрации игры: {registrationResponse}",
            registrationResponse);

        if (!registrationResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Ответ от узла игр на запрос регистрации игры оказался не успешным.");

            return;
        }

        _logger.LogInformation(
            "Регистрация игры в узле игр - выполнено успешно.");
    }

    /// <summary>
    /// Корректны ли настройки.
    /// </summary>
    /// <returns>True - если настройки корректны, иначе - false.</returns>
    private bool AreSettingsCorrect()
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

            using var httpClient =
                _httpClientFactory.CreateClient(
                    StringConstants.HTTP_CLIENT_NAME);

            // Не ждём, т.к. эта задача должна выполняется в фоне.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            RegisterGameAsync(httpClient, stoppingToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

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