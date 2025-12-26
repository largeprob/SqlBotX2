using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SqlBoTx.Net.ApiService.SqlPlugin
{

    /// <summary>
    /// SQL 插件
    /// </summary>
    public class SqlBotPlugin
    {
        private readonly SqlServerDatabaseService  _databaseService;
        private readonly ILogger<SqlBotPlugin>   _logger;

        public SqlBotPlugin(SqlServerDatabaseService databaseService, ILogger<SqlBotPlugin> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }



        /// <summary>
        /// 意图识别
        /// </summary>
        public SqlStepIntention Intention {  get; private set; }
        public IntentAnalysis IntentAnalysis {  get; private set; }


        public string[] TableIntentions {  get; private set; }


        public SqlStepResult SqlStepResult {  get; private set; }


        /// <summary>
        /// 意图分析结果
        /// </summary>
        /// <param name="thoughtProcess"></param>
        /// <param name="category"></param>
        /// <param name="isDefaultFallback"></param>
        /// <param name="primaryEntity"></param>
        /// <param name="userGuidance"></param>
        /// <returns></returns>
        [KernelFunction("RecordIntentAnalysis")]
        [Description("Write result into")]
        public async Task RecordIntentAnalysis(
            [Description("ThoughtProcess")] string thoughtProcess,
            [Description("Category")] IntentAnalysisCategory category,
            [Description("IsDefaultFallback")] bool isDefaultFallback,
            [Description("PrimaryEntity")] string primaryEntity,
            [Description("UserGuidance")] string userGuidance
            )
        {
            try
            {
                IntentAnalysis = new IntentAnalysis
                {
                    ThoughtProcess = thoughtProcess,
                    Category = category,
                    IsDefaultFallback = isDefaultFallback,
                    PrimaryEntity = primaryEntity,
                    UserGuidance = userGuidance
                };

                var json = JsonSerializer.Serialize(IntentAnalysis, IntentAnalysis.GetType(), AgentJsonOptions.SSEJsonOptions);

                _logger.LogInformation("意图识别 Success：" + json);
            }
            catch (Exception ex)
            {
                _logger.LogError("意图识别 Exception：" + ex.Message);
                throw;
            }
        }
 

        /// <summary>
        /// 表选择
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="reason"></param>
        [KernelFunction("TableIntention")]
        [Description("Write result into")]
        public void TableIntention(
           [Description("List of table names to be used")]
            string[] tables,
           [Description("Reasons for selecting these tables")]
            string reason)
        {
            this.TableIntentions = tables;
            _logger.LogInformation("表选择 Success：" + JsonSerializer.Serialize(tables, AgentJsonOptions.SSEJsonOptions));
            _logger.LogInformation("表选择 Success：" + reason);
        }


        /// <summary>
        /// 将SQL结果写入
        /// </summary>
        /// <param name="resultJson"></param>
        [KernelFunction("SQLResult")]
        [Description("Write result into")]
        public void SQLResult([Description("resultJson")] string resultJson)
        {
            _logger.LogInformation("SQLResult Success：" + resultJson);

            SqlStepResult = JsonSerializer.Deserialize<SqlStepResult>(resultJson);
        }



    }
}
