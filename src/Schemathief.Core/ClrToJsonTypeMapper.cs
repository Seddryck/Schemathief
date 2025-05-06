using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schemathief.Core;
internal static class ClrToJsonTypeMapper
{
    public static string Map(Type clrType)
    {
        clrType = Nullable.GetUnderlyingType(clrType) ?? clrType;
        if (clrType == typeof(string) || clrType == typeof(char)) return "string";
        if (clrType == typeof(bool)) return "boolean";
        if (clrType == typeof(int) || clrType == typeof(long)) return "integer";
        if (clrType == typeof(float) || clrType == typeof(double) || clrType == typeof(decimal)) return "number";
        return "object";
    }
}
