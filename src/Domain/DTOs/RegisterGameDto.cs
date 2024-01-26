namespace Domain.DTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// ДТО регистрации игры.
/// </summary>
public record RegisterGameDto
{
    /// <summary>
    /// Наименование игры.
    /// </summary>
    [Required(AllowEmptyStrings = false,
        ErrorMessage =
            "Наименование игры не должно быть пустым или состоять только из пробелов.")]
    public required string Name { get; init; }

    /// <summary>
    /// Изображение-предпросмотр игры.
    /// </summary>
    public string? PreviewPicture { get; init; }
}