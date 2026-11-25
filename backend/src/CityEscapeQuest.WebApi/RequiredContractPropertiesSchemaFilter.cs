using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CityEscapeQuest.WebApi;

public sealed class RequiredContractPropertiesSchemaFilter : ISchemaFilter
{
    private static readonly NullabilityInfoContext Nullability = new();

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var property in context.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var name = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                ?? JsonNamingPolicy.CamelCase.ConvertName(property.Name);
            if (!schema.Properties.ContainsKey(name))
            {
                continue;
            }

            var isRequired = property.PropertyType.IsValueType
                ? Nullable.GetUnderlyingType(property.PropertyType) is null
                : Nullability.Create(property).ReadState == NullabilityState.NotNull;
            if (isRequired)
            {
                schema.Required.Add(name);
            }
        }
    }
}
