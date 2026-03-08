using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class ColumnDomainMappingResult
    {
        /// <summary>
        /// 字段-业务域映射列表
        /// </summary>
        [JsonPropertyName("column_domain_mapping")]  
        public List<ColumnDomainMappingItem> ColumnDomainMapping { get; set; } = new List<ColumnDomainMappingItem>();
    }

    /// <summary>
    /// 单个字段-业务域映射项，对应数组中的元素
    /// </summary>
    public class ColumnDomainMappingItem
    {
        /// <summary>
        /// 字段名称（字符串类型）
        /// </summary>
        [JsonPropertyName("column")] // Newtonsoft.Json 特性
                                 // [JsonPropertyName("column")] // System.Text.Json 特性
        public string Column { get; set; }

        /// <summary>
        /// 范围类型（固定值：atom/full）
        /// </summary>
        [JsonPropertyName("range_type")] // Newtonsoft.Json 特性
                                     // [JsonPropertyName("range_type")] // System.Text.Json 特性
        public string RangeType { get; set; }

        /// <summary>
        /// 映射是否成功（布尔类型，而非字符串）
        /// </summary>
        [JsonPropertyName("success")] // Newtonsoft.Json 特性
                                  // [JsonPropertyName("success")] // System.Text.Json 特性
        public bool Success { get; set; }

        /// <summary>
        /// 业务域名称（字符串类型）
        /// </summary>
        [JsonPropertyName("domain_name")] // Newtonsoft.Json 特性
                                      // [JsonPropertyName("domain_name")] // System.Text.Json 特性
        public string DomainName { get; set; }

        /// <summary>
        /// 业务域ID（数值类型）
        /// </summary>
        [JsonPropertyName("domain_id")] // Newtonsoft.Json 特性
                                    // [JsonPropertyName("domain_id")] // System.Text.Json 特性
        public int DomainId { get; set; } // 若ID可能为小数，可改为 double 类型

        /// <summary>
        /// 思考内容/映射理由
        /// </summary>
        [JsonPropertyName("reason")] // Newtonsoft.Json 特性
                                 // [JsonPropertyName("reason")] // System.Text.Json 特性
        public string Reason { get; set; }
    }
}
