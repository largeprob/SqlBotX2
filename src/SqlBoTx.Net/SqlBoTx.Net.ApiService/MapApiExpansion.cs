using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SqlBoTx.Net.ApiService.Dto;
using SqlBoTx.Net.ApiService.SqlPlugin;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static Dapper.SqlMapper;

namespace SqlBoTx.Net.ApiService
{
    /// <summary>
    /// minimal API 映射扩展
    /// </summary>
    public static class MapApiExpansion
    {
        extension(WebApplication app) {

            public WebApplication ChatApi()
            {
                app.MapPost("/chat", async ([FromBody] SQLChatMessage message,
                    [FromServices] Kernel _kernel, 
                    [FromServices] IChatCompletionService _chatCompletion, 
                    [FromServices] ILogger<Program> _logger,
                    [FromServices] SqlServerDatabaseService _sqlServerDatabase,
                    HttpContext context
                    ) =>
                {
                    context.Response.ContentType = "text/event-stream";
                    context.Response.Headers.Append("Cache-Control", "no-cache");
                    context.Response.Headers.Append("X-Accel-Buffering", "no");


                    var promptSettings = new OpenAIPromptExecutionSettings
                    {
                        Temperature = 0,
                        ResponseFormat = "json_object",
                    };

                    //1.意图识别
                    var intentionChat = await _kernel.InvokeAsync(_kernel.CreateFunctionFromPrompt(
                      SystemPrompts.Intention,
                      promptSettings), new KernelArguments
                      {
                          ["input"] = message.Content
                      });
              
                    //本次消耗tokens
                    if (intentionChat.Metadata != null && intentionChat.Metadata.ContainsKey("Usage"))
                    {
                        var usage = (dynamic)intentionChat.Metadata["Usage"];
                        _logger.LogInformation($"intentionChat消耗tokens:{usage.TotalTokenCount}");
                    }

                    //数据
                    var intentionChatJson = intentionChat.GetValue<string>();
                    _logger.LogInformation($"intentionChat:{intentionChatJson}");
                    var sqlStepIntention = JsonSerializer.Deserialize<SqlStepIntention>(intentionChatJson);

                    //2.生成SQL
                    var getTableSchema = await _sqlServerDatabase.GetTableSchema(await _sqlServerDatabase.GetAllTableNames());
                    var _dbSchema = getTableSchema.Item1;

                    var sqlChat = await _kernel.InvokeAsync(_kernel.CreateFunctionFromPrompt(
                        SystemPrompts.SqlSystemPrompt,
                        promptSettings
                    ), new KernelArguments
                    {
                        ["schema"] = _dbSchema,
                        ["input"] = message.Content
                    });
                    //本次消耗tokens
                    if (sqlChat.Metadata != null && sqlChat.Metadata.ContainsKey("Usage"))
                    {
                        var usage = (dynamic)sqlChat.Metadata["Usage"];
                        _logger.LogInformation($"sqlChat消耗tokens:{usage.TotalTokenCount}");
                    }

                    //数据
                    var sqlChatJson = sqlChat.GetValue<string>();
                    _logger.LogInformation($"sqlChat:{sqlChatJson}");
                    var sqlStepResult = JsonSerializer.Deserialize<SqlStepResult>(sqlChatJson);
                    if (!string.IsNullOrEmpty(sqlStepResult.Message))
                    {
                        await SendDeltaAsync(context, sqlStepResult.Message);
                    }
                    if (!string.IsNullOrEmpty(sqlStepResult.Sql))
                    {
                        await SendSqlBlockAsync(context, [sqlStepResult.Sql]);
                    }

                    //如果需要图表
                    if (sqlStepIntention.OutVisualType == OutVisualType.Echarts)
                    {
                        var echartsChat = await _kernel.InvokeAsync(_kernel.CreateFunctionFromPrompt(
                           SystemPrompts.Echarts,
                           promptSettings
                           ), new KernelArguments
                           {
                               ["input"] = message.Content,
                               ["sql"] = sqlStepResult.Sql,
                           });

                        //本次消耗tokens
                        if (echartsChat.Metadata != null && echartsChat.Metadata.ContainsKey("Usage"))
                        {
                            var usage = (dynamic)echartsChat.Metadata["Usage"];
                            _logger.LogInformation($"echartsChat消耗tokens:{usage.TotalTokenCount}");
                        }

                        //数据
                        var echartsChatJson = echartsChat.GetValue<string>();
                        _logger.LogInformation($"echartsChat:{echartsChatJson}");

                        //执行sql
                        var sqlExecutionResult = await _sqlServerDatabase.ExecuteQueryAsync(sqlStepResult.Sql);

                        var processedOption = Helpers.InjectDataIntoEchartsOption(echartsChatJson, sqlExecutionResult.ToArray());
                        await SendEchartsBlockAsync(context, processedOption);
                    }

                    //如果需要基础表格
                    if (sqlStepIntention.OutVisualType == OutVisualType.BasicTable)
                    {
                        var sqlExecutionResult = await _sqlServerDatabase.ExecuteQueryAsync(sqlStepResult.Sql);
                        await SendTableBlockAsync(context, sqlStepResult.Columns, sqlExecutionResult, sqlExecutionResult.Count());
                    }

                })
                    .WithName("chat")
                    .AddOpenApiOperationTransformer((opperation, context, ct) =>
                    {
                        opperation.Summary = "对话";
                        opperation.Description = "基础对话SSE";
                        return Task.CompletedTask;
                    });


                app.MapGet("/chatTool", async (string userQuery, [FromServices] Kernel _kernel, [FromServices] IChatCompletionService _chatCompletion, [FromServices] ILogger<Program> _logger, [FromServices] SqlPlugin.Plugin _plugin) =>
                {
                    string _dbSchema = @"
        Table: Products (Id, Name, Category, Price, Stock)
        Table: Sales (Id, ProductId, SaleDate, Quantity, Amount)
    ";

                    // 核心 Prompt 模板
                    const string SqlSystemPrompt = @"
基础数据：
    这是数据库的 Schema 定义：
    {{$schema}}

角色定义：
你是一个智能调用工具的助手，在恰当时通过调用提供的工具来协助完成任务。
你是一个拥有数据库操作权限的 SQL Server 智能助手。

核心规则：
1. **Schema 限制**：你的理解范围仅限于提供的 Schema 定义。不要臆造不存在的表或字段。
2. **语法要求**：确保 sql 在 SQL Server 中可直接执行。涉及日期请使用标准格式。
3. **越界处理**：当用户请求超出 Schema 范围时，尝试从工具获取，若工具中数据不符合，请在 `message` 参数中礼貌拒绝。

重要安全规则（必须严格遵守）：
1. **默认行数限制**：如果用户没有明确要求“全部”或指定具体数量，生成的 SQL 必须包含 `TOP 20`。
2. **大数据熔断保护**：
   - 警告：即使用户明确说“查询所有数据”或“显示全部”，**绝对禁止**生成 `SELECT *`（这会导致性能事故）。
   - 操作：你必须强制将查询限制为 `TOP 50`。
   - 解释：必须在工具的 `message` 参数中告知用户：“为了保护数据库性能，已为您展示前 50 条数据。如需更多，请指定具体过滤条件。”
3. **分页语法**：如果用户要求“下一页”，必须在 `sql_query` 中使用 `ORDER BY ... OFFSET ... ROWS FETCH NEXT ... ROWS ONLY` 语法。

强制性工作流程：
第一步：分析用户的自然语言意图（结合 {{$history}}）。
第二步：判断是否需要可视化（决定 `NeedChart` 参数）。
第三步：结合 Schema 生成安全的 SQL 语句（构建 `sql` 参数）。
第四步：检查 SQL 是否违反“大数据保护”规则，如有必要进行修正。

最终返回值规则：
在最后你选择结束本次对话时，你的返回值必须是一个 JSON 对象，结构为：
``Message(string),Sql(string), NeedChart(bool)``。

用户当前输入: 
{{$input}}
";




                    //_kernel.Plugins.AddFromObject(_plugin);
                    //KernelFunction YourSelf = _kernel.Plugins.GetFunction("Plugin", "YourSelf");
                    //KernelFunction GetTableSchema = _kernel.Plugins.GetFunction("Plugin", "GetTableSchema");
                    //KernelFunction GetAllTable = _kernel.Plugins.GetFunction("Plugin", "GetAllTable");

                    var promptSettings = new OpenAIPromptExecutionSettings
                    {
                        Temperature = 0,
                        ResponseFormat = "json_object",
                        //FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(
                        //    functions: [YourSelf, GetTableSchema, GetAllTable]
                        //    ),
                        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    };
                    var convertFunction = _kernel.CreateFunctionFromPrompt(
                        SqlSystemPrompt,
                        promptSettings
                    );

                    var sqlResult = await _kernel.InvokeAsync(convertFunction, new KernelArguments
                    {
                        ["schema"] = _dbSchema,
                        ["input"] = userQuery
                    });

                    //var sqlResult = await _kernel.InvokePromptAsync(SqlSystemPrompt, new(promptSettings)
                    //{
                    //    ["schema"] = _dbSchema,
                    //    ["input"] = userQuery
                    //});


                    // 解析 JSON
                    var jsonStr = sqlResult.GetValue<string>();
                    _logger.LogInformation(jsonStr);

                    var resultObj = JsonSerializer.Deserialize<SqlStepResult>(jsonStr);
            

                    //本次消耗tokens
                    var metadata = sqlResult.Metadata;
                    if (metadata != null && metadata.ContainsKey("Usage"))
                    {
                        var usage = (dynamic)metadata["Usage"];
                        _logger.LogInformation($"本次消耗：{usage.TotalTokenCount} tokens");
                    }

                    //将sql放入空置数据库执行，以验证sql是否正确

                    //如果不需要图表，直接返回结果


                    //如果需要图表，开启第二轮对话，将结果放入以生成Echarts JSON


                    // 核心 Prompt 模板
                    const string EchartsSystemPrompt = @"
角色定义：
你是一个 Echarts 专家,你的任务是将数据结果转为Echarts渲染数据集。

核心规则：
你对表架构的理解范围仅限于提供的 Schema 定义。
确保 生成的Echarts JSON 可运行。

强制性工作流程：
第一步：了解用户的意图（考虑对话历史）
第二步：判断用户是否需要可视化图表（例如提到'趋势'、'图'、'占比'或数据显然适合图表展示）
第三步：结合 Schema 生成符合用户意图的 SQL 查询语句
第四步：校验 SQL 语句的正确性以及是否符合规则
第五步：直接向用户返回数据
";






                })
                  .WithName("chatTool")
                  .AddOpenApiOperationTransformer((opperation, context, ct) =>
                  {
                      opperation.Summary = "对话包含工具调用";
                      opperation.Description = "基础对话SSE";
                      return Task.CompletedTask;
                  });


                app.MapGet("/chatToolSSE", async (string userQuery, 
                    [FromServices] Kernel _kernel,
                    [FromServices]  IChatCompletionService _chatCompletion, 
                    [FromServices]  ILogger<Program> _logger) =>
                {
                    string _dbSchema = @"
        Table: Products (Id, Name, Category, Price, Stock)
        Table: Sales (Id, ProductId, SaleDate, Quantity, Amount)
    ";

                    // 核心 Prompt 模板
                    const string SqlSystemPrompt = @"
基础数据：
    这是数据库的 Schema 定义：
    {{$schema}}

角色定义：
你是一个拥有数据库操作权限的 SQL Server 智能助手。

核心规则：
1. Schema：你的理解范围仅限于提供的 Schema 定义，不要臆造不存在的表或字段。
2. Schema越界处理：当用户请求超出 Schema 范围时，尝试从工具获取，若工具中数据不符合，请在 `Message` 参数中礼貌拒绝。
3. 语法要求：确保 sql 在 SQL Server 中可直接执行。涉及日期请使用标准格式。

重要安全规则（必须严格遵守）：
1. **默认行数限制**：如果用户没有明确要求“全部”或指定具体数量，生成的 SQL 必须包含 `TOP 20`。
2. **大数据熔断保护**：
   - 警告：即使用户明确说“查询所有数据”或“显示全部”，**绝对禁止**生成 `SELECT *`（这会导致性能事故）。
   - 操作：你必须强制将查询限制为 `TOP 50`。
   - 解释：必须在工具的 `Message` 参数中告知用户：“为了保护数据库性能，已为您展示前 50 条数据。如需更多，请指定具体过滤条件。”
3. **分页语法**：如果用户要求“下一页”，必须在 `sql_query` 中使用 `ORDER BY ... OFFSET ... ROWS FETCH NEXT ... ROWS ONLY` 语法。

强制性工作流程：
1：分析用户的自然语言意图（结合 {{$history}}）。
2：判断是否需要可视化（决定 `NeedChart` 参数）。
3：判断是否超出 Schema 范围，尝试从工具中寻找解决方案。
4：结合 Schema 生成安全的 SQL 语句（构建 `sql` 参数）。
5：检查 SQL 是否违反“大数据保护”规则，如有必要进行修正。

最终返回值规则：
在最后你选择结束本次对话时，你的返回值必须是一个 JSON 对象，结构为：
``Message(string),Sql(string), NeedChart(bool)``。

用户当前输入: 
{{$input}}
";
                    var promptSettings = new OpenAIPromptExecutionSettings
                    {
                        Temperature = 0,
                        ResponseFormat = "json_object",
                        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    };
                    var convertFunction = _kernel.CreateFunctionFromPrompt(
                        SqlSystemPrompt,
                        promptSettings
                    );

                    //流
                    var sqlResult = _kernel.InvokeStreamingAsync(convertFunction, new KernelArguments
                    {
                        ["schema"] = _dbSchema,
                        ["input"] = userQuery
                    });
                    await foreach (var chunk in sqlResult)
                    {
                        Console.Write(chunk);
                    }

                    Console.WriteLine(sqlResult);

                    //// 解析 JSON
                    //var jsonStr = sqlResult.GetValue<string>();
                    //_logger.LogInformation(jsonStr);

                    //var resultObj = JsonSerializer.Deserialize<SqlStepResult>(jsonStr);


                    ////本次消耗tokens
                    //var metadata = sqlResult.Metadata;
                    //if (metadata != null && metadata.ContainsKey("Usage"))
                    //{
                    //    var usage = (dynamic)metadata["Usage"];
                    //    _logger.LogInformation($"本次消耗：{usage.TotalTokenCount} tokens");
                    //}

                    //将sql放入空置数据库执行，以验证sql是否正确

                    //如果不需要图表，直接返回结果


                    //如果需要图表，开启第二轮对话，将结果放入以生成Echarts JSON


                    // 核心 Prompt 模板
                    const string EchartsSystemPrompt = @"
角色定义：
你是一个 Echarts 专家,你的任务是将数据结果转为Echarts渲染数据集。

核心规则：
你对表架构的理解范围仅限于提供的 Schema 定义。
确保 生成的Echarts JSON 可运行。

强制性工作流程：
第一步：了解用户的意图（考虑对话历史）
第二步：判断用户是否需要可视化图表（例如提到'趋势'、'图'、'占比'或数据显然适合图表展示）
第三步：结合 Schema 生成符合用户意图的 SQL 查询语句
第四步：校验 SQL 语句的正确性以及是否符合规则
第五步：直接向用户返回数据
";

                })
                .WithName("chatToolSSE")
                .AddOpenApiOperationTransformer((opperation, context, ct) =>
                {
                    opperation.Summary = "对话包含工具调用";
                    opperation.Description = "基础对话SSE";
                    return Task.CompletedTask;
                });



                return app;
            }

