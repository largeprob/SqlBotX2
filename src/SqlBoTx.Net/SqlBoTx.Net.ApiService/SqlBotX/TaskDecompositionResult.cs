using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class TaskDecompositionResult
    {

        [JsonPropertyName("original_input")]
        public string OriginalInput { get; set; }
        [JsonPropertyName("is_split")]
        public bool IsSplit { get; set; }
        [JsonPropertyName("split_reason")]
        public string SplitReason { get; set; }
        [JsonPropertyName("tasks")]
        public List<string> Tasks { get; set; } = new List<string>();

    }
}
