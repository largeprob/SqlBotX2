using System.Text.Json;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlPlugin
{

    /// <summary>
    ///Json 序列化选项
    /// </summary>

    public class AgentJsonOptions
    {
        /// <summary>
        /// 默认
        /// </summary>
        public static JsonSerializerOptions Default { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// 流式传输格式化选项
        /// </summary>
        public static JsonSerializerOptions SSEJsonOptions { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }
}
