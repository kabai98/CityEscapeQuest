using Microsoft.OpenApi.Models;

namespace CityEscapeQuest.WebApi.Configuration;

internal static class OpenApiConfiguration
{
    internal static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "City Escape Quest API", Version = "v1" });
            options.SupportNonNullableReferenceTypes();
            options.SchemaFilter<RequiredContractPropertiesSchemaFilter>();
            options.CustomOperationIds(api =>
            {
                api.ActionDescriptor.RouteValues.TryGetValue("controller", out var controller);
                api.ActionDescriptor.RouteValues.TryGetValue("action", out var action);
                return controller is null || action is null ? null : $"{controller}{action}";
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                }] = []
            });
        });
        return services;
    }
}
