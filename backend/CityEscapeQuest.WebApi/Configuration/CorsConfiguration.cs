namespace CityEscapeQuest.WebApi.Configuration;

internal static class CorsConfiguration
{
    internal static IServiceCollection AddFrontendCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var frontendOrigins = configuration.GetSection("Frontend:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options =>
        {
            options.AddPolicy("frontend", policy =>
            {
                if (frontendOrigins.Length > 0)
                {
                    policy.WithOrigins(frontendOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });
        return services;
    }
}
