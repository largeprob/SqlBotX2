using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.Dto
{
    /// <summary>
    /// 消息状态
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SQLBotMessageStatus
    {
        /// <summary>
        /// 流，代表进行中
        /// </summary>
        [JsonPropertyName("streaming")]
        Streaming,
        /// <summary>
        /// 本次信息完成
        /// </summary>
        [JsonPropertyName("done")]
        Done,
        /// <summary>
        /// 未预料的错误
        /// </summary>
        [JsonPropertyName("error")]
        Error
    }


    /// <summary>
    /// SQLBotMessage
    /// </summary>
    public class SQLBotMessage
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        [JsonPropertyName("sessionId")]
        public Guid? SessionId { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        [JsonPropertyName("role")]
        public Role Role { get; set; }
 
        /// <summary>
        /// 内容块
        /// </summary>
        [JsonPropertyName("content")]
        public List<BaseContentBlock> Content { get; set; } = new();

        /// <summary>
        /// 消息状态
        /// </summary>
        [JsonPropertyName("status")]
        public SQLBotMessageStatus Status { get; set; }
 
        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonPropertyName("createdAt")]
        public long CreatedAt { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonPropertyName("errorMsg")]
        public string? ErrorMsg { get; set; } 
    }


    /// <summary>
    /// 推送状态（消息）
    /// </summary>
    public struct SSEMessageStatus
    {
        /// <summary>
        /// 消息状态
        /// </summary>
        [JsonPropertyName("status")]
        public SQLBotMessageStatus Status { get; set; }

        /// <summary>
        /// 状态提示
        /// </summary>
        [JsonPropertyName("Str")]
        public string Str{ get; set; }
    }

    /// <summary>
    /// 推送状态（数据块）
    /// </summary>
    public struct SSEBlockStatus
    {
        /// <summary>
        /// 消息状态
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 消息状态
        /// </summary>
        [JsonPropertyName("status")]
        public SQLBotMessageStatus Status { get; set; }

        /// <summary>
        /// 状态提示
        /// </summary>
        [JsonPropertyName("Str")]
        public string Str { get; set; }
    }
}
