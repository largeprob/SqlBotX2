using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using SqlBoTx.Net.Domain.Share.Enums;
using SqlBoTx.Net.Domain.Share.TableRelationships;
using System.Text;

namespace SqlBoTx.Net.ApiService.SqlBotX
{
    public class TableRelationshipFormatter
    {
        /// <summary>
        /// 将表关系列表转换为模型友好的描述格式
        /// </summary>
        public static string FormatForAI(
            List<ListTableRelationshipDto> relationships,
            List<ListTableStructureDto>? tables = null)
        {
            if (relationships == null || relationships.Count == 0)
                return "# 表关系：暂无表关联信息";

            var sb = new StringBuilder();
            sb.AppendLine("# 表关系结构");
            sb.AppendLine();

            // 构建表名映射，用于显示友好的表名
            var tableMap = BuildTableMap(tables);

            foreach (var rel in relationships)
            {
                if (rel == null) continue;

                AppendRelationship(sb, rel, tableMap);
            }

            // 添加关系图总结
            sb.AppendLine();
            AppendRelationshipSummary(sb, relationships, tableMap);

            return sb.ToString();
        }

        /// <summary>
        /// 构建表名映射
        /// </summary>
        private static Dictionary<string, string> BuildTableMap(List<ListTableStructureDto>? tables)
        {
            var map = new Dictionary<string, string>();

            if (tables != null)
            {
                foreach (var table in tables)
                {
                    if (table.TableName != null && table.DisplayName != null)
                    {
                        map[table.TableName] = table.DisplayName;
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// 格式化单个关系
        /// </summary>
        private static void AppendRelationship(
            StringBuilder sb,
            ListTableRelationshipDto rel,
            Dictionary<string, string> tableMap)
        {
            // 获取表显示名
            var sourceName = GetTableDisplayName(rel.SourceTableName, rel.SourceTableDisplayName, tableMap);
            var targetName = GetTableDisplayName(rel.TargetTableName, rel.TargetTableDisplayName, tableMap);

            // 构建关系描述
            var relationSymbol = GetRelationSymbol(rel.RelationshipType);
            var relationDesc = GetRelationDescription(rel.RelationshipType);

            // 构建条件描述
            var conditionDesc = BuildConditionDescription(rel.JoinConditions);

            sb.AppendLine($"# 关系ID: {rel.Id}");
            sb.Append($"{sourceName} {relationSymbol} {targetName}");

            if (!string.IsNullOrEmpty(conditionDesc))
                sb.Append($"  # {relationDesc}: {conditionDesc}");
            else
                sb.Append($"  # {relationDesc}");

            sb.AppendLine();

            // 如果有详细条件，显示它们
            if (rel.JoinConditions != null && rel.JoinConditions.Count > 0)
            {
                foreach (var condition in rel.JoinConditions)
                {
                    AppendCondition(sb, condition, 2);
                }
            }

            sb.AppendLine(); // 空行分隔
        }

        /// <summary>
        /// 获取表显示名
        /// </summary>
        private static string GetTableDisplayName(
            string? tableName,
            string? displayName,
            Dictionary<string, string> tableMap)
        {
            //
            // 使用显示名，其次从映射中获取，最后用表名
            if (!string.IsNullOrEmpty(displayName))
                return displayName;

            if (!string.IsNullOrEmpty(tableName))
            {
                if (tableMap.TryGetValue(tableName, out var mappedName))
                    return mappedName;

                return tableName;
            }

            return "未知表";
        }

        /// <summary>
        /// 获取关系符号
        /// </summary>
        private static string GetRelationSymbol(TableRelationshipType type)
        {
            return type switch
            {
                TableRelationshipType.OneToMany => "--<",
                TableRelationshipType.ManyToOne => ">--",
                TableRelationshipType.OneToOne => "--",
                TableRelationshipType.ManyToMany => ">--<",
                _ => "??"
            };
        }

        /// <summary>
        /// 获取关系描述
        /// </summary>
        private static string GetRelationDescription(TableRelationshipType type)
        {
            return type switch
            {
                TableRelationshipType.OneToMany => "一对多",
                TableRelationshipType.ManyToOne => "多对一",
                TableRelationshipType.OneToOne => "一对一",
                TableRelationshipType.ManyToMany => "多对多",
                _ => "未知关系"
            };
        }

        /// <summary>
        /// 构建条件描述
        /// </summary>
        private static string BuildConditionDescription(List<TableRelationshipCondition>? conditions)
        {
            if (conditions == null || conditions.Count == 0)
                return "";

            var parts = new List<string>();

            foreach (var cond in conditions)
            {
                var part = FormatCondition(cond);
                if (!string.IsNullOrEmpty(part))
                    parts.Add(part);
            }

            return string.Join(" 且 ", parts);
        }

        /// <summary>
        /// 格式化单个条件
        /// </summary>
        private static string FormatCondition(TableRelationshipCondition condition)
        {
            if (condition == null) return "";

            return condition.Type switch
            {
                ConditionType.Key => $"{condition.SourceColumn} = {condition.TargetColumn}",
                ConditionType.SourceConstant => $"{condition.SourceColumn} = '{condition.ConstantValue}'",
                ConditionType.TargetConstant => $"{condition.TargetColumn} = '{condition.ConstantValue}'",
                _ => "未知条件"
            };
        }

        /// <summary>
        /// 附加条件详细信息
        /// </summary>
        private static void AppendCondition(StringBuilder sb, TableRelationshipCondition condition, int indent)
        {
            var indentStr = new string(' ', indent);

            sb.Append($"{indentStr}- ");

            switch (condition.Type)
            {
                case ConditionType.Key:
                    sb.AppendLine($"外键关联: {condition.SourceColumn} → {condition.TargetColumn}");
                    break;

                case ConditionType.SourceConstant:
                    sb.AppendLine($"常量条件: {condition.SourceColumn} = '{condition.ConstantValue}'");
                    break;

                case ConditionType.TargetConstant:
                    sb.AppendLine($"常量条件: {condition.TargetColumn} = '{condition.ConstantValue}'");
                    break;

                default:
                    sb.AppendLine($"条件类型: {condition.Type}");
                    break;
            }
        }

        /// <summary>
        /// 添加关系图总结
        /// </summary>
        private static void AppendRelationshipSummary(
            StringBuilder sb,
            List<ListTableRelationshipDto> relationships,
            Dictionary<string, string> tableMap)
        {
            sb.AppendLine("# 关系图总结");

            // 收集所有表
            var allTables = new HashSet<string>();
            foreach (var rel in relationships)
            {
                if (rel.SourceTableName != null)
                    allTables.Add(GetTableDisplayName(rel.SourceTableName, rel.SourceTableDisplayName, tableMap));
                if (rel.TargetTableName != null)
                    allTables.Add(GetTableDisplayName(rel.TargetTableName, rel.TargetTableDisplayName, tableMap));
            }

            // 按表分组关系
            var tableRelations = new Dictionary<string, List<string>>();

            foreach (var table in allTables)
            {
                tableRelations[table] = new List<string>();
            }

            // 为每个表构建关系描述
            foreach (var rel in relationships)
            {
                var sourceName = GetTableDisplayName(rel.SourceTableName, rel.SourceTableDisplayName, tableMap);
                var targetName = GetTableDisplayName(rel.TargetTableName, rel.TargetTableDisplayName, tableMap);
                var relationSymbol = GetRelationSymbol(rel.RelationshipType);

                // 源表的关系
                if (tableRelations.ContainsKey(sourceName))
                {
                    tableRelations[sourceName].Add($"{sourceName} {relationSymbol} {targetName}");
                }

                // 目标表的关系
                if (tableRelations.ContainsKey(targetName))
                {
                    // 反向关系
                    var reverseSymbol = GetReverseRelationSymbol(rel.RelationshipType);
                    if (reverseSymbol != null)
                    {
                        tableRelations[targetName].Add($"{targetName} {reverseSymbol} {sourceName}");
                    }
                }
            }

            // 输出总结
            foreach (var kvp in tableRelations)
            {
                if (kvp.Value.Count > 0)
                {
                    sb.AppendLine($"## {kvp.Key}");
                    foreach (var relation in kvp.Value)
                    {
                        sb.AppendLine($"  {relation}");
                    }
                    sb.AppendLine();
                }
            }
        }

        /// <summary>
        /// 获取反向关系符号
        /// </summary>
        private static string? GetReverseRelationSymbol(TableRelationshipType type)
        {
            return type switch
            {
                TableRelationshipType.OneToMany => ">--",  // 一对多的反向是多对一
                TableRelationshipType.ManyToOne => "--<",  // 多对一的反向是一对多
                TableRelationshipType.OneToOne => "--",    // 一对一的反向还是一对一
                TableRelationshipType.ManyToMany => ">--<", // 多对多的反向还是多对多
                _ => null
            };
        }

        /// <summary>
        /// 转换为极简模式
        /// </summary>
        public static string FormatMinimal(List<ListTableRelationshipDto> relationships)
        {
            if (relationships == null || relationships.Count == 0)
                return "无关系";

            var sb = new StringBuilder();

            foreach (var rel in relationships)
            {
                if (rel == null) continue;

                var source = rel.SourceTableName ?? "表A";
                var target = rel.TargetTableName ?? "表B";
                var symbol = GetMinimalRelationSymbol(rel.RelationshipType);

                sb.AppendLine($"{source} {symbol} {target}");
            }

            return sb.ToString();
        }

        private static string GetMinimalRelationSymbol(TableRelationshipType type)
        {
            return type switch
            {
                TableRelationshipType.OneToMany => "->",
                TableRelationshipType.ManyToOne => "<-",
                TableRelationshipType.OneToOne => "<->",
                TableRelationshipType.ManyToMany => "<>",
                _ => "?"
            };
        }

        /// <summary>
        /// 生成Mermaid关系图语法（用于可视化）
        /// </summary>
        public static string FormatMermaidDiagram(List<ListTableRelationshipDto> relationships)
        {
            if (relationships == null || relationships.Count == 0)
                return "";

            var sb = new StringBuilder();
            sb.AppendLine("```mermaid");
            sb.AppendLine("erDiagram");

            foreach (var rel in relationships)
            {
                if (rel == null || rel.SourceTableName == null || rel.TargetTableName == null)
                    continue;

                var source = rel.SourceTableName;
                var target = rel.TargetTableName;
                var cardinality = GetMermaidCardinality(rel.RelationshipType);

                sb.AppendLine($"    {source} {cardinality} {target} : \"关系{rel.Id}\"");
            }

            sb.AppendLine("```");
            return sb.ToString();
        }

        private static string GetMermaidCardinality(TableRelationshipType type)
        {
            return type switch
            {
                TableRelationshipType.OneToMany => "||--o{",
                TableRelationshipType.ManyToOne => "}o--||",
                TableRelationshipType.OneToOne => "||--||",
                TableRelationshipType.ManyToMany => "}o--o{",
                _ => "||--||"
            };
        }
    }
}
