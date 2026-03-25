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
        /// 一
        /// </summary>
        [Description("一")]
        One = 1,

        /// <summary>
        /// 多
        /// </summary>
        [Description("多")]
        Many = 2,
    }
}
