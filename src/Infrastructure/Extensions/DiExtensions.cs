namespace Infrastructure.Extensions;

using Infrastructure.Middlewares;
using Infrastructure.Services.Implementations;
using Infrastructure.Services.Interfaces;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

/// <summary>
/// Расширения для DI.
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Добавить промежуточные слои приложения.
    /// </summary>
    /// <param name="builder">Строитель приложения.</param>
    public static void AddAppMiddlewares(this IApplicationBuilder builder)
    {
        Log.Logger.Information("Добавление промежуточных слоев приложения.");

        builder.UseMiddleware<ExceptionsMiddleware>();

        Log.Logger.Information("Промежуточные слои приложения добавлены.");
    }

    /// <summary>
    /// Добавить сервисы.
    /// </summary>
    /// <param name="services">Сервисы приложения.</param>
    public static void AddServices(this IServiceCollection services)
    {
        Log.Logger.Information("Добавление сервисов.");

        // Singleton в данном примере используется для проверки запросов. Конкретно - для получения сущности по id после ее добавления.
        services.AddSingleton<ITestEntitesService, TestEntitesService>();

        Log.Logger.Information("Сервисы добавлены.");
    }
}