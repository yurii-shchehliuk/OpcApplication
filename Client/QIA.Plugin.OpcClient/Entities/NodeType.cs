using System.Text.Json.Serialization;

namespace QIA.Plugin.OpcClient.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NodeType
    {
        Object,
        Method,
        Subscription,
        Value
    }
}
