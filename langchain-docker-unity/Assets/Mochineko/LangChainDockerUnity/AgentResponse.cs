#nullable enable
using Newtonsoft.Json;

namespace Mochineko.LangChainDockerUnity
{
    [JsonObject]
    public sealed class AgentResponse
    {
        [JsonProperty("result")]
        public string Result { get; private set; } = string.Empty;
    }
}
