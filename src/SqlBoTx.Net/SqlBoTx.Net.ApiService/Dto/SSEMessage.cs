using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.Dto
{
    /// <summary>
    /// SSE 事件类型
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SSEEventType
    {
        Delta,  // 增量文本（流式输出）
        Block,  // 内容块（SQL、数据、图表等）
        Done,   // 完成
        Error   // 错误
    }

    /// <summary>
    /// SSE 消息基类
    /// </summary>
    public abstract class SSEMessage
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        [JsonPropertyName("type")]
        public abstract SSEEventType Type { get; }
    }

    /// <summary>
    /// 增量文本消息（流式文本输出）
    /// </summary>
    public class DeltaMessage : SSEMessage
    {
        [JsonPropertyName("type")]
        public override SSEEventType Type => SSEEventType.Delta;

        /// <summary>
        /// 增量文本内容
        /// </summary>
        [JsonPropertyName("delta")]
        public string Delta { get; set; } = string.Empty;
    }

    /// <summary>
    /// 内容块消息
    /// </summary>
    public class BlockMessage : SSEMessage
    {
        [JsonPropertyName("type")]
        public override SSEEventType Type => SSEEventType.Block;

        /// <summary>
        /// 内容块
        /// </summary>
        [JsonPropertyName("block")]
        public ContentBlock Block { get; set; } = new SqlBlock();
    }

    /// <summary>
    /// 完成消息
    /// </summary>
    public class DoneMessage : SSEMessage
    {
        [JsonPropertyName("type")]
        public override SSEEventType Type => SSEEventType.Done;

        /// <summary>
        /// 执行耗时(毫秒)
        /// </summary>
        [JsonPropertyName("elapsedMs")]
        public long ElapsedMs { get; set; }
    }


    /// <summary>
    /// 错误消息
    /// </summary>
    public class ErrorMessage : SSEMessage
    {
        [JsonPropertyName("type")]
        public override SSEEventType Type => SSEEventType.Error;

        /// <summary>
        /// 错误代码
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 错误消息
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 详细信息
        /// </summary>
        [JsonPropertyName("details")]
        public string? Details { get; set; }
    }


}
