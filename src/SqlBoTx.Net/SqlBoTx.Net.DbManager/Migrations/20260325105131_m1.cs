using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlBoTx.Net.DbManager.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "business_objective",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    business_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "业务域名称"),
                    parent_id = table.Column<int>(type: "int", nullable: true, comment: "父业务域ID"),
                    synonyms = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "近义词"),
                    description = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "业务解释"),
                    summary = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "业务域总结"),
                    key_words = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "关键词：快速定位该领域，而非近义词"),
                    tags = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "领域标签：核心域、支撑域、通用域、系统域"),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间"),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_objective", x => x.id);
                    table.ForeignKey(
                        name: "FK_business_objective_business_objective_parent_id",
                        column: x => x.parent_id,
                        principalTable: "business_objective",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "业务域");

            migrationBuilder.CreateTable(
                name: "database_connection",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间"),
                    last_modified_date = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "最后修改时间"),
                    connection_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "连接名称"),
                    connection_type = table.Column<int>(type: "int", nullable: false, comment: "连接类型（1=SQL Server、2=MySQL、3=Oracle、4=PostgreSQL等）"),
                    connection_string = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false, comment: "数据库连接字符串"),
                    user_name = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "用户名"),
                    user_password = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "密码"),
                    description = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "描述信息")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_database_connection", x => x.id);
                },
                comment: "数据库连接配置");

            migrationBuilder.CreateTable(
                name: "domain_metric",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metric_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "指标名称"),
                    metric_code = table.Column<string>(type: "NVARCHAR(500)", nullable: false, comment: "指标编码"),
                    alias = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "近义词/检索关键词"),
                    scope = table.Column<int>(type: "int", nullable: false, comment: "作用域"),
                    domain_id = table.Column<int>(type: "int", nullable: true, comment: "外键-归属业务模块Id"),
                    description = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "指标解释"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "生命周期"),
                    expression = table.Column<string>(type: "NVARCHAR(500)", nullable: false, comment: "计算公式（非原子）"),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间"),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domain_metric", x => x.id);
                    table.ForeignKey(
                        name: "FK_domain_metric_business_objective_domain_id",
                        column: x => x.domain_id,
                        principalTable: "business_objective",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "业务指标");

            migrationBuilder.CreateTable(
                name: "domain_term",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    domain_id = table.Column<int>(type: "int", nullable: false, comment: "所属业务域ID"),
                    name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "术语名"),
                    alias = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "别名近义词"),
                    description = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "解释说明")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domain_term", x => x.id);
                    table.ForeignKey(
                        name: "FK_domain_term_business_objective_domain_id",
                        column: x => x.domain_id,
                        principalTable: "business_objective",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "领域术语");

            migrationBuilder.CreateTable(
                name: "table_structure",
                columns: table => new
                {
                    table_id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    connection_id = table.Column<int>(type: "int", nullable: false, comment: "数据库连接ID"),
                    table_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "表名"),
                    schema_name = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "模式"),
                    alias = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "别名"),
                    field_count = table.Column<int>(type: "int", nullable: false, comment: "字段数量"),
                    description = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "表描述")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_structure", x => x.table_id);
                    table.ForeignKey(
                        name: "FK_table_structure_database_connection_connection_id",
                        column: x => x.connection_id,
                        principalTable: "database_connection",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "数据库表结构");

            migrationBuilder.CreateTable(
                name: "metric_field_placeholder",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metric_id = table.Column<int>(type: "int", nullable: false, comment: "指标外键"),
                    index = table.Column<int>(type: "int", nullable: false, comment: "占位符序号"),
                    entity_id = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "归属实体Id"),
                    attr_id = table.Column<string>(type: "NVARCHAR(500)", nullable: false, comment: "属性Id"),
                    attr_name = table.Column<string>(type: "NVARCHAR(500)", nullable: false, comment: "属性名")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metric_field_placeholder", x => x.id);
                    table.ForeignKey(
                        name: "FK_metric_field_placeholder_domain_metric_metric_id",
                        column: x => x.metric_id,
                        principalTable: "domain_metric",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "指标字段占位符");

            migrationBuilder.CreateTable(
                name: "domain_entity",
                columns: table => new
                {
                    id = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    domain_id = table.Column<int>(type: "int", nullable: false),
                    reference_connect_id = table.Column<int>(type: "int", nullable: false),
                    reference_table_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    alias = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    description = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    tags = table.Column<string>(type: "NVARCHAR(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domain_entity", x => x.id);
                    table.ForeignKey(
                        name: "FK_domain_entity_business_objective_domain_id",
                        column: x => x.domain_id,
                        principalTable: "business_objective",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_domain_entity_database_connection_reference_connect_id",
                        column: x => x.reference_connect_id,
                        principalTable: "database_connection",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_domain_entity_table_structure_reference_table_id",
                        column: x => x.reference_table_id,
                        principalTable: "table_structure",
                        principalColumn: "table_id");
                },
                comment: "DomainEntity");

            migrationBuilder.CreateTable(
                name: "table_relationship",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    source_table_id = table.Column<int>(type: "int", nullable: false, comment: "源表ID"),
                    target_table_id = table.Column<int>(type: "int", nullable: false, comment: "目标表ID"),
                    source_cardinality = table.Column<int>(type: "int", nullable: false, comment: "源端基数（One/Many）"),
                    target_cardinality = table.Column<int>(type: "int", nullable: false, comment: "目标端基数（One/Many）"),
                    conditions = table.Column<string>(type: "NVARCHAR(500)", nullable: false, comment: "关联条件"),
                    relationship_description = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "关系类型"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_relationship", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_relationship_table_structure_source_table_id",
                        column: x => x.source_table_id,
                        principalTable: "table_structure",
                        principalColumn: "table_id");
                    table.ForeignKey(
                        name: "FK_table_relationship_table_structure_target_table_id",
                        column: x => x.target_table_id,
                        principalTable: "table_structure",
                        principalColumn: "table_id");
                },
                comment: "表关系维护");

            migrationBuilder.CreateTable(
                name: "table_structure_column",
                columns: table => new
                {
                    field_id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    table_id = table.Column<int>(type: "int", nullable: false, comment: "外键"),
                    label = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "标题"),
                    column_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "字段名称"),
                    data_type = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "字段数据类型"),
                    data_type_schema = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "类型描述"),
                    default_value = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "默认值"),
                    description = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "字段说明"),
                    is_primary_key = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否为主键"),
                    is_identity = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否为自增字段"),
                    is_required = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否必填"),
                    is_unique = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否唯一"),
                    is_reference = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "外键引用"),
                    reference_table_name = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "引用表名"),
                    reference_column = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "引用字段"),
                    is_computed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否计算列"),
                    expression = table.Column<string>(type: "NVARCHAR(255)", nullable: false, defaultValue: "False", comment: "表达式"),
                    is_index = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否索引"),
                    indexs = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "索引")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_structure_column", x => x.field_id);
                    table.ForeignKey(
                        name: "FK_table_structure_column_table_structure_table_id",
                        column: x => x.table_id,
                        principalTable: "table_structure",
                        principalColumn: "table_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "表字段明细信息");

            migrationBuilder.CreateTable(
                name: "domain_entity_rel",
                columns: table => new
                {
                    id = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "主键自增ID"),
                    type = table.Column<int>(type: "int", nullable: false, comment: "关联类型"),
                    source_entity_id = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "源实体标识符"),
                    target_entity_id = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "目标实体标识符"),
                    source_role = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "源角色名称"),
                    target_role = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "目标角色名称"),
                    source_cardinality = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "源端基数（One/Many）"),
                    target_cardinality = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "目标端基数（One/Many）"),
                    inverse_of = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "反向关系Id"),
                    cascade_delete = table.Column<int>(type: "int", nullable: false, comment: "是否级联删除"),
                    join_conditions = table.Column<string>(type: "NVARCHAR(500)", nullable: false, comment: "关联条件")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domain_entity_rel", x => x.id);
                    table.ForeignKey(
                        name: "FK_domain_entity_rel_domain_entity_source_entity_id",
                        column: x => x.source_entity_id,
                        principalTable: "domain_entity",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_domain_entity_rel_domain_entity_target_entity_id",
                        column: x => x.target_entity_id,
                        principalTable: "domain_entity",
                        principalColumn: "id");
                },
                comment: "实体关系");

            migrationBuilder.CreateTable(
                name: "domain_entity_attr",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    entity_id = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "实体ID"),
                    column_id = table.Column<int>(type: "int", nullable: false, comment: "引用库字段ID"),
                    column_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "属性列名"),
                    label = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "属性名称"),
                    is_required = table.Column<bool>(type: "bit", nullable: false, comment: "是否必要"),
                    data_type = table.Column<int>(type: "int", nullable: false, comment: "数据类型"),
                    data_type_schema = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "数据类型概要（用来描述取值）"),
                    default_value = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "默认值"),
                    description = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "字段说明（字段额外的解释）"),
                    structure_role = table.Column<int>(type: "int", nullable: false, comment: "结构语义"),
                    semantic_type = table.Column<int>(type: "int", nullable: false, comment: "语义类型"),
                    dimension_category = table.Column<int>(type: "int", nullable: true, comment: "维度分类（SemanticType = Dimension 时有效）"),
                    time_granularity = table.Column<int>(type: "int", nullable: true, comment: "时间粒度（SemanticType = DateTime 时有效）"),
                    supported_aggregations = table.Column<int>(type: "int", nullable: true, comment: "仅当 SemanticType = Measure 时有效。可使用的度量函数"),
                    ForeignKeyMetaData = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domain_entity_attr", x => x.id);
                    table.ForeignKey(
                        name: "FK_domain_entity_attr_domain_entity_entity_id",
                        column: x => x.entity_id,
                        principalTable: "domain_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_domain_entity_attr_table_structure_column_column_id",
                        column: x => x.column_id,
                        principalTable: "table_structure_column",
                        principalColumn: "field_id");
                },
                comment: "实体属性");

            migrationBuilder.CreateIndex(
                name: "IX_business_objective_parent_id",
                table: "business_objective",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_domain_entity_domain_id",
                table: "domain_entity",
                column: "domain_id");

            migrationBuilder.CreateIndex(
                name: "IX_domain_entity_reference_connect_id",
                table: "domain_entity",
                column: "reference_connect_id");

            migrationBuilder.CreateIndex(
                name: "IX_domain_entity_reference_table_id",
                table: "domain_entity",
                column: "reference_table_id");

            migrationBuilder.CreateIndex(
                name: "IX_domain_entity_attr_column_id",
                table: "domain_entity_attr",
                column: "column_id");

            migrationBuilder.CreateIndex(
                name: "IX_domain_entity_attr_entity_id",
                table: "domain_entity_attr",
                column: "entity_id");

            migrationBuilder.CreateIndex(
                name: "IX_domain_entity_rel_source_entity_id",
                table: "domain_entity_rel",
                column: "source_entity_id");

            migrationBuilder.CreateIndex(
                name: "IX_domain_entity_rel_target_entity_id",
                table: "domain_entity_rel",
                column: "target_entity_id");

            migrationBuilder.CreateIndex(
                name: "IX_domain_metric_domain_id",
                table: "domain_metric",
                column: "domain_id");

            migrationBuilder.CreateIndex(
                name: "IX_domain_term_domain_id",
                table: "domain_term",
                column: "domain_id");

            migrationBuilder.CreateIndex(
                name: "IX_metric_field_placeholder_metric_id",
                table: "metric_field_placeholder",
                column: "metric_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_relationship_source_table_id",
                table: "table_relationship",
                column: "source_table_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_relationship_target_table_id",
                table: "table_relationship",
                column: "target_table_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_structure_connection_id",
                table: "table_structure",
                column: "connection_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_structure_column_table_id",
                table: "table_structure_column",
                column: "table_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "domain_entity_attr");

            migrationBuilder.DropTable(
                name: "domain_entity_rel");

            migrationBuilder.DropTable(
                name: "domain_term");

            migrationBuilder.DropTable(
                name: "metric_field_placeholder");

            migrationBuilder.DropTable(
                name: "table_relationship");

            migrationBuilder.DropTable(
                name: "table_structure_column");

            migrationBuilder.DropTable(
                name: "domain_entity");

            migrationBuilder.DropTable(
                name: "domain_metric");

            migrationBuilder.DropTable(
                name: "table_structure");

            migrationBuilder.DropTable(
                name: "business_objective");

            migrationBuilder.DropTable(
                name: "database_connection");
        }
    }
}
