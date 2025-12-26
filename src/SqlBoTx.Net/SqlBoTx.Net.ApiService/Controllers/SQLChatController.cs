using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using SqlBoTx.Net.ApiService.Dto;
using SqlBoTx.Net.ApiService.SqlPlugin;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ChatMessage = SqlBoTx.Net.ApiService.Dto.ChatMessage;

namespace SqlBoTx.Net.ApiService.Controllers
{
    /// <summary>
    /// sql Chat
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SQLChatController : ControllerBase
    {
        private readonly SqlBotPlugin _plugin;
        private readonly Kernel _kernel;
        private readonly ILogger<Program> _logger;
        private readonly SqlServerDatabaseService  _sqlServerDatabaseService;

        public SQLChatController(SqlBotPlugin plugin, Kernel kernel, ILogger<Program> logger, SqlServerDatabaseService sqlServerDatabaseService)
        {
            _plugin = plugin;
            _kernel = kernel;
            _logger = logger;
            _sqlServerDatabaseService = sqlServerDatabaseService;
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

            #region 1.意图识别 千问3-4B
            //发送空信息标记正在进行中，一次对话产生一个ChatMessage
            var msgId = Guid.CreateVersion7();

            //新增一个信息，推送状态
            await SendMessageAsync(HttpContext, new SQLBotMessage
            {
                SessionId = sessionId,
                Id = msgId,
                Role = Role.Assistant,
                Status = SQLBotMessageStatus.Streaming,
                CreatedAt = new DateTime().Ticks,
            }, ct);
            await SendMessageStreamingAsync(HttpContext, ct, msgId);

            _kernel.Plugins.AddFromObject(_plugin, "SqlBotPlugin");
            KernelFunction RecordIntentAnalysis = _kernel.Plugins.GetFunction("SqlBotPlugin", "RecordIntentAnalysis");

            var intentionChat = _kernel.InvokeStreamingAsync(_kernel.CreateFunctionFromPrompt(SystemPrompts.IntentionTool_3, new OpenAIPromptExecutionSettings
            {
                ServiceId = "qwen3-4b",
                Temperature = 0,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [RecordIntentAnalysis]),
            }), new KernelArguments
            {
                ["input"] = userInput
            });

            //流信息,发送block块
            await SendBlockAsync(HttpContext, new ContentBlockText
            {
                Id = Guid.CreateVersion7(),
                Status = BlockStatus.Streaming,
                CreatedAt = DateTime.Now.Ticks,
            }, ct);
            dynamic? tokenUsage = null;
            await foreach (var item in intentionChat)
            {
                var str = item.ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    await SendDeltaAsync(HttpContext, item.ToString(), ct);
                }

                if (item.Metadata != null && item.Metadata.TryGetValue("Usage", out var usage))
                {
                    tokenUsage = usage;
                }
            }

            //_logger.LogInformation($"意图分析消耗tokens:{tokenUsage.TotalTokenCount}");

            //阿里千问模型Usage 
            _logger.LogInformation($"意图分析消耗tokens:{tokenUsage.Details.TotalTokenCount}");

            if (_plugin.IntentAnalysis.Category ==  IntentAnalysisCategory.INVALID)
            {
                await SendBlockDoneAsync(HttpContext, ct);
                await SendMessageDoneAsync(HttpContext, ct);
                return;
            }
            #endregion

            #region 2.推断关系
           var relationships = _sqlServerDatabaseService.GetTableRelationships();
            var relationChat = _kernel.InvokeStreamingAsync(_kernel.CreateFunctionFromPrompt(SystemPrompts.TableIntentionTool, new OpenAIPromptExecutionSettings
            {
                ServiceId = "qwen3-4b",
                Temperature = 0,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [_kernel.Plugins.GetFunction("SqlBotPlugin", "TableIntention")]),
            }
            ), new KernelArguments
            {
                ["relations"] = relationships,
                ["intention"] = _plugin.IntentAnalysis.ThoughtProcess,
                ["input"] = userInput
            });
            await foreach (var item in relationChat)
            {
                var str = item.ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    await SendDeltaAsync(HttpContext, item.ToString(), ct);
                }
            }
            await SendBlockDoneAsync(HttpContext, ct);
            #endregion

            #region 3.生成SQL
            var getTableSchema = await _sqlServerDatabaseService.GetTableSchema(_plugin.TableIntentions);
            var _dbSchema = getTableSchema.Item1;

            var sqlPrompt = _kernel.CreateFunctionFromPrompt(SystemPrompts.SqlSystemPrompt, new OpenAIPromptExecutionSettings
            {
                ServiceId = "qwen3-4b",
                Temperature = 0,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [_kernel.Plugins.GetFunction("SqlBotPlugin", "SQLResult")]),
            });
            var sqlArgs = new KernelArguments
            {
                ["schema"] = _dbSchema,
                ["AGGREGATE"] = _plugin.IntentAnalysis.Category == IntentAnalysisCategory.AGGREGATE ? "当前用户查询是**聚合查询**" : "当前用户查询是**明细查询**",
                ["input"] = userInput,
            };
            var sqlChat = _kernel.InvokeStreamingAsync(sqlPrompt, sqlArgs);
            await foreach (var item in sqlChat)
            {
                var str = item.ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    Console.WriteLine(str);
                }
            }

            await SendBlockAsync(HttpContext, new ContentBlockSql
            {
                Id = Guid.CreateVersion7(),
                Status = BlockStatus.Done,
                CreatedAt = DateTime.Now.Ticks,
                Sql = _plugin.SqlStepResult.Sql
            }, ct);
            #endregion

            #region 4.查询SQL、生成图表
            var sqlItem = await _sqlServerDatabaseService.ExecuteQueryAsync(_plugin.SqlStepResult.Sql);
            Console.WriteLine(JsonSerializer.Serialize(sqlItem));

            await SendBlockAsync(HttpContext, new ContentBlockTable
            {
                Id = Guid.CreateVersion7(),
                Status = BlockStatus.Done,
                CreatedAt = DateTime.Now.Ticks,
                Columns = _plugin.SqlStepResult.Columns,
                Item = sqlItem.Select(x => (IDictionary<string, object>)x),
                Pagination = new TablePagination
                {
                    Current = 1,
                    PageSize = 20,
                    Total = 100,
                }
            }, ct);

            if (_plugin.SqlStepResult.NeedChart)
            {

            }

            #endregion

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
