// SchemaDeltaCli/Program.cs
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Threading.Tasks;
using Schemathief.Cli;
using Schemathief.Core;
using System.Diagnostics.CodeAnalysis;
using System.CommandLine;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var deltaService = new DeltaService(new HttpBaseSchemaLoader(new HttpClient()));

        var rootCommand = new RootCommand("SchemaDelta CLI tool")
            {
                new DeltaCommand(deltaService).Create()
            };

        return await rootCommand.InvokeAsync(args);
    }
}
