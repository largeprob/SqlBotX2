namespace SqlBoTx.Net.ApiService.SqlBotX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    namespace SqlBuilder.Models
    {
       
        public class SqlQueryStructure
        {
            #region 属性定义

            /// <summary>
            /// 选择的表列表
            /// </summary>
            [JsonPropertyName("selected_tables")]
            public List<string> SelectedTables { get; set; } = new List<string>();

            /// <summary>
            /// 选择的字段列表
            /// </summary>
            [JsonPropertyName("selected_table_fields")]
            public List<SelectedField> SelectedFields { get; set; } = new List<SelectedField>();

            /// <summary>
            /// 分组字段列表
            /// </summary>
            [JsonPropertyName("group_by")]
            public List<GroupByField> GroupBy { get; set; } = new List<GroupByField>();

            /// <summary>
            /// WHERE条件组
            /// </summary>
            [JsonPropertyName("where")]
            public ConditionGroup Where { get; set; }

            /// <summary>
            /// 排序字段列表
            /// </summary>
            [JsonPropertyName("order")]
            public List<OrderField> Order { get; set; }

            /// <summary>
            /// HAVING条件组
            /// </summary>
            [JsonPropertyName("having")]
            public ConditionGroup Having { get; set; }

            #endregion


            #region SQL生成方法

            /// <summary>
            /// 将查询结构转换为SQL语句
            /// </summary>
            public string ToSql()
            {
                var sql = new StringBuilder();

                // 1. SELECT 子句
                sql.Append("SELECT ");
                if (SelectedFields.Count == 0)
                {
                    sql.Append("*");
                }
                else
                {
                    var fieldParts = new List<string>();
                    foreach (var field in SelectedFields)
                    {
                        var fieldSql = field.ToSql();
                        if (!string.IsNullOrEmpty(fieldSql))
                        {
                            fieldParts.Add(fieldSql);
                        }
                    }
                    sql.Append(string.Join(", ", fieldParts));
                }
                sql.AppendLine();

                // 2. FROM 子句
                if (SelectedTables.Count > 0)
                {
                    sql.Append("FROM ");
                    sql.AppendLine(string.Join(", ", SelectedTables.Select(t => $"[{t}]")));
                }
                else
                {
                    throw new InvalidOperationException("至少需要选择一个表");
                }

                // 3. WHERE 子句
                if (Where != null)
                {
                    var whereSql = Where.ToSql();
                    if (!string.IsNullOrWhiteSpace(whereSql))
                    {
                        sql.Append("WHERE ");
                        sql.AppendLine(whereSql);
                    }
                }

                // 4. GROUP BY 子句
                if (GroupBy.Count > 0)
                {
                    sql.Append("GROUP BY ");
                    var groupByParts = new List<string>();
                    foreach (var groupField in GroupBy)
                    {
                        var groupSql = groupField.ToSql();
                        if (!string.IsNullOrEmpty(groupSql))
                        {
                            groupByParts.Add(groupSql);
                        }
                    }
                    sql.AppendLine(string.Join(", ", groupByParts));
                }

                // 5. HAVING 子句
                if (Having != null)
                {
                    var havingSql = Having.ToSql();
                    if (!string.IsNullOrWhiteSpace(havingSql))
                    {
                        sql.Append("HAVING ");
                        sql.AppendLine(havingSql);
                    }
                }

                // 6. ORDER BY 子句
                if (Order != null && Order.Count > 0)
                {
                    sql.Append("ORDER BY ");
                    var orderParts = new List<string>();
                    foreach (var orderField in Order)
                    {
                        var orderSql = orderField.ToSql();
                        if (!string.IsNullOrEmpty(orderSql))
                        {
                            orderParts.Add(orderSql);
                        }
                    }
                    sql.AppendLine(string.Join(", ", orderParts));
                }

                return sql.ToString();
            }

            /// <summary>
            /// 获取带注释的SQL语句
            /// </summary>
            public string ToAnnotatedSql()
            {
                var sql = new StringBuilder();

                sql.AppendLine("-- ==================================================");
                sql.AppendLine("-- 生成的SQL查询语句");
                sql.AppendLine("-- ==================================================");
                sql.AppendLine();

                // 表注释
                if (SelectedTables.Count > 0)
                {
                    sql.AppendLine($"-- 查询表: {string.Join(", ", SelectedTables)}");
                }

                // 字段注释
                if (SelectedFields.Count > 0)
                {
                    sql.AppendLine($"-- 查询字段数: {SelectedFields.Count}");
                    foreach (var field in SelectedFields)
                    {
                        sql.AppendLine($"--   - {field.FieldName} (表: {field.TableName})");
                    }
                }

                sql.AppendLine();
                sql.AppendLine(ToSql());

                return sql.ToString();
            }

            #endregion

            #region 公共方法

            /// <summary>
            /// 验证查询结构的完整性
            /// </summary>
            public (bool IsValid, string ErrorMessage) Validate()
            {
                if (SelectedTables.Count == 0)
                {
                    return (false, "至少需要选择一个表");
                }

                // 验证字段的表名是否在选择表中
                foreach (var field in SelectedFields)
                {
                    if (!string.IsNullOrEmpty(field.TableName) && !SelectedTables.Contains(field.TableName))
                    {
                        return (false, $"字段'{field.FieldName}'引用了未选择的表'{field.TableName}'");
                    }
                }

                // 验证分组字段
                foreach (var groupField in GroupBy)
                {
                    if (!string.IsNullOrEmpty(groupField.TableName) && !SelectedTables.Contains(groupField.TableName))
                    {
                        return (false, $"分组字段'{groupField.FieldName}'引用了未选择的表'{groupField.TableName}'");
                    }
                }

                // 验证排序字段
                if (Order != null)
                {
                    foreach (var orderField in Order)
                    {
                        if (!string.IsNullOrEmpty(orderField.TableName) && !SelectedTables.Contains(orderField.TableName))
                        {
                            return (false, $"排序字段'{orderField.FieldName}'引用了未选择的表'{orderField.TableName}'");
                        }
                    }
                }

                // 验证条件
                if (Where != null)
                {
                    var whereValidation = Where.Validate(SelectedTables);
                    if (!whereValidation.IsValid)
                    {
                        return (false, $"WHERE条件错误: {whereValidation.ErrorMessage}");
                    }
                }

                if (Having != null)
                {
                    var havingValidation = Having.Validate(SelectedTables);
                    if (!havingValidation.IsValid)
                    {
                        return (false, $"HAVING条件错误: {havingValidation.ErrorMessage}");
                    }
                }

                return (true, string.Empty);
            }
            #endregion
        }

        /// <summary>
        /// 选择的字段
        /// </summary>
        public class SelectedField
        {
            [JsonPropertyName("fieldName")]
            public string FieldName { get; set; }

            [JsonPropertyName("tableName")]
            public string TableName { get; set; }

            [JsonPropertyName("is_calculation_column")]
            public bool IsCalculationColumn { get; set; }

            [JsonPropertyName("calculate_expression")]
            public string CalculateExpression { get; set; }

            /// <summary>
            /// 转换为SQL字段表达式
            /// </summary>
            public string ToSql()
            {
                if (IsCalculationColumn && !string.IsNullOrEmpty(CalculateExpression))
                {
                    // 计算字段
                    return $"{CalculateExpression} AS [{FieldName}]";
                }
                else
                {
                    // 普通字段
                    if (!string.IsNullOrEmpty(TableName))
                    {
                        return $"[{TableName}].[{FieldName}]";
                    }
                    return $"[{FieldName}]";
                }
            }

            public SelectedField Clone()
            {
                return new SelectedField
                {
                    FieldName = FieldName,
                    TableName = TableName,
                    IsCalculationColumn = IsCalculationColumn,
                    CalculateExpression = CalculateExpression
                };
            }
        }

        /// <summary>
        /// 分组字段（结构与SelectedField相同）
        /// </summary>
        public class GroupByField : SelectedField
        {
            // 继承所有属性，可以添加分组特定的方法
            public new GroupByField Clone()
            {
                return new GroupByField
                {
                    FieldName = FieldName,
                    TableName = TableName,
                    IsCalculationColumn = IsCalculationColumn,
                    CalculateExpression = CalculateExpression
                };
            }
        }

        /// <summary>
        /// 排序字段
        /// </summary>
        public class OrderField : SelectedField
        {
            [JsonPropertyName("order")]
            public string Order { get; set; } = "ASC"; // 默认升序

            /// <summary>
            /// 转换为SQL排序表达式
            /// </summary>
            public new string ToSql()
            {
                string fieldExpression;

                if (IsCalculationColumn && !string.IsNullOrEmpty(CalculateExpression))
                {
                    // 计算字段
                    fieldExpression = CalculateExpression;
                }
                else
                {
                    // 普通字段
                    if (!string.IsNullOrEmpty(TableName))
                    {
                        fieldExpression = $"[{TableName}].[{FieldName}]";
                    }
                    else
                    {
                        fieldExpression = $"[{FieldName}]";
                    }
                }

                // 添加排序方向
                var direction = Order?.ToUpper() == "DESC" ? "DESC" : "ASC";
                return $"{fieldExpression} {direction}";
            }

            public new OrderField Clone()
            {
                return new OrderField
                {
                    FieldName = FieldName,
                    TableName = TableName,
                    IsCalculationColumn = IsCalculationColumn,
                    CalculateExpression = CalculateExpression,
                    Order = Order
                };
            }
        }

        #region 条件相关类（嵌套结构）

        /// <summary>
        /// 条件基类（抽象）
        /// </summary>
        [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
        [JsonDerivedType(typeof(Condition), typeDiscriminator: "condition")]
        [JsonDerivedType(typeof(ConditionGroup), typeDiscriminator: "group")]
        public abstract class ConditionBase
        {

            /// <summary>
            /// 转换为SQL条件表达式
            /// </summary>
            public abstract string ToSql();

            /// <summary>
            /// 验证条件
            /// </summary>
            public abstract (bool IsValid, string ErrorMessage) Validate(List<string> availableTables);

           
        }

        /// <summary>
        /// 条件组（可以包含多个子条件）
        /// </summary>
        public class ConditionGroup : ConditionBase
        {
        
            [JsonPropertyName("logic")]
            public string Logic { get; set; } = "AND"; // 默认AND

            [JsonPropertyName("children")]
            public List<ConditionBase> Children { get; set; } = new List<ConditionBase>();

            /// <summary>
            /// 转换为SQL条件表达式
            /// </summary>
            public override string ToSql()
            {
                if (Children == null || Children.Count == 0)
                {
                    return string.Empty;
                }

                if (Children.Count == 1)
                {
                    return Children[0].ToSql();
                }

                var childSqls = new List<string>();
                foreach (var child in Children)
                {
                    var childSql = child.ToSql();
                    if (!string.IsNullOrWhiteSpace(childSql))
                    {
                        // 如果子条件也是组，可能需要加括号
                        if (child is ConditionGroup)
                        {
                            childSqls.Add($"({childSql})");
                        }
                        else
                        {
                            childSqls.Add(childSql);
                        }
                    }
                }

                if (childSqls.Count == 0)
                {
                    return string.Empty;
                }

                if (childSqls.Count == 1)
                {
                    return childSqls[0];
                }

                var logic = Logic?.ToUpper() == "OR" ? " OR " : " AND ";
                return string.Join(logic, childSqls);
            }

            /// <summary>
            /// 验证条件组
            /// </summary>
            public override (bool IsValid, string ErrorMessage) Validate(List<string> availableTables)
            {
                if (Children == null || Children.Count == 0)
                {
                    return (true, string.Empty);
                }

                foreach (var child in Children)
                {
                    var validation = child.Validate(availableTables);
                    if (!validation.IsValid)
                    {
                        return validation;
                    }
                }

                return (true, string.Empty);
            }

           

            /// <summary>
            /// 添加条件
            /// </summary>
            public ConditionGroup AddCondition(ConditionBase condition)
            {
                Children.Add(condition);
                return this;
            }

            /// <summary>
            /// 添加简单条件
            /// </summary>
            public ConditionGroup AddSimpleCondition(string tableName, string field, string @operator, params string[] values)
            {
                Children.Add(new Condition
                {
                    TableName = tableName,
                    Field = field,
                    Operator = @operator,
                    Value = values.ToList()
                });
                return this;
            }
        }

        /// <summary>
        /// 单个条件
        /// </summary>
        public class Condition : ConditionBase
        {
         

            [JsonPropertyName("tableName")]
            public string TableName { get; set; }

            [JsonPropertyName("field")]
            public string Field { get; set; }

            [JsonPropertyName("operator")]
            public string Operator { get; set; }

            [JsonPropertyName("value")]
            public List<string> Value { get; set; } = new List<string>();

            /// <summary>
            /// 转换为SQL条件表达式
            /// </summary>
            public override string ToSql()
            {
                if (string.IsNullOrWhiteSpace(Field) || string.IsNullOrWhiteSpace(Operator))
                {
                    return string.Empty;
                }

                // 构建字段表达式
                string fieldExpression;
                if (!string.IsNullOrEmpty(TableName))
                {
                    fieldExpression = $"[{TableName}].[{Field}]";
                }
                else
                {
                    fieldExpression = $"[{Field}]";
                }

                // 处理不同操作符
                return FormatCondition(fieldExpression, Operator, Value);
            }

            /// <summary>
            /// 格式化条件表达式
            /// </summary>
            private string FormatCondition(string fieldExpression, string @operator, List<string> values)
            {
                switch (@operator.ToUpper())
                {
                    case "=":
                    case "!=":
                    case "<>":
                    case ">":
                    case "<":
                    case ">=":
                    case "<=":
                        if (values.Count == 1)
                        {
                            return $"{fieldExpression} {@operator} {FormatValue(values[0])}";
                        }
                        break;

                    case "IN":
                        if (values.Count > 0)
                        {
                            var formattedValues = values.Select(FormatValue);
                            return $"{fieldExpression} IN ({string.Join(", ", formattedValues)})";
                        }
                        break;

                    case "NOT IN":
                        if (values.Count > 0)
                        {
                            var formattedValues = values.Select(FormatValue);
                            return $"{fieldExpression} NOT IN ({string.Join(", ", formattedValues)})";
                        }
                        break;

                    case "BETWEEN":
                        if (values.Count == 2)
                        {
                            return $"{fieldExpression} BETWEEN {FormatValue(values[0])} AND {FormatValue(values[1])}";
                        }
                        break;

                    case "LIKE":
                        if (values.Count == 1)
                        {
                            return $"{fieldExpression} LIKE {FormatValue(values[0])}";
                        }
                        break;

                    case "NOT LIKE":
                        if (values.Count == 1)
                        {
                            return $"{fieldExpression} NOT LIKE {FormatValue(values[0])}";
                        }
                        break;

                    case "IS NULL":
                        return $"{fieldExpression} IS NULL";

                    case "IS NOT NULL":
                        return $"{fieldExpression} IS NOT NULL";
                }

                throw new ArgumentException($"不支持的操作符: {@operator} 或值数量不正确");
            }

            /// <summary>
            /// 格式化值（字符串加引号，数字保持原样）
            /// </summary>
            private string FormatValue(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return "NULL";
                }

                // 尝试解析为数字
                if (int.TryParse(value, out _) || decimal.TryParse(value, out _))
                {
                    return value;
                }

                // 处理布尔值
                if (bool.TryParse(value, out var boolValue))
                {
                    return boolValue ? "1" : "0";
                }

                // 字符串值加单引号，并转义单引号
                return $"'{value.Replace("'", "''")}'";
            }

            /// <summary>
            /// 验证条件
            /// </summary>
            public override (bool IsValid, string ErrorMessage) Validate(List<string> availableTables)
            {
                if (string.IsNullOrWhiteSpace(Field))
                {
                    return (false, "条件字段不能为空");
                }

                if (string.IsNullOrWhiteSpace(Operator))
                {
                    return (false, "条件操作符不能为空");
                }

                if (!string.IsNullOrEmpty(TableName) && !availableTables.Contains(TableName))
                {
                    return (false, $"条件引用了未选择的表'{TableName}'");
                }

                // 验证操作符和值
                switch (Operator.ToUpper())
                {
                    case "BETWEEN":
                        if (Value.Count != 2)
                        {
                            return (false, "BETWEEN操作符需要2个值");
                        }
                        break;

                    case "IN":
                    case "NOT IN":
                        if (Value.Count == 0)
                        {
                            return (false, $"{Operator}操作符至少需要1个值");
                        }
                        break;

                    case "IS NULL":
                    case "IS NOT NULL":
                        if (Value.Count > 0)
                        {
                            return (false, $"{Operator}操作符不需要值");
                        }
                        break;

                    default:
                        if (Value.Count != 1)
                        {
                            return (false, $"{Operator}操作符需要1个值");
                        }
                        break;
                }

                return (true, string.Empty);
            }

        
        }

        #endregion

        #region 扩展方法

        /// <summary>
        /// SqlQueryStructure扩展方法
        /// </summary>
        public static class SqlQueryStructureExtensions
        {
            /// <summary>
            /// 添加表
            /// </summary>
            public static SqlQueryStructure AddTable(this SqlQueryStructure query, string tableName)
            {
                if (!query.SelectedTables.Contains(tableName))
                {
                    query.SelectedTables.Add(tableName);
                }
                return query;
            }

            /// <summary>
            /// 添加普通字段
            /// </summary>
            public static SqlQueryStructure AddField(this SqlQueryStructure query, string fieldName, string tableName = null)
            {
                query.SelectedFields.Add(new SelectedField
                {
                    FieldName = fieldName,
                    TableName = tableName,
                    IsCalculationColumn = false
                });
                return query;
            }

            /// <summary>
            /// 添加计算字段
            /// </summary>
            public static SqlQueryStructure AddCalculationField(this SqlQueryStructure query, string fieldName, string calculateExpression, string tableName = null)
            {
                query.SelectedFields.Add(new SelectedField
                {
                    FieldName = fieldName,
                    TableName = tableName,
                    IsCalculationColumn = true,
                    CalculateExpression = calculateExpression
                });
                return query;
            }

            /// <summary>
            /// 添加分组字段
            /// </summary>
            public static SqlQueryStructure AddGroupBy(this SqlQueryStructure query, string fieldName, string tableName = null)
            {
                query.GroupBy.Add(new GroupByField
                {
                    FieldName = fieldName,
                    TableName = tableName,
                    IsCalculationColumn = false
                });
                return query;
            }

            /// <summary>
            /// 添加排序字段
            /// </summary>
            public static SqlQueryStructure AddOrderBy(this SqlQueryStructure query, string fieldName, bool ascending = true, string tableName = null)
            {
                if (query.Order == null)
                {
                    query.Order = new List<OrderField>();
                }

                query.Order.Add(new OrderField
                {
                    FieldName = fieldName,
                    TableName = tableName,
                    IsCalculationColumn = false,
                    Order = ascending ? "ASC" : "DESC"
                });
                return query;
            }

            /// <summary>
            /// 设置WHERE条件
            /// </summary>
            public static SqlQueryStructure SetWhereCondition(this SqlQueryStructure query, ConditionGroup whereCondition)
            {
                query.Where = whereCondition;
                return query;
            }

            /// <summary>
            /// 设置HAVING条件
            /// </summary>
            public static SqlQueryStructure SetHavingCondition(this SqlQueryStructure query, ConditionGroup havingCondition)
            {
                query.Having = havingCondition;
                return query;
            }

            /// <summary>
            /// 获取查询摘要
            /// </summary>
            public static string GetSummary(this SqlQueryStructure query)
            {
                var summary = new StringBuilder();
                summary.AppendLine($"SQL查询结构:");
                summary.AppendLine($"- 表: {string.Join(", ", query.SelectedTables)}");
                summary.AppendLine($"- 字段: {query.SelectedFields.Count}个");
                summary.AppendLine($"- 分组: {query.GroupBy.Count}个字段");
                summary.AppendLine($"- 排序: {(query.Order?.Count ?? 0)}个字段");
                summary.AppendLine($"- WHERE条件: {(query.Where != null ? "有" : "无")}");
                summary.AppendLine($"- HAVING条件: {(query.Having != null ? "有" : "无")}");
                return summary.ToString();
            }
        }

        #endregion
    }
}
