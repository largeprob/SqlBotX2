using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using System.Text;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class TableStructureFormatter
    {
        /// <summary>
        /// 将表结构列表转换为模型友好的描述格式
        /// </summary>
        public static string FormatForAI(List<ListTableStructureDto> tables)
        {
            if (tables == null || tables.Count == 0)
                return "# 数据库：暂无表结构信息";

            var sb = new StringBuilder();

            // 添加数据库标题
            sb.AppendLine("# 数据库表结构");
            sb.AppendLine();

            foreach (var table in tables)
            {
                if (table.TableFields == null || table.TableFields.Count == 0)
                    continue;

                AppendTable(sb, table);
                sb.AppendLine(); // 表之间空一行
            }

            return sb.ToString();
        }

        /// <summary>
        /// 将单个表转换为模型友好格式
        /// </summary>
        private static void AppendTable(StringBuilder sb, ListTableStructureDto table)
        {
            // 表名和描述
            var tableDisplayName = !string.IsNullOrEmpty(table.DisplayName)
                ? table.DisplayName
                : table.TableName ?? "未命名表";

            sb.AppendLine($"# {tableDisplayName}");
            if (!string.IsNullOrEmpty(table.Description))
                sb.AppendLine($"# {table.Description}");

            sb.AppendLine($"{table.TableName ?? "table"} {{");

            // 按字段类型分组显示
            var groupedFields = table.TableFields
                .Where(f => f != null && !string.IsNullOrEmpty(f.FieldName))
                .OrderByDescending(f => f.IsPrimaryKey) // 主键在前
                .ThenBy(f => f.FieldName)
                .ToList();

            if (groupedFields.Count > 0)
            {
                foreach (var field in groupedFields)
                {
                    AppendField(sb, field, 2); // 缩进2空格
                }
            }
            else
            {
                sb.AppendLine("  # 暂无字段信息");
            }

            sb.AppendLine("}");

            // 添加统计信息
            sb.AppendLine($"# 字段数: {table.FieldCount}");
        }

        /// <summary>
        /// 格式化单个字段
        /// </summary>
        private static void AppendField(StringBuilder sb, ListTableFieldDto field, int indent)
        {
            var indentStr = new string(' ', indent);

            // 构建字段前缀（主键标记）
            var prefix = field.IsPrimaryKey ? "*" : " ";

            // 构建字段名
            var fieldName = field.FieldName ?? "field";

            // 构建数据类型（简化处理）
            var dataType = SimplifyDataType(field.DataType);

            // 构建约束列表
            var constraints = BuildConstraints(field);

            // 构建注释
            var comment = BuildComment(field);

            // 构建完整的字段行
            sb.Append($"{indentStr}{prefix}{fieldName}: {dataType}");

            if (!string.IsNullOrEmpty(constraints))
                sb.Append($" [{constraints}]");

            if (!string.IsNullOrEmpty(comment))
                sb.Append($"  # {comment}");

            sb.AppendLine();
        }

        /// <summary>
        /// 简化数据类型表示
        /// </summary>
        private static string SimplifyDataType(string? dataType)
        {
            if (string.IsNullOrEmpty(dataType))
                return "unknown";

            // 移除不必要的信息，保留模型理解的核心
            var type = dataType.ToLower();

            // 常见类型映射
            if (type.Contains("varchar") || type.Contains("nvarchar") || type.Contains("text"))
                return "string";
            if (type.Contains("int") || type.Contains("smallint") || type.Contains("bigint"))
                return "int";
            if (type.Contains("decimal") || type.Contains("numeric") || type.Contains("money"))
                return "decimal";
            if (type.Contains("datetime") || type.Contains("date") || type.Contains("time"))
                return "datetime";
            if (type.Contains("bit") || type.Contains("bool"))
                return "bool";
            if (type.Contains("uniqueidentifier"))
                return "guid";

            // 如果包含长度信息，保留简化的
            if (type.Contains("("))
            {
                var openParen = type.IndexOf('(');
                var baseType = type.Substring(0, openParen);
                var args = type.Substring(openParen);

                // 只保留varchar(n)这种重要信息
                if (baseType == "varchar" || baseType == "nvarchar" || baseType == "decimal")
                    return $"{baseType}{args}";

                return baseType;
            }

            return type;
        }

        /// <summary>
        /// 构建约束字符串
        /// </summary>
        private static string BuildConstraints(ListTableFieldDto field)
        {
            var constraints = new List<string>();

            if (field.IsPrimaryKey)
                constraints.Add("PK");
            if (field.IsIdentity)
                constraints.Add("自增");
            if (!field.IsNullable)
                constraints.Add("非空");
            if (!string.IsNullOrEmpty(field.DefaultValue))
                constraints.Add($"默认:{SimplifyDefaultValue(field.DefaultValue)}");
            if (field.IsAvailable.HasValue && !field.IsAvailable.Value)
                constraints.Add("禁用");

            return string.Join(", ", constraints);
        }

        /// <summary>
        /// 简化默认值表示
        /// </summary>
        private static string SimplifyDefaultValue(string defaultValue)
        {
            if (string.IsNullOrEmpty(defaultValue))
                return "";

            // 移除GETDATE()、NOW()等函数的括号
            if (defaultValue.Equals("GETDATE()", StringComparison.OrdinalIgnoreCase) ||
                defaultValue.Equals("NOW()", StringComparison.OrdinalIgnoreCase))
                return "当前时间";

            // 移除多余的括号和引号
            var result = defaultValue.Trim('\'', '"', '(', ')');

            // 如果是数字或简单值，直接返回
            if (result.Length <= 20) // 避免过长的默认值
                return result;

            return "..." + result.Substring(Math.Max(0, result.Length - 10));
        }

        /// <summary>
        /// 构建注释
        /// </summary>
        private static string BuildComment(ListTableFieldDto field)
        {
            var comments = new List<string>();

            // 字段说明
            if (!string.IsNullOrEmpty(field.FieldDescription))
                comments.Add(field.FieldDescription);

            // 如果没有说明但有字段名，可以添加简单的描述
            if (comments.Count == 0 && !string.IsNullOrEmpty(field.FieldName))
            {
                // 根据字段名推断简单描述
                var inferred = InferDescriptionFromFieldName(field.FieldName);
                if (!string.IsNullOrEmpty(inferred))
                    comments.Add(inferred);
            }

            return string.Join("; ", comments);
        }

        /// <summary>
        /// 从字段名推断描述
        /// </summary>
        private static string InferDescriptionFromFieldName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return "";

            fieldName = fieldName.ToLower();

            // 常见字段名映射
            if (fieldName.EndsWith("id"))
                return "ID标识";
            if (fieldName.EndsWith("name"))
                return "名称";
            if (fieldName.Contains("date") || fieldName.Contains("time"))
                return "时间";
            if (fieldName.Contains("amount") || fieldName.Contains("price") || fieldName.Contains("money"))
                return "金额";
            if (fieldName.Contains("count") || fieldName.Contains("quantity"))
                return "数量";
            if (fieldName.Contains("status") || fieldName.Contains("state"))
                return "状态";
            if (fieldName.Contains("description") || fieldName.Contains("desc"))
                return "描述";
            if (fieldName.StartsWith("is_") || fieldName.StartsWith("has_"))
                return "是否" + fieldName.Substring(3);

            return "";
        }

        /// <summary>
        /// 转换为极简模式（用于上下文已熟悉的情况）
        /// </summary>
        public static string FormatMinimal(List<ListTableStructureDto> tables)
        {
            if (tables == null || tables.Count == 0)
                return "无表";

            var sb = new StringBuilder();

            foreach (var table in tables)
            {
                if (table.TableFields == null || table.TableFields.Count == 0)
                    continue;

                var tableName = table.TableName ?? "table";
                var fields = table.TableFields
                    .Where(f => f != null)
                    .Select(f => FormatFieldMinimal(f))
                    .Where(f => !string.IsNullOrEmpty(f))
                    .ToList();

                if (fields.Count > 0)
                    sb.AppendLine($"{tableName}({string.Join(", ", fields)})");
            }

            return sb.ToString();
        }

        private static string FormatFieldMinimal(ListTableFieldDto field)
        {
            if (string.IsNullOrEmpty(field.FieldName))
                return "";

            var prefix = field.IsPrimaryKey ? "*" : "";
            return $"{prefix}{field.FieldName}";
        }

        /// <summary>
        /// 转换为详细模式（用于复杂系统）
        /// </summary>
        public static string FormatDetailed(List<ListTableStructureDto> tables)
        {
            if (tables == null || tables.Count == 0)
                return "# 数据库：暂无表结构信息";

            var sb = new StringBuilder();

            sb.AppendLine("# 数据库详细结构");
            sb.AppendLine();

            foreach (var table in tables)
            {
                sb.AppendLine($"## {table.DisplayName ?? table.TableName}");
                sb.AppendLine($"- 表名: {table.TableName}");
                if (!string.IsNullOrEmpty(table.Description))
                    sb.AppendLine($"- 描述: {table.Description}");
                sb.AppendLine($"- 字段数: {table.FieldCount}");
                sb.AppendLine();

                sb.AppendLine("### 字段详情");

                if (table.TableFields != null && table.TableFields.Count > 0)
                {
                    // 按重要性排序：主键 -> 非空 -> 其他
                    var sortedFields = table.TableFields
                        .Where(f => f != null)
                        .OrderByDescending(f => f.IsPrimaryKey)
                        .ThenByDescending(f => !f.IsNullable)
                        .ThenBy(f => f.FieldName)
                        .ToList();

                    foreach (var field in sortedFields)
                    {
                        sb.AppendLine(FormatFieldDetailed(field));
                    }
                }
                else
                {
                    sb.AppendLine("暂无字段信息");
                }

                sb.AppendLine(); // 表之间空两行
                sb.AppendLine();
            }

            return sb.ToString();
        }
        private static string FormatFieldDetailed(ListTableFieldDto field)
        {
            var sb = new StringBuilder();

            sb.Append($"- **{field.FieldName}** ({field.DataType})");

            var flags = new List<string>();
            if (field.IsPrimaryKey) flags.Add("主键");
            if (field.IsIdentity) flags.Add("自增");
            if (!field.IsNullable) flags.Add("非空");
            if (!string.IsNullOrEmpty(field.DefaultValue))
                flags.Add($"默认值: {field.DefaultValue}");

            if (flags.Count > 0)
                sb.Append($" [{string.Join(", ", flags)}]");

            if (!string.IsNullOrEmpty(field.FieldDescription))
                sb.Append($" - {field.FieldDescription}");

            return sb.ToString();
        }
    }
}
