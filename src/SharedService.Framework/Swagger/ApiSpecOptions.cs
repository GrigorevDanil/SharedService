namespace SharedService.Framework.Swagger;

/// <summary>
/// Представляет параметры описания API, используемые для конфигурации Swagger документации.
/// </summary>
public class ApiDescriptionOptions
{
    /// <summary>
    /// Заголовок API.
    /// </summary>
    public string Title { get; set; } = "My API";

    /// <summary>
    /// Описание для сервиса.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Контактная информация.
    /// </summary>
    public ApiContactOptions Contact { get; set; } = new();

    /// <summary>
    /// Описание ендпоинтов для Swagger.
    /// </summary>
    public ApiEndpointOptions[] Endpoints { get; set; } = [];
}

/// <summary>
/// Представляет контактную информацию для Swagger.
/// </summary>
public class ApiContactOptions
{
    /// <summary>
    /// Имя контактного лица или команды.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Электронная почта.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// URL контакта.
    /// </summary>
    public string? Url { get; set; }
}

/// <summary>
/// Представляет контактную информацию для Swagger.
/// </summary>
public class ApiEndpointOptions
{
    /// <summary>
    /// Версии API.
    /// </summary>
    public string Version { get; set; } = "1";

    /// <summary>
    /// Добавлять схему безопасности для Swagger документации.
    /// </summary>
    public bool Authorization { get; set; }
}