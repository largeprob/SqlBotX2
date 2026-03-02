using System.Text.Json;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class SqlBotXJsonOptions
    {
        /// <summary>
        /// Serialize
        /// </summary>
        public static JsonSerializerOptions Serialize { get; } = new JsonSerializerOptions
        {
            //命名策略
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //空值处理
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            //编码
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>
        /// Serialize
        /// </summary>
        public static JsonSerializerOptions Deserialize { get; } = new JsonSerializerOptions
        {
            //命名策略
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //空值处理
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            //枚举映射
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }
}
