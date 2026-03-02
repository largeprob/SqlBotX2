using System.ComponentModel;

namespace SqlBoTx.Net.Domain.Share.Enums
{
    /// <summary>
    /// 表关系类型枚举
    /// </summary>
    [Description("表关系类型")]
    public enum TableRelationshipType
    {
        /// <summary>
        /// 一对多
        /// </summary>
        [Description("一对多")]
        OneToMany = 1,

        /// <summary>
        /// 多对一
        /// </summary>
        [Description("多对一")]
        ManyToOne = 2,

        /// <summary>
        /// 一对一
        /// </summary>
        [Description("一对一")]
        OneToOne = 3,

        /// <summary>
        /// 多对多
        /// </summary>
        [Description("多对多")]
        ManyToMany = 4
    }
}
