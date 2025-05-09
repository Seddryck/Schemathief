using System;
using System.IO;
using NUnit.Framework;
using Schemathief.Core.Renderers;

namespace Schemathief.Core.Testing.Renderers;

[TestFixture]
public class OutputRendererFactoryTests
{
    private OutputRendererFactory _factory = null!;
    private string _tempDirectory = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new OutputRendererFactory();
        _tempDirectory = Path.Combine(Path.GetTempPath(), "SchemathiefTests");
        Directory.CreateDirectory(_tempDirectory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, true);
    }

    [Test]
    public void CreateRenderer_WithNullPath_ReturnsConsoleRenderer()
    {
        var renderer = _factory.CreateRenderer(null);
        Assert.That(renderer, Is.InstanceOf<ConsoleOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithEmptyPath_ReturnsConsoleRenderer()
    {
        var renderer = _factory.CreateRenderer(string.Empty);
        Assert.That(renderer, Is.InstanceOf<ConsoleOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithValidRelativePath_ReturnsFileRenderer()
    {
        var path = "test.json";

        var renderer = _factory.CreateRenderer(path);
        Assert.That(renderer, Is.InstanceOf<FileOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithValidAbsolutePath_ReturnsFileRenderer()
    {
        var path = Path.Combine(_tempDirectory, "test.json");

        var renderer = _factory.CreateRenderer(path);
        Assert.That(renderer, Is.InstanceOf<FileOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithValidPath_SetsCorrectFilePath()
    {
        var path = Path.Combine(_tempDirectory, "test.json");

        var renderer = _factory.CreateRenderer(path) as FileOutputRenderer;
        Assert.That(renderer, Is.Not.Null);
        Assert.That(renderer!.FilePath, Is.EqualTo(Path.GetFullPath(path)));
    }

    [Test]
    public void CreateRenderer_WithValidPath_CreatesDirectoryIfNotExists()
    {
        var subDir = Path.Combine(_tempDirectory, "subdir");
        var path = Path.Combine(subDir, "test.json");

        _factory.CreateRenderer(path);
        Assert.That(Directory.Exists(subDir), Is.True);
    }

    [Test]
    public void CreateRenderer_WithInvalidPath_ThrowsArgumentException()
    {
        var invalidPath = "test:invalid.json";
        Assert.That(() => _factory.CreateRenderer(invalidPath),
            Throws.ArgumentException.With.Message.Contains("Invalid output path"));
    }

    [Test]
    public void CreateRenderer_WithInvalidCharacters_ThrowsArgumentException()
    {
        var invalidPath = "test<>.json";
        Assert.That(() => _factory.CreateRenderer(invalidPath),
            Throws.ArgumentException.With.Message.Contains("Invalid output path"));
    }
} 
