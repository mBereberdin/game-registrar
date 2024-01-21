namespace Infrastructure.Extensions.Settings;

using Domain.Settings;

using Serilog;

/// <summary>
/// Расширения для <see cref="RegistrarSettings"/>.
/// </summary>
public static class RegistrarSettingsExtensions
{
    /// <summary>
    /// Шаблон ошибки сообщения.
    /// </summary>
    private const string ERROR_MESSAGE_TEMPLATE =
        "Невозможно использовать настройки регистратора игр так как: {0}.";

    /// <summary>
    /// Валидны ли настройки.
    /// </summary>
    /// <returns>True - если настройки валидны, иначе - false.</returns>
    public static bool AreValid(this RegistrarSettings? settings)
    {
        var areValid = true;

        if (settings is null)
        {
            Log.Error(string.Format(ERROR_MESSAGE_TEMPLATE,
                "не указаны настройки регистратора"));

            return false;
        }

        if (string.IsNullOrWhiteSpace(settings.GameName))
        {
            Log.Error(string.Format(ERROR_MESSAGE_TEMPLATE,
                "не указано наименование игры"));

            areValid = false;
        }

        if (settings.RegistrationTimeSeconds <= 0)
        {
            Log.Error(string.Format(ERROR_MESSAGE_TEMPLATE,
                "время регистрации не может быть меньше или равно 0"));

            areValid = false;
        }

        return areValid;
    }
}