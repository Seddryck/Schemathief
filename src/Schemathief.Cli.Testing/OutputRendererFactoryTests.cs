using System;
using NUnit.Framework;
using Schemathief.Cli.Renderers;
using Schemathief.Core;

namespace Schemathief.Cli.Testing;

public class OutputRendererFactoryTests
{
    private readonly OutputRendererFactory _factory;

    public OutputRendererFactoryTests()
    {
        _factory = new OutputRendererFactory();
    }

    [Test]
    public void CreateRenderer_WithNullPath_ReturnsConsoleRenderer()
    {
        var renderer = _factory.CreateRenderer(null);
        Assert.That(renderer, Is.TypeOf<ConsoleOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithEmptyPath_ReturnsConsoleRenderer()
    {
        var renderer = _factory.CreateRenderer(string.Empty);
        Assert.That(renderer, Is.TypeOf<ConsoleOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithValidPath_ReturnsFileRenderer()
    {
        var path = "test.json";
        var renderer = _factory.CreateRenderer(path);
        Assert.That(renderer, Is.TypeOf<FileOutputRenderer>());
    }

    [Test]
    public void CreateRenderer_WithValidPath_SetsCorrectFilePath()
    {
        var path = "test.json";
        var renderer = _factory.CreateRenderer(path) as FileOutputRenderer;
        Assert.That(renderer, Is.Not.Null);
        Assert.That(renderer.FilePath, Is.EqualTo(path));
    }
} 
