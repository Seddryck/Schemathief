using System;
using System.Threading.Tasks;
using Schemathief.Core;

namespace Schemathief.Cli.Renderers;

public class FileOutputRenderer : IOutputRenderer
{
    public string FilePath { get; }

    public FileOutputRenderer(string filePath)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public async Task RenderAsync(string content)
    {
        try
        {
            await System.IO.File.WriteAllTextAsync(FilePath, content);
            System.Console.WriteLine($"Schema written to {FilePath}");
        }
        catch (Exception ex)
        {
            System.Console.Error.WriteLine($"Error writing to file: {ex.Message}");
            throw;
        }
    }
} 