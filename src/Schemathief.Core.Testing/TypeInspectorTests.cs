using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Schemathief.Core.Testing;
internal class TypeInspectorTests
{
    private class DummyType
    {
        public int A { get; set; }
        public string B { get; set; } = string.Empty;
        public bool ReadOnly { get; }
        public double WriteOnly { set { } }
    }

    [Test]
    public void GetDefinedProperties_ReturnsOnlyReadWriteProperties_ExcludingSpecified()
    {
        var excludes = new[] { "B" };
        var result = TypeInspector.GetDefinedProperties(typeof(DummyType), excludes);
        var names = result.Select(p => p.Name).ToHashSet(StringComparer.Ordinal);
        Assert.That(names, Is.EquivalentTo(new[] { "A" }));
    }

    [Test]
    public void GetDefinedProperties_ExclusionIsCaseInsensitive()
    {
        var excludes = new[] { "a" }; // lowercase 'a' should exclude property 'A'
        var result = TypeInspector.GetDefinedProperties(typeof(DummyType), excludes);
        var names = result.Select(p => p.Name).ToHashSet(StringComparer.Ordinal);
        Assert.That(names, Is.EquivalentTo(new[] { "B" }));
    }

    [Test]
    public void GetBaseProperties_ReturnsKeys_WhenPropertiesPresent()
    {
        var propsObj = new JsonObject
        {
            ["Foo"] = new JsonObject(),
            ["Bar"] = new JsonObject()
        };
        var baseSchema = new JsonObject
        {
            ["properties"] = propsObj
        };

        var result = TypeInspector.GetBaseProperties(baseSchema);
        Assert.That(result, Is.EquivalentTo(new[] { "Foo", "Bar" }));
    }

    [Test]
    public void GetBaseProperties_ReturnsEmpty_WhenPropertiesMissingOrNull()
    {
        // Case 1: "properties" absent
        var emptySchema = new JsonObject();
        var result1 = TypeInspector.GetBaseProperties(emptySchema);
        Assert.That(result1, Is.Empty);

        // Case 2: "properties" present but null
        var nullSchema = new JsonObject { ["properties"] = null };
        var result2 = TypeInspector.GetBaseProperties(nullSchema);
        Assert.That(result2, Is.Empty);
    }
}
