using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;
using SqlBoTx.Net.ApiService.Dto;
using SqlBoTx.Net.ApiService.SqlPlugin;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives;
using SqlBoTx.Net.Application.Contracts.TableStructures;
using SqlBoTx.Net.Application.Vectors;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    
 
    public class SQLChatXService
    {
        private readonly SqlBotPlugin _plugin;
        private readonly Kernel _kernel;
        private readonly ILogger<Program> _logger;
        private readonly SqlServerDatabaseService _sqlServerDatabaseService;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly QdrantVectorService _qdrantVectorService;
        private readonly IBusinessObjectiveService _businessObjectiveService;
        private readonly ITableRelationshipService _tableRelationshipService;
        private readonly ITableStructureService _tableStructureService;

        public SQLChatXService(SqlBotPlugin plugin, Kernel kernel, ILogger<Program> logger, SqlServerDatabaseService sqlServerDatabaseService, IChatCompletionService chatCompletionService, IServiceProvider serviceProvider, QdrantVectorService qdrantVectorService, IBusinessObjectiveService businessObjectiveService, ITableRelationshipService tableRelationshipService, ITableStructureService tableStructureService)
        {
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
        /// 语义任务拆分
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        public async Task<TaskDecompositionResult> SplitTaskAsync(string userInput)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("deepseek-chat")!;
            var chatClient = client.GetService<ChatClient>()!;

            var msgs = new List<ChatMessage> {
                 new (ChatRole.System, SQLBotxPrompts.SplitTask),
                 new (ChatRole.User,userInput)
            };

            var apiResult = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(new
            {
                model = "deepseek-chat",
                messages = msgs.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                response_format = new { type = "json_object" },
                enable_thinking = false,
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            }));
            QwenChatCompletion qwenChatCompletion = apiResult.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;
            var content = qwenChatCompletion.Choices.First().Message.Content;
            _logger.LogInformation("语义任务拆分返回：{0}", content);

            var result = JsonSerializer.Deserialize<TaskDecompositionResult>(content, SqlBotXJsonOptions.Deserialize)!;
            return result;
        }


        /// <summary>
        /// 语义提取
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="userInput"></param>
        /// <returns>闲聊/业务</returns>
        public async Task<string> ChatAsync(string prompt, string userInput)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("deepseek-chat")!;
            var chatClient = client.GetService<ChatClient>()!;
            var msgs = new List<ChatMessage> {
                 new (ChatRole.System, prompt),
                 new (ChatRole.User,userInput)
            };
            var result = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(new
            {
                model = "deepseek-chat",
                messages = msgs.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                enable_thinking = false
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            }));
            QwenChatCompletion qwenChatCompletion = result.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;
            var content = qwenChatCompletion.Choices.First().Message.Content;
            return content;
        }

        /// <summary>
        /// 语义分析
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns>闲聊/业务</returns>
        public async Task<IntentAnalysisResult> Step1Async(string userInput)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("deepseek-chat")!;
            var chatClient = client.GetService<ChatClient>()!;

            var msgs = new List<ChatMessage> {
                 new (ChatRole.System, @"
SQLBotX 是一个专业的 NL2SQL (Natural Language to SQL) 和 BI 分析平台，专注于将用户的自然语言转化为数据库查询语句。

# Role (角色设定):
你是指挥 **SQLBotX 智能问数系统** 流量分发的“意图守门员”。

# Goal (任务目标):
你的唯一任务是精准判断用户的输入属于 **[ 业务查询 (DATA_QUERY) ]** 还是 **[ 闲聊/通用问答 (CHIT_CHAT) ]**。
你需要保护后端的 SQL 生成引擎，防止非数据类的请求进入核心计算流程。

# Classification Criteria (分类标准):
## 1. 🟢 DATA_QUERY (业务查询)
当用户意图**必须访问业务数据库**才能得到答案时，属于此类。
- **特征词**：查询、统计、分析、多少、占比、趋势、明细、所有、Top N、环比、同比、给我。
- **业务对象**：提及了具体的业务实体/业务领域/数据表。
- **模糊检索**：看起来像是对数据库中某个具体的“名称”或“单号”的搜索（如：“查一下华为的合同”、“看下单号 A1002”）。
- **元数据查询**：询问系统有哪些数据（如：“你有哪几张表？”、“库里有关于销售的数据吗？”）。

## 2. 🔴 CHIT_CHAT (闲聊/通用问答)
当用户意图与**当前业务数据库的数据无关**，或仅涉及系统本身的使用说明时，属于此类。
- **寒暄**：你好、在吗、你是谁、早上好。
- **身份询问**：你叫什么名字？你是 GPT 吗？
- **通用知识**：今天天气怎么样？Python 怎么写？什么是元宇宙？（即使涉及知识，但不是从用户私有库里查的，都算闲聊）。
- **操作帮助**：这个系统怎么用？能给我个说明书吗？（这是系统帮助，无需生成 SQL）。
- **无意义乱码**：asdf、123（非特定ID）。 

# Decision Logic (判断逻辑 - 思考链):
1. **Check Entities**: 输入中是否包含可能存在于数据库中的业务术语或实体？(是 -> 倾向 Query)
2. **Check Action**: 用户是否想要“获取、计算、展示”某些信息？(是 -> 倾向 Query)
3. **Check Context**: 如果输入是“苹果”，在 SQLBotX 语境下，它更有可能是查询“苹果公司”的资料，而不是问“苹果是不是水果”。(倾向 Query)
4. **Exclusion**: 如果用户问“你是谁”，虽然包含“是”，但这不需要查库。(倾向 Chat)

# Output Schema (输出规范):
严格输出 JSON 格式，不要包含 Markdown 标记。
{
  ""category"": ""DATA_QUERY"" | ""CHIT_CHAT"",
  ""confidence"": 0.0 - 1.0,
  ""intent_summary"": ""一句话概括用户想做什么"",
  ""suggested_reply"": ""如果是CHIT_CHAT，在此生成一句简短幽默的回复（以SQLBotX身份）；如果是DATA_QUERY，输出 null""
}
"),
                 new (ChatRole.User,userInput)
            };

            var request = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(new
            {
                model = "deepseek-chat",
                messages = msgs.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                response_format = new { type = "json_object" },
                enable_thinking = false,
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            }));
            QwenChatCompletion qwenChatCompletion = request.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;
            var content = qwenChatCompletion.Choices.First().Message.Content;
            _logger.LogInformation("语意分析返回：{0}", content);

            var result = JsonSerializer.Deserialize<IntentAnalysisResult>(content,SqlBotXJsonOptions.Deserialize)!;
            return result;
        }


        /**
         * 给我销售大表
         * 给我 xxx
         * 
         * 
         * 
         */

        /// <summary>
        /// 语义提取
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns>闲聊/业务</returns>
        public async Task<FuzzyIntentResult> Step2Async(string userInput)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("deepseek-chat")!;
            var chatClient = client.GetService<ChatClient>()!;

            List<ChatMessage> msgs = new List<ChatMessage>() {
                new(ChatRole.System, @"
# 语义解析系统 (Semantic Parsing System)

## 描述
你是一项非常先进的技能，专门用于解构复杂的自然语言查询。你的核心能力是首先识别相关的**业务域**，然后针对每个业务域执行详细的意图识别，最终生成一个结构化的、嵌套的 JSON 输出。

## 上下文信息

### 系统日期
`{CURRENT_DATE}` (**关键**: 所有相对日期必须基于此日期进行计算)。

## 系统级原则（Core）
用户的所有输入，无论表述如何，其最终目的都是获取一个数据结果集，一个数据查询任务，您的目标永远是构建 **一张统一的二维数据表** ,涉及的不同业务对象一定是需要关联查询的
 
## 提示词 (Prompt)
你是一名 **语义解析架构师 (Semantic Parsing Architect)**。你的目标是将非结构化的用户请求精准地转化为一个结构完美、层级分明的 JSON 对象。你的首要指令是 **以清晰无歧义的方式，保留用户用于数据操作的原始描述性语言**。
 
## 指令 (Instructions)
当接收到用户查询时，你必须严格遵循以下操作顺序：
-  **`intent`**: 分类为 `ANALYTIC`（分析型） 或 `DATA`（检索型）。
-  **`global_filters`**: 按照下文描述 `全局过滤树解析`
-  **`dimensions`**:按照下文描述 `维度解析`
-  **`metrics`**:  按照下文描述 `指标解析`
-  **`columns`**: 按照下文描述 `显示列解析`
-  **`business_objects`**: 按照下文描述 `业务对象解析`
-  **`limit`**:  按照下文描述 `Limit&Pagination解析`
-  **`pagination`**:  按照下文描述 `Limit&Pagination解析`

## 全局过滤树解析
- 当检测到限制条件时，请问自己一个问题：‘这个条件是写在每一张原始单据上的（属性），还是算完总账后才有的（指标）？
- 如果是 **算出来的数值** 的限制条件请忽略，仅仅提取作为全局范围限制的过滤语句
- **映射运算符到中文**: 将逻辑转换为标准中文运算符列表中的一项：`[等于, 不等于, 大于, 小于, 大于等于, 小于等于, 包含, 在列表, 区间,前匹配,后匹配，其他]`。
- **构建树**: 构建 `group` 和 `condition` 对象的嵌套结构，以精确表示用户的逻辑。
- **禁止推断字段名**: 在 global_filters 中，如果用户没有明确提到了字段名称，严禁自动脑补

### Predefined Tags
你必须将提取到的字段分类到以下 **唯一** 的标签列表中。如果无法匹配前项，则归类为“其他”。
-  **标签列表**:
 `[地区, 部门, 组织, 角色, 日期时间, 分类, 其他]`

### 1. 显式字段
*   **判定标准**: 用户在句子中明确的说出了字段名称
*   **句式**: `[字段名] [谓语] [值]`
*   Case: ‘创建日期为2026年’ -> 用户说了'创建日期'，这是显式
*   Case: '客户等级是VIP' -> 用户说了'客户等级'，这是显式

### 2. 隐式字段
 *  **判定标准**: 用户未提及具体字段名，只说了值。字段名是你推断出来的
*   **句式**: `[谓语] [值]` 或 `[值]`
*   Case: '**VIP**客户' -> 用户没说'等级'，'VIP'修饰客户。-> **field_type: 隐式**, Name: '客户等级' (推断), Value: 'VIP'
*   Case: '**2026年**的...' -> 用户没说'日期'。-> **field_type: 隐式**, Name: '日期' (推断), Value: '2026-01-01'...


### 3. 标签映射逻辑
提取出字段名称后，请根据其语义与值类型判断将其归类到预定义标签：
*   **地理/位置** -> `地区` (如: 城市、省份、区域)
*   **内部架构** -> `部门`  (如: 销售部、市场部)
*   **内部架构** -> `组织` (如: 某子公司、分公司、某事业部)
*   **人员属性** -> `角色` (如: 经理、员工、负责人)
*   **时间颗粒度** -> `日期时间`
*   **属性/类型** -> `分类` (如: 行业、等级、颜色、状态)
*   **无法归类** -> `其他`

### 4. 日期/时间值标准化 (关键规则)
当 `field_tag` 为 `日期/时间` 时，必须执行以下转换：
1.  **绝对化**: 将所有模糊时间词（Q1-Q4, 上周, 去年, 8月）转换为具体的 **YYYY-MM-DD** 格式。
2.  **区间化**:
    *   如果用户提到的是一个**时间段**（如'2026年', 'Q3', '8月'），`field_operator` 必须设为 **'区间'**。
    *   `field_value` 必须包含该时间段的 **[起始日期, 结束日期]**。
3.  **计算逻辑 (基于当前时间)**:
    *   **年份**: '2026年' -> `[2026-01-01, 2026-12-31]`
    *   **季度 (Q1-Q4)**:
        *   Q1: 01-01 ~ 03-31
        *   Q2: 04-01 ~ 06-30
        *   Q3: 07-01 ~ 09-30
        *   Q4: 10-01 ~ 12-31
    *   **月份**: '8月' (假设当年) -> `[2026-08-01, 2026-08-31]`
    *   **相对词**: '上周' -> 计算出上周一到上周日的具体日期。


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
1.  **对象结构**: `{ ""page"": Number, ""size"": Number }`。
2.  **`page` (页码)**:
    *   提取 '第N页' 中的 N。
    *   如果未提及页码，但提及了每页条数，**默认 `page: 1`**。
3.  **`size` (每页条数)**:
    *   提取 '每页N条'、'显示N个' 中的 N。
    *   如果未提及条数，但提及了页码，**默认 `size: 20`** (系统默认值)。
4.  **模糊场景处理 (关键)**:
    *   如果用户只说了“查 10 条”、“看 5 个” (没有“前/Top”等修饰词)，视为 **分页的 `size`**，即 `{ ""page"": 1, ""size"": 10 }`，**而不是** `limit`。

## 业务对象解析
- 用户想看到的东西从什么中获取？直接从用户输入中识别正在被分析的主要主体或业务对象，保留定义该业务实体的 **根名词**。
- **去重原则**：同一个主体在数组中只能出现一次。
- 全量字段判断:
从 `显示列` 中判断哪些字段是一个包含多个相关字段的数据集合。
[“资料”、“信息”、“详情”、“全部”、“所有”] 等词汇通常暗示用户希望获取一个完整的业务对象，而不仅仅是一个单一的字段。因此，一个 `业务对象` 的全量（full）判断标准可以参考以下：
1. `显示列` 包含上述相似词汇或以此结尾的查询
2. `显示列` 以业务域的**根名词** + 上述相似词汇结尾组成或以业务域的**根名词** 包含上述相似词汇组成

## 显示列解析
- 提取用户明确提及的、需要展示的内容的 **根名词**

## 维度解析
### Predefined Tags
你必须将提取到的维度映射到以下 **唯一** 的标签列表中。如果无法匹配前项，则归类为“其他”。
-**标签列表**: `[地区, 部门, 组织, 角色, 年, 月, 日, 天, 时, 分, 秒, 实体, 分类, 其他]`

### 1. 维度类型判断 (Dimension Type)
*   **显式维度 (EXPLICIT)**:
    *   **判定标准**: 用户明确使用了指示分组的介词或量词。
    *   **关键词**: “按...”、“根据...”、“分...”、“各...”、“每一个...”、“不同...”、“分别是...”。
    *   *示例*: “按**地区**统计” -> 显式维度: `地区`。

*   **隐式维度 (IMPLICIT)**:
    *   **判定标准**: 用户未提及具体字段名，但其分析意图（如趋势、排名、分布）在逻辑上必须依赖某个特定维度。
    *   **常见映射**:
        *   “趋势/走势/变化” -> 隐含 **时间维度** (通常映射为 `月` 或 `年`，视上下文而定)。
        *   “排名/Top N/谁” -> 隐含 **实体维度** (如客户、产品)。
        *   “分布/占比/结构” -> 隐含 **分类维度**。
    *   *示例*: “销售**趋势**” -> 隐式维度: `月` (假设默认粒度)。

### 2. 标签映射逻辑 (Tag Mapping)
提取出维度名称后，请根据其语义将其归类到预定义标签：
*   **地理/位置** -> `地区` (如: 城市、省份、区域)
*   **内部架构** -> `部门` / `组织` (如: 销售部、分公司)
*   **人员属性** -> `角色` (如: 经理、员工、负责人)
*   **时间颗粒度** -> `年` / `月` / `日` / `天` / `时` / `分` / `秒` (严格匹配粒度)
*   **核心业务对象** -> `实体` (如: 客户、产品、门店、合同、项目)
*   **属性/类型** -> `分类` (如: 行业、等级、颜色、状态)
*   **无法归类** -> `其他`

### 3. **严禁提取筛选值 (CRITICAL: Filter vs Dimension)**:
    *   **这是最容易犯错的地方！**
    *   如果用户提到的是一个 **具体的值 (Instance)**，它属于筛选条件 (Where)，**绝对不是** 维度。
    *   *Case A*: “查询**北京**的销售额” -> `北京` 是值，不是维度。Dimensions: `[]` (空)
    *   *Case B*: “查询**各城市**的销售额” -> `城市` 是类别。Dimensions: `[“城市”]`
    *   *Case C*: “查询**2026年**的利润” -> `2026年` 是值。Dimensions: `[]` (空)

### 4. **标量计算不提取维度 (Scalar Calculation)**:
    *   如果用户要求的是两个具体值的对比（产生一个数字），则没有维度。
    *   *Case*: “2026年Q3与Q2的差值” -> 这是一个减法公式，结果是一行数字。Dimensions: `[]` (空)


## 指标解析

### 核心筛选原则
**只提取可用于数据分析的指标字段**
1. **数值型可计算字段**：可进行求和、平均、最大最小等数值运算
2. **计数类指标**：当用户明确需要计数时，背后的计数对象可提取
3. **比率/复合指标**：符合业务分析需求的指标

### 提取规则
1. **过滤非指标字段**：首先根据排除规则，过滤掉所有不应作为指标提取的字段。
2. **识别所有指标**：从过滤后的用户输入中找出所有需要计算的指标项。
4. **确定指标名称和tag**：
    - 输出该指标对应的**指标名称**（如果是计数类，输出“用户数”“订单量”等计数概念名称，而非背后的ID字段）
5. **去重处理**：相同的指标名称只输出一次。

### 指标条件提取
- 当检测到限制条件时，请问自己一个问题：‘这个条件是写在每一张原始单据上的（属性），还是算完总账后才有的（指标）？
- 如果是 **算出来的数值** (如总额>W、排名Top3、增长>10%) $\rightarrow$ 归类为 **`metrics.filters` (对应 HAVING)**

### 指标依赖维度提取
- 对于每一个提取到的指标，请识别它 **必须依赖的分组维度**。从`dimensions` 中选择适合的维度

## 重要规则
1.  **动态业务域提取 (关键)**: `domain` 字段必须是一个 **直接从用户查询中提取** 的简练、描述性短语，它能最好地代表核心业务领域。不要试图将其映射到预定义列表。
3.  **过滤值的一致性 (关键)**: 任何过滤 `condition` 中的 `value` 字段 **必须始终是一个数组**，即使它只包含一个元素。
4.  **具体且结构化**: 过滤器不要生成扁平列表。必须构建逻辑树。
 
## Output Schema
你的输出 **必须** 严格遵守此 JSON 结构。
```json
  {
        ""intent"": ""Enum<ANALYTIC, Data>"",
        ""target_core"": ""String (用户目标的简练描述)"",
        ""global_filters"": {
            ""filter_type"": ""group | condition"",
            ""logic"": ""AND | OR"",
            ""children"": [
              {
                ""filter_type"": ""group | condition"",
                ""field_type"":""显式/隐式"",
                ""field_tag"": ""字段标签"",
                ""field_name"": ""字段名称"",
                ""field_operator"": ""映射运算符"",
                ""field_value"": ""Array<String> (注意：必须始终为数组)"" 
              }
            ]
        } | null,
        ""dimensions"": [
          { ""type"": ""显示|隐式"", ""name"": """", ""tag"": """" }
        ],
        ""metrics"": [
          {
            ""name"": """", 
            ""dependent_dims"":[""""],
            ""filters"":{
                ""field_name"": ""字段名称"",
                ""field_type"":""显式/隐式"",
                ""field_tag"": ""字段标签"",
                ""field_operator"": ""映射运算符"",
                ""field_value"": ""Array<String> (注意：必须始终为数组)"" 
            },
          }
        ],
        ""business_objects"":[
            {
                ""name"":""String(业务对象名)"",
                ""has_full"": ""Boolean(是否拥有全量字段)"",
                ""full_column"":""String(全量字段名)""
            }
        ],
        ""columns"":[""""],
        ""limit"": ""Number | null"",
        ""pagination"":{ ""page"": ""Number"", ""size"": ""Number"" }
  }
```
 ".Replace("{CURRENT_DATE}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))),
                new(ChatRole.User,userInput)
            };
            var request = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(new
            {
                model = "deepseek-chat",
                messages = msgs.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                response_format = new { type = "json_object" },
                enable_thinking = false
            },new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            }));
            QwenChatCompletion qwenChatCompletion = request.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;
            var content = qwenChatCompletion.Choices.First().Message.Content;
            _logger.LogInformation("语意要素返回：{0}", content);
            var result = JsonSerializer.Deserialize<FuzzyIntentResult>(content,  SqlBotXJsonOptions.Deserialize)!;
            return result;
        }
 

        /// <summary>
        /// 意图结构化
        /// </summary>
        public async Task<QwenIntent> Step3Async(string userInput)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("deepseek-chat")!;
            var chatClient = client.GetService<ChatClient>()!;

            var msgs = new List<ChatMessage> {
                new ChatMessage(ChatRole.System, SQLBotxPrompts.Intent.Replace("{CURRENT_DATE}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))),
                new ChatMessage(ChatRole.User,userInput)
            };

            var requestBody = new
            {
                model = "deepseek-chat",
                messages = msgs.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                // 标准参数
                temperature = 0.1,
                // 强制 JSON 模式
                response_format = new { type = "json_object" },
                enable_thinking = false
            };
            using System.ClientModel.BinaryContent binaryContent = System.ClientModel.BinaryContent.CreateJson(requestBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            });
            var result = await chatClient.CompleteChatAsync(binaryContent);
            QwenChatCompletion qwenChatCompletion = result.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;
            var content = qwenChatCompletion.Choices.First().Message.Content;
            _logger.LogInformation("意图结构化返回：{0}", content);
            var qwenIntent = JsonSerializer.Deserialize<QwenIntent>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            })!;
            return qwenIntent;
        }

        /// <summary>
        /// 歧义消除
        /// </summary>
        public async Task<QueryDefinition> Step4Async(string userInput)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("qwen3-4b")!;
            var chatClient = client.GetService<ChatClient>()!;

            var msg = new List<ChatMessage> {
                new (ChatRole.System, SQLBotxPrompts.Step1),
                new (ChatRole.User,userInput)
            };
            var requestBodyStep1 = new
            {
                model = "qwen3-4b",
                messages = msg.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                response_format = new { type = "json_object" },
                enable_thinking = false
            };
            var resultStep1 = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(requestBodyStep1));
            QwenChatCompletion qwenChatCompletionStep1 = resultStep1.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;
            var content = qwenChatCompletionStep1.Choices.First().Message.Content;
            _logger.LogInformation("语意消歧返回：{0}", content);
 
            var result = JsonSerializer.Deserialize<QueryDefinition>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            })!;
            return result;
        }

        /// <summary>
        /// 歧义消除
        /// </summary>
        public async Task<Step5Result> Step5Async(string userInput)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("qwen3-4b")!;
            var chatClient = client.GetService<ChatClient>()!;
            var msg = new List<ChatMessage> {
                new (ChatRole.System, SQLBotxPrompts.test11),
                new (ChatRole.User,userInput)
            };
            var requestBodyStep1 = new
            {
                model = "qwen3-4b",
                messages = msg.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                response_format = new { type = "json_object" },
                enable_thinking = false
            };
            var resultStep1 = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(requestBodyStep1));
            QwenChatCompletion qwenChatCompletionStep1 = resultStep1.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;
            var content = qwenChatCompletionStep1.Choices.First().Message.Content;
            _logger.LogInformation("语意格式化返回：{0}", content);
            var result = JsonSerializer.Deserialize<Step5Result>(content, SqlBotXJsonOptions.Deserialize)!;
            return result;
        }

        /// <summary>
        /// 指标、维度清洗
        /// </summary>
        public async Task<Step5Result> Step6Async(string userInput,string schema, string relationships)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("qwen3-4b")!;
            var chatClient = client.GetService<ChatClient>()!;
            
            var msg = new List<ChatMessage> {
                new (ChatRole.System, SQLBotxPrompts.test12
                .Replace("$schema",schema)
                .Replace("$relationships",relationships)
                .Replace("$systemTime",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                ),
                new (ChatRole.User,userInput)
            };
            var requestBodyStep1 = new
            {
                model = "qwen3-4b",
                messages = msg.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                response_format = new { type = "json_object" },
                enable_thinking = false
            };
            var resultStep1 = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(requestBodyStep1));
            QwenChatCompletion qwenChatCompletionStep1 = resultStep1.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;
            var content = qwenChatCompletionStep1.Choices.First().Message.Content;
            _logger.LogInformation("清洗类SQL返回：{0}", content);

            //var result = JsonSerializer.Deserialize<Step5Result>(content, new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //    WriteIndented = false,
            //    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            //    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            //})!;
            return  new Step5Result();
        }



        /// <summary>
        /// 解析出指标字段和普通字段
        /// </summary>
        public async Task<Step5Result> StepZhiBiaoAsync(string userInput, string schema, string relationships)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("qwen3-14b")!;
            var chatClient = client.GetService<ChatClient>()!;

            var msg = new List<ChatMessage> {
                new (ChatRole.System,  @"
您是一个在SQLBotX中运行的指标字段提取助手。您的主要目标是尽可能有效的帮助用户完成需求任务，同时严格遵守系统和开发人员的说明。

# Core Role
您是一个专业的中文语义解析助手，理解用户语境同时精通T-SQL。

# Core Goal
根据用户提供的候选字段数组列表和原始的自然语言输入，判断哪些字段是需要进行度量的字段。
- 度量字段（Measure）是指在数据分析中用于量化、计算和聚合的数值型数据字段，通常代表业务过程中的可测量指标或事实（如销售额、利润、用户数、点击量等）。它支持数学运算（求和、平均值、计数、最大值、最小值等），是构建指标、KPI和进行趋势分析的核心元素。与描述性的维度字段（如时间、地区、产品类别）不同，度量字段是被聚合分析的对象，用于回答“多少”“多大”“多快”等问题。

# Response Format (输出格式)
你必须仅输出一个 JSON 对象，不要包含 Markdown 代码块标记（```json），也不要包含其他解释性文字。
格式如下：
{
  ""out_field"": ""[""指标字段1"",""指标字段2"",""..."",]"",
}"),
                new (ChatRole.User,userInput)
            };
            var requestBody = new
            {
                model = "qwen3-14b",
                messages = msg.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                response_format = new { type = "json_object" },
                enable_thinking = false
            };
            var result = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(requestBody));
            QwenChatCompletion qwenChatCompletionStep1 = result.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;
            var content = qwenChatCompletionStep1.Choices.First().Message.Content;
            _logger.LogInformation("指标字段返回：{0}", content);
            return new Step5Result();
        }

        /// <summary>
        /// 语义提取
        /// </summary>
        /// <param name="prompt"></param>
        public async Task<ColumnDomainMappingResult> RecallFieldAsync(string prompt)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("deepseek-chat")!;
            var chatClient = client.GetService<ChatClient>()!;

            List<ChatMessage> msgs = new List<ChatMessage>() {
                new(ChatRole.System,prompt)
            };
            var request = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(new
            {
                model = "deepseek-chat",
                messages = msgs.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                response_format = new { type = "json_object" },
                enable_thinking = false
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            }));
            QwenChatCompletion qwenChatCompletion = request.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;

            var content = qwenChatCompletion.Choices.First().Message.Content;
            _logger.LogInformation("字段召回分类 Send：{prompt}", prompt);
            _logger.LogInformation("字段召回分类 Result：{content}", content);
            var result = JsonSerializer.Deserialize<ColumnDomainMappingResult>(content, SqlBotXJsonOptions.Deserialize)!;
            return result;
        }

        /// <summary>
        /// 语义提取
        /// </summary>
        /// <param name="prompt"></param>
        public async Task<MetricSelectionResult> SelectMetricAsync(string prompt)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("deepseek-chat")!;
            var chatClient = client.GetService<ChatClient>()!;

            List<ChatMessage> msgs = new List<ChatMessage>() {
                new(ChatRole.System,prompt)
            };
            var request = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(new
            {
                model = "deepseek-chat",
                messages = msgs.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                response_format = new { type = "json_object" },
                enable_thinking = false
            },new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            }));
            QwenChatCompletion qwenChatCompletion = request.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;

            var content = qwenChatCompletion.Choices.First().Message.Content;
            _logger.LogInformation("SelectMetric Send：{prompt}", prompt);
            _logger.LogInformation("SelectMetric Result：{content}", content);
            var result = JsonSerializer.Deserialize<MetricSelectionResult>(content, SqlBotXJsonOptions.Deserialize)!;
            return result;
        }

        /// <summary>
        /// 业务域召回
        /// </summary>
        /// <param name="prompt"></param>
        public async Task<DomainSelectionResult> SelectDomainAsync(string prompt)
        {
            IChatClient client = _serviceProvider.GetKeyedService<IChatClient>("deepseek-chat")!;
            var chatClient = client.GetService<ChatClient>()!;

            List<ChatMessage> msgs = new List<ChatMessage>() {
                new(ChatRole.System,prompt)
            };
            var request = await chatClient.CompleteChatAsync(System.ClientModel.BinaryContent.CreateJson(new
            {
                model = "deepseek-chat",
                messages = msgs.Select(x => new
                {
                    role = x.Role,
                    content = x.Text,
                }),
                temperature = 0.1,
                response_format = new { type = "json_object" },
                enable_thinking = false
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            }));
            QwenChatCompletion qwenChatCompletion = request.GetRawResponse().Content.ToObjectFromJson<QwenChatCompletion>()!;

            var content = qwenChatCompletion.Choices.First().Message.Content;
            _logger.LogInformation("SelectDomain Send：{prompt}", prompt);
            _logger.LogInformation("SelectDomain Result：{content}", content);
            var result = JsonSerializer.Deserialize<DomainSelectionResult>(content, SqlBotXJsonOptions.Deserialize)!;
            return result;
        }
    }
}
