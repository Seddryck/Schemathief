using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Schemathief.Core.Testing;
public class StringExtensionsTests
{
    [TestCase(null, null)]
    [TestCase("", "")]
    [TestCase("hello", "Hello")]
    [TestCase("hello_world", "HelloWorld")]
    [TestCase("_leading", "Leading")]
    [TestCase("multiple__underscores", "MultipleUnderscores")]
    [TestCase("alreadyPascal", "AlreadyPascal")]
    public void ToPascalCase_ReturnsExpected(string? input, string? expected)
        => Assert.That(input.ToPascalCase(), Is.EqualTo(expected));

    [TestCase(null, null)]
    [TestCase("", "")]
    [TestCase("hello", "hello")]
    [TestCase("Hello", "hello")]
    [TestCase("hello_world", "helloWorld")]
    [TestCase("_leading", "leading")]
    [TestCase("multiple__underscores", "multipleUnderscores")]
    [TestCase("alreadyCamel", "alreadyCamel")]
    public void ToCamelCase_ReturnsExpected(string? input, string? expected)
        => Assert.That(input.ToCamelCase(), Is.EqualTo(expected));
}
