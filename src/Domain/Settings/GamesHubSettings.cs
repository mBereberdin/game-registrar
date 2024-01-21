namespace Domain.Settings;

/// <summary>
/// Настройки узла игр.
/// </summary>
public class GamesHubSettings
{
    /// <summary>
    /// Хост узла.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Конечная точка для регистрации игры в узле.
    /// </summary>
    public string? RegisterEndpoint { get; set; }
}