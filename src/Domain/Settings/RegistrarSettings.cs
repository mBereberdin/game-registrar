namespace Domain.Settings;

/// <summary>
/// Настройки регистратора.
/// </summary>
public class RegistrarSettings
{
    /// <summary>
    /// Наименование игры для регистрации.
    /// </summary>
    public string? GameName { get; set; }

    /// <summary>
    /// Время для регистрации игры в секундах.
    /// </summary>
    public int RegistrationTimeSeconds { get; set; } = 5;
}