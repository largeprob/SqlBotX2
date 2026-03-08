using Newtonsoft.Json;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    /// <summary>
    /// 指标选择结果主类，对应整个JSON根结构
    /// </summary>
    public class MetricSelectionResult
    {
        /// <summary>
        /// 选中的指标项类，对应selected_metrics数组中的单个元素
        /// </summary>
        public class SelectedMetric
        {
            /// <summary>
            /// 指标名称
            /// </summary>
            [JsonProperty("metric_name")] 
            public string? MetricName { get; set; }

            /// <summary>
            /// 字段唯一标识
            /// </summary>
            [JsonProperty("selected_field_Id")]
            public string? SelectedFieldId { get; set; }

            /// <summary>
            /// 是否选中（布尔类型，而非字符串）
            /// </summary>
            [JsonProperty("is_selected")] 
            public bool IsSelected { get; set; }

            /// <summary>
            /// 选择此指标的理由（成功/失败理由）
            /// </summary>
            [JsonProperty("selection_reason")]
            public string? SelectionReason { get; set; }
        }

        /// <summary>
        /// 思考内容/解释为什么
        /// </summary>
        [JsonProperty("reason")]
        public string? Reason { get; set; }

        /// <summary>
        /// 选中的指标列表
        /// </summary>
        [JsonProperty("selected_metrics")] 
        public List<SelectedMetric> SelectedMetrics { get; set; } = new List<SelectedMetric>();
    }
}
