
using ImTools;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;
using OpenAI.Chat;
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


                // 指标候选
                string selectMetricPrompt = @"
# Role
你是一个智能数据查询助手。你的任务是根据用户的原始查询、已提取的指标，以及每个指标对应的候选物理字段信息，为**每个指标**选择一个最合适的字段。

# 上下文信息

## 用户原始查询
""{user_query}""

## 指标信息
1. 指标名称：{metric_name}
    - `相似度评分`:
    - `候选字段`: 
        - `字段唯一标识`:  
        - `物理名称`:  
        - `说明`:  
    - `所在业务域`: 
        - `名称`:  
        - `说明`:  
        - `similarity_score`: 
";
                if (semanticElementsResult.Metrics != null)
                {
                    foreach (var metric in semanticElementsResult.Metrics)
                    {
                        var f = semanticElementsResult.Select.FirstOrDefault(x => x.What.Contains(metric.Name));
                        if (f == null)
                        {
                            _logger.LogError("抱歉，系统中没有找到与‘{0}’对应的模块信息", f.From);
                            return;
                        }

                        // 业务域召回
                        var objective_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveEmbeddingModel>("business_objective", f.From, top: 5));
                        if (objective_vc == null || objective_vc.Count() <= 0)
                        {
                            _logger.LogError("抱歉，系统中没有找到与‘{0}’对应的模块信息", f.From);
                            return;
                        }

                        var objectiveIds = objective_vc.Select(x => x.Record.MataData.Id).ToList();
                        var field_vc = (await _qdrantVectorService.SearchAsync<ulong, BusinessObjectiveFieldEmbeddingModel>("business_objective_field", metric.Name,
                        filter: x => objectiveIds.Contains(x.ObjectiveMataData.Id))).Where(x => x.Score > 0.6);
                        if (field_vc == null || field_vc.Count() <= 0)
                        {
                            _logger.LogError("抱歉，系统中没有找到与‘{0}’相关的度量内容", f.From);
                            return;
                        }

                        for (int i = 0; i < field_vc.Count(); i++)
                        {
                            var item = field_vc.ElementAt(i);
                            selectMetricPrompt += @$"
{i++}. 指标名称：{metric.Name}
    - `相似度评分`:{item.Score}
    - `候选字段`: 
        - `字段唯一标识`:   {item.Record.MataData.Id}
        - `物理名称`:   {item.Record.MataData.Name}
        - `说明`:   {item.Record.MataData.Description}
    - `所在业务域`: 
        - `名称`:   {item.Record.ObjectiveMataData.Name}
        - `说明`:   {item.Record.ObjectiveMataData.Description}
";
                        }

                    }
                }
                selectMetricPrompt += @"
# Core
1. 对于每个指标，首先分析它的名称和含义，理解用户的查询意图。
2. 然后，评估每个候选字段与指标的相关性，考虑相似度评分、字段说明、以及字段所在业务域的信息。
3. 无论是否候选成功，都请给出选择理由，若没有候选成功，更要详细说明为什么没有找到合适的字段。

## Schema Definition
你的输出 **必须** 严格遵守此 JSON 结构:
```json
[{
  ""metric_name"": ""指标名称"",
  ""selected_field_Id"": ""字段唯一标识"",
  ""selection_reason"": ""选择理由""
}]
```
";
                await _chatXService.Step2Async(selectMetricPrompt);






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
                    var businessObjective = new BusinessVectorItem() { ObjectiveId = body_vc.Record.MataData.Id };

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
                        filter: x => x.ObjectiveMataData.Id == body_vc.Record.MataData.Id)).Where(x => x.Score > 0.6).MaxBy(x => x.Score);
                        if (field_vc != null && businessObjective.Columns.Count(x => x.FieldName == "ALL") <= 0)
                        {
                            var record = field_vc.Record.MataData;
                            businessObjective.Columns.Add(new ColumnVectorMatch
                            {
                                Id = field_vc.Record.MataData.Id,
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
                            (not null, null) => broad_result1.Record.MataData.Id,
                            (null, not null) => broad_result2.Record.ObjectiveMataData.Id,
                            (not null, not null) => broad_result1.Score >= broad_result2.Score ? (int)broad_result1.Record.Id : broad_result2.Record.ObjectiveMataData.Id
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
                                _logger.LogWarning("字段{0}存在歧义，召回了多个相似字段，分别是{1}，需要后续通过倒排索引加权重或者业务域血缘等方式进行筛选", field, string.Join(", ", fields.Select(x => x.Record.MataData.Name)));

                            }
                            else
                            {
                                var vector_field = fields.FirstOrDefault();
                                var thisBody = businessVectorItems.Find(x => x.ObjectiveId == vector_field.Record.ObjectiveMataData.Id);
                                if (thisBody != null)
                                {
                                    thisBody.Columns.Add(new ColumnVectorMatch
                                    {
                                        Id = vector_field.Record.MataData.Id,
                                        FieldName = field
                                    });
                                }
                                else
                                {
                                    businessVectorItems.Add(new BusinessVectorItem
                                    {

                                        ObjectiveId = vector_field.Record.ObjectiveMataData.Id,
                                        Columns = new List<ColumnVectorMatch> { new ColumnVectorMatch
                                    {
                                         Id = vector_field.Record.MataData.Id,
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
                        filter: x => domainIds.Contains(x.ObjectiveMataData.Id))).Where(x => x.Score > 0.6).MaxBy(x => x.Score);

                        var thisBody = businessVectorItems.Find(x => x.ObjectiveId == field_vc.Record.ObjectiveMataData.Id);
                        if (thisBody != null)
                        {
                            thisBody.Columns.Add(new ColumnVectorMatch
                            {
                                Id = field_vc.Record.MataData.Id,
                                FieldName = condition.FieldName,
                            });
                        }
                        else
                        {
                            businessVectorItems.Add(new BusinessVectorItem
                            {
                                ObjectiveId = field_vc.Record.ObjectiveMataData.Id,
                                Columns = new List<ColumnVectorMatch> {
                                    new ColumnVectorMatch
                                    {
                                        Id = field_vc.Record.MataData.Id,
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
