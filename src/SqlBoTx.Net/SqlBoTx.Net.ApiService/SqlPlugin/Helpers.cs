using System.Text.Json;

namespace SqlBoTx.Net.ApiService.SqlPlugin
{
    public class Helpers
    {
        /// <summary>
        /// 将查询结果数据注入到 ECharts option 字符串中,替换占位符
        /// </summary>
        public static string InjectDataIntoEchartsOption(string optionTemplate, dynamic[] queryResults)
        {
            Console.WriteLine("Injecting data into ECharts option template");

            if (string.IsNullOrWhiteSpace(optionTemplate) || queryResults == null || queryResults.Length == 0)
            {
                Console.WriteLine(
                    "Invalid input for data injection: optionTemplate is empty or queryResults is null/empty");
                return optionTemplate;
            }

            try
            {
                // 将动态结果转换为可序列化的格式
                var dataJson = JsonSerializer.Serialize(queryResults, AgentJsonOptions.Default);

                // 替换各种可能的占位符
                var result = optionTemplate;

                // 替换 {{DATA_PLACEHOLDER}}
                result = result.Replace("{{DATA_PLACEHOLDER}}", dataJson);
                result = result.Replace("{DATA_PLACEHOLDER}", dataJson);

                // 如果需要分别处理 X 轴和 Y 轴数据
                if (queryResults.Length > 0)
                {
                    if (queryResults[0] is IDictionary<string, object> { Count: >= 2 } firstItem)
                    {
                        var keys = firstItem.Keys.ToArray();

                        // 提取 X 轴数据 (通常是第一列)
                        var xAxisData = queryResults.Select(row =>
                        {
                            var dict = row as IDictionary<string, object>;
                            return dict?[keys[0]];
                        }).ToArray();

                        var xAxisJson = JsonSerializer.Serialize(xAxisData, new JsonSerializerOptions
                        {
                            WriteIndented = false
                        });

                        // 提取 Y 轴数据 (通常是第二列或后续列)
                        var yAxisData = queryResults.Select(row =>
                        {
                            var dict = row as IDictionary<string, object>;
                            return dict?[keys[1]];
                        }).ToArray();

                        var yAxisJson = JsonSerializer.Serialize(yAxisData, new JsonSerializerOptions
                        {
                            WriteIndented = false
                        });

                        result = result.Replace("\"{{DATA_PLACEHOLDER_X}}\"", xAxisJson);
                        result = result.Replace("\"{DATA_PLACEHOLDER_X}\"", xAxisJson);
                        result = result.Replace("\"{{DATA_PLACEHOLDER_Y}}\"", yAxisJson);
                        result = result.Replace("\"{DATA_PLACEHOLDER_Y}\"", yAxisJson);

                        Console.WriteLine("Data injection completed for X and Y axes");
                    }
                    else
                    {
                        Console.WriteLine("Data injection completed for single data placeholder");
                    }
                }

                Console.WriteLine("Data injection into ECharts option completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return optionTemplate;
            }
        }
    }
}
