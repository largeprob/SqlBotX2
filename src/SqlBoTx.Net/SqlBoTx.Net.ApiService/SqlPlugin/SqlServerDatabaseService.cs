using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SqlBoTx.Net.ApiService.Dto;
using System.Data;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace SqlBoTx.Net.ApiService.SqlPlugin
{
    /// <summary>
    /// Provides methods for connecting to a SQL Server database and retrieving schema information for specified tables.
    /// </summary>
    public class SqlServerDatabaseService
    {
        private const string _connectionString = "Server=.;Database=sqlbotx_1;User Id=sa;Password=123456;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;";

        /// <summary>
        /// 获取链接
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private string BuildMasterConnectionString(string baseConnectionString)
        {
            var builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = "master", 
                ConnectTimeout = 30,       
                ApplicationName = "DatabaseCreator"
            };

            return builder.ConnectionString;
        }

        /// <summary>
        /// 获取表结构信息
        /// </summary>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public async Task<(string,List<TableSchemaInfo>)> GetTableSchema(string[] tableNames)
        {
            using var connection = GetConnection();
            var stringBuilder = new StringBuilder();

            var result = new  List<TableSchemaInfo>();

            foreach (var table in tableNames)
            {
                var schema = "dbo";
                var name = table;
                if (table.Contains('.'))
                {
                    var parts = table.Split('.', 2);
                    schema = parts[0].Trim('[', ']', '"');
                    name = parts[1].Trim('[', ']', '"');
                }
                else
                {
                    name = table.Trim('[', ']', '"');
                }

                var obj = await connection.QueryFirstOrDefaultAsync(@"
                SELECT o.object_id,
                       s.name AS [schema],
                       o.name AS [name],
                       CAST(ep.value AS NVARCHAR(MAX)) AS description
                FROM sys.objects o
                JOIN sys.schemas s ON s.schema_id = o.schema_id
                LEFT JOIN sys.extended_properties ep
                  ON ep.class = 1 AND ep.major_id = o.object_id AND ep.minor_id = 0 AND ep.name = 'MS_Description'
                WHERE s.name = @schema
                  AND o.name = @table
                  AND o.type IN ('U','V');", new { schema, table = name });

                int objectId = 0;
                string tableDescription = string.Empty;

                if (obj == null)
                {
                    stringBuilder.AppendLine("table:" + $"{schema}.{name}");
                    stringBuilder.AppendLine("tableDescription:table not found");
                    stringBuilder.AppendLine("columns:" + JsonSerializer.Serialize(Array.Empty<object>(), AgentJsonOptions.Default));
                    stringBuilder.AppendLine();
                    continue;
                }
                else
                {
                    try
                    {
                        if (obj is IDictionary<string, object> d)
                        {
                            if (d.TryGetValue("object_id", out var oid) && oid != null) objectId = Convert.ToInt32(oid);
                            if (d.TryGetValue("description", out var desc)) tableDescription = desc?.ToString() ?? string.Empty;
                        }
                        else
                        {
                            dynamic ti = obj;
                            objectId = (int)ti.object_id;
                            tableDescription = (string)(ti.description ?? string.Empty);
                        }
                    }
                    catch
                    {
                    }
                }


                var colRows = await connection.QueryAsync(@"
                SELECT
                    c.column_id AS ord,
                    c.name AS name,
                    CASE
                        WHEN t.name IN ('varchar','nvarchar','char','nchar','varbinary','binary')
                            THEN t.name + '(' + CASE WHEN c.max_length = -1 THEN 'max' ELSE
                                CAST(CASE WHEN t.name IN ('nchar','nvarchar') THEN c.max_length/2 ELSE c.max_length END AS VARCHAR(10)) END + ')'
                        WHEN t.name IN ('decimal','numeric')
                            THEN t.name + '(' + CAST(c.precision AS VARCHAR(10)) + ',' + CAST(c.scale AS VARCHAR(10)) + ')'
                        ELSE t.name
                    END AS [type],
                    CASE WHEN c.is_nullable = 0 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS notnull,
                    CASE
                        WHEN EXISTS (
                            SELECT 1
                            FROM sys.indexes i
                            JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                            WHERE i.object_id = c.object_id
                              AND i.is_primary_key = 1
                              AND ic.column_id = c.column_id
                        ) THEN CAST(1 AS bit) ELSE CAST(0 AS bit)
                    END AS pk,
                    OBJECT_DEFINITION(c.default_object_id) AS defaultValue,
                    CAST(epc.value AS NVARCHAR(MAX)) AS description
                FROM sys.columns c
                JOIN sys.types t ON t.user_type_id = c.user_type_id
                LEFT JOIN sys.extended_properties epc
                    ON epc.class = 1 AND epc.major_id = c.object_id AND epc.minor_id = c.column_id AND epc.name = 'MS_Description'
                WHERE c.object_id = @objectId
                ORDER BY c.column_id;", new { objectId });

                var columns = new List<TableSchemaColumnInfo>();
                foreach (var c in colRows)
                {
                    if (c is IDictionary<string, object> colDict)
                    {
                        colDict.TryGetValue("name", out var colName);
                        colDict.TryGetValue("type", out var colType);
                        colDict.TryGetValue("notnull", out var colNotNull);
                        colDict.TryGetValue("pk", out var colPk);
                        colDict.TryGetValue("defaultValue", out var colDefault);
                        colDict.TryGetValue("description", out var colDesc);

                        columns.Add(new TableSchemaColumnInfo
                        {
                            Name = string.Format("{0}", colName),
                            Type = string.Format("{0}", colType),
                            IsNullable = string.Format("{0}", colNotNull) == "1" ? true : false,
                            IsPk = string.Format("{0}", colPk) == "1" ? true : false,
                            DefaultValue = string.Format("{0}", colDefault),
                            Description = string.Format("{0}", colDesc)
                        });
                    }
                    else
                    {
                        columns.Add(new TableSchemaColumnInfo
                        {
                            Name = (c as dynamic)?.name,
                            Type = (c as dynamic)?.type,
                            IsNullable = (c as dynamic)?.notnull,
                            IsPk = (c as dynamic)?.pk,
                            DefaultValue = (c as dynamic)?.defaultValue,
                            Description = (c as dynamic)?.description
                        });
                    }
                }
                stringBuilder.AppendLine("table:" + $"{schema}.{name}");
                stringBuilder.AppendLine("tableDescription:" + tableDescription);
                stringBuilder.AppendLine("columns:" + JsonSerializer.Serialize(columns, AgentJsonOptions.Default));
                stringBuilder.AppendLine();

                result.Add(new TableSchemaInfo
                {
                    TableName = $"{schema}.{name}",
                    Description = tableDescription,
                    Columns = new List<TableSchemaColumnInfo>()
                });
            }

            return
                ($"""
             <system-remind>
             Note: The following is the structure information of the table:
             {stringBuilder}
             </system-remind>
             """,
             result);
        }

        /// <summary>
        /// 获取表结构信息
        /// </summary>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public async Task<string[]> GetAllTableNames()
        {
            const string sql = @"SELECT 
    t.name AS [tableName],
    ISNULL(ep.value, '无描述') AS [description],
    t.create_date AS [createDate],
    t.modify_date AS [modifyDate]
FROM sys.tables t
LEFT JOIN sys.extended_properties ep 
    ON t.object_id = ep.major_id 
    AND ep.minor_id = 0 
    AND ep.name = 'MS_Description'
WHERE t.type = 'U'
ORDER BY t.name;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var results = await connection.QueryAsync<DatabaseTableInfo>(sql);
                return results.Select(table => table.TableName).ToArray()!;
            }
        }

        public async Task<IEnumerable<dynamic>> ExecuteQueryAsync(string sql) {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync(sql);
            }
        }
    }
}
