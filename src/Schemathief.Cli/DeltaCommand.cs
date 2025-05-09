using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using Schemathief.Core;

namespace Schemathief.Cli;
public class DeltaCommand
{
    private readonly IDeltaService _deltaService;

    public Option<string> AssemblyOption { get; }
    public Option<string> ClassOption { get; }
    public Option<string> BaseSchemaOption { get; }
    public Option<string[]> ExcludeOption { get; }
    public Option<string> OutputOption { get; }

    public DeltaCommand(IDeltaService deltaService)
    {
        _deltaService = deltaService;

        AssemblyOption = new Option<string>(
            aliases: new[] { "--assembly", "-a" },
            description: "Path to the assembly containing the class.")
        { IsRequired = true };

        ClassOption = new Option<string>(
            aliases: new[] { "--class", "-c" },
            description: "Fully qualified class name.")
        { IsRequired = true };

        BaseSchemaOption = new Option<string>(
            aliases: new[] { "--base", "-b" },
            description: "URL to the base JSON schema.")
        { IsRequired = true };

        ExcludeOption = new Option<string[]>(
            aliases: new[] { "--exclude", "-x" },
            description: "Pipe-separated list of excluded properties.",
            parseArgument: result =>
            {
                var value = result.Tokens.SingleOrDefault()?.Value;
                return value?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            });

        OutputOption = new Option<string>(
            aliases: new[] { "--output", "-o" },
            description: "Path to write the output to. If not specified, output will be written to console.");
    }

    public Command Create()
    {
        var command = new Command("delta", "Generate delta schema")
        {
            AssemblyOption,
            ClassOption,
            BaseSchemaOption,
            ExcludeOption,
            OutputOption
        };

        command.SetHandler(async (string assemblyPath, string fqcn, string baseSchemaUrl, string[] excludes, string outputPath) =>
        {
            var result = await _deltaService.GenerateAsync(assemblyPath, fqcn, baseSchemaUrl, excludes, outputPath);
            if (result is null)
            {
                Console.Error.WriteLine("No delta schema generated.");
            }
        }, AssemblyOption, ClassOption, BaseSchemaOption, ExcludeOption, OutputOption);

        return command;
    }
}
