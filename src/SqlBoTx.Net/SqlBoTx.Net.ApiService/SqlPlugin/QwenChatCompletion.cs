using Qdrant.Client.Grpc;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlPlugin
{
    public class QwenChatCompletion
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("system_fingerprint")]
        public string SystemFingerprint { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("logprobs")]
        public object Logprobs { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("reasoning_content")]
        public string ReasoningContent { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    // 如果需要解析Message.Content中的JSON，可以创建对应的类
    public class ContentData
    {
        [JsonPropertyName("intent")]
        public string Intent { get; set; }

        [JsonPropertyName("target_entity")]
        public string TargetEntity { get; set; }

        [JsonPropertyName("time_range")]
        public TimeRange TimeRange { get; set; }

        [JsonPropertyName("raw_filters")]
        public List<object> RawFilters { get; set; }

        [JsonPropertyName("output_fields")]
        public List<object> OutputFields { get; set; }
    }

    public class TimeRange
    {
        [JsonPropertyName("start")]
        public string Start { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }
    }
}
