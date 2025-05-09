using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Schemathief.Cli.Renderers;

namespace Schemathief.Cli.Testing;

public class RendererIntegrationTests : IDisposable
{
    private readonly string _testFilePath;
    private readonly TextWriter _originalConsoleOut;

    public RendererIntegrationTests()
    {
        _testFilePath = Path.GetTempFileName();
        _originalConsoleOut = Console.Out;
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
        Console.SetOut(_originalConsoleOut);
    }

    [Test]
    public async Task ConsoleOutputRenderer_ShouldWriteToConsole()
    {
        var renderer = new ConsoleOutputRenderer();
        var testContent = "Test content";
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        await renderer.RenderAsync(testContent);

        Assert.That(stringWriter.ToString(), Is.EqualTo(testContent + Environment.NewLine));
    }

    [Test]
    public async Task FileOutputRenderer_ShouldWriteToFile()
    {
        var renderer = new FileOutputRenderer(_testFilePath);
        var testContent = "Test content";

        await renderer.RenderAsync(testContent);

        var fileContent = await File.ReadAllTextAsync(_testFilePath);
        Assert.That(fileContent, Is.EqualTo(testContent));
    }

    [Test]
    public async Task FileOutputRenderer_ShouldShowSuccessMessage()
    {
        var renderer = new FileOutputRenderer(_testFilePath);
        var testContent = "Test content";
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        await renderer.RenderAsync(testContent);

        Assert.That(stringWriter.ToString(), Does.Contain($"Schema written to {_testFilePath}"));
    }

    [Test]
    public void FileOutputRenderer_WithInvalidPath_ShouldThrowException()
    {
        var invalidPath = Path.Combine(Path.GetTempPath(), "nonexistent", "test.json");
        var renderer = new FileOutputRenderer(invalidPath);

        Assert.That(
        async () => await renderer.RenderAsync("Test content"),
        Throws.TypeOf<DirectoryNotFoundException>());
    }
} 
