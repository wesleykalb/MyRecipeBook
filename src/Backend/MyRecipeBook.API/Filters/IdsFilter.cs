using Microsoft.OpenApi.Models;
using MyRecipeBook.API.Binders;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyRecipeBook.API.Filters;

public class IdsFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var encriptedIds = context.ApiDescription
            .ParameterDescriptions
            .Where(x => x.ModelMetadata.BinderType == typeof(MyRecipeBookIdBinder))
            .ToDictionary(d => d.Name, d => d);
        
        foreach (var parameter in operation.Parameters)
        {
            if (encriptedIds.TryGetValue(parameter.Name, out var _))
            {
                parameter.Schema.Format = string.Empty;
                parameter.Schema.Type = "string";
            }
        }

        foreach (var schema in context.SchemaRepository.Schemas.Values)
        {
            foreach (var property in schema.Properties)
            {
                if (encriptedIds.TryGetValue(property.Key, out var _))
                {
                    property.Value.Format = string.Empty;
                    property.Value.Type = "string";
                }
            }
        }
    }
}