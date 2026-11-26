using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;

namespace CityEscapeQuest.WebApi.IntegrationTests;

public sealed class HealthEndpointTests : IClassFixture<HealthEndpointTests.ApiFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(ApiFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public async Task LiveHealth_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/health/live");
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, $"HTTP {(int)response.StatusCode}: {responseBody}");
    }

    [Fact]
    public async Task AuthenticationRateLimit_RejectsExcessRequests()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            using var allowedResponse = await _client.PostAsJsonAsync("/api/auth/login", new { });
            Assert.NotEqual(HttpStatusCode.TooManyRequests, allowedResponse.StatusCode);
        }

        using var limitedResponse = await _client.PostAsJsonAsync("/api/auth/login", new { });
        Assert.Equal(HttpStatusCode.TooManyRequests, limitedResponse.StatusCode);
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
