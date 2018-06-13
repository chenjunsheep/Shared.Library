using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Shared.Api.Swagger.Filter
{
    public class SchemaFilter : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (context == null)
                throw new ArgumentNullException("context");

            if (context.SystemType.IsEnum) {
                SchemaAdapter.FillEnumExtension(model, context.SystemType);
            }
        }
    }
}
