
using ImTools;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
using System;
using System.ClientModel;
using System.Text;
using System.Text.Json;
using Wolverine;
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
        [HttpPost("SQLChat")]
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
                    ? string.Join("，", domain.DependencyTables.Select(x => $"{x.TableName}({x.Description}))"))
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
                        filter: x => objectiveIds.Contains(x.ObjectiveMetaDataId) )).Where(x => x.Score > 0.6).ToList();
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
                //foreach (var item in semanticElementsResult.Select)
                //{
                //    // 业务域召回
                //    var body_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective", item.From))
                //        .Where(x => x.Score > 0.6).MaxBy(x => x.Score);
                //    if (body_vc == null)
                //    {
                //        _logger.LogError("为找到与{0}的相似业务域信息", item.From);
                //        return;
                //    }

                //    var body_record = body_vc.Record;
                //    var businessObjective = new BusinessVectorItem() { ObjectiveId = body_vc.Record.MetaDataId };

                //    // 字段召回
                //    foreach (var field in item.What)
                //    {
                //        if (field == "ALL")
                //        {
                //            businessObjective.Columns.Add(new ColumnVectorMatch
                //            {
                //                FieldName = "ALL",
                //            });
                //            continue;
                //        }

                //        // 限定业务域-召回
                //        var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", field,
                //        filter: x => x.ObjectiveMetaDataId == body_vc.Record.MetaDataId)).Where(x => x.Score > 0.6).MaxBy(x => x.Score);
                //        if (field_vc != null && businessObjective.Columns.Count(x => x.FieldName == "ALL") <= 0)
                //        {
                       
                //            businessObjective.Columns.Add(new ColumnVectorMatch
                //            {
                //                Id = field_vc.Record.MetaDataId,
                //                FieldName = field,
                //            });
                //            continue;
                //        }


                //        // 召回隐藏域 例如字段中存在行业术语指向的某个特定域
                //        var broad_result1 = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective", field)
                //        ).Where(x => x.Score > 0.85).MaxBy(x => x.Score);
                //        var broad_result2 = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveSynonymEmbeddingModel>("business_objective_synonyms", field)
                //            ).Where(x => x.Score > 0.85).MaxBy(x => x.Score);
                //        int? selectedRecordId = (broad_result1, broad_result2) switch
                //        {
                //            (null, null) => null,
                //            (not null, null) => broad_result1.Record.MetaDataId,
                //            (null, not null) => broad_result2.Record.ObjectiveMetaDataId,
                //            (not null, not null) => broad_result1.Score >= broad_result2.Score ? (int)broad_result1.Record.Id : broad_result2.Record.ObjectiveMetaDataId
                //        };
                //        if (selectedRecordId != null)
                //        {
                //            var thisBody = businessVectorItems.Find(x => x.ObjectiveId == selectedRecordId);
                //            if (thisBody != null)
                //            {
                //                thisBody.Columns = new List<ColumnVectorMatch> {
                //                    new ColumnVectorMatch
                //                    {
                //                        FieldName = "ALL",
                //                    }
                //                };
                //            }
                //            else
                //            {
                //                businessVectorItems.Add(new BusinessVectorItem
                //                {
                //                    ObjectiveId = selectedRecordId.Value,
                //                    Columns = new List<ColumnVectorMatch> {
                //                    new ColumnVectorMatch
                //                    {
                //                        FieldName = "ALL",
                //                    }
                //                }
                //                });
                //            }
                //            continue;
                //        }


                //        //全局召回
                //        var field_all_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", field)).Where(x => x.Score > 0.6);
                //        if (field_all_vc.Count() > 0)
                //        {
                //            var fields = field_all_vc.Where(x => x.Score == field_all_vc.Max(x => x.Score));
                //            if (fields.Count() > 1)
                //            {
                //                //TODO 歧义字段  倒排索引加权重/业务域血缘
                //                _logger.LogWarning("字段{0}存在歧义，召回了多个相似字段，分别是{1}，需要后续通过倒排索引加权重或者业务域血缘等方式进行筛选", field, string.Join(", ", fields.Select(x => x.Record.MetaDataName)));

                //            }
                //            else
                //            {
                //                var vector_field = fields.FirstOrDefault();
                //                var thisBody = businessVectorItems.Find(x => x.ObjectiveId == vector_field.Record.ObjectiveMetaDataId);
                //                if (thisBody != null)
                //                {
                //                    thisBody.Columns.Add(new ColumnVectorMatch
                //                    {
                //                        Id = vector_field.Record.MetaDataId,
                //                        FieldName = field
                //                    });
                //                }
                //                else
                //                {
                //                    businessVectorItems.Add(new BusinessVectorItem
                //                    {

                //                        ObjectiveId = vector_field.Record.ObjectiveMetaDataId,
                //                        Columns = new List<ColumnVectorMatch> { new ColumnVectorMatch
                //                    {
                //                         Id = vector_field.Record.MetaDataId,
                //                         FieldName = field
                //                    }}
                //                    });
                //                }
                //            }
                //            continue;
                //        }


                //        //TODO 业务术语（高价值用户、极简用户）

                //        //TODO 维度术语（日期/时间、地区、部门、角色）

                //        // 未定义字段或者非法字段
                //        noWhatFild.Add(field);
                //    }
                //}

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

        [HttpPost("SQLChat22")]
        public async Task SQLChat22([FromBody] SQLBotMessage userMessage, CancellationToken ct)
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

                // 1. 将所有的业务域放入LLM，让LLM选择最适合的域信息，同时做歧义处理







                #region 第一遍 召回业务域  1000%没错
                var llmDomainResult = await _chatXService.LLMDomainAsync(userInput);

                // 召回业务域（宽表）
                var domains = new List<BusinessObjectiveEmbeddingModel>();
                foreach (var item in llmDomainResult.BusinessObjects)
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
                string domainStr = string.Empty;
                for (int i = 0; i < domainsOfDb.Count; i++)
                {
                    var domain = domainsOfDb[i];
                    var tableInfo = (domain.DependencyTables?.Any() == true)
                    ? string.Join("，", domain.DependencyTables.Select(x => $"[{x.TableName}({x.Description})]"))
                    : null;
                    domainStr += $"{i + 1}. 业务域名称: {domain.BusinessName}\n";
                    domainStr += $"     - 唯一标识: {domain.Id}\n";
                    domainStr += $"     - 说明: {domain.Description}\n";
                    domainStr += $"     - 近义词: {domain.Synonyms}\n";
                    if (!string.IsNullOrEmpty(tableInfo))
                    {
                        domainStr += $"     - 关联表: {tableInfo}";
                    }
                    domainStr += "\n";
                }

                var Step1_3 = await _chatXService.Step1_3(userInput, domainStr);

                if (Step1_3.QueryMode== "聚合查询")
                {
                    
                }

                if (Step1_3.QueryMode == "明细查询")
                {

                }

                if (Step1_3.QueryMode == "明细+聚合 混合查询")
                {

                }

                var Step1_32 = await _chatXService.Step1_3(userInput, domainStr, "");

                return;


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
6. 根据你选择的`业务域` 以及关联的相关表信息，判断这是一个指标分析型查询（AGGREGATE）还是一个普通的数据查询（DATA）

