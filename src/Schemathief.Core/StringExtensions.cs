using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schemathief.Core;
public static class StringExtensions
{
    public static string? ToPascalCase(this string? str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        var sb = new StringBuilder();
        bool toUpper = true;
        foreach (char c in str)
        {
            if (c == '_')
            {
                toUpper = true;
                continue;
            }
            sb.Append(toUpper ? char.ToUpper(c) : c);
            toUpper = false;
        }
        return sb.ToString();
    }

    public static string? ToCamelCase(this string? str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        var sb = new StringBuilder();
        bool toUpper = false;
        bool firstChar = true;
        foreach (char c in str)
        {
            if (c == '_')
            {
                toUpper = !firstChar;
                continue;
            }
            sb.Append(toUpper ? char.ToUpper(c) : firstChar ? char.ToLower(c) : c);
            toUpper = false;
            firstChar = false;
        }
        return sb.ToString();
    }
}
