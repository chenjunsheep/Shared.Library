using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Shared.Util.Extension;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Shared.Api.Swagger.Filter
{
    public class DocumentFilter : IDocumentFilter
    {
        private static List<string> AssemblyPaths { get; set; } = new List<string>();
        private static List<Type> TypeList { get; set; } = new List<Type>();

        internal static void Register(string assemblyPath)
        {
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                AssemblyPaths.Add(assemblyPath);
            }
        }

        internal static void Register(Type type)
        {
            if (type != null)
            {
                TypeList.Add(type);
            }
        }

        private bool IsAvailable(Type type)
        {
            return type != null && !type.IsAnonymousType();
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var adapter = new SchemaAdapter(swaggerDoc.Definitions);
            foreach (var assemblyPath in AssemblyPaths)
            {
                if (!string.IsNullOrEmpty(assemblyPath))
                {
                    try
                    {
                        var target = Assembly.LoadFile(assemblyPath);
                        foreach (var type in target.GetTypes())
                        {
                            if (IsAvailable(type))
                            {
                                try
                                {
                                    adapter.Fill(SchemaAdapter.DefaultSchema, type);
                                }
                                catch (Exception)
                                {
                                    //ignore exception
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //ignore exception
                    }
                }
            }

            foreach (var type in TypeList)
            {
                if (type != null)
                {
                    try
                    {
                        if (IsAvailable(type))
                        {
                            adapter.Fill(SchemaAdapter.DefaultSchema, type);
                        }
                    }
                    catch (Exception)
                    {
                        //ignore exception
                    }
                }
            }

            swaggerDoc.Definitions = swaggerDoc.Definitions.OrderBy(o => o.Key).ToDictionary(c => c.Key, c => c.Value);
        }
    }
}