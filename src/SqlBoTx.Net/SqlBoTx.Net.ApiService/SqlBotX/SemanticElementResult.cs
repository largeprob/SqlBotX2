using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class SemanticElementResult
    {
        /// <summary>
        /// 引导或确认回复
        /// </summary>
        [JsonPropertyName("data")]
        public List<SemanticElement>? Data { get; set; }

        /// <summary>
        /// 引导或确认回复
        /// </summary>
        [JsonPropertyName("response")]
        public string? Response { get; set; }
    }

    public class SemanticElement
    {
        /// <summary>
        /// 用户想要查询的内容
        /// </summary>
        [JsonPropertyName("what")]
        public List<SemanticElementFiled> What { get; set; } = new List<SemanticElementFiled>();

        /// <summary>
        /// 用户想要查询的数据来源
        /// </summary>
        [JsonPropertyName("from_what")]
        public string From { get; set; } = string.Empty;
    }

    public class SemanticElementFiled
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [JsonPropertyName("field_name")]
        public string FieldName { get; set; } = string.Empty;

        /// <summary>
        /// 是否度量字段
        /// </summary>
        [JsonPropertyName("is_metric")]
        public bool IsMetric { get; set; } 

        /// <summary>
        /// 度量方式
        /// </summary>
        [JsonPropertyName("calculation")]
        public string Calculation { get; set; } = string.Empty;
    }
}