## Schema Definition
你的输出 **必须** 严格遵守此 JSON 结构。
```json
{
    ""reason"":""思考内容/解释为什么"",    
    ""intent"":""AGGREGATE | DATA"",    
    ""selected_domains"":[
    {
      ""domains_id"": ""业务域唯一标识(Number)"",
      ""domains_name"": ""业务域名称"",
      ""selection_reason"": ""选择此业务域的理由""
    }] | []
}
```
"
.Replace("{userInput}", userInput)
.Replace("{domainsInfo}", domainStr)
;
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
                var selectedDbDoamins = domainsOfDb.Where(x => selectedDomainIds.Contains(x.Id)).ToList();
                #endregion

                #region 


                #endregion


                //TODO 锚点判断

                // 语义粗筛
                var semanticElementsResult = await _chatXService.Step22Async(userInput);



                #region 第二次召回业务域

                #endregion

                #region 隐式归类 
                // 将隐式维度、条件 归类到对应的业务域中


                #endregion

                #region 隐式维度、条件
                string implicitPrompt = @"
# Semantic Parsing System

## 上下文信息

### 系统日期
`{CURRENT_DATE}` (**关键**: 所有相对日期必须基于此日期进行计算)。

### 用户原始输入查询
`{USER_INPUT}`

### 业务域列表
`{DOMAINS}`

## System-Level Principle
用户的所有输入，无论表述如何，其最终目的都是获取一个数据结果集，一个数据查询任务，您的目标永远是构建 **一张统一的二维数据表** ,涉及的不同业务对象一定是需要关联查询的。

## 背景
我们正在开发一款 **智能问数&NL2SQL**产品

## Role
你是一名 **Semantic Parsing Architect**，已知 `用户原始输入查询` 与 `业务域列表`，仔细阅读上述信息选择最符合目的的业务域，为每个业务域选择归属其下的各项信息。

## 核心任务
1. 分析 `用户原始输入查询` 与 `业务域列表` 判断用户需要从那些业务域中获取数据？
2. 判断每个业务域是否存在全量字段：从用户语义修饰中判断哪些字段是一个包含多个相关字段的数据集合。[“资料”、“信息”、“详情”、“全部”、“所有”] 等词汇通常暗示用户希望获取一个完整的业务对象，而不仅仅是一个单一的字段。因此，一个 `业务对象` 的全量（full）判断标准可以参考以下：
    - 业务对象的显示内容包含上述相似词汇或以此结尾的查询
    - 业务对象的显示内容以业务域的**根名词** + 上述相似词汇结尾组成或以业务对象的 **根名词** 包含上述相似词汇组成
    - 将全量描述事实提取到 full_column 
3. 理解 `维度解析` 将其放在归属的业务域下的  `dimensions`
4. 理解 `指标解析` 将其放在归属的业务域下  `metrics`
5. 理解 `显示列解析` 将其放在归属的业务域下  `columns`
6. 理解 `条件解析` 将其放在归属的业务域下  `filters`
7. 理解 `排序解析` 将其放在归属的业务域下  `sorts`

## 维度解析

### 维度类型判断 (Dimension Type)
1. **显式维度 (EXPLICIT)**:
    *   **判定标准**: 用户明确使用了指示分组的介词或量词。
    *   **关键词**: “按...”、“根据...”、“分...”、“各...”、“每一个...”、“不同...”、“分别是...”。
    *   *示例*: “按**地区**统计” -> 显式维度: `地区`。

