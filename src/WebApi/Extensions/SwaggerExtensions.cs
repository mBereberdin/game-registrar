namespace WebApi.Extensions;

using System.Reflection;

using Infrastructure.Exceptions;

using Microsoft.OpenApi.Models;

using Serilog;

/// <summary>
/// Расширения swagger.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Хост запроса.
    /// </summary>
    private const string HOST = "X-Forwarded-Host";

    /// <summary>
    /// Префикс сервиса.
    /// </summary>
    private const string SERVICE_PREFIX = "X-Forwarded-Prefix";

    /// <summary>
    /// Добавить и настроить swagger.
    /// </summary>
    /// <param name="app">Веб приложение используемое для конфигурирования http пайплайна и маршрутов.</param>
    public static void AddAndConfigureSwagger(this WebApplication app)
    {
        Log.Logger.Information("Добавление и конфигурирование Swagger.");

        var isDevelopment = app.Environment.IsDevelopment();
        app.UseSwagger(options =>
        {
            if (!isDevelopment)
            {
                // Фикс прокси nginx.
                options.PreSerializeFilters.Add(
                    (openApiDocument, httpRequest) =>
                    {
                        Log.Logger.Information(
                            "Добавление фильтра перед сериализацией.");

                        ThrowIfInvalidHeaders(httpRequest.Headers);

                        var servicePrefix = httpRequest.Headers[SERVICE_PREFIX];
                        var host = httpRequest.Headers[HOST];
                        var requestScheme = httpRequest.Scheme;
                        var serverUrl =
                            $"{requestScheme}://{host}/{servicePrefix}/";

                        Log.Logger.Debug(
                            $"Для конфигурирования swagger получено значение заголовка {nameof(SERVICE_PREFIX)}: {servicePrefix}.");
                        Log.Logger.Debug(
                            $"Для конфигурирования swagger получено значение заголовка {nameof(HOST)}: {host}.");
                        Log.Logger.Debug(
                            $"Для конфигурирования swagger получено значение схемы запроса: {requestScheme}.");
                        Log.Logger.Debug(
                            $"Для конфигурирования swagger сформирован адрес сервера: {serverUrl}.");

                        openApiDocument.Servers = new List<OpenApiServer>
                            {new() {Url = serverUrl}};
                    });
                Log.Logger.Information("Добавлен фильтр перед сериализацией.");
            }

            Log.Logger.Information("Swagger добавлен и сконфигурирован.");
        });

        Log.Logger.Information("Добавление и конфигурирование Swagger UI.");

        app.UseSwaggerUI(options =>
        {
            const string url = "v1/swagger.json";
            const string name = "v1";
            options.SwaggerEndpoint(url, name);
            Log.Logger.Debug(
                $"Для Swagger UI добавлена конечная точка с адресом: {url}, и наименованием: {name}.");
        });
        Log.Logger.Information("Swagger UI добавлен и сконфигурирован.");
    }

    /// <summary>
    /// Выкинуть исключение если заголовки запроса не валидные.
    /// </summary>
    /// <param name="headers">Заголовки запроса.</param>
    /// <exception cref="HeadersException">Когда заголовки запроса не валидные.</exception>
    private static void ThrowIfInvalidHeaders(IHeaderDictionary headers)
    {
        Log.Logger.Information(
            "Проверка заголовков запроса для добавления Swagger.");
        if (!headers.ContainsKey(HOST))
        {
            Log.Logger.Error(
                "Не удалось найти заголовок запроса: {host}. Заголовки запроса: {headers}",
                HOST, headers);

            throw new HeadersException(
                $"Не удалось найти заголовок запроса: {nameof(HOST)}.");
        }

        if (!headers.ContainsKey(SERVICE_PREFIX))
        {
            Log.Logger.Error(
                "Не удалось найти заголовок запроса {service_prefix}. Заголовки запроса: {headers}",
                SERVICE_PREFIX, headers);

            throw new HeadersException(
                $"Не удалось найти заголовок запроса: {nameof(SERVICE_PREFIX)}.");
        }

        Log.Logger.Information(
            "Заголовки запроса для добавления Swagger проверены и являются корректными.");
    }

    /// <summary>
    /// Добавить генерацию swagger.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов приложения.</param>
    public static void AddSwaggerGeneration(
        this IServiceCollection serviceCollection)
    {
        Log.Logger.Information("Добавление генерации swagger.");

        serviceCollection.AddSwaggerGen(options =>
        {
            Log.Logger.Information("Формирование swagger документа.");

            const string swaggerDocName = "v1";
            var openApiInfo = new OpenApiInfo
            {
                Version = "v1",
                Title = "WebApi",
                Description = "ASP.NET Core Web API для проверки работы WebApi."
            };
            options.SwaggerDoc(swaggerDocName, openApiInfo);

            Log.Logger.Information("Swagger документ сформирован.");
            Log.Logger.Debug("Наименование документа: {title}.",
                openApiInfo.Title);

            var xmlFilename =
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            Log.Logger.Debug(
                "Сформировано наименование xml файла с описанием swagger: {xmlFilename}",
                xmlFilename);


            var xmlCommentsFilePath =
                Path.Combine(AppContext.BaseDirectory, xmlFilename);

            options.IncludeXmlComments(xmlCommentsFilePath);
            Log.Logger.Debug(
                "Сформирован путь xml файла с описанием swagger: {xmlCommentsFilePath}",
                xmlCommentsFilePath);
        });
        Log.Logger.Information("Генерация swagger добавлена.");
    }
}