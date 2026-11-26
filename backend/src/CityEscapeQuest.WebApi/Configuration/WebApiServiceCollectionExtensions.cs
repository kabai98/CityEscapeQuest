using CityEscapeQuest.WebApi.Validation;
using FluentValidation;

namespace CityEscapeQuest.WebApi.Configuration;

public static class WebApiServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddValidatorsFromAssemblyContaining<ReportLocationRequestValidator>();
        services.AddProblemDetails();
        services.AddExceptionHandler<ApiExceptionHandler>();
        services.AddHealthChecks();
        services.AddFrontendCors(configuration);
        services.AddApiRateLimiting();
        services.AddJwtAuthentication(configuration);
        services.AddApiDocumentation();
        services.AddAuthorization();
        return services;
    }
}