            public WebApplication SystemApi()
            {
                app.MapGet("/login", async (string account, string password, HttpContext context
                    ) =>
                {
                    if (account != "admin" || password != "123456")
                    {
                        return Results.BadRequest("用户名或密码错误");
                    }

                    // 构造用户信息
                    var user = new UserInfo
                    {
                        Id = 1,
                        Username = "测试账号",
                        Email = "admin@example.com",
                        FirstName = "largeprob",
                        LastName = "largeprob",
                        Age = 25,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    // 将用户信息序列化存入 Cookie（也可存 Token）
                    var json = JsonSerializer.Serialize(user);

                    context.Response.Cookies.Append("auth_user", json, new CookieOptions
                    {
                        HttpOnly = true,         
                        Secure = true,       
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
                    return Results.Ok(user);

                }).WithName("login");

                app.MapGet("/checkUser", async (HttpContext context) =>
                {
                    if (context.Request.Cookies.TryGetValue("auth_user", out var json))
                    {
                        var user = JsonSerializer.Deserialize<UserInfo>(json);
                        return Results.Ok(new { data = user });
                    }
                    return Results.Unauthorized(); 
                }).WithName("checkUser");

                return app;
            }

            /// <summary>
            /// 发送增量文本（流式输出）
            /// </summary>
            private static async Task SendDeltaAsync(HttpContext context, string delta)
            {
                var message = new Dto.DeltaMessage { Delta = delta };
                await SendMessageAsync(context, message);
            }



            /// <summary>
            /// 发送内容块
            /// </summary>
            private static async Task SendBlockAsync(HttpContext context, Dto.ContentBlock block)
            {
                var message = new Dto.BlockMessage { Block = block };
                await SendMessageAsync(context, message);
            }

            /// <summary>
            /// 发送 SQL 块
            /// </summary>
            private static async Task SendSqlBlockAsync(HttpContext context, string[] sqls, string? dialect = null)
            {
                foreach (var sql in sqls)
                {
                    var block = new Dto.SqlBlock
                    {
                        Sql = sql,
                        Dialect = dialect
                    };
                    await SendBlockAsync(context, block);
                }
            }

            /// <summary>
            /// 发送内容块
            /// </summary>
            private static async Task SendEchartsBlockAsync(HttpContext context, string options)
            {
                var chartBlock = new EchartsBlock
                {
                    EchartsOption = options
                };
                await SendBlockAsync(context, chartBlock);
            }

            /// <summary>
            /// 发送数据块
            /// </summary>
            private static async Task SendTableBlockAsync(HttpContext context, SqlStepColumns[] columns, IEnumerable<dynamic> rows, int totalRows)
            {
                var block = new Dto.TableBlock
                {
                    Columns = columns,
                    Rows = rows,
                    TotalRows = totalRows
                };
                await SendBlockAsync(context, block);
            }

            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="context"></param>
            /// <param name="message"></param>
            /// <returns></returns>
            private static async Task SendMessageAsync(HttpContext context, Dto.SSEMessage message)
            {
                var json = JsonSerializer.Serialize(message, message.GetType(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                var data = $"data: {json}\n\n";
                var bytes = Encoding.UTF8.GetBytes(data);

                await context.Response.Body.WriteAsync(bytes);
                await context.Response.Body.FlushAsync();
            }

       
        }
    }
}
