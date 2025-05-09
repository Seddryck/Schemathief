using System.Threading.Tasks;

namespace Schemathief.Core;

public interface IOutputRenderer
{
    Task RenderAsync(string content);
} 