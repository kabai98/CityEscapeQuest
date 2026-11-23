using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace CityEscapeQuest.WebApi.IntegrationTests;

public sealed class HealthEndpointTests : IClassFixture<HealthEndpointTests.ApiFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LiveHealth_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/health/live");
        response.EnsureSuccessStatusCode();
    }

    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:SigningKey"] = "TEST_ONLY_SIGNING_KEY_WITH_AT_LEAST_32_CHARS",
                    ["Jwt:Issuer"] = "CityEscapeQuest.Tests",
                    ["Jwt:Audience"] = "CityEscapeQuest.Tests"
                });
            });
        }
    }
}