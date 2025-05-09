using System;
using Schemathief.Core;

namespace Schemathief.Cli.Renderers;

public interface IOutputRendererFactory
{
    IOutputRenderer CreateRenderer(string? outputPath);
}

public class OutputRendererFactory : IOutputRendererFactory
{
    public IOutputRenderer CreateRenderer(string? outputPath)
    {
        return string.IsNullOrEmpty(outputPath)
            ? new ConsoleOutputRenderer()
            : new FileOutputRenderer(outputPath);
    }
} 