using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schemathief.Core;
internal static class TypeInspectorHelper
{
    public static bool IsSimpleType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(DateTime)
               || type == typeof(Guid);
    }

    public static bool IsEnumerable(Type type, [NotNullWhen(true)] out Type? itemType)
    {
        itemType = null;
        if (type == typeof(string)) return false;
        if (type.IsArray)
        {
            itemType = type.GetElementType()!;
            return true;
        }
        if (type.IsGenericType)
        {
            var genDef = type.GetGenericTypeDefinition();
            if (genDef == typeof(List<>) || genDef == typeof(IEnumerable<>))
            {
                itemType = type.GetGenericArguments().First();
                return true;
            }
        }
        var enumerableInterface = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if (enumerableInterface != null)
        {
            itemType = enumerableInterface.GetGenericArguments().First();
            return true;
        }
        return false;
    }
}
