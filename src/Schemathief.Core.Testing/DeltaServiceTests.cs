using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace Schemathief.Core.Testing;
internal class DeltaServiceTests
{
    private MockHttpMessageHandler _mockHttp;
    private HttpClient _httpClient;
    private HttpBaseSchemaLoader _loader;
    private DeltaService _service;
    private string _assemblyPath;
    private string _validTypeName;

    [SetUp]
    public void SetUp()
    {
        // Use the current test assembly as the target assembly
        _assemblyPath = Assembly.GetExecutingAssembly().Location;
        // Use a known type in this test assembly
        _validTypeName = typeof(TestModel).FullName!;

        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _loader = new HttpBaseSchemaLoader(_httpClient);
        _service = new DeltaService(_loader);
    }

    [TearDown]
    public void TearDown()
    {
        _mockHttp.Dispose();
        _httpClient.Dispose();
    }

    // A dummy model for testing
    public class TestModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    [Test]
    public async Task GenerateAsync_ReturnsNull_WhenBaseSchemaMissingProperties()
    {
        string url = "https://example.com/schema-no-props.json";
        string json = @"{ 'title': 'NoProps' }".Replace('\'', '"');
        _mockHttp.When(url).Respond("application/json", json);

        var result = await _service.GenerateAsync(_assemblyPath, _validTypeName, url, []);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GenerateAsync_ReturnsNull_WhenTypeNotFound()
    {
        string url = "https://example.com/schema.json";
        string schemaJson = @"{
                'title': 'Test',
                'properties': { 'name': { 'type': 'string' } }
            }".Replace('\'', '"');

        _mockHttp.When(url).Respond("application/json", schemaJson);

        var result = await _service.GenerateAsync(_assemblyPath, "Non.Existent.Type", url, []);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GenerateAsync_ReturnsNull_WhenNoDelta()
    {
        string url = "https://example.com/schema-full.json";
        string schemaJson = @"{
                'title': 'TestModel',
                'properties': {
                    'name': { 'type': 'string' },
                    'age': { 'type': 'integer' }
                }
            }".Replace('\'', '"');

        _mockHttp.When(url).Respond("application/json", schemaJson);
        var result = await _service.GenerateAsync(_assemblyPath, _validTypeName, url, []);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GenerateAsync_ReturnsDeltaSchema_WhenDeltaExists()
    {
        string url = "https://example.com/schema-partial.json";
        string schemaJson = @"{
                'title': 'Test',
                'properties': { 'name': { 'type': 'string' } }
            }".Replace('\'', '"');

        _mockHttp.When(url).Respond("application/json", schemaJson);

        var result = await _service.GenerateAsync(_assemblyPath, _validTypeName, url, Array.Empty<string>());

        Assert.That(result, Is.Not.Null, "Expected delta schema when new properties exist");
        Assert.That(result!["title"]!.ToString(), Is.EqualTo("Delta Test"));

        var allOf = result["allOf"]!.AsArray();
        var props = allOf[1]!["properties"]!.AsObject();
        Assert.That(props.ContainsKey("age"), Is.True);
        Assert.That(props["age"]!["type"]!.ToString(), Is.EqualTo("integer"));
    }
}
