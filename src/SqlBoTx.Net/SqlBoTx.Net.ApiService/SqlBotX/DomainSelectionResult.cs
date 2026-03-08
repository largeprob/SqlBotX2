using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class DomainSelectionResult
    {
        public class SelectedDomain
        {
            /// <summary>
            /// 业务域唯一标识
            /// </summary>
            [JsonPropertyName("domains_id")]
            public int? DomainsId { get; set; }

            /// <summary>
            /// 业务域名称
            /// </summary>
            [JsonPropertyName("domains_name")]
            public string? DomainsName { get; set; }

            /// <summary>
            /// 选择此业务域的理由
            /// </summary>
            [JsonPropertyName("selection_reason")]
            public string? SelectionReason { get; set; }
        }

        /// <summary>
        /// 思考内容/解释为什么
        /// </summary>
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        /// <summary>
        /// 选中的业务域列表
        /// </summary>
        [JsonPropertyName("selected_domains")]
        public List<SelectedDomain> SelectedDomains { get; set; } = new List<SelectedDomain>();
    }
}
