using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Schemathief.Core.Renderers;

namespace Schemathief.Core;
public class DeltaService : IDeltaService
{
    private readonly IBaseSchemaLoader _loader;
    private readonly IOutputRendererFactory _rendererFactory;

    public DeltaService(IBaseSchemaLoader loader, IOutputRendererFactory rendererFactory)
    {
        _loader = loader;
        _rendererFactory = rendererFactory;
    }

    public async Task<JsonObject?> GenerateAsync(
        string assemblyPath,
        string fullyQualifiedClassName,
        string baseSchemaUrl,
        string[] excludes,
        string? outputPath = null)
    {
        var baseSchema = await _loader.LoadAsync(baseSchemaUrl);
        if (baseSchema == null)
            return null;

        var targetType = Assembly.LoadFrom(assemblyPath)
                                 .GetType(fullyQualifiedClassName);
        if (targetType == null)
            return null;

        var definedProps = TypeInspector.GetDefinedProperties(targetType, excludes);
        var baseProps = TypeInspector.GetBaseProperties(baseSchema);
        var deltaProps = SchemaDeltaBuilder.BuildDeltaProperties(definedProps, baseProps);

        if (deltaProps.Count == 0)
            return null;

        var result = SchemaDeltaBuilder.BuildFinalSchema(baseSchema, deltaProps, baseSchemaUrl);
        
        if (result != null)
        {
            var renderer = _rendererFactory.CreateRenderer(outputPath);
            await renderer.RenderAsync(result.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        }

        return result;
    }
}
