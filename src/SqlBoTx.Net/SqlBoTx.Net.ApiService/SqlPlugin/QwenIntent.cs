using OpenTelemetry.Metrics;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlPlugin
{
    public class QwenIntent
    {
        [JsonPropertyName("query_tasks")]
        public List<QueryTask> QueryTasks { get; set; } = new List<QueryTask>();
    }

    public class QueryTask
    {
        [JsonPropertyName("domain")]
        public string Domain { get; set; } // A preferred domain OR a descriptive phrase

        [JsonPropertyName("intent")]
        public IntentType Intent { get; set; }

        [JsonPropertyName("target_core")]
        public string TargetCore { get; set; } // A concise description of the user's goal

        [JsonPropertyName("filters")]
        public FilterNode? Filters { get; set; } // Nullable, as filters might not be present

        [JsonPropertyName("dimensions")]
        public List<Dimension> Dimensions { get; set; } = new List<Dimension>();

        [JsonPropertyName("metrics")]
        public List<Metric> Metrics { get; set; } = new List<Metric>();

        [JsonPropertyName("output_fields")]
        public List<string> OutputFields { get; set; } = new List<string>();

        [JsonPropertyName("sort_by")]
        public List<string> SortBy { get; set; } = new List<string>();

        [JsonPropertyName("limit")]
        public string? Limit { get; set; }
    }

    public enum IntentType
    {
        [JsonPropertyName("ANALYTIC")]
        ANALYTIC,
        [JsonPropertyName("RETRIEVAL")]
        RETRIEVAL
    }
    public class FilterNode
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } // "group" or "condition"

        // === 以下是FilterGroup特有的属性 ===
        [JsonPropertyName("logic")]
        public LogicType? Logic { get; set; } // Only for Type == "group"

        [JsonPropertyName("children")]
        public List<FilterNode>? Children { get; set; } // Only for Type == "group"

        // === 以下是FilterCondition特有的属性 ===
        [JsonPropertyName("field")]
        public string? Field { get; set; } // Only for Type == "condition"

        [JsonPropertyName("operator")]
        public string? Operator { get; set; } // Only for Type == "condition"

        [JsonPropertyName("value")]
        public List<string>? Value { get; set; } // Only for Type == "condition", always an array of strings
    }

    public enum LogicType
    {
        [JsonPropertyName("AND")]
        AND,
        [JsonPropertyName("OR")]
        OR
    }

    public class Dimension
    {
        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("function")]
        public string Function { get; set; } // Natural language description
    }

    public class Metric
    {
        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("function")]
        public string Function { get; set; } // Natural language description
    }
}
