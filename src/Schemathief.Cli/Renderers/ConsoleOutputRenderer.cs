using System.Threading.Tasks;
using Schemathief.Core;

namespace Schemathief.Cli.Renderers;

public class ConsoleOutputRenderer : IOutputRenderer
{
    public Task RenderAsync(string content)
    {
        System.Console.WriteLine(content);
        return Task.CompletedTask;
    }
} 