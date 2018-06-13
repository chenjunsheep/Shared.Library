using Shared.Util;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Shared.Api.Swagger
{
    public class SchemaAdapter
    {
        #region Private Member

        private const BindingFlags REFLECTION_RULES = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
        private IDictionary<string, Schema> Dictionary { get; set; }

        #endregion

        #region Property

        private static SchemaAdapter DefaultSchemaAdapter
        {
            get { return new SchemaAdapter(); }
        }

        public bool EnableFullNamespace { get; set; } = true;

        public static Schema DefaultSchema
        {
            get
            {
                return new Schema() { Type = PropertyType.PROP_OBJECT };
            }
        }

        #endregion

        #region Constructor

        private SchemaAdapter() { }

        public SchemaAdapter(IDictionary<string, Schema> dictionary)
        {
            Dictionary = dictionary;
        }

        #endregion

        #region Public Method

        public string GetNamespance(Type typeOwner)
        {
            if (typeOwner != null)
            {
                return EnableFullNamespace ? 
                    TypeParser.GetStringValue($"{typeOwner.Namespace}.{typeOwner.Name}").Replace(".", string.Empty) : 
                    TypeParser.GetStringValue(typeOwner.Name);
            }
            return string.Empty;
        }

        public Schema Fill(Schema schemaOwner, Type typeOwner)
        {
            if (schemaOwner != null && typeOwner != null)
            {
                if (TryReuseDefinition(schemaOwner, typeOwner))
                {
                    return schemaOwner;
                }
                else
                {
                    if (!BuildBaseTypeSchema(schemaOwner, typeOwner))
                    {
                        var schemaRegistered = DefaultSchema;
                        if (TryRegisterDefinition(schemaRegistered, typeOwner))
                        {
                            schemaOwner.Ref = GetNamespance(typeOwner);
                            BuildCompositeTypeSchema(schemaRegistered, typeOwner);
                        }
                        else
                        {
                            BuildCompositeTypeSchema(schemaOwner, typeOwner);
                        }
                    }
                }
            }

            return schemaOwner;
        }

        public static void FillEnumExtension(Schema schemaOwner, Type typeOwner)
        {
            if (schemaOwner != null && typeOwner != null)
            {
                if (!schemaOwner.Extensions.ContainsKey(PropertyType.PROP_ENUM_MS))
                {
                    var propEnum = new List<object>();
                    foreach (int enumVal in Enum.GetValues(typeOwner))
                    {
                        var enumNm = Enum.GetName(typeOwner, enumVal);
                        propEnum.Add(new { name = enumNm, value = enumVal });
                    }

                    schemaOwner.Extensions.Add(PropertyType.PROP_ENUM_MS, new
                    {
                        name = DefaultSchemaAdapter.GetNamespance(typeOwner),
                        modelAsString = false,
                        values = propEnum.ToArray()
                    });
                }
            }
        }

        #endregion

        #region Private Method

        private bool TryReuseDefinition(Schema schemaOwner, Type typeOwner)
        {
            if (Dictionary != null && schemaOwner != null && typeOwner != null)
            {
                var key = GetNamespance(typeOwner);
                if (!string.IsNullOrEmpty(key) && Dictionary.ContainsKey(key))
                {
                    schemaOwner.Ref = key;
                    return true;
                }
            }

            return false;
        }

        private bool TryRegisterDefinition(Schema schemaOwner, Type typeOwner)
        {
            if (Dictionary != null && schemaOwner != null && typeOwner != null)
            {
                var key = GetNamespance(typeOwner);
                if (!string.IsNullOrEmpty(key) && !Dictionary.ContainsKey(key))
                {
                    Dictionary.Add(key, schemaOwner);
                    return true;
                }
            }

            return false;
        }

        private bool BuildBaseTypeSchema(Schema schemaOwner, Type typeOwner)
        {
            if (schemaOwner != null && typeOwner != null)
            {
                if (typeOwner.IsArray) //int[], Array[], Object[], MyClass[]...
                {
                    var arrayType = typeOwner.GetElementType();
                    if (typeof(byte).Equals(arrayType))
                    {
                        //byte[] and byte is in the same formatting schema in swagger
                        schemaOwner.Format = PropertyType.PROP_BYTE;
                        schemaOwner.Type = PropertyType.PROP_STRING;
                        return true;
                    }
                    else
                    {
                        schemaOwner.Type = PropertyType.PROP_ARRARY;
                        schemaOwner.Items = Fill(DefaultSchema, arrayType);
                    }
                    return true;
                }
                if (typeOwner.IsConstructedGenericType)
                {
                    if (typeOwner.IsValueType)
                    {
                        foreach (var argType in typeOwner.GenericTypeArguments)
                        {
                            return BuildBaseTypeSchema(schemaOwner, argType);
                        }
                    }
                    else
                    {
                        var argTypes = typeOwner.GenericTypeArguments;
                        var argsCount = argTypes.Count();
                        if (argsCount > 0)
                        {
                            if (argsCount > 1) //Dictionary<S, T>
                            {
                                schemaOwner.Type = PropertyType.PROP_OBJECT;
                                schemaOwner.AdditionalProperties = Fill(DefaultSchema, argTypes[argsCount - 1]);
                                return true;
                            }
                            if (argsCount == 1)
                            {
                                if (!typeOwner.IsSecurityCritical) //List<any>
                                {
                                    schemaOwner.Type = PropertyType.PROP_ARRARY;
                                    foreach (var typeMember in argTypes)
                                    {
                                        schemaOwner.Items = Fill(DefaultSchema, typeMember);
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (typeOwner.IsTypeDefinition)
                    {
                        if (typeOwner.IsValueType)
                        {
                            if (typeOwner.IsEnum)
                            {
                                return false;
                            }

                            if (typeof(int).Equals(typeOwner))
                            {
                                schemaOwner.Format = PropertyType.PROP_INT32;
                                schemaOwner.Type = PropertyType.PROP_INTEGER;
                                return true;
                            }

                            if (typeof(bool).Equals(typeOwner))
                            {
                                schemaOwner.Type = PropertyType.PROP_BOOLEAN;
                                return true;
                            }

                            if (typeof(long).Equals(typeOwner))
                            {
                                schemaOwner.Format = PropertyType.PROP_INT64;
                                schemaOwner.Type = PropertyType.PROP_INTEGER;
                                return true;
                            }

                            if (typeof(float).Equals(typeOwner))
                            {
                                schemaOwner.Format = PropertyType.PROP_FLOAT;
                                schemaOwner.Type = PropertyType.PROP_NUMBER;
                                return true;
                            }

                            if (typeof(double).Equals(typeOwner))
                            {
                                schemaOwner.Format = PropertyType.PROP_DOUBLE;
                                schemaOwner.Type = PropertyType.PROP_NUMBER;
                                return true;
                            }

                            if (typeof(byte).Equals(typeOwner))
                            {
                                schemaOwner.Format = PropertyType.PROP_BYTE;
                                schemaOwner.Type = PropertyType.PROP_STRING;
                                return true;
                            }

                            if (typeof(DateTime).Equals(typeOwner))
                            {
                                schemaOwner.Format = PropertyType.PROP_DATETIME;
                                schemaOwner.Type = PropertyType.PROP_STRING;
                                return true;
                            }
                        }
                        else
                        {
                            if (typeOwner.IsInterface || typeOwner.IsSecurityCritical)
                            {
                                return false;
                            }

                            if (typeof(string).Equals(typeOwner))
                            {
                                schemaOwner.Type = typeOwner.Name.ToLower();
                                return true;
                            }
                            if (typeof(Array).Equals(typeOwner))
                            {
                                schemaOwner.Type = PropertyType.PROP_OBJECT; //TBD
                                return true;
                            }
                            if (typeof(Stream).Equals(typeOwner))
                            {
                                schemaOwner.Type = PropertyType.PROP_BINARY;
                                return true;
                            }
                            if (typeof(object).Equals(typeOwner))
                            {
                                schemaOwner.Type = PropertyType.PROP_OBJECT;
                                return true;
                            }
                        }

                    }
                }
            }

            return false;
        }

        private bool BuildCompositeTypeSchema(Schema schemaOwner, Type typeOwner)
        {
            if (schemaOwner != null && typeOwner != null)
            {
                if (typeOwner.IsEnum)
                {
                    schemaOwner.Type = PropertyType.PROP_STRING;
                    schemaOwner.Enum = Enum.GetNames(typeOwner).OfType<object>().ToList();
                    FillEnumExtension(schemaOwner, typeOwner);

                    return true;
                }

                if (typeOwner.IsGenericTypeDefinition) //MyClass<>
                {
                    schemaOwner.Type = PropertyType.PROP_OBJECT;
                    return true;
                }

                if (typeOwner.GenericTypeArguments.Count() == 1)
                {
                    if (typeOwner.IsSecurityCritical) //MyClass<any>
                    {
                        schemaOwner.Type = PropertyType.PROP_OBJECT;
                        schemaOwner.Properties = new Dictionary<string, Schema>();
                        foreach (var propSub in typeOwner.GetProperties(REFLECTION_RULES))
                        {
                            schemaOwner.Properties.Add(propSub.Name, Fill(DefaultSchema, propSub.PropertyType));
                        }
                        return true;
                    }
                }

                if (typeOwner.IsClass || typeOwner.IsInterface) //MyClass or IMyInterface
                {
                    schemaOwner.Type = PropertyType.PROP_OBJECT;
                    schemaOwner.Properties = new Dictionary<string, Schema>();
                    foreach (var propSub in typeOwner.GetProperties(REFLECTION_RULES))
                    {
                        schemaOwner.Properties.Add(propSub.Name, Fill(DefaultSchema, propSub.PropertyType));
                    }
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
