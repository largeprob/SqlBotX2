using System.Text;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{

    /// <summary>
    /// 分析请求根对象
    /// </summary>
    public class Step5Result
    {
        /// <summary>
        /// 维度字段列表
        /// </summary>
        [JsonPropertyName("dimensions")]
        public List<string> Dimensions { get; set; } = new List<string>();

        /// <summary>
        /// 查询字段列表（包含维度和指标）
        /// </summary>
        [JsonPropertyName("fields")]
        public List<string> Fields { get; set; } = new List<string>();

        /// <summary>
        /// 查询条件表达式
        /// </summary>
        [JsonPropertyName("conditions")]
        public string? Conditions { get; set; }

        /// <summary>
        /// 排序字段列表
        /// </summary>
        [JsonPropertyName("order_by")]
        public List<string> OrderBy { get; set; } = new List<string>();

        /// <summary>
        /// 限制条件描述
        /// </summary>
        [JsonPropertyName("limit")]
        public string? Limit { get; set; }

        /// <summary>
        /// 计算字段定义
        /// </summary>
        [JsonPropertyName("calculations")]
        public List<CalculationDefinition> Calculations { get; set; } = new List<CalculationDefinition>();
    }

    /// <summary>
    /// 计算字段类型枚举
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CalculationFieldType
    {
        /// <summary>
        /// 维度字段
        /// </summary>
        dimension,

        /// <summary>
        /// 指标字段
        /// </summary>
        metric
    }

    /// <summary>
    /// 计算类型枚举
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CalculationType
    {
        /// <summary>
        /// 标准计算
        /// </summary>
        standard,

        /// <summary>
        /// 自定义计算
        /// </summary>
        custom
    }

    /// <summary>
    /// 计算字段定义
    /// </summary>
    public class CalculationDefinition
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [JsonPropertyName("field_name")]
        [JsonRequired]
        public string FieldName { get; set; } = string.Empty;

        /// <summary>
        /// 字段类型（维度/指标）
        /// </summary>
        [JsonPropertyName("field_type")]
        [JsonRequired]
        public CalculationFieldType FieldType { get; set; }

        /// <summary>
        /// 计算类型（仅指标使用）
        /// </summary>
        [JsonPropertyName("calc_type")]
        public CalculationType? CalcType { get; set; }

        /// <summary>
        /// 计算公式（仅指标使用）
        /// </summary>
        [JsonPropertyName("formula")]
        public string? Formula { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// 计算参数
        /// </summary>
        [JsonPropertyName("parameters")]
        public CalculationParameters? Parameters { get; set; }

        /// <summary>
        /// 验证计算定义是否有效
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(FieldName))
                return false;

            if (FieldType == CalculationFieldType.metric)
            {
                // 指标字段必须要有计算公式或指定标准计算
                if (string.IsNullOrWhiteSpace(Formula) && CalcType != CalculationType.standard)
                    return false;
            }
            else
            {
                // 维度字段通常没有公式
                if (!string.IsNullOrWhiteSpace(Formula) || CalcType.HasValue)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 创建维度字段定义的便捷方法
        /// </summary>
        public static CalculationDefinition CreateDimension(
            string fieldName,
            string? description = null)
        {
            return new CalculationDefinition
            {
                FieldName = fieldName,
                FieldType = CalculationFieldType.dimension,
                Description = description
            };
        }

        /// <summary>
        /// 创建指标字段定义的便捷方法
        /// </summary>
        public static CalculationDefinition CreateMetric(
            string fieldName,
            CalculationType calcType,
            string? formula = null,
            string? description = null,
            CalculationParameters? parameters = null)
        {
            return new CalculationDefinition
            {
                FieldName = fieldName,
                FieldType = CalculationFieldType.metric,
                CalcType = calcType,
                Formula = formula,
                Description = description,
                Parameters = parameters
            };
        }

        /// <summary>
        /// 创建标准指标字段定义的便捷方法
        /// </summary>
        public static CalculationDefinition CreateStandardMetric(
            string fieldName,
            string? description = null,
            CalculationParameters? parameters = null)
        {
            return CreateMetric(fieldName, CalculationType.standard, null, description, parameters);
        }

        /// <summary>
        /// 创建自定义指标字段定义的便捷方法
        /// </summary>
        public static CalculationDefinition CreateCustomMetric(
            string fieldName,
            string formula,
            string? description = null,
            CalculationParameters? parameters = null)
        {
            return CreateMetric(fieldName, CalculationType.custom, formula, description, parameters);
        }
    }

    /// <summary>
    /// 计算参数
    /// </summary>
    public class CalculationParameters
    {
        /// <summary>
        /// 依赖的基础字段列表
        /// </summary>
        [JsonPropertyName("base_field")]
        public List<string>? BaseField { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [JsonPropertyName("unit")]
        public string? Unit { get; set; }

        /// <summary>
        /// 其他扩展参数
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object>? ExtensionData { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CalculationParameters() { }

        /// <summary>
        /// 创建计算参数的便捷方法
        /// </summary>
        public static CalculationParameters Create(
            List<string>? baseField = null,
            string? unit = null)
        {
            return new CalculationParameters
            {
                BaseField = baseField,
                Unit = unit
            };
        }
    }

 


}
