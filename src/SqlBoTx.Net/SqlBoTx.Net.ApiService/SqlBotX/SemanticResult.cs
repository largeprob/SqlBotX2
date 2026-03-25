using SqlBoTx.Net.ApiService.SqlPlugin;
using System.Text.Json.Serialization;
using static SqlBoTx.Net.ApiService.SqlBotX.DomainSelectionResult;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class SemanticResult
    {
        [JsonPropertyName("选择的业务域")]
        public List<SelectedDomain> SelectedDomains { get; set; } = new();

        [JsonPropertyName("查询类型")]
        public string QueryMode { get; set; } = string.Empty;

        [JsonPropertyName("分组维度")]
        public List<Dimension> Dimensions { get; set; } = new();

        [JsonPropertyName("CTE周期")]
        public List<CtePeriod> CtePeriods { get; set; } = new();
    }

    public class SelectedDomain
    {
        [JsonPropertyName("domains_id")]
        public int DomainsId { get; set; }

        [JsonPropertyName("domains_name")]
        public string DomainsName { get; set; } = string.Empty;

        [JsonPropertyName("has_full")]
        public bool HasFull { get; set; }

        [JsonPropertyName("full_according")]
        public string FullAccording { get; set; } = string.Empty;

        [JsonPropertyName("selection_reason")]
        public string SelectionReason { get; set; } = string.Empty;
    }

    public class Dimension
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("granularity")]
        public string Granularity { get; set; } = string.Empty;

        [JsonPropertyName("alias")]
        public string Alias { get; set; } = string.Empty;
    }
    public class CtePeriod
    {
        [JsonPropertyName("cte_label")]
        public string CteLabel { get; set; } = string.Empty;

        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;

        [JsonPropertyName("depends_dimensions")]
        public List<string> DependsDimensions { get; set; } = new();

        [JsonPropertyName("where嵌套额外的过滤条件")]
        public WhereCondition? WhereCondition { get; set; }
    }


    public class WhereCondition
    {
        [JsonPropertyName("logic")]
        public string Logic { get; set; } = string.Empty;

        [JsonPropertyName("children")]
        public List<ConditionNode> Children { get; set; } = new();
    }

    public class ConditionNode
    {
        [JsonPropertyName("logic")]
        public string? Logic { get; set; }

        [JsonPropertyName("children")]
        public List<ConditionNode>? Children { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("operator")]
        public string? Operator { get; set; }

        [JsonPropertyName("value")]
        public List<string>? Value { get; set; }
    }
}
