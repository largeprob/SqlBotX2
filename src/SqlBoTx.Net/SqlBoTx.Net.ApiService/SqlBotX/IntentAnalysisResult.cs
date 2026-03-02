using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    /// <summary>
    /// 对应 LLM 意图分类器的输出结果
    /// </summary>
    public class IntentAnalysisResult
    {
        /// <summary>
        /// 分类结果： "DATA_QUERY" | "CHIT_CHAT"
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("confidence")]
        public float Confidence { get; set; } = 0f;

        [JsonPropertyName("intent_summary")]
        public string IntentSummary { get; set; } = string.Empty;

        [JsonPropertyName("suggested_reply")]
        public string SuggestedReply { get; set; } = string.Empty;
    }
}
