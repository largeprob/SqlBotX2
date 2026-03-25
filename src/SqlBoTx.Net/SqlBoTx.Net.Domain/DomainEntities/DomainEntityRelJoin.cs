using SqlBoTx.Net.Domain.Share;
using SqlBoTx.Net.Domain.Share.TableRelationships;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.BusinessEntities
{
    /// <summary>
    /// 表关系条件
    /// </summary>
    [Description("表关系条件")]
    public class DomainEntityRelJoin : ValueObject
    {
        /// <summary>
        /// 关联类型
        /// </summary>
        [Description("关联类型")]
        public ConditionType Type { get; set; }

        /// <summary>
        /// 来源表列
        /// </summary>
        [Description("来源表列")]
        public string? SourceColumn { get; set; }

        /// <summary>
        /// 目标表列
        /// </summary>
        [Description("目标表列")]
        public string? TargetColumn { get; set; }

        /// <summary>
        /// 多态关联值
        /// </summary>
        [Description("多态关联值")]
        public string? ConstantValue { get; set; }

        public DomainEntityRelJoin(ConditionType type, string? sourceColumn, string? targetColumn, string? constantValue)
        {
            Type = type;
            SourceColumn = sourceColumn;
            TargetColumn = targetColumn;
            ConstantValue = constantValue;
        }

        /// <summary>
        /// 外键关联
        /// </summary>
        /// <param name="sourceColumn"></param>
        /// <param name="targetColumn"></param>
        /// <returns></returns>
        public static DomainEntityRelJoin Key(string? sourceColumn, string? targetColumn)
        {
            return new DomainEntityRelJoin(ConditionType.Key, sourceColumn, targetColumn, string.Empty);
        }

        /// <summary>
        /// 多态-来源表固定字段
        /// </summary>
        /// <param name="sourceColumn"></param>
        /// <param name="constantValue"></param>
        /// <returns></returns>
        public static DomainEntityRelJoin SourceConstant(string? sourceColumn, string? constantValue)
        {
            return new DomainEntityRelJoin(ConditionType.SourceConstant, sourceColumn, string.Empty, constantValue);
        }

        /// <summary>
        /// 多态-目标表固定字段
        /// </summary>
        /// <param name="sourceColumn"></param>
        /// <param name="constantValue"></param>
        /// <returns></returns>
        public static DomainEntityRelJoin TargetConstant(string? targetColumn, string? constantValue)
        {
            return new DomainEntityRelJoin(ConditionType.TargetConstant, string.Empty, targetColumn, constantValue);
        }
    }

}
