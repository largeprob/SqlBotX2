using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums.DomainEntity
{
    public enum ForeignKeyType
    {
        /// <summary>
        /// 普通外键
        /// </summary>
        [Description("普通外键")]
        Normal,      
        /// <summary>
        /// 普通外键
        /// </summary>
        [Description("多态外键")]
        Polymorphic, 
    }
}
