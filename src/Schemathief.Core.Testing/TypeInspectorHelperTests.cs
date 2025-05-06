using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Schemathief.Core.Testing;

internal class TypeInspectorHelperTests
{
    [TestCase(typeof(int))]
    [TestCase(typeof(double))]
    [TestCase(typeof(bool))]
    [TestCase(typeof(char))]
    [TestCase(typeof(decimal))]
    [TestCase(typeof(string))]
    [TestCase(typeof(DateTime))]
    [TestCase(typeof(Guid))]
    [TestCase(typeof(int?))]
    [TestCase(typeof(DateTime?))]
    [TestCase(typeof(Guid?))]
    public void IsSimpleType_ReturnsTrue_ForSimpleTypes(Type type)
        => Assert.That(TypeInspectorHelper.IsSimpleType(type), Is.True, $"Expected {type.Name} to be simple type");

    [TestCase(typeof(object))]
    [TestCase(typeof(Uri))]
    [TestCase(typeof(List<int>))]
    [TestCase(typeof(int[]))]
    [TestCase(typeof(Dictionary<string, string>))]
    public void IsSimpleType_ReturnsFalse_ForNonSimpleTypes(Type type)
        => Assert.That(TypeInspectorHelper.IsSimpleType(type), Is.False, $"Expected {type.Name} to be non-simple type");

    [Test]
    public void IsEnumerable_ReturnsFalse_ForStringType()
    {
        Assert.That(TypeInspectorHelper.IsEnumerable(typeof(string), out var itemType), Is.False);
        Assert.That(itemType, Is.Null);
    }

    [Test]
    public void IsEnumerable_ReturnsTrue_ForArrayType()
    {
        Assert.That(TypeInspectorHelper.IsEnumerable(typeof(int[]), out var itemType), Is.True);
        Assert.That(itemType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void IsEnumerable_ReturnsTrue_ForGenericList()
    {
        Assert.That(TypeInspectorHelper.IsEnumerable(typeof(List<string>), out var itemType), Is.True);
        Assert.That(itemType, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void IsEnumerable_ReturnsTrue_ForIEnumerableOfT()
    {
        Assert.That(TypeInspectorHelper.IsEnumerable(typeof(IEnumerable<Guid>), out var itemType), Is.True);
        Assert.That(itemType, Is.EqualTo(typeof(Guid)));
    }

    [Test]
    public void IsEnumerable_ReturnsTrue_ForCollectionImplementingEnumerable()
    {
        // Use HashSet<double> which implements IEnumerable<T>
        Assert.That(TypeInspectorHelper.IsEnumerable(typeof(HashSet<double>), out var itemType), Is.True);
        Assert.That(itemType, Is.EqualTo(typeof(double)));
    }

    [Test]
    public void IsEnumerable_ReturnsFalse_ForNonEnumerableType()
    {
        Assert.That(TypeInspectorHelper.IsEnumerable(typeof(int), out var itemType), Is.False);
        Assert.That(itemType, Is.Null);
    }
}
