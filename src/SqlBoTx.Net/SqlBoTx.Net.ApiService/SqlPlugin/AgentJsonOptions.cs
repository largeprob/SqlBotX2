using System.Text.Json;

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
    }
}
