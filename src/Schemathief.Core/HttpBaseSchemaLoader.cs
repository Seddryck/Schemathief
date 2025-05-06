using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Schemathief.Core;
public class HttpBaseSchemaLoader : IBaseSchemaLoader
{
    private readonly HttpClient _httpClient;

    public HttpBaseSchemaLoader(HttpClient httpClient)
        => _httpClient = httpClient;

    public async Task<JsonObject?> LoadAsync(string url)
    {
        var json = await _httpClient.GetStringAsync(url);
        return JsonNode.Parse(json)?["properties"]?.Parent?.AsObject();
    }
}
