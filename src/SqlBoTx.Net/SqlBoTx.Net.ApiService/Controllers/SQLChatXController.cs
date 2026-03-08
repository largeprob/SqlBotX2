
using ImTools;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;
using OpenAI.Chat;
using OpenTelemetry.Metrics;
using Qdrant.Client.Grpc;
using SqlBoTx.Net.ApiService.Dto;
using SqlBoTx.Net.ApiService.SqlBotX;
using SqlBoTx.Net.ApiService.SqlBotX.SqlBuilder.Models;
using SqlBoTx.Net.ApiService.SqlPlugin;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings;
using SqlBoTx.Net.Application.Contracts.TableStructures;
using SqlBoTx.Net.Application.TableStructures;
using SqlBoTx.Net.Application.Vectors;
using SqlBoTx.Net.Domain.TableFields;
using System;
using System.ClientModel;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace SqlBoTx.Net.ApiService.Controllers
{

    public class BusinessVectorItem
    {
        public int ObjectiveId { get; set; }
        public List<ColumnVectorMatch> Columns { get; set; } = new List<ColumnVectorMatch>();
    }

    public class ColumnVectorMatch
    {
        /// <summary>
        /// 召回向量库的字段ID
        /// </summary>
        public int Id { get; set; }
        public string FieldName { get; set; }
    }


    /// <summary>
    /// sql Chat
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SQLChatXController : ControllerBase
    {
        private readonly SQLChatXService  _chatXService;
        private readonly SqlBotPlugin _plugin;
        private readonly Kernel _kernel;
        private readonly ILogger<Program> _logger;
        private readonly SqlServerDatabaseService  _sqlServerDatabaseService;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly IServiceProvider  _serviceProvider;
        private readonly QdrantVectorService _qdrantVectorService;
        private readonly IBusinessObjectiveService _businessObjectiveService;
        private readonly ITableRelationshipService _tableRelationshipService;
        private readonly ITableStructureService _tableStructureService;

        public SQLChatXController(SQLChatXService chatXService, SqlBotPlugin plugin, Kernel kernel, ILogger<Program> logger, SqlServerDatabaseService sqlServerDatabaseService, IChatCompletionService chatCompletionService, IServiceProvider serviceProvider, QdrantVectorService qdrantVectorService, IBusinessObjectiveService businessObjectiveService, ITableRelationshipService tableRelationshipService, ITableStructureService tableStructureService)
        {
            _chatXService = chatXService;
            _plugin = plugin;
            _kernel = kernel;
            _logger = logger;
            _sqlServerDatabaseService = sqlServerDatabaseService;
            _chatCompletionService = chatCompletionService;
            _serviceProvider = serviceProvider;
            _qdrantVectorService = qdrantVectorService;
            _businessObjectiveService = businessObjectiveService;
            _tableRelationshipService = tableRelationshipService;
            _tableStructureService = tableStructureService;
        }

        /// <summary>
        /// sql对话
        /// </summary>
        /// <param name="userMessage"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SQLChat([FromBody] SQLBotMessage userMessage, CancellationToken ct)
        {
            HttpContext.Response.ContentType = "text/event-stream";
            HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
            HttpContext.Response.Headers.Append("X-Accel-Buffering", "no");

            //打开会话
            var sessionId = Guid.CreateVersion7();
            await SendSessionAsync(HttpContext, sessionId, ct);

            //获取文本输入
            var userInput = (userMessage.Content[0] as ContentBlockText).Text;
 
            var splitTaskResult = await _chatXService.SplitTaskAsync(userInput);
            foreach (var task in splitTaskResult.Tasks)
            {
                // 模式确认
                var intentAnalysisResult = await _chatXService.Step1Async(task);
                if (intentAnalysisResult.Category == "CHIT_CHAT")
                {
                    await SendMessageAsync(HttpContext, new SQLBotMessage
                    {
                        SessionId = sessionId,
                        Id = Guid.CreateVersion7(),
                        Role = Role.Assistant,
                        Status = SQLBotMessageStatus.Done,
                        Content = new List<BaseContentBlock>{ new ContentBlockText()
                        {
                            Text = intentAnalysisResult.SuggestedReply!
                        } },
                        CreatedAt = new DateTime().Ticks,
                    }, ct);
                    return;
                }

                //TODO 锚点判断

                // 语义粗筛
                var semanticElementsResult = await _chatXService.Step2Async(userInput);

                #region 召回业务域  1000%没错
                // 召回业务域（宽表）
                var domains = new List<BusinessObjectiveEmbeddingModel>();
                foreach (var item in semanticElementsResult.BusinessObjects)
                {
                    // top K
                    var vc =
                    await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective", item.Name, top: 5);
                    if (vc.Count() == 0)
                    {
                        _logger.LogError("抱歉，系统中没有找到与‘{0}’对应的模块信息", item);
                        return;
                    }
                    domains.AddRange(vc.Select(x => x.Record));
                }

                // 数据库
                var domainsOfDb = await _businessObjectiveService.ListByIdAsync(domains.Select(x => x.MetaDataId).ToArray());

                //LLM 召回
                var selectDomainsPrompt = @"
# 业务域识别系统 (Business Domain Recognition System)

# Role
你是一个运行在SQLBotX中智能助手，你所处的阶段是 `识别业务域`以缩小查询范围。你的上一个阶段是 `向量业务域召回`，你的下一个阶段是：在你本次选择的业务域范围内进行 `向量指标召回`。
现在你将负责根据用户的自然语言查询，从给定的业务域列表中选择最相关的业务域。业务域是业务主题的逻辑分组，每个域包含相关的数据表。

# 用户原始查询
""{userInput}""

# 业务域列表
{domainsInfo}

# Core
1. 仔细阅读用户查询，理解其业务意图。
2. 从上述业务域列表中，选择与本次用户查询最相关的**1个或多个**业务域。
3. 如果查询明显涉及多个业务域，可以列出多个域，并按相关性从高到低排序。
4. 如果无法从列表中找到相关业务域，请在 `reason` 中解释为什么。
5. 只从给定列表中选择，不要创建新的业务域名称。

## Schema Definition
你的输出 **必须** 严格遵守此 JSON 结构。
```json
{
    ""reason"":""思考内容/解释为什么"",    
    ""selected_domains"":[
    {
      ""domains_id"": ""业务域唯一标识(Number)"",
      ""domains_name"": ""业务域名称"",
      ""selection_reason"": ""选择此业务域的理由""
    }] | []
}
```
";
                string domainStr = string.Empty;
                for (int i = 0; i < domainsOfDb.Count; i++)
                {
                    var domain = domainsOfDb[i];
                    var tableInfo = (domain.DependencyTables?.Any() == true)
                    ? string.Join("，", domain.DependencyTables.Select(x => $"{x.TableName}({x.Description})({x.Granularity})({x.GranularityLevelStr})"))
                    : null;
                    domainStr += $"{i + 1}. 业务域名称: {domain.BusinessName}";
                    domainStr += $"   - 唯一标识: {domain.Id}";
                    domainStr += $"   - 说明: {domain.Description}";
                    domainStr += $"   - 近义词: {domain.Synonyms}";
                    if (!string.IsNullOrEmpty(tableInfo))
                    {
                        domainStr += $"   - 关联表: {tableInfo}\n";
                    }
                }
                var domainSelectionResult = await _chatXService.SelectDomainAsync(selectDomainsPrompt);

                // LLM Result
                if (domainSelectionResult.SelectedDomains.Count == 0)
                {
                    _logger.LogError(domainSelectionResult.Reason);
                    return;
                }
                var selectedDomainIds = domainSelectionResult.SelectedDomains.Select(x => x.DomainsId).ToArray();
                var selectedDomains = domainSelectionResult.SelectedDomains.Select(x => new 
                {
                    Id = x.DomainsId,
                    Name = x.DomainsName,
                    HasFull = true
                });
                #endregion

                #region LLM进行指标解析
                // 指标字段、计算方式（标准指标计算方式(_，自定义指标计算方式）
                // 原子指标、复合指标
                #endregion


                #region 召回字段
                var fullColumns = semanticElementsResult.BusinessObjects.Where(x=>x.HasFull).Select(x => x.FullColumn);
                var metricColumns = semanticElementsResult.Metrics.Select(x => x.Name);
                var columns = semanticElementsResult.Columns.Where(x => !fullColumns.Contains(x) && !metricColumns.Contains(x));
                var dbColumns = new List<dynamic>();
                foreach (var column in columns)
                {
                    var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", column,
                        filter: x => selectedDomainIds.Contains(x.ObjectiveMetaDataId))).MaxBy(x => x.Score);
                    if (field_vc == null) {
                        _logger.LogError("未找到字段{clomun}", column);
                        continue;
                    }

                    //如果是全量字段，且所属业务域未被选中，则不予召回
                    if (selectedDomains.Where(x => x.HasFull == true && x.Id == field_vc.Record.ObjectiveMetaDataId).Count() > 0)
                    {
                        continue;
                    }

                    dbColumns.Add(new
                    {
                        Id = field_vc.Record.MetaDataId,
                        Name = field_vc.Record.MetaDataName,
                        DomainId = field_vc.Record.ObjectiveMetaDataId,
                        DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                    });
                }
                #endregion

                #region 召回指标
                var dbMetrics = new List<dynamic>();
                // 指标 召回
                if (semanticElementsResult.Metrics != null && semanticElementsResult.Metrics.Length >= 0)
                {
                    // 是不是通用指标


                    var selectMetricPrompt = new StringBuilder(@$"
# Role
你是一个智能数据查询助手。你的任务是根据用户的原始查询、已提取的指标，以及每个指标对应的候选物理字段信息，为**每个指标**选择一个最合适的字段。

# 上下文信息

## 用户原始查询
""{userInput}""

## 指标信息
");
                    for (int i = 0; i < semanticElementsResult.Metrics.Length; i++)
                    {
                        var metric = semanticElementsResult.Metrics[i];

                        // TODO 判断是否为自定义指标

                        var objectiveIds = domains.Select(x => x.MetaDataId).ToList();
                        var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", metric.Name,
                        filter: x => objectiveIds.Contains(x.ObjectiveMetaDataId) && x.MetaDataBusinesBIRole == 3)).Where(x => x.Score > 0.6).ToList();
                        if (field_vc == null || field_vc.Count() <= 0)
                        {
                            _logger.LogError("抱歉，系统中没有找到与‘{0}’相关的度量内容", metric.Name);
                            return;
                        }

                        selectMetricPrompt.AppendLine($"{i + 1}. 指标名称：{metric.Name}");
                        for (int j = 0; j < field_vc.Count(); j++)
                        {
                            var item = field_vc.ElementAt(j);
                            selectMetricPrompt.AppendLine($"    - `候选字段`:");
                            selectMetricPrompt.AppendLine($"        - `相似度评分`:{item.Score}");
                            selectMetricPrompt.AppendLine($"        - `唯一标识`:{item.Record.MetaDataId}");
                            selectMetricPrompt.AppendLine($"        - `物理名称`:{item.Record.MetaDataName}");
                            selectMetricPrompt.AppendLine($"        - `说明`:{item.Record.MetaDataDescription}");
                            selectMetricPrompt.AppendLine($"        - `字段业务域`: ");
                            selectMetricPrompt.AppendLine($"            - `名称`:{item.Record.ObjectiveMetaDataName}");
                            selectMetricPrompt.AppendLine($"            - `说明`:{item.Record.ObjectiveMetaDataDescription}");
                        }
                    }
                    selectMetricPrompt.AppendLine(@"
# Core
1. 对于每个指标，首先分析它的名称和含义，理解用户的查询意图。
2. 然后，评估每个候选字段与指标的相关性，考虑相似度评分、字段说明、以及字段所在业务域的信息。
3. 无论是否候选成功，都请给出选择理由，若没有候选成功，更要详细说明为什么没有找到合适的字段。

## Schema Definition
你的输出 **必须** 严格遵守此 JSON 结构:
```json
{
    ""reason"":""思考内容/解释为什么"",
    ""selected_metrics"":[
    {
      ""metric_name"": ""指标名称"",
      ""selected_field_Id"": ""字段唯一标识"",
      ""is_selected"": ""true/fasle"",
      ""selection_reason"": ""成功理由/失败理由""
    }]
}
```
");
                    var metricSelectionResult = await _chatXService.SelectMetricAsync(selectMetricPrompt.ToString());
                    var failedMetrics = metricSelectionResult.SelectedMetrics.Where(x => x.IsSelected == false).ToList();

                    // TODO 未找到的指标
                    _logger.LogError("未找到此指标信息，{0}", string.Join(",", failedMetrics.Select(x => x.MetricName)));
                    return;
                }

                #endregion





                // 字段召回

                var businessVectorItems = new List<BusinessVectorItem>();

                List<string> noWhatFild = new List<string>();

                // 召回业务域
                foreach (var item in semanticElementsResult.Select)
                {
                    // 业务域召回
                    var body_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective", item.From))
                        .Where(x => x.Score > 0.6).MaxBy(x => x.Score);
                    if (body_vc == null)
                    {
                        _logger.LogError("为找到与{0}的相似业务域信息", item.From);
                        return;
                    }

                    var body_record = body_vc.Record;
                    var businessObjective = new BusinessVectorItem() { ObjectiveId = body_vc.Record.MetaDataId };

                    // 字段召回
                    foreach (var field in item.What)
                    {
                        if (field == "ALL")
                        {
                            businessObjective.Columns.Add(new ColumnVectorMatch
                            {
                                FieldName = "ALL",
                            });
                            continue;
                        }

                        // 限定业务域-召回
                        var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", field,
                        filter: x => x.ObjectiveMetaDataId == body_vc.Record.MetaDataId)).Where(x => x.Score > 0.6).MaxBy(x => x.Score);
                        if (field_vc != null && businessObjective.Columns.Count(x => x.FieldName == "ALL") <= 0)
                        {
                       
                            businessObjective.Columns.Add(new ColumnVectorMatch
                            {
                                Id = field_vc.Record.MetaDataId,
                                FieldName = field,
                            });
                            continue;
                        }


                        // 召回隐藏域 例如字段中存在行业术语指向的某个特定域
                        var broad_result1 = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective", field)
                        ).Where(x => x.Score > 0.85).MaxBy(x => x.Score);
                        var broad_result2 = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveSynonymEmbeddingModel>("business_objective_synonyms", field)
                            ).Where(x => x.Score > 0.85).MaxBy(x => x.Score);
                        int? selectedRecordId = (broad_result1, broad_result2) switch
                        {
                            (null, null) => null,
                            (not null, null) => broad_result1.Record.MetaDataId,
                            (null, not null) => broad_result2.Record.ObjectiveMetaDataId,
                            (not null, not null) => broad_result1.Score >= broad_result2.Score ? (int)broad_result1.Record.Id : broad_result2.Record.ObjectiveMetaDataId
                        };
                        if (selectedRecordId != null)
                        {
                            var thisBody = businessVectorItems.Find(x => x.ObjectiveId == selectedRecordId);
                            if (thisBody != null)
                            {
                                thisBody.Columns = new List<ColumnVectorMatch> {
                                    new ColumnVectorMatch
                                    {
                                        FieldName = "ALL",
                                    }
                                };
                            }
                            else
                            {
                                businessVectorItems.Add(new BusinessVectorItem
                                {
                                    ObjectiveId = selectedRecordId.Value,
                                    Columns = new List<ColumnVectorMatch> {
                                    new ColumnVectorMatch
                                    {
                                        FieldName = "ALL",
                                    }
                                }
                                });
                            }
                            continue;
                        }


                        //全局召回
                        var field_all_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", field)).Where(x => x.Score > 0.6);
                        if (field_all_vc.Count() > 0)
                        {
                            var fields = field_all_vc.Where(x => x.Score == field_all_vc.Max(x => x.Score));
                            if (fields.Count() > 1)
                            {
                                //TODO 歧义字段  倒排索引加权重/业务域血缘
                                _logger.LogWarning("字段{0}存在歧义，召回了多个相似字段，分别是{1}，需要后续通过倒排索引加权重或者业务域血缘等方式进行筛选", field, string.Join(", ", fields.Select(x => x.Record.MetaDataName)));

                            }
                            else
                            {
                                var vector_field = fields.FirstOrDefault();
                                var thisBody = businessVectorItems.Find(x => x.ObjectiveId == vector_field.Record.ObjectiveMetaDataId);
                                if (thisBody != null)
                                {
                                    thisBody.Columns.Add(new ColumnVectorMatch
                                    {
                                        Id = vector_field.Record.MetaDataId,
                                        FieldName = field
                                    });
                                }
                                else
                                {
                                    businessVectorItems.Add(new BusinessVectorItem
                                    {

                                        ObjectiveId = vector_field.Record.ObjectiveMetaDataId,
                                        Columns = new List<ColumnVectorMatch> { new ColumnVectorMatch
                                    {
                                         Id = vector_field.Record.MetaDataId,
                                         FieldName = field
                                    }}
                                    });
                                }
                            }
                            continue;
                        }


                        //TODO 业务术语（高价值用户、极简用户）

                        //TODO 维度术语（日期/时间、地区、部门、角色）

                        // 未定义字段或者非法字段
                        noWhatFild.Add(field);
                    }
                }

                // 条件字段召回
                var domainIds = businessVectorItems.Select(x => x.ObjectiveId).ToList();
                var conditions = GlobalFiltersHelper.FlattenFilters(semanticElementsResult.GlobalFilters);
                foreach (var condition in conditions)
                {
                    if (condition.FieldType == SqlBotX.FieldType.显式)
                    {
                        var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", condition.FieldName,
                        filter: x => domainIds.Contains(x.ObjectiveMetaDataId))).Where(x => x.Score > 0.6).MaxBy(x => x.Score);

                        var thisBody = businessVectorItems.Find(x => x.ObjectiveId == field_vc.Record.ObjectiveMetaDataId);
                        if (thisBody != null)
                        {
                            thisBody.Columns.Add(new ColumnVectorMatch
                            {
                                Id = field_vc.Record.MetaDataId,
                                FieldName = condition.FieldName,
                            });
                        }
                        else
                        {
                            businessVectorItems.Add(new BusinessVectorItem
                            {
                                ObjectiveId = field_vc.Record.ObjectiveMetaDataId,
                                Columns = new List<ColumnVectorMatch> {
                                    new ColumnVectorMatch
                                    {
                                        Id = field_vc.Record.MetaDataId,
                                        FieldName = condition.FieldName,
                                    }
                             }
                            });
                        }
                    }


                    if (condition.FieldType == SqlBotX.FieldType.隐式)
                    {
                        //TODO  隐式标签
                    }

                    //TODO 业务术语（高价值用户、极简用户）

                    //TODO 维度术语（日期/时间、地区、部门、角色）

                    // 未定义字段或者非法字段
                    noWhatFild.Add(condition.FieldName);
                }

                //LLM 精细召回
            }


            //var splitTaskResult = await _chatXService.SplitTaskAsync(userInput);
            //foreach (var task in splitTaskResult.Tasks)
            //{
            //    // 模式确认
            //    var intentAnalysisResult = await _chatXService.Step1Async(task);
            //    if (intentAnalysisResult.Category == "CHIT_CHAT")
            //    {
            //        await SendMessageAsync(HttpContext, new SQLBotMessage
            //        {
            //            SessionId = sessionId,
            //            Id = Guid.CreateVersion7(),
            //            Role = Role.Assistant,
            //            Status = SQLBotMessageStatus.Done,
            //            Content = new List<BaseContentBlock>{ new ContentBlockText()
            //            {
            //                Text = intentAnalysisResult.SuggestedReply!
            //            } },
            //            CreatedAt = new DateTime().Ticks,
            //        }, ct);
            //        return;
            //    }

            //    //TODO 锚点判断

            //    // 语义粗筛
            //    var semanticElementsResult = await _chatXService.Step2Async(userInput);
            //    var businessVectorItems = new List<BusinessVectorItem>();

            //    List<string> noWhatFild = new List<string>();

            //    // 召回业务域
            //    foreach (var item in semanticElementsResult.Select)
            //    {
            //        // 业务域召回
            //        var body_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective", item.From))
            //            .Where(x => x.Score > 0.6).MaxBy(x => x.Score);
            //        if (body_vc == null)
            //        {
            //            _logger.LogError("为找到与{0}的相似业务域信息", item.From);
            //            return;
            //        }

            //        var body_record = body_vc.Record;
            //        var businessObjective = new BusinessVectorItem() { ObjectiveId = body_vc.Record.MataData.Id };

            //        // 字段召回
            //        foreach (var field in item.What)
            //        {
            //            if (field == "ALL")
            //            {
            //                businessObjective.Columns.Add(new ColumnVectorMatch
            //                {
            //                    FieldName = "ALL",
            //                });
            //                continue;
            //            }

            //            // 限定业务域-召回
            //            var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", field,
            //            filter: x => x.ObjectiveMataData.Id == body_vc.Record.MataData.Id)).Where(x => x.Score > 0.6).MaxBy(x => x.Score);
            //            if (field_vc != null && businessObjective.Columns.Count(x => x.FieldName == "ALL") <= 0)
            //            {
            //                var record = field_vc.Record.MataData;
            //                businessObjective.Columns.Add(new ColumnVectorMatch
            //                {
            //                    Id = field_vc.Record.MataData.Id,
            //                    FieldName = field,
            //                });
            //                continue;
            //            }


            //            // 召回隐藏域 例如字段中存在行业术语指向的某个特定域
            //            var broad_result1 = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective", field)
            //            ).Where(x => x.Score > 0.85).MaxBy(x => x.Score);
            //            var broad_result2 = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveSynonymEmbeddingModel>("business_objective_synonyms", field)
            //                ).Where(x => x.Score > 0.85).MaxBy(x => x.Score);
            //            int? selectedRecordId = (broad_result1, broad_result2) switch
            //            {
            //                (null, null) => null,
            //                (not null, null) => broad_result1.Record.MataData.Id,
            //                (null, not null) => broad_result2.Record.ObjectiveMataData.Id,
            //                (not null, not null) => broad_result1.Score >= broad_result2.Score ? (int)broad_result1.Record.Id : broad_result2.Record.ObjectiveMataData.Id
            //            };
            //            if (selectedRecordId != null)
            //            {
            //                var thisBody = businessVectorItems.Find(x => x.ObjectiveId == selectedRecordId);
            //                if (thisBody != null)
            //                {
            //                    thisBody.Columns = new List<ColumnVectorMatch> {
            //                        new ColumnVectorMatch
            //                        {
            //                            FieldName = "ALL",
            //                        }
            //                    };
            //                }
            //                else
            //                {
            //                    businessVectorItems.Add(new BusinessVectorItem
            //                    {
            //                        ObjectiveId = selectedRecordId.Value,
            //                        Columns = new List<ColumnVectorMatch> {
            //                        new ColumnVectorMatch
            //                        {
            //                            FieldName = "ALL",
            //                        }
            //                    }
            //                    });
            //                }
            //                continue;
            //            }


            //            //全局召回
            //            var field_all_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", field)).Where(x => x.Score > 0.6);
            //            if (field_all_vc.Count() > 0)
            //            {
            //                var fields = field_all_vc.Where(x => x.Score == field_all_vc.Max(x => x.Score));
            //                if (fields.Count() > 1)
            //                {
            //                    //TODO 歧义字段  倒排索引加权重/业务域血缘
            //                    _logger.LogWarning("字段{0}存在歧义，召回了多个相似字段，分别是{1}，需要后续通过倒排索引加权重或者业务域血缘等方式进行筛选", field, string.Join(", ", fields.Select(x => x.Record.MataData.Name)));

            //                }
            //                else
            //                {
            //                    var vector_field = fields.FirstOrDefault();
            //                    var thisBody = businessVectorItems.Find(x => x.ObjectiveId == vector_field.Record.ObjectiveMataData.Id);
            //                    if (thisBody != null)
            //                    {
            //                        thisBody.Columns.Add(new ColumnVectorMatch
            //                        {
            //                            Id = vector_field.Record.MataData.Id,
            //                            FieldName = field
            //                        });
            //                    }
            //                    else
            //                    {
            //                        businessVectorItems.Add(new BusinessVectorItem
            //                        {

            //                            ObjectiveId = vector_field.Record.ObjectiveMataData.Id,
            //                            Columns = new List<ColumnVectorMatch> { new ColumnVectorMatch
            //                        {
            //                             Id = vector_field.Record.MataData.Id,
            //                             FieldName = field
            //                        }}
            //                        });
            //                    }
            //                }
            //                continue;
            //            }


            //            //TODO 业务术语（高价值用户、极简用户）

            //            //TODO 维度术语（日期/时间、地区、部门、角色）

            //            // 未定义字段或者非法字段
            //            noWhatFild.Add(field);
            //        }
            //    }

            //    // 条件字段召回
            //    var domainIds = businessVectorItems.Select(x => x.ObjectiveId).ToList();
            //    var conditions = GlobalFiltersHelper.FlattenFilters(semanticElementsResult.GlobalFilters);
            //    foreach (var condition in conditions)
            //    {
            //        if (condition.FieldType == SqlBotX.FieldType.显式)
            //        {
            //            var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", condition.FieldName,
            //            filter: x => domainIds.Contains(x.ObjectiveMataData.Id))).Where(x => x.Score > 0.6).MaxBy(x => x.Score);

            //            var thisBody = businessVectorItems.Find(x => x.ObjectiveId == field_vc.Record.ObjectiveMataData.Id);
            //            if (thisBody != null)
            //            {
            //                thisBody.Columns.Add(new ColumnVectorMatch
            //                {
            //                    Id = field_vc.Record.MataData.Id,
            //                    FieldName = condition.FieldName,
            //                });
            //            }
            //            else
            //            {
            //                businessVectorItems.Add(new BusinessVectorItem
            //                {
            //                    ObjectiveId = field_vc.Record.ObjectiveMataData.Id,
            //                    Columns = new List<ColumnVectorMatch> {
            //                        new ColumnVectorMatch
            //                        {
            //                            Id = field_vc.Record.MataData.Id,
            //                            FieldName = condition.FieldName,
            //                        }
            //                 }
            //                });
            //            }
            //        }


            //        if (condition.FieldType == SqlBotX.FieldType.隐式)
            //        {
            //            //TODO  隐式标签
            //        }

            //        //TODO 业务术语（高价值用户、极简用户）

            //        //TODO 维度术语（日期/时间、地区、部门、角色）

            //        // 未定义字段或者非法字段
            //        noWhatFild.Add(condition.FieldName);
            //    }

            //    //LLM 精细召回
            //}
        }

        /// <summary>
        /// 建立新会话
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sessionId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendSessionAsync(HttpContext context, Guid sessionId, CancellationToken ct)
        {
            await context.Response.WriteAsync(
                $"event: session\n" +
                $"data: {sessionId}\n\n",
                ct);
            await context.Response.Body.FlushAsync(ct);
        }

        /// <summary>
        /// 发送消息体
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendMessageAsync(HttpContext context, SQLBotMessage message, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(message, message.GetType(), AgentJsonOptions.SSEJsonOptions);
            await context.Response.WriteAsync(
                $"event: message\n" +
                $"data: {json}\n\n",
                ct);
            await context.Response.Body.FlushAsync(ct);
        }

        /// <summary>
        /// 发送数据块
        /// </summary>
        /// <param name="context"></param>
        /// <param name="block"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendBlockAsync(HttpContext context, BaseContentBlock block, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize<BaseContentBlock>(block, AgentJsonOptions.SSEJsonOptions);

            //var bytes = Encoding.UTF8.GetBytes($"event: block\ndata: {json}\n\n");
            //await context.Response.Body.WriteAsync(bytes,ct);

            await context.Response.WriteAsync($"event: block\ndata: {json}\n\n", ct);
            await context.Response.Body.FlushAsync(ct);
        }

        /// <summary>
        /// 发送增量文本
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendDeltaAsync(HttpContext context, string text, CancellationToken ct)
        {
            await context.Response.WriteAsync(
                $"event: delta\n" +
                $"data: {text}\n\n",
                ct);
            await context.Response.Body.FlushAsync(ct);
        }
      

        #region 消息状态

        /// <summary>
        /// 推送消息状态
        /// </summary>
        /// <param name="context"></param>
        /// <param name="status"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendMessageStatusAsync(HttpContext context, CancellationToken ct, SSEMessageStatus status)
        {
            var json = JsonSerializer.Serialize(status, status.GetType(), AgentJsonOptions.SSEJsonOptions);

            await context.Response.WriteAsync(
                $"event: message-status\n" +
                $"data: {json}\n\n",
                ct);
            await context.Response.Body.FlushAsync(ct);
        }

        /// <summary>
        /// 标记此信息已经完成
        /// </summary>
        /// <param name="context"></param>
        /// <param name="str"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendMessageDoneAsync(HttpContext context, CancellationToken ct, string str = "思考完成")
        {
            await SendMessageStatusAsync(context, ct, new SSEMessageStatus
            {
                Status = SQLBotMessageStatus.Done,
                Str = str
            });
        }

        /// <summary>
        /// 标记此信息正在进行中
        /// </summary>
        /// <param name="context"></param>
        /// <param name="str"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendMessageStreamingAsync(HttpContext context, CancellationToken ct, Guid id, string str = "正在思考")
        {
            await SendMessageStatusAsync(context, ct, new SSEMessageStatus
            {
                Status = SQLBotMessageStatus.Streaming,
                Str = str
            });
        }

        /// <summary>
        /// 标记此信息发生了错误
        /// </summary>
        /// <param name="context"></param>
        /// <param name="str"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendMessageErrorAsync(HttpContext context, CancellationToken ct, string str = "发生了未预料的错误")
        {
            await SendMessageStatusAsync(context, ct, new SSEMessageStatus
            {
                Status = SQLBotMessageStatus.Error,
                Str = str
            });
        }

        #endregion


        #region 数据块状态

        /// <summary>
        /// 推送消息状态
        /// </summary>
        /// <param name="context"></param>
        /// <param name="status"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendBlockStatusAsync(HttpContext context, CancellationToken ct, SSEBlockStatus status)
        {
            var json = JsonSerializer.Serialize(status, status.GetType(), AgentJsonOptions.SSEJsonOptions);

            await context.Response.WriteAsync(
                $"event: block-status\n" +
                $"data: {json}\n\n",
                ct);
            await context.Response.Body.FlushAsync(ct);
        }

        /// <summary>
        /// 标记此信息已经完成
        /// </summary>
        /// <param name="context"></param>
        /// <param name="str"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendBlockDoneAsync(HttpContext context, CancellationToken ct, string str = "思考完成")
        {
            await SendBlockStatusAsync(context, ct, new SSEBlockStatus
            {
                Status = SQLBotMessageStatus.Done,
                Str = str
            });
        }

        /// <summary>
        /// 标记此信息正在进行中
        /// </summary>
        /// <param name="context"></param>
        /// <param name="str"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendBlockStreamingAsync(HttpContext context, CancellationToken ct, string str = "正在思考")
        {
            await SendBlockStatusAsync(context, ct, new SSEBlockStatus
            {
                Status = SQLBotMessageStatus.Streaming,
                Str = str
            });
        }

        /// <summary>
        /// 标记此信息发生了错误
        /// </summary>
        /// <param name="context"></param>
        /// <param name="str"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task SendBlockErrorAsync(HttpContext context, CancellationToken ct, string str = "发生了未预料的错误")
        {
            await SendBlockStatusAsync(context, ct, new SSEBlockStatus
            {
                Status = SQLBotMessageStatus.Error,
                Str = str
            });
        }

        #endregion
    }
}