2. **隐式维度 (IMPLICIT)**:
    *   **判定标准**: 用户未提及具体字段名，但其分析意图（如趋势、排名、分布）在逻辑上必须依赖某个特定维度。
    *   **常见映射**:
        *   “趋势/走势/变化” -> 隐含 **时间维度** (通常映射为 `月` 或 `年`，视上下文而定)。
        *   “排名/Top N/谁” -> 隐含 **实体维度** (如客户、产品)。
        *   “分布/占比/结构” -> 隐含 **分类维度**。
    *   *示例*: “销售**趋势**” -> 隐式维度: `月` (假设默认粒度)。

### 标签映射 (Tag Mapping)
提取出维度名称后，请根据其语义将其归类到预定义标签：
*   **地理/位置** -> `地区` (如: 城市、省份、区域)
*   **内部架构** -> `部门` / `组织` (如: 销售部、分公司)
*   **人员属性** -> `角色` (如: 经理、员工、负责人)
*   **时间颗粒度** -> `年` / `月` / `日` / `天` / `时` / `分` / `秒` (严格匹配粒度)
*   **核心业务对象** -> `实体` (如: 客户、产品、门店、合同、项目)
*   **属性/类型** -> `分类` (如: 行业、等级、颜色、状态)
*   **无法归类** -> `其他`

### 严禁提取筛选值 (CRITICAL: Filter vs Dimension)
    *   **这是最容易犯错的地方！**
    *   如果用户提到的是一个 **具体的值 (Instance)**，它属于筛选条件 (Where)，**绝对不是** 维度。
    *   *Case A*: “查询**北京**的销售额” -> `北京` 是值，不是维度。Dimensions: `[]` (空)
    *   *Case B*: “查询**各城市**的销售额” -> `城市` 是类别。Dimensions: `[“城市”]`
    *   *Case C*: “查询**2026年**的利润” -> `2026年` 是值。Dimensions: `[]` (空)


## 指标解析

### 系统内置计算公式
- 聚合运算类: 求和（SUM）、平均（AVG）、计数（COUNT）、最大（MAX）、最小（MIN）、标准偏差（Stdev）、总体标准偏差（Stdevp）、方差（Var）、总体方差（Varp）、近似计数（ApproxCountDistinct）
- 周期对比类: 
  - 同比（（本期数值 - 去年同期数值） / 去年同期数值 × 100%）
  - 环比（（本期数值 - 上期数值） / 上期数值 × 100%）
  - 定基比（（比较期数值 / 固定基期数值） × 100%）
  - 占比（(某一分项的数值 / 总项的数值) × 100%）
- 自定义计算公式: 暂无

### 提取规则
- 识别符合 `系统内置计算公式` 显示的计算或隐含的计算逻辑的指标描述。
- 若指标描述中存在字段根名词，应提取根名词，否则提取指标描述的关键事实词作为指标名称，剔除计算公式
- 如果是计数类指标（用户量、合同量、订单量等），tag 应该是 `计数`，否则都为  `计算`。
- **去重**：相同的指标名称只输出一次。
- **禁止推断字段名**：非用户明确提及的字段 **根名词** 请勿推断

## 条件解析
- 当检测到限制条件时，请问自己一个问题：‘这个条件是写在每一张原始单据上的（属性），还是对计算指标的过滤条件？如果是 **计算指标的过滤条件** 的限制条件请忽略
- **禁止推断字段名**: 非用户明确提及的字段 **根名词** 请勿推断

1. 显式字段
    *   **判定标准**: 用户在句子中明确的说出了字段名称
    *   **句式**: `[字段名] [谓语] [值]`
    *   Case: ‘创建日期为2026年’ -> 用户说了'创建日期'，这是显式
    *   Case: '客户等级是VIP' -> 用户说了'客户等级'，这是显式

2. 隐式字段
    *  **判定标准**: 用户未提及具体字段名，只说了值。
    *   **句式**: `[谓语] [值]` 或 `[值]`
    *   Case: '**VIP**客户' -> 用户没说'等级'，'VIP'修饰客户。-> **type: 隐式**, Name: '' 
    *   Case: '**2026年**的...' -> 用户没说'日期'。-> **type: 隐式**, Name: '' 

3. 标签映射逻辑：
提取出字段名称后，请根据其语义与值类型判断将其归类到预定义标签：
    *   **地理/位置** -> `地区` (如: 城市、省份、区域)
    *   **内部架构** -> `部门`  (如: 销售部、市场部)
    *   **内部架构** -> `组织` (如: 某子公司、分公司、某事业部)
    *   **人员属性** -> `角色` (如: 经理、员工、负责人)
    *   **时间颗粒度** -> `日期时间`
    *   **属性/类型** -> `分类` (如: 行业、等级、颜色、状态)
    *   **无法归类** -> `其他`
 
## 排序解析
- 当用户表达了排序意图时，首先判断排序目标是否对计算后的指标进行排序，如果是，请忽略这个排序条件
- 如果 `columns` 或 `dimensions` 或 `metrics` 或  `filters` 中已存在此排序字段，则沿用其字段名
- **显示排序**: 用户在排序描述中明确的说出了字段名称和排序方式（如 “按…正序”、按…倒序”），type = 显示
- **隐式排序**: 用户在排序描述中明确的说出了字段名称但未表明排序方式（如“按…排序”），type = 隐式
- **无效排序**: 用户在排序描述中 无明确字段名称 且 不符合 `极值字段` 且 无排序描述符，忽略此不完整的排序信息
- **禁止推断字段名**: 除非用户明确提及

