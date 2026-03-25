using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class Step22Result
    {

        /// <summary>
        /// 用户目标的简练描述
        /// </summary>
        [JsonPropertyName("target_core")]
        public string target_core { get; set; }

        /// <summary>
        /// 业务对象列表
        /// </summary>
        [JsonPropertyName("business_objects")]
        public List<Step22BusinessObject> BusinessObjects { get; set; }

        /// <summary>
        /// 维度列表
        /// </summary>
        [JsonPropertyName("dimensions")]
        public List<Step22Dimension> Dimensions { get; set; }

        /// <summary>
        /// 指标列表
        /// </summary>
        [JsonPropertyName("metrics")]
        public List<Step22Metric> Metrics { get; set; }

        /// <summary>
        /// 显示列列表
        /// </summary>
        [JsonPropertyName("columns")]
        public List<string> Columns { get; set; }

        /// <summary>
        /// 条件列表
        /// </summary>
        [JsonPropertyName("filters")]
        public List<Step22Filter> Filters { get; set; }

        /// <summary>
        /// 业务截断数量
        /// </summary>
        public int? limit { get; set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        [JsonPropertyName("pagination")]
        public Step22Pagination Pagination { get; set; }

        /// <summary>
        /// 排序列表
        /// </summary>
        [JsonPropertyName("sorts")]
        public List<Step22Sort> Sorts { get; set; }
    }


    /// <summary>
    /// 业务对象
    /// </summary>
    public class Step22BusinessObject
    {
        /// <summary>
        /// 业务对象名
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 是否拥有全量字段
        /// </summary>
        [JsonPropertyName("has_full")]
        public bool HasFull { get; set; }

        /// <summary>
        /// 全量字段名
        /// </summary>
        [JsonPropertyName("full_column")]
        public string FullColumn { get; set; }
    }

    /// <summary>
    /// 维度
    /// </summary>
    public class Step22Dimension
    {
        /// <summary>
        /// 维度类型：显示|隐式
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// 维度名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 维度标签
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
    }

    /// <summary>
    /// 指标
    /// </summary>
    public class Step22Metric
    {
        /// <summary>
        /// 指标名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 指标标签：计数|计算
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
    }


    /// <summary>
    /// 条件
    /// </summary>
    public class Step22Filter
    {
        /// <summary>
        /// 条件字段类型：显式|隐式
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// 条件字段标签
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// 条件字段名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 分页信息
    /// </summary>
    public class Step22Pagination
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int size { get; set; }
    }

    /// <summary>
    /// 排序
    /// </summary>
    public class Step22Sort
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        [JsonPropertyName("by")]
        public string By { get; set; }

        /// <summary>
        /// 排序标签
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// 排序方向：DESC|ASC
        /// </summary>
        [JsonPropertyName("order")]
        public string Order { get; set; }

        /// <summary>
        /// 排序类型：显示|隐式|无效
        /// </summary>
        public string type { get; set; }
    }
}
