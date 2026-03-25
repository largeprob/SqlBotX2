using SqlBoTx.Net.Domain.Share;
using SqlBoTx.Net.Domain.Share.Enums.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.DomainEntities
{
    public class ForeignKeyMetaData : ValueObject
    {
        /// <summary>
        /// 外键类型
        /// </summary>
        [Description("外键类型")]
        public ForeignKeyType Type { get; set; }

        /// <summary>
        /// 普通外键实体ID
        /// </summary>
        [Description("普通外键实体ID")]
        public string? TargetEntityId { get; set; }

        /// <summary>
        /// 多态外键
        /// </summary>
        [Description("多态外键")]
        public PolymorphicForeignKey? Polymorphic { get; set; }
    }

    public class PolymorphicForeignKey
    {
        /// <summary>
        /// 对应实体
        /// </summary>
        [Description("对应实体")]
        public List<PolymorphicMapping> Mappings { get; set; } = new();
    }

    public class PolymorphicMapping
    {
        /// <summary>
        /// 区分字段
        /// </summary>
        [Description("区分字段")]
        public string DiscriminatorColumn { get; set; } = null!;

        /// <summary>
        /// 区分字段的值
        /// </summary>
        [Description("区分字段的值")]
        public string DiscriminatorValue { get; set; } = null!;  

        /// <summary>
        /// 外键实体ID
        /// </summary>
        [Description("外键实体ID")]
        public string TargetEntityId { get; set; } = null!;
    }
}
