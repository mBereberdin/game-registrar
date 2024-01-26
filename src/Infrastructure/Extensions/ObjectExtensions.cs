namespace Infrastructure.Extensions;

using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

using Domain.Constants;

using Serilog;

/// <summary>
/// Расширения объектов.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Привести объект к строковому содержимому в формате JSON.
    /// </summary>
    /// <param name="object">Объект, который необходимо привести к строковому содержимому в формате JSON.</param>
    /// <returns>Объект, приведенный к строковому содержимому в формате JSON.</returns>
    public static StringContent ToJsonStringContent(this object @object)
    {
        Log.Information(
            "Приведение объекта к строковому содержимому в формате JSON.");
        Log.Debug("Объект: {object}", @object);

        var jsonObject = JsonSerializer.Serialize(@object) ??
                         throw new SerializationException(
                             "Не удалось сериализовать объект в json формат.");

        var jsonObjectContent = new StringContent(jsonObject, Encoding.UTF8,
            StringConstants.CONTENT_TYPE);

        Log.Information(
            "Приведение объекта к строковому содержимому в формате JSON - завершено.");
        Log.Debug("Объект: {object}", @object);

        return jsonObjectContent;
    }
}