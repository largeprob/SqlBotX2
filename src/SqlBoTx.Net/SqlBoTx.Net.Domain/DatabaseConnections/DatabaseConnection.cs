using SqlBoTx.Net.Domain.Share.Enums;
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SqlBoTx.Net.Domain.DatabaseConnections
{
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    [Description("数据库连接配置")]
    public class DatabaseConnection
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [Description("最后修改时间")]
        public DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// 连接ID（主键）
        /// </summary>
        [Description("主键自增ID")]
        public int ConnectionId { get; set; }

        /// <summary>
        /// 连接名称
        /// </summary>
        [Description("连接名称")]
        public string? ConnectionName { get; set; }

        /// <summary>
        /// 连接类型（SQL Server、MySQL、Oracle、PostgreSQL等）
        /// </summary>
        [Description("连接类型（1=SQL Server、2=MySQL、3=Oracle、4=PostgreSQL等）")]
        public ConnectionType? ConnectionType { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        [Description("数据库连接字符串")]
        public string? ConnectionString { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Description("用户名")]
        public string? UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Description("密码")]
        public string? UserPassword { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [Description("描述信息")]
        public string? Description { get; set; }

        /// <summary>
        /// 导航属性 - 表
        /// </summary>
        public virtual ICollection<TableStructure> TableStructures { get; set; } = new List<TableStructure>();
    }
}
