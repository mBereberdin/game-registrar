using Infrastructure.Extensions;

using Serilog;

using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
// Настройка сервисов приложения.

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

Log.Logger.Information("Инициализация конфигурации.");

var isUseSwagger = builder.Configuration.GetValue<bool>("IsUseSwagger");
if (isUseSwagger)
{
    builder.Services.AddSwaggerGeneration();
}

builder.Services.AddControllers();
builder.Services.AddServices();

Log.Logger.Information("Конфигурация была проинициализирована.");

var app = builder.Build();
// Настройка приложения.

Log.Logger.Information("Инициализация приложения.");

if (isUseSwagger)
{
    app.AddAndConfigureSwagger();
}

app.AddAppMiddlewares();

app.MapControllers();

Log.Logger.Information("Приложение было проинициализировано.");

app.Run();