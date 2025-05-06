using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Schemathief.Core.Testing;
internal class ClrToJsonTypeMapperTests
{
    [TestCase(typeof(string), "string")]
    [TestCase(typeof(char), "string")]
    [TestCase(typeof(char?), "string")]
    public void Map_ReturnsString_ForStringOrCharTypes(Type clrType, string expected)
    {
        var result = ClrToJsonTypeMapper.Map(clrType);
        Assert.That(expected, Is.EqualTo(result));
    }

    [TestCase(typeof(bool), "boolean")]
    [TestCase(typeof(bool?), "boolean")]
    public void Map_ReturnsBoolean_ForBoolTypes(Type clrType, string expected)
    {
        var result = ClrToJsonTypeMapper.Map(clrType);
        Assert.That(expected, Is.EqualTo(result));
    }

    [TestCase(typeof(int), "integer")]
    [TestCase(typeof(long), "integer")]
    [TestCase(typeof(int?), "integer")]
    [TestCase(typeof(long?), "integer")]
    public void Map_ReturnsInteger_ForIntegralTypes(Type clrType, string expected)
    {
        var result = ClrToJsonTypeMapper.Map(clrType);
        Assert.That(expected, Is.EqualTo(result));
    }

    [TestCase(typeof(float), "number")]
    [TestCase(typeof(double), "number")]
    [TestCase(typeof(decimal), "number")]
    [TestCase(typeof(float?), "number")]
    [TestCase(typeof(double?), "number")]
    [TestCase(typeof(decimal?), "number")]
    public void Map_ReturnsNumber_ForFloatingPointTypes(Type clrType, string expected)
    {
        var result = ClrToJsonTypeMapper.Map(clrType);
        Assert.That(expected, Is.EqualTo(result));
    }

    [TestCase(typeof(DateTime), "object")]
    [TestCase(typeof(Guid), "object")]
    [TestCase(typeof(object), "object")]
    [TestCase(typeof(TestDummy), "object")]
    public void Map_ReturnsObject_ForOtherTypes(Type clrType, string expected)
    {
        var result = ClrToJsonTypeMapper.Map(clrType);
        Assert.That(expected, Is.EqualTo(result));
    }

    // Dummy type for testing default mapping
    private class TestDummy { }
}
