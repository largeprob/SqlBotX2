using Microsoft.AspNetCore.Components.Server;
using Newtonsoft.Json.Linq;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings;
using SqlBoTx.Net.Application.Vectors;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlBotX.Tools
{
    public class GetBusinessTerminology
    {

        public class Parameters
        {
            [JsonPropertyName("key")]
            [ToolParameter(Description = "业务域Id")]
            public required int Key { get; init; }
        }

    }


    public class GetstandardCalculationFormulay
    {
        public string Execute()
        {
            return @"
| 类型 | 公式 |
|---|---|
| 聚合类 |SUM、AVG、COUNT、MAX、MIN、Stdev、Stdevp、Var、Varp、ApproxCountDistinct |
| 周期对比类 | 同比 = (本期 - 去年同期) / 去年同期 × 100 |
| 周期对比类 | 环比 = (本期 - 上期) / 上期 × 100 |
| 周期对比类 | 定基比 = 比较期 / 固定基期 × 100 |
| 周期对比类 | 占比 = 分项 / 总项 × 100 |
";
        }

        public class Parameters
        {
        }
    }

    public class QueryVectorFieldsOfDomain
    {
        private readonly QdrantVectorService _qdrantVectorService;

        public QueryVectorFieldsOfDomain(QdrantVectorService qdrantVectorService)
        {
            _qdrantVectorService = qdrantVectorService;
        }

        public async Task<string> QueryVector(Parameters parameters)
        {
            var fields = parameters.Fields;

            string toolResult = "\n";
            int count = 1;
            foreach (var field in fields)
            {
                var vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", field,
                    filter: x => x.ObjectiveMetaDataId == parameters.Id)).Where(x => x.Score > 0.6);
                if (vc == null || vc.Count() <= 0) continue;
                toolResult += $"{count}. {field}：\n";
                count++;
                foreach (var v in vc)
                {
                    var record = v.Record;
                    toolResult += $"    - [{record.MetaDataName}]，Score [{v.Score}]";

                    if (record.SemanticType != null)
                    {
                        toolResult += $"，类型为 [{record.SemanticType}]";
                    }
                    if (!string.IsNullOrEmpty(record.MetaDataDescription))
                    {
                        toolResult += $"，Description [{record.MetaDataDescription}]。\n";
                    }
                    toolResult += "\n";
                }
            }
            return toolResult;
        }

        public class Parameters
        {
            [JsonPropertyName("id")]
            [ToolParameter(Description = "领域Id")]
            public required int Id { get; init; }

            [JsonPropertyName("fields")]
            [ToolParameter(Description = "字段/指标名")]
            public required List<string> Fields { get; init; }
        }
    }


    public class QueryVectorFieldsGlobal
    {
        private readonly QdrantVectorService _qdrantVectorService;

        public QueryVectorFieldsGlobal(QdrantVectorService qdrantVectorService)
        {
            _qdrantVectorService = qdrantVectorService;
        }

        public async Task<string> QueryVector(Parameters parameters)
        {
            var fields = parameters.Fields;

            string toolResult = "\n";
            int count = 1;
            foreach (var field in fields)
            {
                var vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", field))
                    .Where(x => x.Score > 0.6);
                if (vc == null || vc.Count() <= 0) continue;
                toolResult += $"{count}. {field}：\n";
                count++;
                foreach (var v in vc)
                {
                    var record = v.Record;
                    toolResult += $"    - [{record.ObjectiveMetaDataName}域] 的 [{record.MetaDataName}]，Score [{v.Score}]";

                    if (record.SemanticType != null)
                    {
                        toolResult += $"，类型为 [{record.SemanticType}]";
                    }
                    if (!string.IsNullOrEmpty(record.MetaDataDescription))
                    {
                        toolResult += $"，Description [{record.MetaDataDescription}]。\n";
                    }
                    toolResult += "\n";
                }
            }
            return toolResult;
        }

        public class Parameters
        {
            [JsonPropertyName("fields")]
            [ToolParameter(Description = "字段/指标名")]
            public required List<string> Fields { get; init; }
        }
    }


    public class AskUser
    {
        private readonly QdrantVectorService _qdrantVectorService;
        private readonly ILogger<AskUser>  _logger;

        public AskUser(QdrantVectorService qdrantVectorService, ILogger<AskUser> logger)
        {
            _qdrantVectorService = qdrantVectorService;
            _logger = logger;
        }

        public async Task<string> Execute(Parameters parameters)
        {
            // SSE通知用户
            _logger.LogInformation(parameters.Text);

            return "";
        }

        public class Parameters
        {
            [JsonPropertyName("text")]
            [ToolParameter(Description = "内容")]
            public required string Text { get; init; }
        }
    }
}
