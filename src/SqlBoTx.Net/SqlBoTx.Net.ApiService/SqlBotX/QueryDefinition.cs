using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class QueryDefinition
    {
        [JsonPropertyName("ambiguity_resolution")]
        public List<AmbiguityResolutionItem> AmbiguityResolution { get; set; }

        [JsonPropertyName("business_logic")]
        public BusinessLogic BusinessLogic { get; set; }
    }

    public class AmbiguityResolutionItem
    {
        [JsonPropertyName("term")]
        public string Term { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("assumption")]
        public string Assumption { get; set; }
    }

    public class BusinessLogic
    {
        [JsonPropertyName("main_query")]
        public string MainQuery { get; set; }

        [JsonPropertyName("contrast_case")]
        public string ContrastCase { get; set; }

        [JsonPropertyName("data_validity_filters")]
        public string DataValidityFilters { get; set; }
    }
}
