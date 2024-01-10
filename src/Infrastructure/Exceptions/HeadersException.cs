namespace Infrastructure.Exceptions;

/// <summary>
/// Ошибка заголовков.
/// </summary>
public class HeadersException : Exception
{
    /// <inheritdoc cref="HeadersException"/>
    /// <param name="message">Сообщение ошибки.</param>
    public HeadersException(string message) : base(message)
    {
    }
}