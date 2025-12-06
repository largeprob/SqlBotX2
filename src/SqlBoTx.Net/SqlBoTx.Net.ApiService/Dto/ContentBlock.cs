using SqlBoTx.Net.ApiService.SqlPlugin;
using System.Text.Json.Serialization;
using static SqlBoTx.Net.ApiService.Dto.BlockMessage;

namespace SqlBoTx.Net.ApiService.Dto
{


    /// <summary>
    /// 内容块类型
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ContentBlockType
    {
        Sql,    // SQL 代码
        Data,   // 数据表格
        Chart,  // 图表
        Error   // 错误
    }


    /// <summary>
    /// 内容块基类
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(SqlBlock), "sql")]
    [JsonDerivedType(typeof(TableBlock), "table")]
    [JsonDerivedType(typeof(EchartsBlock), "echarts")]
    [JsonDerivedType(typeof(ErrorBlock), "error")]
    public abstract class ContentBlock
    {
        /// <summary>
        /// 块ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// SQL 代码块
    /// </summary>
    public class SqlBlock : ContentBlock
    {
        /// <summary>
        /// SQL 语句
        /// </summary>
        [JsonPropertyName("sql")]
        public string Sql { get; set; } = string.Empty;

        /// <summary>
        /// SQL 方言
        /// </summary>
        [JsonPropertyName("dialect")]
        public string? Dialect { get; set; }
    }

    /// <summary>
    /// 数据表格块
    /// </summary>
    public class TableBlock : ContentBlock
    {
        /// <summary>
        /// 列名
        /// </summary>
        [JsonPropertyName("columns")]
        public SqlStepColumns[] Columns { get; set; } = Array.Empty<SqlStepColumns>();

        /// <summary>
        /// 数据行
        /// </summary>
        [JsonPropertyName("rows")]
        public IEnumerable<dynamic> Rows { get; set; } = Array.Empty<object[]>();

        /// <summary>
        /// 总行数
        /// </summary>
        [JsonPropertyName("totalRows")]
        public int TotalRows { get; set; }
    }

    /// <summary>
    /// 图表块
    /// </summary>
    public class EchartsBlock : ContentBlock
    {
        /// <summary>
        /// 图表配置
        /// </summary>
        public class ChartConfig
        {
            /// <summary>
            /// X轴字段
            /// </summary>
            [JsonPropertyName("xAxis")]
            public string? XAxis { get; set; }

            /// <summary>
            /// Y轴字段
            /// </summary>
            [JsonPropertyName("yAxis")]
            public string[]? YAxis { get; set; }

            /// <summary>
            /// 标题
            /// </summary>
            [JsonPropertyName("title")]
            public string? Title { get; set; }

            /// <summary>
            /// 显示图例
            /// </summary>
            [JsonPropertyName("showLegend")]
            public bool ShowLegend { get; set; } = true;
        }


        /// <summary>
        /// 图表类型
        /// </summary>
        [JsonPropertyName("chartType")]
        public string ChartType { get; set; } = "bar";

        /// <summary>
        /// ECharts option 配置 JSON 字符串
        /// </summary>
        [JsonPropertyName("echartsOption")]
        public string? EchartsOption { get; set; }

        /// <summary>
        /// 图表配置（兼容旧版）
        /// </summary>
        [JsonPropertyName("config")]
        public ChartConfig Config { get; set; } = new();

        /// <summary>
        /// 图表数据（兼容旧版）
        /// </summary>
        [JsonPropertyName("data")]
        public object Data { get; set; } = new { };
    }


    /// <summary>
    /// 错误块
    /// </summary>
    public class ErrorBlock : ContentBlock
    {
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
