using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Schemathief.Core;

namespace Schemathief.Cli.Testing;

internal class DeltaCommandTests
{
    private Mock<IDeltaService> _mockService;
    private RootCommand _rootCommand;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IDeltaService>(MockBehavior.Strict);
        var deltaCommand = new DeltaCommand(_mockService.Object);
        _rootCommand = new RootCommand("SchemaDelta CLI tool")
            {
                deltaCommand.Create()
            };
    }

    [Test]
    public async Task Invoke_WithAllRequiredOptions_CallsGenerateAsyncAndReturnsZero()
    {
        // Arrange
        const string assembly = "path/to/MyLib.dll";
        const string @class = "My.Namespace.MyType";
        const string baseUrl = "https://example.com/schema.json";
        var excludes = new[] { "PropA", "PropB" };

        _mockService
            .Setup(s => s.GenerateAsync(assembly, @class, baseUrl, excludes))
            .ReturnsAsync([])
            .Verifiable();

        var args = $"delta -a {assembly} -c {@class} -b {baseUrl} -x PropA|PropB";

        // Act
        var exitCode = await _rootCommand.InvokeAsync(args);

        // Assert
        Assert.That(exitCode, Is.Zero);
        _mockService.Verify();
    }

    [Test]
    public async Task Invoke_WithoutExcludeOption_PassesEmptyArrayToService()
    {
        // Arrange
        const string assembly = "lib.dll";
        const string @class = "C.T";
        const string baseUrl = "http://schema";
        var excludes = new string[0];

        _mockService
            .Setup(s => s.GenerateAsync(assembly, @class, baseUrl, excludes))
            .ReturnsAsync([])
            .Verifiable();

        var args = $"delta -a {assembly} -c {@class} -b {baseUrl}";
        var exitCode = await _rootCommand.InvokeAsync(args);

        Assert.That(exitCode, Is.Zero);
        _mockService.Verify();
    }

    [Test]
    public async Task Invoke_MissingRequiredOption_ReturnsNonZeroAndDoesNotCallService()
    {
        const string assembly = "lib.dll";
        const string baseUrl = "http://schema";
        var args = $"delta -a {assembly} -b {baseUrl}";

        var exitCode = await _rootCommand.InvokeAsync(args);

        Assert.That(exitCode, Is.Not.Zero);
        _mockService.VerifyNoOtherCalls();
    }
}
