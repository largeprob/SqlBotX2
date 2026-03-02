using SqlBoTx.Net.ApiService.SqlBotX.SqlBuilder.Models;
using SqlBoTx.Net.ApiService.SqlPlugin;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    /// <summary>
    /// 意图枚举
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IntentType
    {
        [JsonPropertyName("ANALYTIC")]
        ANALYTIC,

        [JsonPropertyName("DATA")]
        Data
    }

    /// <summary>
    /// 逻辑运算符枚举
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LogicOperator
    {
        [JsonPropertyName("AND")]
        AND,

        [JsonPropertyName("OR")]
        OR
    }

    /// <summary>
    /// 字段类型枚举
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FieldType
    {
        [JsonPropertyName("显式")]
        显式,

        [JsonPropertyName("隐式")]
        隐式
    }

    /// <summary>
    /// 字段标签枚举
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FieldTag
    {
        [JsonPropertyName("地区")]
        地区,

        [JsonPropertyName("部门")]
        部门,

        [JsonPropertyName("组织")]
        组织,

        [JsonPropertyName("角色")]
        角色,

        [JsonPropertyName("日期/时间")]
        日期时间,

        [JsonPropertyName("分类")]
        分类,

        [JsonPropertyName("其他")]
        其他
    }

    /// <summary>
    /// 字段运算符枚举
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FieldOperator
    {
        [JsonPropertyName("等于")]
        等于,

        [JsonPropertyName("不等于")]
        不等于,

        [JsonPropertyName("大于")]
        大于,

        [JsonPropertyName("小于")]
        小于,

        [JsonPropertyName("大于等于")]
        大于等于,

        [JsonPropertyName("小于等于")]
        小于等于,

        [JsonPropertyName("包含")]
        包含,

        [JsonPropertyName("在列表")]
        在列表,

        [JsonPropertyName("区间")]
        区间,

        [JsonPropertyName("前匹配")]
        前匹配,

        [JsonPropertyName("后匹配")]
        后匹配,

        [JsonPropertyName("其他")]
        其他
    }

    /// <summary>
    /// 过滤条件基类
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "filter_type")]
    [JsonDerivedType(typeof(FilterGroup), typeDiscriminator: "group")]
    [JsonDerivedType(typeof(FilterCondition), typeDiscriminator: "condition")]
    public abstract class FilterBase
    {
        [JsonPropertyName("filter_type")]
        public abstract string FilterType { get; }
    }

    /// <summary>
    /// 过滤条件组
    /// </summary>
    public class FilterGroup : FilterBase
    {
        public override string FilterType => "group";

        [JsonPropertyName("logic")]
        public LogicOperator Logic { get; set; }

        [JsonPropertyName("children")]
        public List<FilterBase> Children { get; set; } = new();
    }

    /// <summary>
    /// 过滤条件
    /// </summary>
    public class FilterCondition : FilterBase
    {
        public override string FilterType => "condition";

        [JsonPropertyName("field_type")]
        public FieldType FieldType { get; set; }

        [JsonPropertyName("field_tag")]
        public FieldTag FieldTag { get; set; }

        [JsonPropertyName("field_name")]
        public string FieldName { get; set; } = string.Empty;

        [JsonPropertyName("field_operator")]
        public FieldOperator FieldOperator { get; set; }

        [JsonPropertyName("field_value")]
        public List<string> FieldValue { get; set; } = new();
    }

    /// <summary>
    /// 分页信息
    /// </summary>
    public class PaginationInfo
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }
    }

    /// <summary>
    /// 选择字段
    /// </summary>
    public class SelectField
    {
        [JsonPropertyName("from_what")]
        public string From { get; set; } = string.Empty;

        [JsonPropertyName("what")]
        public string[] What { get; set; }
    }

    /// <summary>
    /// 字段内容
    /// </summary>
    public class WhatField
    {
        [JsonPropertyName("field_name")]
        public string FieldName { get; set; } = string.Empty;
    }


    public class Dimensions
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("tag")]
        public string Tag { get; set; } = string.Empty;
    }

    public class Metrics
    {
        public class MetricsFilter
        {
            [JsonPropertyName("operator")]
            public FieldOperator? Operator { get; set; }

            [JsonPropertyName("value")]
            public string[]? Value { get; set; }
 
        }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("dependent_dims")]
        public string[]? DependentDims { get; set; } 

        [JsonPropertyName("filters")]
        public MetricsFilter? Filters { get; set; }
    }

    /// <summary>
    /// 模糊意图结果
    /// </summary>
    public class FuzzyIntentResult
    {
        [JsonPropertyName("intent")]
        public IntentType Intent { get; set; }

        [JsonPropertyName("target_core")]
        public string TargetCore { get; set; } = string.Empty;

        [JsonPropertyName("global_filters")]
        public FilterBase? GlobalFilters { get; set; }


        [JsonPropertyName("dimensions")]
        public Dimensions?  Dimensions{ get; set; }

        [JsonPropertyName("metrics")]
        public Metrics[]? Metrics { get; set; }

        [JsonPropertyName("select")]
        public List<SelectField> Select { get; set; } = new();

        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("pagination")]
        public PaginationInfo Pagination { get; set; } = new();
    }




    public static class GlobalFiltersHelper
    {

        public static List<FilterCondition> FlattenFilters( FilterBase filter)
        {
            var result = new List<FilterCondition>();
            FlattenRecursive(filter, result);
            return result;
        }
        private static void FlattenRecursive(FilterBase node, List<FilterCondition> result)
        {
            if (node is FilterCondition cond)
            {
                result.Add(cond);
            }
            else if (node is FilterGroup group)
            {
                foreach (var child in group.Children)
                {
                    FlattenRecursive(child, result);
                }
            }
        }

        /// <summary>
        /// 根据过滤条件树生成SQL WHERE子句（不含WHERE关键字）
        /// </summary>
        public static string BuildSqlWhere(this FilterBase filter)
        {
            if (filter == null) return string.Empty;
            return BuildSqlRecursive(filter);
        }

        private static string BuildSqlRecursive(FilterBase node)
        {
            if (node is FilterCondition cond)
            {
                return BuildConditionSql(cond);
            }
            else if (node is FilterGroup group)
            {
                if (group.Children.Count == 0) return string.Empty;

                var childSqls = new List<string>();
                foreach (var child in group.Children)
                {
                    var sql = BuildSqlRecursive(child);
                    if (!string.IsNullOrEmpty(sql))
                        childSqls.Add(sql);
                }

                if (childSqls.Count == 0) return string.Empty;

                // 用逻辑符连接子句，并加上括号
                string logic = group.Logic == LogicOperator.AND ? "AND" : "OR";
                return $"({string.Join($" {logic} ", childSqls)})";
            }

            return string.Empty;
        }

        private static string BuildConditionSql(FilterCondition cond)
        {
            // 字段名（需要根据实际情况决定是否加引号或转义，这里假设字段名是合法的）
            string fieldName = cond.FieldName;

            // 根据运算符生成SQL片段
            switch (cond.FieldOperator)
            {
                case FieldOperator.等于:
                    return $"{fieldName} = {FormatValue(cond.FieldValue[0])}";
                case FieldOperator.不等于:
                    return $"{fieldName} != {FormatValue(cond.FieldValue[0])}";
                case FieldOperator.大于:
                    return $"{fieldName} > {FormatValue(cond.FieldValue[0])}";
                case FieldOperator.小于:
                    return $"{fieldName} < {FormatValue(cond.FieldValue[0])}";
                case FieldOperator.大于等于:
                    return $"{fieldName} >= {FormatValue(cond.FieldValue[0])}";
                case FieldOperator.小于等于:
                    return $"{fieldName} <= {FormatValue(cond.FieldValue[0])}";
                case FieldOperator.包含:
                case FieldOperator.在列表:
                    // 对于IN，需要将多个值格式化
                    var valueList = cond.FieldValue.Select(FormatValue);
                    return $"{fieldName} IN ({string.Join(", ", valueList)})";
                case FieldOperator.区间:
                    if (cond.FieldValue.Count >= 2)
                    {
                        return $"{fieldName} BETWEEN {FormatValue(cond.FieldValue[0])} AND {FormatValue(cond.FieldValue[1])}";
                    }
                    break;
                case FieldOperator.前匹配:
                    return $"{fieldName} LIKE {FormatLikeValue(cond.FieldValue[0], isPrefix: true)}";
                case FieldOperator.后匹配:
                    return $"{fieldName} LIKE {FormatLikeValue(cond.FieldValue[0], isPrefix: false)}";
                case FieldOperator.其他:
                default:
                    // 对于无法映射的运算符，返回1=1（无过滤）或抛出异常
                    return "1=1";
            }

            return "1=1"; // 兜底
        }

        /// <summary>
        /// 格式化值，对字符串加单引号，数字不加（这里简化处理，假设所有值都是字符串）
        /// </summary>
        private static string FormatValue(string value)
        {
            // 简单转义单引号
            string escaped = value.Replace("'", "''");
            return $"'{escaped}'";
        }

        /// <summary>
        /// 格式化LIKE值
        /// </summary>
        private static string FormatLikeValue(string value, bool isPrefix)
        {
            string escaped = value.Replace("'", "''").Replace("%", "\\%").Replace("_", "\\_");
            string likePattern = isPrefix ? $"{escaped}%" : $"%{escaped}";
            return $"'{likePattern}'";
        }


    }
}