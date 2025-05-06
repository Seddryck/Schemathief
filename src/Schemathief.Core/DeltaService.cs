using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Schemathief.Core;
public class DeltaService : IDeltaService
{
    private readonly IBaseSchemaLoader _loader;

    public DeltaService(IBaseSchemaLoader loader)
        => _loader = loader;

    public async Task<JsonObject?> GenerateAsync(
        string assemblyPath,
        string fullyQualifiedClassName,
        string baseSchemaUrl,
        string[] excludes)
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

        return SchemaDeltaBuilder.BuildFinalSchema(baseSchema, deltaProps, baseSchemaUrl);
    }
}
