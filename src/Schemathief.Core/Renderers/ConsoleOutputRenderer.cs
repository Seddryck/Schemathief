using System.Threading.Tasks;

namespace Schemathief.Core.Renderers;

public class ConsoleOutputRenderer : IOutputRenderer
{
    public Task RenderAsync(string content)
    {
        System.Console.WriteLine(content);
        return Task.CompletedTask;
    }
} 