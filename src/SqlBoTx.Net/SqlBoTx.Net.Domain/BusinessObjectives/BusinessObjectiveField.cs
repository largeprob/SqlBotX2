using SqlBoTx.Net.Domain.Share.Enums.BusinessObjective;
using SqlBoTx.Net.Domain.TableFields;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Domain.BusinessObjectives
{

    /// <summary>
    /// 业务目标字段
    /// </summary>
    [Description("业务目标字段")]
    public class BusinessObjectiveField
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Description("主键")]
        public int Id { get; set; }

        /// <summary>
        /// 外键-归属业务模块Id
        /// </summary>
        [Description("外键-归属业务模块Id")]
        public int BusinessObjectiveId { get; set; }

        /// <summary>
        /// 导航属性 - 归属业务模块
        /// </summary>
        [Description("导航属性 - 归属业务模块")]
        public virtual BusinessObjective? BusinessObjective { get; set; }

        /// <summary>
        /// 表字段ID
        /// </summary>
        [Description("表字段ID")]
        public int FieldId { get; set; }

        /// <summary>
        /// 表字段ID
        /// </summary>
        [Description("表字段ID")]
        public virtual TableField? TableField { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        [Description("字段名称")]
        public string? Name { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        [Description("字段说明")]
        public string? Description { get; set; }


        /// <summary>
        /// 业务分析角色
        /// </summary>
        [Description("业务分析角色")]
        public BusinessObjectiveFieldBusinesBIRole? BusinesBIRole { get; set; }

        /// <summary>
        /// 维度层次
        /// </summary>
        [Description("维度层次")]
        public BusinessObjectiveFieldDimensionLayer? DimensionLayer { get; set; }

        /// <summary>
        /// 度量层次
        /// </summary>
        [Description("度量层次")]
        public BusinessObjectiveFieldMetricLayer? MetricLayer { get; set; }
    }
}
