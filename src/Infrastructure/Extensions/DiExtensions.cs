namespace Infrastructure.Extensions;

using Domain.Settings;

using Infrastructure.Services.Implementations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

/// <summary>
/// Расширения для DI.
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Добавить регистрацию игры в узле игр.
    /// </summary>
    /// <param name="services">Сервисы приложения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddGamesHubRegistration(this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Logger.Information(
            "Добавление сервисов и настроек для регистрации игры в узле игр.");

        // GamesHubService регистрируется как singleton, в данном случае клиент регистрируется для этого сервиса, чтобы клиент жил нужное время.
        // https://github.com/dotnet/runtime/issues/80303
        services.AddHttpClient("GamesHub");
        services.AddHostedService<GamesHubService>();

        services.Configure<GamesHubSettings>(
            configuration.GetSection(nameof(GamesHubSettings)));

        services.Configure<RegistrarSettings>(
            configuration.GetSection(nameof(RegistrarSettings)));

        Log.Logger.Information(
            "Сервисы и настройки для регистрации игры в узле игр - добавлены.");
    }
}