### 标签映射
对排序描述进行极值判断，请根据其语义判断将其归类到预定义 `tag` 标签：
- 最高、最大、最多、最贵、峰值、顶峰、冠军 -> `MAX`,DESC
- 最低、最小、最少、最便宜、低谷、倒数第一 -> `MIN`,ASC
- 最新、最近 -> `DATE_MAX`,DESC
- 最早、最旧 -> `DATE_MIN`,ASC
- 不符合极值 -> `其他`


## Limit&Pagination解析

### 1. 核心定义与区分
你必须严格区分用户是想要 **“截断数据（只看前几名）”** 还是 **“翻页浏览（看第几页）”**。
*   **业务截断 (`limit`)**:
    *   **定义**: 属于 **分析逻辑** 的一部分。用户只想关注排名靠前或靠后的特定数量的数据。
    *   **特征词**: '前N名', 'Top N', '排名前N', '最高的N个', '最低的N个', '冠军(1)', '前三'。
*   **展示分页 (`pagination`)**:
    *   **定义**: 属于 **交互逻辑** 的一部分。用户面对大量数据，希望分批次查看或明确页信息。
    *   **特征词**: '第N页', '每页N条', '下一页', '显示N条', '分批看'。

### 2. 提取规则
**规则 A: 提取 `limit` (整数 | null)**
*   仅当用户明确表达了 **“排名截断”** 意图时提取。
*   如果用户说“取 10 条看看”，通常是分页意图（默认第一页），除非语境是“取**最好的** 10 条”。

**规则 B: 提取 `pagination` (Object | null)**
1.  **对象结构**: `{ """"page"""": Number, """"size"""": Number }`。
2.  **`page` (页码)**:
    *   提取 '第N页' 中的 N。
    *   如果未提及页码，但提及了每页条数，**默认 `page: 1`**。
3.  **`size` (每页条数)**:
    *   提取 '每页N条'、'显示N个' 中的 N。
    *   如果未提及条数，但提及了页码，**默认 `size: 20`** (系统默认值)。
4.  **模糊场景处理 (关键)**:
    *   如果用户只说了“查 10 条”、“看 5 个” (没有“前/Top”等修饰词)，视为 **分页的 `size`**，即 `{ """"page"""": 1, """"size"""": 10 }`，**而不是** `limit`。

## 显示列解析
- **排除**已存在于 `业务对象` 中的全量字段、属于维度或指标的字段
- 提取用户明确提及的、需要展示的内容的 **根名词**，并将其归类为 `columns`。如果用户没有明确提及任何显示列，则 `columns` 应为一个空数组 `[]`。
- **禁止推断显示列**: 只有当用户明确提及时才提取显示列。不要基于上下文或常识推断用户想要显示哪些列。
 
