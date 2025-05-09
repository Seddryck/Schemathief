using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Moq;
using Schemathief.Core.Renderers;

namespace Schemathief.Core.Testing;
internal class DeltaServiceTests
{
    private MockHttpMessageHandler _mockHttp = null!;
    private HttpClient _httpClient = null!;
    private HttpBaseSchemaLoader _loader = null!;
    private Mock<IOutputRendererFactory> _rendererFactoryMock = null!;
    private DeltaService _service = null!;
    private string _assemblyPath = null!;
    private string _validTypeName = null!;

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
        _rendererFactoryMock = new Mock<IOutputRendererFactory>();
        _service = new DeltaService(_loader, _rendererFactoryMock.Object);
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

        var mockRenderer = new Mock<IOutputRenderer>();
        _rendererFactoryMock.Setup(f => f.CreateRenderer(It.IsAny<string>()))
            .Returns(mockRenderer.Object);

        var result = await _service.GenerateAsync(_assemblyPath, _validTypeName, url, Array.Empty<string>());

        Assert.That(result, Is.Not.Null, "Expected delta schema when new properties exist");
        Assert.That(result!["title"]!.ToString(), Is.EqualTo("Delta Test"));

        var allOf = result["allOf"]!.AsArray();
        var props = allOf[1]!["properties"]!.AsObject();
        Assert.That(props.ContainsKey("age"), Is.True);
        Assert.That(props["age"]!["type"]!.ToString(), Is.EqualTo("integer"));
    }

    [Test]
    public async Task GenerateAsync_WithOutputPath_CreatesAndUsesFileRenderer()
    {
        
        string url = "https://example.com/schema-partial.json";
        string schemaJson = @"{
                'title': 'Test',
                'properties': { 'name': { 'type': 'string' } }
            }".Replace('\'', '"');

        _mockHttp.When(url).Respond("application/json", schemaJson);

        var mockRenderer = new Mock<IOutputRenderer>();
        _rendererFactoryMock.Setup(f => f.CreateRenderer(It.IsAny<string>()))
            .Returns(mockRenderer.Object);

        await _service.GenerateAsync(_assemblyPath, _validTypeName, url, Array.Empty<string>(), "test.json");

        _rendererFactoryMock.Verify(f => f.CreateRenderer("test.json"), Times.Once);
        mockRenderer.Verify(r => r.RenderAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task GenerateAsync_WithoutOutputPath_CreatesAndUsesConsoleRenderer()
    {
        string url = "https://example.com/schema-partial.json";
        string schemaJson = @"{
                'title': 'Test',
                'properties': { 'name': { 'type': 'string' } }
            }".Replace('\'', '"');

        _mockHttp.When(url).Respond("application/json", schemaJson);

        var mockRenderer = new Mock<IOutputRenderer>();
        _rendererFactoryMock.Setup(f => f.CreateRenderer(null))
            .Returns(mockRenderer.Object);

        await _service.GenerateAsync(_assemblyPath, _validTypeName, url, Array.Empty<string>());

        _rendererFactoryMock.Verify(f => f.CreateRenderer(null), Times.Once);
        mockRenderer.Verify(r => r.RenderAsync(It.IsAny<string>()), Times.Once);
    }
}
