using Shared.Api.Swagger.Filter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace Shared.Api.Swagger
{
    public class SwaggerManager
    {
        public SwaggerManager Register(string assemblyPath)
        {
            DocumentFilter.Register(assemblyPath);
            return this;
        }

        public SwaggerManager Register(Type type)
        {
            DocumentFilter.Register(type);
            return this;
        }

        public static SwaggerManager Instance { get; } = new SwaggerManager();
    }

    public static class SwaggerExtension
    {
        public static void UseSwaggerUIDefault(this SwaggerUIOptions ops)
        {
            if (ops != null)
            {
                ops.RoutePrefix = "apidocs";
                ops.DocExpansion(DocExpansion.None);
            }
        }

        public static IServiceCollection UseSwaggerDefault(this IServiceCollection services, string filePathXmlComments)
        {
            if (services == null) return services;

            services.UseSwaggerSettingsDefault();
            services.UseSwaggerFilterDefault();
            services.UseSwaggerCommentsDefault(filePathXmlComments);
            services.UseSwaggerAuthDefault();
            return services;
        }

        #region Private Method

        private static IServiceCollection UseSwaggerSettingsDefault(this IServiceCollection services)
        {
            if (services == null) return services;

            services.AddSwaggerGen(c =>
            {
                c.DescribeAllEnumsAsStrings();
                c.UseReferencedDefinitionsForEnums();
                c.IgnoreObsoleteActions();
                c.OrderActionsBy(s => s.ActionDescriptor.AttributeRouteInfo.Template);
            });
            return services;
        }

        private static IServiceCollection UseSwaggerFilterDefault(this IServiceCollection services)
        {
            if (services == null) return services;

            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<SchemaFilter>();
                c.DocumentFilter<DocumentFilter>();
            });

            return services;
        }

        private static IServiceCollection UseSwaggerCommentsDefault(this IServiceCollection services, string filePathXmlComments)
        {
            if (services == null) return services;

            if (!string.IsNullOrEmpty(filePathXmlComments))
            {
                services.AddSwaggerGen(c =>
                {
                    c.IncludeXmlComments(filePathXmlComments);
                });
            }
            return services;
        }

        private static IServiceCollection UseSwaggerAuthDefault(this IServiceCollection services)
        {
            if (services == null) return services;

            services.AddSwaggerGen(c =>
            {
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
            });

            return services;
        }

        #endregion
    }
}