## Output Schema
你的输出 **必须** 严格遵守此 JSON 结构。
```json
{
    ""business_objects"": [
        {
            ""domain_id"": """",
            ""name"": ""String(业务对象名)"",
            ""has_full"": ""Boolean(是否拥有全量字段)"",
            ""full_column"": ""String(全量事实)"",
            ""columns"": [
                ""column1"",
                ""column2""
            ],
            ""dimensions"": [
                {
                    ""type"": ""显示|隐式"",
                    ""name"": """",
                    ""tag"": """"
                }
            ],
            ""metrics"": [
                {
                    ""name"": """",
                    ""tag"": ""计数|计算""
                }
            ],
            ""filters"": [
                {
                    ""type"": ""条件字段类型（显式|隐式）"",
                    ""tag"": ""条件字段标签"",
                    ""name"": ""条件字段名称""
                }
            ],
            ""sort"": [
                {
                    ""by"": """",
                    ""tag"": """",
                    ""order"": ""DESC|ASC"",
                    ""type"": ""显示|隐式|无效""
                }
            ]
        }
    ],
    ""limit"": ""Number | null"",
    ""pagination"": {
        ""page"": ""Number"",
        ""size"": ""Number""
    }
}
```
";

                #endregion


                // 基于业务域召回 columns、metrics、dimensions、filters、sort
                #region 召回字段
                var fullColumns = semanticElementsResult.BusinessObjects.Where(x => x.HasFull).Select(x => x.FullColumn);
                var metricColumns = semanticElementsResult.Metrics.Select(x => x.Name);
                var columns = semanticElementsResult.Columns.Where(x => !fullColumns.Contains(x) && !metricColumns.Contains(x));
                var dbColumns = new List<dynamic>();
                foreach (var column in columns)
                {
                    var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", column,
                        filter: x => selectedDomainIds.Contains(x.ObjectiveMetaDataId))).MaxBy(x => x.Score);
                    if (field_vc == null)
                    {
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
                        LLmName = column,
                        Id = field_vc.Record.MetaDataId,
                        Name = field_vc.Record.MetaDataName,
                        DomainId = field_vc.Record.ObjectiveMetaDataId,
                        DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                    });
                }
                #endregion

                #region 召回指标与指标的度量字段
                var dbMetrics = new List<dynamic>();
                foreach (var item in semanticElementsResult.Metrics)
                {
                    var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", item.Name,
                        filter: x => selectedDomainIds.Contains(x.ObjectiveMetaDataId))).MaxBy(x => x.Score);
                    if (field_vc == null)
                    {
                        _logger.LogError("未找到指标{clomun}", item.Name);
                        continue;
                    }

                    if (field_vc.Record.MetaDataDescription == "calculated")
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                    else
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                }
                #endregion

                #region 召回维度字段
                var dbDimensions = new List<dynamic>();
                foreach (var item in semanticElementsResult.Dimensions)
                {
                    var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", item.Name,
                        filter: x => selectedDomainIds.Contains(x.ObjectiveMetaDataId))).MaxBy(x => x.Score);
                    if (field_vc == null)
                    {
                        _logger.LogError("未找到指标{clomun}", item.Name);
                        continue;
                    }

                    if (field_vc.Record.MetaDataDescription == "calculated")
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                    else
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                }
                #endregion

                #region 召回过滤字段
                var dbFilters = new List<dynamic>();
                foreach (var item in semanticElementsResult.Filters)
                {
                    var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", item.Name,
                        filter: x => selectedDomainIds.Contains(x.ObjectiveMetaDataId))).MaxBy(x => x.Score);
                    if (field_vc == null)
                    {
                        _logger.LogError("未找到指标{clomun}", item.Name);
                        continue;
                    }

                    if (field_vc.Record.MetaDataDescription == "calculated")
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                    else
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                }
                #endregion

                #region 召回排序字段
                var dbSorts = new List<dynamic>();
                foreach (var item in semanticElementsResult.Sorts)
                {

                }
                #endregion


            }



        }


        [HttpPost("SQLChat33")]
        public async Task SQLChat33([FromBody] string userInput, CancellationToken ct)
        {
            HttpContext.Response.ContentType = "text/event-stream";
            HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
            HttpContext.Response.Headers.Append("X-Accel-Buffering", "no");

            //打开会话
            var sessionId = Guid.CreateVersion7();
            await SendSessionAsync(HttpContext, sessionId, ct);

            var splitTaskResult = await _chatXService.SplitTaskAsync(userInput);
            foreach (var task in splitTaskResult.Tasks)
            {

                // 1. 将所有的业务域放入LLM，让LLM选择最适合的域信息，同时做歧义处理
                var domainsOfDbs = await _businessObjectiveService.ListAsync();
                string domainStrs = string.Empty;
                for (int i = 0; i < domainsOfDbs.Count; i++)
                {
                    var domain = domainsOfDbs[i];
                    var tableInfo = (domain.DependencyTables?.Any() == true)
                    ? string.Join("，", domain.DependencyTables.Select(x => $"[{x.TableName}({x.Description})]"))
                    : null;
                    domainStrs += $"{i + 1}. 业务域名称: {domain.BusinessName}\n";
                    domainStrs += $"     - 唯一标识: {domain.Id}\n";
                    domainStrs += $"     - 说明: {domain.Description}\n";
                    domainStrs += $"     - 近义词: {domain.Synonyms}\n";
                    if (!string.IsNullOrEmpty(tableInfo))
                    {
                        domainStrs += $"     - 关联表: {tableInfo}";
                    }
                    domainStrs += "\n";
                }

                var llmDomainResult = await _chatXService.LLMDomainAsync(userInput, domainStrs);

                return;



                #region 第一遍 召回业务域  1000%没错


                // 召回业务域（宽表）
                var domains = new List<BusinessObjectiveEmbeddingModel>();
                foreach (var item in llmDomainResult.BusinessObjects)
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
                string domainStr = string.Empty;
                for (int i = 0; i < domainsOfDb.Count; i++)
                {
                    var domain = domainsOfDb[i];
                    var tableInfo = (domain.DependencyTables?.Any() == true)
                    ? string.Join("，", domain.DependencyTables.Select(x => $"[{x.TableName}({x.Description})]"))
                    : null;
                    domainStr += $"{i + 1}. 业务域名称: {domain.BusinessName}\n";
                    domainStr += $"     - 唯一标识: {domain.Id}\n";
                    domainStr += $"     - 说明: {domain.Description}\n";
                    domainStr += $"     - 近义词: {domain.Synonyms}\n";
                    if (!string.IsNullOrEmpty(tableInfo))
                    {
                        domainStr += $"     - 关联表: {tableInfo}";
                    }
                    domainStr += "\n";
                }

                var Step1_3 = await _chatXService.Step1_3(userInput, domainStr);

                if (Step1_3.QueryMode == "聚合查询")
                {

                }

                if (Step1_3.QueryMode == "明细查询")
                {

                }

                if (Step1_3.QueryMode == "明细+聚合 混合查询")
                {

                }

                var Step1_32 = await _chatXService.Step1_3(userInput, domainStr, "");

                return;


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
6. 根据你选择的`业务域` 以及关联的相关表信息，判断这是一个指标分析型查询（AGGREGATE）还是一个普通的数据查询（DATA）

## Schema Definition
你的输出 **必须** 严格遵守此 JSON 结构。
```json
{
    ""reason"":""思考内容/解释为什么"",    
    ""intent"":""AGGREGATE | DATA"",    
    ""selected_domains"":[
    {
      ""domains_id"": ""业务域唯一标识(Number)"",
      ""domains_name"": ""业务域名称"",
      ""selection_reason"": ""选择此业务域的理由""
    }] | []
}
```
"
.Replace("{userInput}", userInput)
.Replace("{domainsInfo}", domainStr)
;
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
                var selectedDbDoamins = domainsOfDb.Where(x => selectedDomainIds.Contains(x.Id)).ToList();
                #endregion

                #region 


                #endregion


                //TODO 锚点判断

                // 语义粗筛
                var semanticElementsResult = await _chatXService.Step22Async(userInput);



                #region 第二次召回业务域

                #endregion

                #region 隐式归类 
                // 将隐式维度、条件 归类到对应的业务域中


                #endregion

                #region 隐式维度、条件
                string implicitPrompt = @"
# Semantic Parsing System

## 上下文信息

### 系统日期
`{CURRENT_DATE}` (**关键**: 所有相对日期必须基于此日期进行计算)。

### 用户原始输入查询
`{USER_INPUT}`

### 业务域列表
`{DOMAINS}`

## System-Level Principle
用户的所有输入，无论表述如何，其最终目的都是获取一个数据结果集，一个数据查询任务，您的目标永远是构建 **一张统一的二维数据表** ,涉及的不同业务对象一定是需要关联查询的。

## 背景
我们正在开发一款 **智能问数&NL2SQL**产品

## Role
你是一名 **Semantic Parsing Architect**，已知 `用户原始输入查询` 与 `业务域列表`，仔细阅读上述信息选择最符合目的的业务域，为每个业务域选择归属其下的各项信息。

## 核心任务
1. 分析 `用户原始输入查询` 与 `业务域列表` 判断用户需要从那些业务域中获取数据？
2. 判断每个业务域是否存在全量字段：从用户语义修饰中判断哪些字段是一个包含多个相关字段的数据集合。[“资料”、“信息”、“详情”、“全部”、“所有”] 等词汇通常暗示用户希望获取一个完整的业务对象，而不仅仅是一个单一的字段。因此，一个 `业务对象` 的全量（full）判断标准可以参考以下：
    - 业务对象的显示内容包含上述相似词汇或以此结尾的查询
    - 业务对象的显示内容以业务域的**根名词** + 上述相似词汇结尾组成或以业务对象的 **根名词** 包含上述相似词汇组成
    - 将全量描述事实提取到 full_column 
3. 理解 `维度解析` 将其放在归属的业务域下的  `dimensions`
4. 理解 `指标解析` 将其放在归属的业务域下  `metrics`
5. 理解 `显示列解析` 将其放在归属的业务域下  `columns`
6. 理解 `条件解析` 将其放在归属的业务域下  `filters`
7. 理解 `排序解析` 将其放在归属的业务域下  `sorts`

## 维度解析

### 维度类型判断 (Dimension Type)
1. **显式维度 (EXPLICIT)**:
    *   **判定标准**: 用户明确使用了指示分组的介词或量词。
    *   **关键词**: “按...”、“根据...”、“分...”、“各...”、“每一个...”、“不同...”、“分别是...”。
    *   *示例*: “按**地区**统计” -> 显式维度: `地区`。

2. **隐式维度 (IMPLICIT)**:
    *   **判定标准**: 用户未提及具体字段名，但其分析意图（如趋势、排名、分布）在逻辑上必须依赖某个特定维度。
    *   **常见映射**:
        *   “趋势/走势/变化” -> 隐含 **时间维度** (通常映射为 `月` 或 `年`，视上下文而定)。
        *   “排名/Top N/谁” -> 隐含 **实体维度** (如客户、产品)。
        *   “分布/占比/结构” -> 隐含 **分类维度**。
    *   *示例*: “销售**趋势**” -> 隐式维度: `月` (假设默认粒度)。

### 标签映射 (Tag Mapping)
提取出维度名称后，请根据其语义将其归类到预定义标签：
*   **地理/位置** -> `地区` (如: 城市、省份、区域)
*   **内部架构** -> `部门` / `组织` (如: 销售部、分公司)
*   **人员属性** -> `角色` (如: 经理、员工、负责人)
*   **时间颗粒度** -> `年` / `月` / `日` / `天` / `时` / `分` / `秒` (严格匹配粒度)
*   **核心业务对象** -> `实体` (如: 客户、产品、门店、合同、项目)
*   **属性/类型** -> `分类` (如: 行业、等级、颜色、状态)
*   **无法归类** -> `其他`

### 严禁提取筛选值 (CRITICAL: Filter vs Dimension)
    *   **这是最容易犯错的地方！**
    *   如果用户提到的是一个 **具体的值 (Instance)**，它属于筛选条件 (Where)，**绝对不是** 维度。
    *   *Case A*: “查询**北京**的销售额” -> `北京` 是值，不是维度。Dimensions: `[]` (空)
    *   *Case B*: “查询**各城市**的销售额” -> `城市` 是类别。Dimensions: `[“城市”]`
    *   *Case C*: “查询**2026年**的利润” -> `2026年` 是值。Dimensions: `[]` (空)


## 指标解析

### 系统内置计算公式
- 聚合运算类: 求和（SUM）、平均（AVG）、计数（COUNT）、最大（MAX）、最小（MIN）、标准偏差（Stdev）、总体标准偏差（Stdevp）、方差（Var）、总体方差（Varp）、近似计数（ApproxCountDistinct）
- 周期对比类: 
  - 同比（（本期数值 - 去年同期数值） / 去年同期数值 × 100%）
  - 环比（（本期数值 - 上期数值） / 上期数值 × 100%）
  - 定基比（（比较期数值 / 固定基期数值） × 100%）
  - 占比（(某一分项的数值 / 总项的数值) × 100%）
- 自定义计算公式: 暂无

### 提取规则
- 识别符合 `系统内置计算公式` 显示的计算或隐含的计算逻辑的指标描述。
- 若指标描述中存在字段根名词，应提取根名词，否则提取指标描述的关键事实词作为指标名称，剔除计算公式
- 如果是计数类指标（用户量、合同量、订单量等），tag 应该是 `计数`，否则都为  `计算`。
- **去重**：相同的指标名称只输出一次。
- **禁止推断字段名**：非用户明确提及的字段 **根名词** 请勿推断

## 条件解析
- 当检测到限制条件时，请问自己一个问题：‘这个条件是写在每一张原始单据上的（属性），还是对计算指标的过滤条件？如果是 **计算指标的过滤条件** 的限制条件请忽略
- **禁止推断字段名**: 非用户明确提及的字段 **根名词** 请勿推断

1. 显式字段
    *   **判定标准**: 用户在句子中明确的说出了字段名称
    *   **句式**: `[字段名] [谓语] [值]`
    *   Case: ‘创建日期为2026年’ -> 用户说了'创建日期'，这是显式
    *   Case: '客户等级是VIP' -> 用户说了'客户等级'，这是显式

2. 隐式字段
    *  **判定标准**: 用户未提及具体字段名，只说了值。
    *   **句式**: `[谓语] [值]` 或 `[值]`
    *   Case: '**VIP**客户' -> 用户没说'等级'，'VIP'修饰客户。-> **type: 隐式**, Name: '' 
    *   Case: '**2026年**的...' -> 用户没说'日期'。-> **type: 隐式**, Name: '' 

3. 标签映射逻辑：
提取出字段名称后，请根据其语义与值类型判断将其归类到预定义标签：
    *   **地理/位置** -> `地区` (如: 城市、省份、区域)
    *   **内部架构** -> `部门`  (如: 销售部、市场部)
    *   **内部架构** -> `组织` (如: 某子公司、分公司、某事业部)
    *   **人员属性** -> `角色` (如: 经理、员工、负责人)
    *   **时间颗粒度** -> `日期时间`
    *   **属性/类型** -> `分类` (如: 行业、等级、颜色、状态)
    *   **无法归类** -> `其他`
 
## 排序解析
- 当用户表达了排序意图时，首先判断排序目标是否对计算后的指标进行排序，如果是，请忽略这个排序条件
- 如果 `columns` 或 `dimensions` 或 `metrics` 或  `filters` 中已存在此排序字段，则沿用其字段名
- **显示排序**: 用户在排序描述中明确的说出了字段名称和排序方式（如 “按…正序”、按…倒序”），type = 显示
- **隐式排序**: 用户在排序描述中明确的说出了字段名称但未表明排序方式（如“按…排序”），type = 隐式
- **无效排序**: 用户在排序描述中 无明确字段名称 且 不符合 `极值字段` 且 无排序描述符，忽略此不完整的排序信息
- **禁止推断字段名**: 除非用户明确提及

### 标签映射
对排序描述进行极值判断，请根据其语义判断将其归类到预定义 `tag` 标签：
- 最高、最大、最多、最贵、峰值、顶峰、冠军 -> `MAX`,DESC
- 最低、最小、最少、最便宜、低谷、倒数第一 -> `MIN`,ASC
- 最新、最近 -> `DATE_MAX`,DESC
- 最早、最旧 -> `DATE_MIN`,ASC
- 不符合极值 -> `其他`


## Limit&Pagination解析

### 1. 核心定义与区分
你必须严格区分用户是想要 **“截断数据（只看前几名）”** 还是 **“翻页浏览（看第几页）”**。
*   **业务截断 (`limit`)**:
    *   **定义**: 属于 **分析逻辑** 的一部分。用户只想关注排名靠前或靠后的特定数量的数据。
    *   **特征词**: '前N名', 'Top N', '排名前N', '最高的N个', '最低的N个', '冠军(1)', '前三'。
*   **展示分页 (`pagination`)**:
    *   **定义**: 属于 **交互逻辑** 的一部分。用户面对大量数据，希望分批次查看或明确页信息。
    *   **特征词**: '第N页', '每页N条', '下一页', '显示N条', '分批看'。

### 2. 提取规则
**规则 A: 提取 `limit` (整数 | null)**
*   仅当用户明确表达了 **“排名截断”** 意图时提取。
*   如果用户说“取 10 条看看”，通常是分页意图（默认第一页），除非语境是“取**最好的** 10 条”。

**规则 B: 提取 `pagination` (Object | null)**
1.  **对象结构**: `{ """"page"""": Number, """"size"""": Number }`。
2.  **`page` (页码)**:
    *   提取 '第N页' 中的 N。
    *   如果未提及页码，但提及了每页条数，**默认 `page: 1`**。
3.  **`size` (每页条数)**:
    *   提取 '每页N条'、'显示N个' 中的 N。
    *   如果未提及条数，但提及了页码，**默认 `size: 20`** (系统默认值)。
4.  **模糊场景处理 (关键)**:
    *   如果用户只说了“查 10 条”、“看 5 个” (没有“前/Top”等修饰词)，视为 **分页的 `size`**，即 `{ """"page"""": 1, """"size"""": 10 }`，**而不是** `limit`。

## 显示列解析
- **排除**已存在于 `业务对象` 中的全量字段、属于维度或指标的字段
- 提取用户明确提及的、需要展示的内容的 **根名词**，并将其归类为 `columns`。如果用户没有明确提及任何显示列，则 `columns` 应为一个空数组 `[]`。
- **禁止推断显示列**: 只有当用户明确提及时才提取显示列。不要基于上下文或常识推断用户想要显示哪些列。
 
## Output Schema
你的输出 **必须** 严格遵守此 JSON 结构。
```json
{
    ""business_objects"": [
        {
            ""domain_id"": """",
            ""name"": ""String(业务对象名)"",
            ""has_full"": ""Boolean(是否拥有全量字段)"",
            ""full_column"": ""String(全量事实)"",
            ""columns"": [
                ""column1"",
                ""column2""
            ],
            ""dimensions"": [
                {
                    ""type"": ""显示|隐式"",
                    ""name"": """",
                    ""tag"": """"
                }
            ],
            ""metrics"": [
                {
                    ""name"": """",
                    ""tag"": ""计数|计算""
                }
            ],
            ""filters"": [
                {
                    ""type"": ""条件字段类型（显式|隐式）"",
                    ""tag"": ""条件字段标签"",
                    ""name"": ""条件字段名称""
                }
            ],
            ""sort"": [
                {
                    ""by"": """",
                    ""tag"": """",
                    ""order"": ""DESC|ASC"",
                    ""type"": ""显示|隐式|无效""
                }
            ]
        }
    ],
    ""limit"": ""Number | null"",
    ""pagination"": {
        ""page"": ""Number"",
        ""size"": ""Number""
    }
}
```
";

                #endregion


                // 基于业务域召回 columns、metrics、dimensions、filters、sort
                #region 召回字段
                var fullColumns = semanticElementsResult.BusinessObjects.Where(x => x.HasFull).Select(x => x.FullColumn);
                var metricColumns = semanticElementsResult.Metrics.Select(x => x.Name);
                var columns = semanticElementsResult.Columns.Where(x => !fullColumns.Contains(x) && !metricColumns.Contains(x));
                var dbColumns = new List<dynamic>();
                foreach (var column in columns)
                {
                    var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", column,
                        filter: x => selectedDomainIds.Contains(x.ObjectiveMetaDataId))).MaxBy(x => x.Score);
                    if (field_vc == null)
                    {
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
                        LLmName = column,
                        Id = field_vc.Record.MetaDataId,
                        Name = field_vc.Record.MetaDataName,
                        DomainId = field_vc.Record.ObjectiveMetaDataId,
                        DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                    });
                }
                #endregion

                #region 召回指标与指标的度量字段
                var dbMetrics = new List<dynamic>();
                foreach (var item in semanticElementsResult.Metrics)
                {
                    var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", item.Name,
                        filter: x => selectedDomainIds.Contains(x.ObjectiveMetaDataId))).MaxBy(x => x.Score);
                    if (field_vc == null)
                    {
                        _logger.LogError("未找到指标{clomun}", item.Name);
                        continue;
                    }

                    if (field_vc.Record.MetaDataDescription == "calculated")
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                    else
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                }
                #endregion

                #region 召回维度字段
                var dbDimensions = new List<dynamic>();
                foreach (var item in semanticElementsResult.Dimensions)
                {
                    var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", item.Name,
                        filter: x => selectedDomainIds.Contains(x.ObjectiveMetaDataId))).MaxBy(x => x.Score);
                    if (field_vc == null)
                    {
                        _logger.LogError("未找到指标{clomun}", item.Name);
                        continue;
                    }

                    if (field_vc.Record.MetaDataDescription == "calculated")
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                    else
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                }
                #endregion

                #region 召回过滤字段
                var dbFilters = new List<dynamic>();
                foreach (var item in semanticElementsResult.Filters)
                {
                    var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", item.Name,
                        filter: x => selectedDomainIds.Contains(x.ObjectiveMetaDataId))).MaxBy(x => x.Score);
                    if (field_vc == null)
                    {
                        _logger.LogError("未找到指标{clomun}", item.Name);
                        continue;
                    }

                    if (field_vc.Record.MetaDataDescription == "calculated")
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                    else
                    {
                        dbMetrics.Add(new
                        {
                            LLmName = item.Name,
                            Id = field_vc.Record.MetaDataId,
                            Name = field_vc.Record.MetaDataName,
                            DomainId = field_vc.Record.ObjectiveMetaDataId,
                            DomainName = domainsOfDb.Where(x => x.Id == field_vc.Record.ObjectiveMetaDataId).Select(x => x.BusinessName).FirstOrDefault()
                        });
                    }
                }
                #endregion

                #region 召回排序字段
                var dbSorts = new List<dynamic>();
                foreach (var item in semanticElementsResult.Sorts)
                {

                }
                #endregion


            }



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
