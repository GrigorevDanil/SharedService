using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SharedService.Framework.Swagger;

/// <summary>
/// Предоставляет методы расширения для настройки Swagger с использованием параметров из конфигурации.
/// </summary>
public static class OpenApiExtensions
{
    public static IApplicationBuilder UseCustomSwaggerUI(this IApplicationBuilder app, IConfiguration configuration, Action<SwaggerUIOptions>? setupAction)
    {
        ApiDescriptionOptions apiOptions = configuration.GetSection("Swagger").Get<ApiDescriptionOptions>() ?? new ApiDescriptionOptions();

        return app.UseSwaggerUI(options =>
        {
            foreach (string version in apiOptions.Versions)
            {
                options.SwaggerEndpoint(
                    $"/openapi/v{version}.json",
                    $"{apiOptions.Title} API v{version}");
            }

            setupAction?.Invoke(options);
        });
    }

   /// <summary>
   /// Регистрирует Swagger документацию в коллекции сервисов, привязывая параметры API из конфигурации.
   /// </summary>
   /// <param name="services">Коллекция сервисов для регистрации Swagger.</param>
   /// <param name="configuration">Конфигурация приложения, содержащая секцию "Swagger".</param>
   /// <returns>Модифицированная коллекция сервисов с добавленным Swagger.</returns>
    public static IServiceCollection AddCustomOpenApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ApiDescriptionOptions apiOptions = configuration.GetSection("Swagger").Get<ApiDescriptionOptions>() ?? new ApiDescriptionOptions();

        foreach (string version in apiOptions.Versions)
        {
            services.AddOpenApi("v" + version,  options =>
            {
                options.AddOpenApiInfo(apiOptions, version);

                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });
        }

        return services;
    }

    private static OpenApiOptions AddOpenApiInfo(this OpenApiOptions options, ApiDescriptionOptions apiOptions, string version)
    {
        return options.AddDocumentTransformer((doc, _, __) =>
        {
            doc.Info = new OpenApiInfo
            {
                Title = apiOptions.Title,
                Version = version,
                Description = apiOptions.Description,
                Contact = new OpenApiContact
                {
                    Name = apiOptions.Contact.Name,
                    Email = apiOptions.Contact.Email,
                    Url = apiOptions.Contact.Url is not null ? new Uri(apiOptions.Contact.Url) : null
                }
            };

            return Task.CompletedTask;
        });
    }
}

public sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        IEnumerable<AuthenticationScheme> authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token",
                    Description = "Введите JWT токен в формате: Bearer {token}"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = securitySchemes;

            foreach (KeyValuePair<HttpMethod, OpenApiOperation> operation in document.Paths.Values.SelectMany(path => path.Operations ?? []))
            {
                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        }
    }
}