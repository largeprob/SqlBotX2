using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.Share.Enums
{
    public enum ConnectionType
    {
        /// <summary>
        /// SQLServer数据库
        /// </summary>
        [Description("SQLServer数据库")]
        SQLServer = 1,
        /// <summary>
        /// MySQL
        /// </summary>
        [Description("MySQL")]
        MySQL = 2,
        /// <summary>
        /// Oracle
        /// </summary>
        [Description("Oracle")]
        Oracle = 3,
        /// <summary>
        /// PostgreSQL数据库
        /// </summary>
        [Description("PostgreSQL数据库")]
        PostgreSQL = 4,
    }
}
