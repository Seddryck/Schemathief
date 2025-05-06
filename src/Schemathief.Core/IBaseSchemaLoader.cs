using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Schemathief.Core;
public interface IBaseSchemaLoader
{
    Task<JsonObject?> LoadAsync(string url);
}
