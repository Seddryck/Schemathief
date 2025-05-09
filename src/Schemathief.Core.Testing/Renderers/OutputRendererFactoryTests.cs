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
        {
            Directory.Delete(_tempDirectory, true);
        }
    }

    [Test]
    public void CreateRenderer_WithNullPath_ReturnsConsoleRenderer()
    {
        // Act
        var renderer = _factory.CreateRenderer(null);

        // Assert
        Assert.That(renderer, Is.InstanceOf<ConsoleOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithEmptyPath_ReturnsConsoleRenderer()
    {
        // Act
        var renderer = _factory.CreateRenderer(string.Empty);

        // Assert
        Assert.That(renderer, Is.InstanceOf<ConsoleOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithValidRelativePath_ReturnsFileRenderer()
    {
        // Arrange
        var path = "test.json";

        // Act
        var renderer = _factory.CreateRenderer(path);

        // Assert
        Assert.That(renderer, Is.InstanceOf<FileOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithValidAbsolutePath_ReturnsFileRenderer()
    {
        // Arrange
        var path = Path.Combine(_tempDirectory, "test.json");

        // Act
        var renderer = _factory.CreateRenderer(path);

        // Assert
        Assert.That(renderer, Is.InstanceOf<FileOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithValidPath_SetsCorrectFilePath()
    {
        // Arrange
        var path = Path.Combine(_tempDirectory, "test.json");

        // Act
        var renderer = _factory.CreateRenderer(path) as FileOutputRenderer;

        // Assert
        Assert.That(renderer, Is.Not.Null);
        Assert.That(renderer!.FilePath, Is.EqualTo(Path.GetFullPath(path)));
    }

    [Test]
    public void CreateRenderer_WithValidPath_CreatesDirectoryIfNotExists()
    {
        // Arrange
        var subDir = Path.Combine(_tempDirectory, "subdir");
        var path = Path.Combine(subDir, "test.json");

        // Act
        _factory.CreateRenderer(path);

        // Assert
        Assert.That(Directory.Exists(subDir), Is.True);
    }

    [Test]
    public void CreateRenderer_WithInvalidPath_ThrowsArgumentException()
    {
        // Arrange
        var invalidPath = "test:invalid.json";

        // Act & Assert
        Assert.That(() => _factory.CreateRenderer(invalidPath),
            Throws.ArgumentException.With.Message.Contains("Invalid output path"));
    }

    [Test]
    public void CreateRenderer_WithInvalidCharacters_ThrowsArgumentException()
    {
        // Arrange
        var invalidPath = "test<>.json";

        // Act & Assert
        Assert.That(() => _factory.CreateRenderer(invalidPath),
            Throws.ArgumentException.With.Message.Contains("Invalid output path"));
    }
} 
