using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.DatabaseConnections;
using SqlBoTx.Net.Domain.TableStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.DomainTerms
{
    /// <summary>
    /// 领域术语
    /// </summary>
    [Description("领域术语")]
    public class DomainTerm
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        [Description("主键自增ID")]
        public int Id { get; set; }

        /// <summary>
        /// 所属业务域ID
        /// </summary>
        [Description("所属业务域ID")]
        public int DomainId { get; set; }


        /// <summary>
        /// 业务域
        /// </summary>
        [Description("业务域")]
        public virtual BusinessObjective? Domain { get; set; }

        /// <summary>
        /// 术语名
        /// </summary>
        [Description("术语名")]
        public string? Name { get; set; }

        /// <summary>
        /// 别名近义词
        /// </summary>
        [Description("别名近义词")]
        public string[]? Alias { get; set; }

        /// <summary>
        /// 解释说明
        /// </summary>
        [Description("解释说明")]
        public string? Description { get; set; }
    }
}
