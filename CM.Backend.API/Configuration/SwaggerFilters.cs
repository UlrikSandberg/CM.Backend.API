using System;
using System.Collections.Generic;
using System.Linq;
using CM.Backend.API.ActionFilters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CM.Backend.API.Configuration
{

    public class ApplyDocumentExtensions : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Info.Extensions.TryAdd("x-logo", new {url = "/explore/cm-logo-white.png", backgroundColor = "#000" });
        }
    }

    public class AddSchemaExamples : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (model.Properties == null) return;
            
            foreach (var property in model.Properties)
            {
                if (property.Value.Format == "uuid")
                    property.Value.Example = Guid.Empty;
            }
        }
    }

    public class AddHeaderParameters : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            operation.Parameters?.Add(new NonBodyParameter
            {
                Description = "Correlate different callstacks",
                Name = "x-correlation-id",
                Type = "string",
                Required = false,
                In = "Header"
            });

            if (context.MethodInfo.GetCustomAttributes(true).OfType<ClientValidationFilter>().Any() || context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<ClientValidationFilter>().Any())
            {
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"client-id", new[] {""}}, 
                        {"client-secret", new[] {""}}
                    }
                };
                
                operation.Parameters?.Add(new NonBodyParameter
                {
                    Description = "Client ID",
                    Name = "x-client-id",
                    Type = "string",
                    Required = true,
                    In = "Header"
                });
            
                operation.Parameters?.Add(new NonBodyParameter
                {
                    Description = "Client secret",
                    Name = "x-client-secret",
                    Type = "string",
                    Required = true,
                    In = "Header"
                });
            }
        }
    }
}