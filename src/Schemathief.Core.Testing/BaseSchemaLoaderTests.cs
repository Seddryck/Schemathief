using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Schemathief.Core;

namespace Schemathief.Core.Testing;

internal class BaseSchemaLoaderTests
{
    [Test]
    public async Task LoadAsync_ReturnsSchema_WhenPropertiesPresent()
    {
        // Arrange
        var schemaJson = @"
        {
          ""title"": ""MySchema"",
          ""properties"": {
            ""Foo"": { ""type"": ""string"" },
            ""Bar"": { ""type"": ""integer"" }
          }
        }";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When("https://example.com/schema.json")
            .Respond("application/json", schemaJson);

        var client = mockHttp.ToHttpClient();
        var loader = new HttpBaseSchemaLoader(client);

        // Act
        JsonObject? result = await loader.LoadAsync("https://example.com/schema.json");

        // Assert
        Assert.That(result, Is.Not.Null);
        var props = result["properties"]!.AsObject().Select(p => p.Key).ToHashSet();
        Assert.That(props.Contains("Foo"));
        Assert.That(props.Contains("Bar"));
    }

    [Test]
    public async Task LoadAsync_ReturnsNull_WhenPropertiesMissing()
    {
        // Arrange
        var noPropsJson = @"{ ""title"": ""EmptySchema"" }";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When("https://example.com/no-props.json")
            .Respond("application/json", noPropsJson);

        var client = mockHttp.ToHttpClient();
        var loader = new HttpBaseSchemaLoader(client);

        JsonObject? result = await loader.LoadAsync("https://example.com/no-props.json");

        Assert.That(result, Is.Null);
    }
}
