namespace Infrastructure.Middlewares;

using System.Text;

using Infrastructure.Exceptions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

/// <summary>
/// Промежуточный слой для обработки ошибок приложения.
/// </summary>
public class ExceptionsMiddleware
{
    /// <summary>
    /// Делегат следующей обработки запроса.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<ExceptionsMiddleware> _logger;

    /// <inheritdoc cref="ExceptionsMiddleware"/>
    /// <param name="logger">Логгер.</param>
    /// <param name="next">Делегат следующей обработки запроса.</param>
    public ExceptionsMiddleware(ILogger<ExceptionsMiddleware> logger,
        RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    /// <summary>
    /// Выполнить операцию.
    /// </summary>
    /// <param name="httpContext">Контекст запроса.</param>
    /// <returns>Задачу.</returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (HeadersException exception)
        {
            _logger.LogError(
                "При выполнении запроса произошла ошибка заголовков.");
            _logger.LogDebug(
                $"Ошибка заголовков: {exception}, трассировка: {exception.StackTrace}.");

            await WriteResponseAsync(httpContext,
                StatusCodes.Status500InternalServerError, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                "При выполнении запроса произошла непредвиденная ошибка.");
            _logger.LogDebug(
                $"Непредвиденная ошибка: {exception}, внутренняя ошибка: {exception.InnerException}.");

            await WriteResponseAsync(httpContext,
                StatusCodes.Status500InternalServerError, exception.Message);
        }
    }

    /// <summary>
    /// Записать ответ.
    /// </summary>
    /// <param name="httpContext">Контекст http запроса.</param>
    /// <param name="statusCode">Код статуса для ответа.</param>
    /// <param name="message">Сообщение для ответа.</param>
    /// <returns>Задачу.</returns>
    private async Task WriteResponseAsync(HttpContext httpContext,
        int statusCode, string message)
    {
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "text/plain";
        await httpContext.Response.WriteAsync(message, Encoding.UTF8);
    }
}