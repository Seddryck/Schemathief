using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Schemathief.Core;
internal static class TypeInspector
{
    public static ISet<PropertyInfo> GetDefinedProperties(Type type, string[] excludes)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   .Where(p => p.CanRead && p.CanWrite)
                   .Where(p => !excludes.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                   .ToHashSet();
    }

    public static ISet<string> GetBaseProperties(JsonObject baseSchema)
    {
        return baseSchema["properties"]
                         ?.AsObject()
                         .Select(p => p.Key)
                         .ToHashSet() ?? new HashSet<string>();
    }
}
