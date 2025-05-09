using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Schemathief.Core.Testing;
internal class SchemaDeltaBuilderTests
{
    private class NestedType
    {
        public bool Flag { get; set; }
    }

    private class DummyType
    {
        public string Name { get; set; } = string.Empty;
        public int? Count { get; set; }
        public List<double> Values { get; set; } = new List<double>();
        public NestedType Details { get; set; } = new NestedType();
    }

    [Test]
    public void BuildDeltaProperties_ExcludesBaseProps_AndGeneratesCorrectSchemaEntries()
    {
        var definedProps = typeof(DummyType)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToHashSet();

        // Exclude "Count" so that nullable int property is treated as base
        var baseProps = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Count" };

        var delta = SchemaDeltaBuilder.BuildDeltaProperties(definedProps, baseProps);

        // Assert keys: Name, Values, Details (camelCase)
        var keys = delta.Select(p => p.Key).ToHashSet();
        Assert.That(keys.SetEquals(new[] { "name", "values", "details" }), Is.True);

        // Check 'name' entry: simple string
        var nameEntry = delta["name"]!.AsObject();
        Assert.That(nameEntry["type"]!.ToString(), Is.EqualTo("string"));

        // Check 'values' entry: array of numbers
        var valuesEntry = delta["values"]!.AsObject();
        Assert.That(valuesEntry["type"]!.ToString(), Is.EqualTo("array"));
        var items = valuesEntry["items"]!.AsObject();
        Assert.That(items["type"]!.ToString(), Is.EqualTo("number"));

        // Check 'details' entry: object with nested 'flag' property
        var detailsEntry = delta["details"]!.AsObject();
        Assert.That(detailsEntry["type"]!.ToString(), Is.EqualTo("object"));
        var nestedProps = detailsEntry["properties"]!.AsObject();
        Assert.That(nestedProps.ContainsKey("flag"), Is.True);
        var flagEntry = nestedProps["flag"]!.AsObject();
        Assert.That(flagEntry["type"]!.ToString(), Is.EqualTo("boolean"));
    }

    [Test]
    public void BuildFinalSchema_CombinesBaseAndDelta_WithCorrectStructure()
    {
        var baseSchema = new JsonObject
        {
            ["title"] = "Original"
        };
        var deltaProps = new JsonObject
        {
            ["foo"] = new JsonObject { ["type"] = "string" }
        };
        var url = "https://example.com/schema.json";

        // Act
        var finalSchema = SchemaDeltaBuilder.BuildFinalSchema(baseSchema, deltaProps, url);

        // Assert top-level
        Assert.That(finalSchema["$schema"]!.ToString(), Is.EqualTo("http://json-schema.org/draft-07/schema#"));
        Assert.That(finalSchema["title"]!.ToString(), Is.EqualTo("Delta Original"));

        // Assert allOf array
        var allOf = finalSchema["allOf"]!.AsArray();
        Assert.That(allOf.Count, Is.EqualTo(2));

        // First element: $ref to url
        var refObj = allOf[0]!.AsObject();
        Assert.That(refObj["$ref"]!.ToString(), Is.EqualTo(url));

        // Second element: object with properties = deltaProps
        var obj2 = allOf[1]!.AsObject();
        Assert.That( obj2["type"]!.ToString(), Is.EqualTo("object"));
        var props = obj2["properties"]!.AsObject();
        Assert.That(props.ContainsKey("foo"), Is.True);
        Assert.That(props["foo"]!["type"]!.ToString(), Is.EqualTo("string"));
    }
}
