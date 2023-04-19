#nullable enable
using Newtonsoft.Json;

namespace Mochineko.LangChainDockerUnity
{
    [JsonObject]
    public sealed class ConversationAgentRequest
    {
        [JsonProperty("content")]
        public string Content { get; private set; } = string.Empty;

        public ConversationAgentRequest(string content)
        {
            this.Content = content;
        }
    }
}