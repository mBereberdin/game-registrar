namespace Infrastructure.Extensions.Settings;

using Domain.Settings;

using Serilog;

/// <summary>
/// Расширения для <see cref="GamesHubSettings"/>.
/// </summary>
public static class GamesHubSettingsExtensions
{
    /// <summary>
    /// Шаблон ошибки сообщения.
    /// </summary>
    private const string ERROR_MESSAGE_TEMPLATE =
        "Невозможно использовать настройки узла игр так как: {0}.";

    /// <summary>
    /// Валидны ли настройки.
    /// </summary>
    /// <returns>True - если настройки валидны, иначе - false.</returns>
    public static bool AreValid(this GamesHubSettings? settings)
    {
        var areValid = true;

        if (settings is null)
        {
            Log.Error(string.Format(ERROR_MESSAGE_TEMPLATE,
                "не указаны настройки узла игр"));

            return false;
        }

        if (string.IsNullOrWhiteSpace(settings.Host))
        {
            Log.Error(string.Format(ERROR_MESSAGE_TEMPLATE,
                "не указан параметр \"хост узла\""));

            areValid = false;
        }

        if (string.IsNullOrWhiteSpace(settings.RegisterEndpoint))
        {
            Log.Error(string.Format(ERROR_MESSAGE_TEMPLATE,
                "не указана конечная точка узла для регистрации игры"));

            areValid = false;
        }

        return areValid;
    }
}