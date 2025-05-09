using System;
using System.IO;
using System.Linq;

namespace Schemathief.Core.Renderers;

public interface IOutputRendererFactory
{
    IOutputRenderer CreateRenderer(string? outputPath);
}

public class OutputRendererFactory : IOutputRendererFactory
{
    public IOutputRenderer CreateRenderer(string? outputPath)
    {
        if (string.IsNullOrEmpty(outputPath))
            return new ConsoleOutputRenderer();

        try
        {
            var filename = Path.GetFileName(outputPath);
            if (string.IsNullOrEmpty(filename) || filename.Any(x => Path.GetInvalidFileNameChars().Contains(x)))
                throw new ArgumentException($"Invalid output path: {outputPath}. The path must be a valid relative or absolute path.", nameof(outputPath));

            var fullPath = Path.GetFullPath(outputPath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && directory.Any(x => Path.GetInvalidPathChars().Contains(x)))
                throw new ArgumentException($"Invalid output path: {outputPath}. The path must be a valid relative or absolute path.", nameof(outputPath));

            // Ensure the directory exists or can be created
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return new FileOutputRenderer(fullPath);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
        {
            throw new ArgumentException($"Invalid output path: {outputPath}. The path must be a valid relative or absolute path.", nameof(outputPath), ex);
        }
    }
} 
