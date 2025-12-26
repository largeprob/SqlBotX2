using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.Dto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Role
    {
        [JsonPropertyName("user")]
        User,
        [JsonPropertyName("assistant")]
        Assistant,
        [JsonPropertyName("system")]
        System
    }

    // 对应 BlockType
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BlockType
    {
        [JsonPropertyName("text")]
        Text,
        [JsonPropertyName("thought")]
        Thought,
        [JsonPropertyName("sql")]
        Sql,
        [JsonPropertyName("echarts")]
        Echarts,
        [JsonPropertyName("table")]
        Table,
        [JsonPropertyName("file")]
        File
    }

    // 对应 BlockStatus
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BlockStatus
    {
        [JsonPropertyName("streaming")]
        Streaming,
        [JsonPropertyName("done")]
        Done,
        [JsonPropertyName("error")]
        Error
    }

    // 对应 MessageStatus
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageStatus
    {
        [JsonPropertyName("streaming")]
        Streaming,
        [JsonPropertyName("done")]
        Done,
        [JsonPropertyName("error")]
        Error
    }


    //多模态解析一定要把类型参数放在第一个属性，默认为： $type
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(ContentBlockThought), typeDiscriminator: "thought")]
    [JsonDerivedType(typeof(ContentBlockText), typeDiscriminator: "text")]
    [JsonDerivedType(typeof(ContentBlockSql), typeDiscriminator: "sql")]
    [JsonDerivedType(typeof(ContentBlockEcharts), typeDiscriminator: "echarts")]
    [JsonDerivedType(typeof(ContentBlockTable), typeDiscriminator: "table")]
    [JsonDerivedType(typeof(ContentBlockFile), typeDiscriminator: "file")]
    public  abstract class BaseContentBlock
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("status")]
        public BlockStatus Status { get; set; }

        [JsonPropertyName("createdAt")]
        public long CreatedAt { get; set; } // 使用 long 对应 TS 的 number (时间戳)
    }

    // 1. 思考模式 ContentBlockThought
    public class ContentBlockThought : BaseContentBlock
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("isCollapsed")]
        public bool? IsCollapsed { get; set; } // 可空 bool

        [JsonPropertyName("duration")]
        public double? Duration { get; set; } // 可空 number
    }

    // 2. 正文文本块 ContentBlockText
    public class ContentBlockText : BaseContentBlock
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    // 3. SQL块 ContentBlockSql
    public class ContentBlockSql : BaseContentBlock
    {
        // 注意：你的 TS 定义中这里写了 blockType: 'sql'，但基类有 type。
        // 为了兼容，建议统一使用 type。如果必须有 blockType 字段，可以加下面这行：
        // [JsonPropertyName("blockType")]
        // public string BlockTypeString => "sql"; 

        [JsonPropertyName("sql")]
        public string Sql { get; set; } = string.Empty;
    }

    // 4. 图表块 ContentBlockEcharts
    public class ContentBlockEcharts : BaseContentBlock
    {
        [JsonPropertyName("options")]
        public string Options { get; set; } = string.Empty; // 你的 TS 定义是 string (可能是 JSON 字符串)
    }

    // 5. 表格块 ContentBlockTable
    public class ContentBlockTable : BaseContentBlock
    {
        [JsonPropertyName("columns")]
        public List<TableColumn> Columns { get; set; } = new();

        // 对应 Record<string, any>[]
        // 使用 Dictionary<string, object> 来处理动态对象
        [JsonPropertyName("item")]
        public IEnumerable<IDictionary<string, object>> Item { get; set; }

        [JsonPropertyName("pagination")]
        public TablePagination? Pagination { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }
    }

    // 表格辅助类：列定义
    public class TableColumn
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("dataIndex")]
        public string DataIndex { get; set; } = string.Empty;

        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;
    }

    // 表格辅助类：分页
    public class TablePagination
    {
        [JsonPropertyName("current")]
        public int Current { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    // 6. 文件 ContentBlockFile
    public class ContentBlockFile : BaseContentBlock
    {
        // 同 SQL，如果必须传 blockType 字段给前端，可以手动添加属性

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("fileType")]
        public string? FileType { get; set; }
    }

    /// <summary>
    /// Sse Message
    /// </summary>
    public class ChatMessage
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
        public MessageStatus Status { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonPropertyName("createdAt")]
        public long CreatedAt { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonPropertyName("errorMsg")]
        public string? ErrorMsg { get; set; } // 可空字符串
    }


}
