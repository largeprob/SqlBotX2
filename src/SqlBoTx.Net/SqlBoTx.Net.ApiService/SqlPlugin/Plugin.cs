using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace SqlBoTx.Net.ApiService.SqlPlugin
{

    /// <summary>
    /// SQL 插件
    /// </summary>
    public class Plugin
    {
        private readonly SqlServerDatabaseService  _databaseService;
        public Plugin(SqlServerDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [KernelFunction("YourSelf")]
        [Description("When users ask or inquire about your information, this method is limited to introducing oneself")]
        public async Task<string> YourSelf()
        {
            Console.WriteLine("调用了插件 users");


            return @"我是一个由程先生开发的人工智能助手，可以帮助你快速了解企业数据,目前我可以支持查询：客户、库存、报价、销售、产品等信息。
    只需要对我说“查询客户”或者“查询库存”等等，我就会帮你查询相关信息。
";
        }


        /// <summary>
        /// 根据表名获取该表的结构化 schema（columns, indexes, createSql, sampleRows）
        /// </summary>
        [KernelFunction("GetTableSchema"), Description(
             """
             Retrieve structured schema information for a specific table.

             Parameters:
             - tableName: Exact table name to retrieve schema for.

             Returns a JSON object with columns, indexes, createSql and sampleRows.
             """)]
        public async Task<string> GetTableSchema(
            [Description("Exact table name to get schema for")]
            string[] tableName)
        {
            Console.WriteLine("调用了插件 GetTableSchema");



            if (tableName.Length == 0)
            {
                return JsonSerializer.Serialize(new { error = "Table name is required." });
            }

            try
            {
                var getTableSchema = await _databaseService.GetTableSchema(tableName);
                var _dbSchema = getTableSchema.Item1;
                return JsonSerializer.Serialize(_dbSchema, AgentJsonOptions.Default);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 所有表信息
        /// </summary>
        /// <returns></returns>
   
        [KernelFunction("GetAllTable"), Description("""
            获取数据库中所有的表信息,result json:
            ``[]TableName(string),Description(string)``。
            """)]
        public async Task<string> GetAllTable()
        {
            Console.WriteLine("调用了插件 GetAllTable");
            var tables = new List<object> {
                 new { TableName = "Customers", Description = "客户信息表" },
                 new { TableName = "Users", Description = "用户信息表" },
            };
            return JsonSerializer.Serialize(tables, AgentJsonOptions.Default);
        }
    }
}
