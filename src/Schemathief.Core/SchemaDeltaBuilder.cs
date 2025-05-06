using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Schemathief.Core;
internal static class SchemaDeltaBuilder
{
    public static JsonObject BuildDeltaProperties(
        ISet<PropertyInfo> definedProps,
        ISet<string> baseProps)
    {
        var deltaProps = new JsonObject();

        foreach (var prop in definedProps)
        {
            if (baseProps.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
                continue;

            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            JsonObject schemaEntry;

            if (TypeInspectorHelper.IsEnumerable(propType, out var itemType))
            {
                var itemsSchema = TypeInspectorHelper.IsSimpleType(itemType)
                    ? new JsonObject { ["type"] = ClrToJsonTypeMapper.Map(itemType!) }
                    : new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = GenerateNestedProperties(itemType!)
                    };

                schemaEntry = new JsonObject
                {
                    ["type"] = "array",
                    ["items"] = itemsSchema
                };
            }
            else if (!TypeInspectorHelper.IsSimpleType(propType))
            {
                schemaEntry = new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = GenerateNestedProperties(propType)
                };
            }
            else
            {
                schemaEntry = new JsonObject
                {
                    ["type"] = ClrToJsonTypeMapper.Map(propType)
                };
            }

            deltaProps[prop.Name.ToCamelCase()!] = schemaEntry;
        }

        return deltaProps;
    }

    public static JsonObject BuildFinalSchema(
        JsonObject baseSchema,
        JsonObject deltaProps,
        string baseSchemaUrl)
    {
        var baseTitle = baseSchema["title"]?.ToString() ?? "Base Schema";
        return new JsonObject
        {
            ["$schema"] = "http://json-schema.org/draft-07/schema#",
            ["title"] = $"Delta {baseTitle}",
            ["allOf"] = new JsonArray
                {
                    new JsonObject { ["$ref"] = baseSchemaUrl },
                    new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = deltaProps
                    }
                }
        };
    }

    private static JsonObject GenerateNestedProperties(Type nestedType)
    {
        var nestedProps = new JsonObject();

        foreach (var nestedProp in nestedType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propType = Nullable.GetUnderlyingType(nestedProp.PropertyType) ?? nestedProp.PropertyType;

            JsonObject schemaEntry;
            if (TypeInspectorHelper.IsEnumerable(propType, out var itemType))
            {
                schemaEntry = new JsonObject
                {
                    ["type"] = "array",
                    ["items"] = TypeInspectorHelper.IsSimpleType(itemType!)
                        ? new JsonObject { ["type"] = ClrToJsonTypeMapper.Map(itemType!) }
                        : new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = GenerateNestedProperties(itemType!)
                        }
                };
            }
            else if (!TypeInspectorHelper.IsSimpleType(propType))
            {
                schemaEntry = new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = GenerateNestedProperties(propType)
                };
            }
            else
            {
                schemaEntry = new JsonObject { ["type"] = ClrToJsonTypeMapper.Map(propType) };
            }

            nestedProps[nestedProp.Name.ToCamelCase()!] = schemaEntry;
        }

        return nestedProps;
    }
}
