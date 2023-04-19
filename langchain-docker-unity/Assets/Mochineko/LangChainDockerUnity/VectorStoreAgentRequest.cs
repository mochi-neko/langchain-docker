#nullable enable
using Newtonsoft.Json;

namespace Mochineko.LangChainDockerUnity
{
    [JsonObject]
    public sealed class VectorStoreAgentRequest
    {
        [JsonProperty("content")] public string Content { get; private set; } = string.Empty;
    }
}