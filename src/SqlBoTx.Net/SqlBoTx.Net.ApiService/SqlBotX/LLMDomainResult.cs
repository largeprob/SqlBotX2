using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class LLMDomainResult
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
        public List<LLMDomainBusinessObject> BusinessObjects { get; set; }
    }

    public class LLMDomainBusinessObject {

        /// <summary>
        /// 用户目标的简练描述
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("has_full")]
        public bool HasFull { get; set; }

        [JsonPropertyName("full_according")]
        public string? FullAccording{ get; set; }

        /// <summary>
        /// 用户目标的简练描述
        /// </summary>
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}
