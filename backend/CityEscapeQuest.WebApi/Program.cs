using CityEscapeQuest.Application;
using CityEscapeQuest.Infrastructure;
using CityEscapeQuest.WebApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebApiServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("frontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/live");

if (!app.Environment.IsEnvironment("Testing"))
{
    await app.SeedApplicationDataAsync();
}

await app.RunAsync();

public partial class Program;
