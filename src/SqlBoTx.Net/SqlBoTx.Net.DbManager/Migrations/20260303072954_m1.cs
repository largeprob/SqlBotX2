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
                    business_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "业务名称"),
                    synonyms = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "近义词"),
                    description = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "业务解释"),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间"),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_objective", x => x.id);
                },
                comment: "业务目标");

            migrationBuilder.CreateTable(
                name: "database_connection",
                columns: table => new
                {
                    connection_id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
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
                    table.PrimaryKey("PK_database_connection", x => x.connection_id);
                },
                comment: "数据库连接配置");

            migrationBuilder.CreateTable(
                name: "business_objective_dependency_table",
                columns: table => new
                {
                    table_id = table.Column<int>(type: "int", nullable: false),
                    business_objective_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_objective_dependency_table", x => new { x.business_objective_id, x.table_id });
                    table.ForeignKey(
                        name: "FK_business_objective_dependency_table_business_objective_business_objective_id",
                        column: x => x.business_objective_id,
                        principalTable: "business_objective",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_structure",
                columns: table => new
                {
                    table_id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    connection_id = table.Column<int>(type: "int", nullable: false, comment: "外键"),
                    table_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "表名"),
                    display_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "显示名称"),
                    field_count = table.Column<int>(type: "int", nullable: false, comment: "字段数量"),
                    description = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "表描述"),
                    granularity = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "颗粒度描述"),
                    granularity_level = table.Column<int>(type: "int", nullable: true, comment: "颗粒度级别")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_structure", x => x.table_id);
                    table.ForeignKey(
                        name: "FK_table_structure_database_connection_connection_id",
                        column: x => x.connection_id,
                        principalTable: "database_connection",
                        principalColumn: "connection_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "数据库表结构");

            migrationBuilder.CreateTable(
                name: "business_objective_metric",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metric_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "指标名称"),
                    metric_code = table.Column<string>(type: "NVARCHAR(500)", nullable: false, comment: "指标编码"),
                    alias = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "近义词/检索关键词"),
                    business_objective_id = table.Column<int>(type: "int", nullable: false, comment: "外键-归属业务模块Id"),
                    description = table.Column<string>(type: "NVARCHAR(500)", nullable: true, comment: "指标解释"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "生命周期"),
                    main_table_id = table.Column<int>(type: "int", nullable: false, comment: "依赖主体-主表"),
                    main_alias = table.Column<string>(type: "NVARCHAR(255)", nullable: false, defaultValue: "Main"),
                    expression = table.Column<string>(type: "NVARCHAR(500)", nullable: false, comment: "计算公式（非原子）"),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间"),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_objective_metric", x => x.id);
                    table.ForeignKey(
                        name: "FK_business_objective_metric_business_objective_business_objective_id",
                        column: x => x.business_objective_id,
                        principalTable: "business_objective",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_business_objective_metric_table_structure_main_table_id",
                        column: x => x.main_table_id,
                        principalTable: "table_structure",
                        principalColumn: "table_id");
                },
                comment: "业务指标");

            migrationBuilder.CreateTable(
                name: "table_field",
                columns: table => new
                {
                    field_id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    table_id = table.Column<int>(type: "int", nullable: false, comment: "外键"),
                    column_name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "字段名称"),
                    data_type = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "字段数据类型"),
                    is_primary_key = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否为主键"),
                    is_nullable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "是否允许为空"),
                    is_identity = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否为自增字段"),
                    default_value = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "默认值"),
                    field_name = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "字段中文名称"),
                    field_description = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "字段说明"),
                    is_available = table.Column<bool>(type: "bit", nullable: true, comment: "是否可用")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_field", x => x.field_id);
                    table.ForeignKey(
                        name: "FK_table_field_table_structure_table_id",
                        column: x => x.table_id,
                        principalTable: "table_structure",
                        principalColumn: "table_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "表字段明细信息");

            migrationBuilder.CreateTable(
                name: "table_relationship",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键自增ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    source_table_id = table.Column<int>(type: "int", nullable: false, comment: "源表ID"),
                    target_table_id = table.Column<int>(type: "int", nullable: false, comment: "目标表ID"),
                    relationship_type = table.Column<int>(type: "int", nullable: false, comment: "关系类型"),
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
                        principalColumn: "table_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_table_relationship_table_structure_target_table_id",
                        column: x => x.target_table_id,
                        principalTable: "table_structure",
                        principalColumn: "table_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "表关系维护");

            migrationBuilder.CreateTable(
                name: "business_metric_join_path",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    business_metric_id = table.Column<int>(type: "int", nullable: false, comment: "业务指标Id"),
                    order = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    table_id = table.Column<int>(type: "int", nullable: false, comment: "目标表Id）"),
                    alias = table.Column<string>(type: "NVARCHAR(500)", nullable: false, comment: "目标表别名"),
                    join_type = table.Column<string>(type: "NVARCHAR(255)", nullable: false, defaultValue: "LEFT JOIN", comment: "连接类型"),
                    on_condition = table.Column<string>(type: "NVARCHAR(500)", nullable: false, comment: "连接条件")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_metric_join_path", x => x.id);
                    table.ForeignKey(
                        name: "FK_business_metric_join_path_business_objective_metric_business_metric_id",
                        column: x => x.business_metric_id,
                        principalTable: "business_objective_metric",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_business_metric_join_path_table_structure_table_id",
                        column: x => x.table_id,
                        principalTable: "table_structure",
                        principalColumn: "table_id");
                },
                comment: "业务指标路径");

            migrationBuilder.CreateTable(
                name: "business_objective_field",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    business_objective_id = table.Column<int>(type: "int", nullable: false, comment: "外键-归属业务模块Id"),
                    field_id = table.Column<int>(type: "int", nullable: false, comment: "表字段ID"),
                    name = table.Column<string>(type: "NVARCHAR(255)", nullable: false, comment: "字段名称"),
                    description = table.Column<string>(type: "NVARCHAR(255)", nullable: true, comment: "字段说明"),
                    busines_birole = table.Column<int>(type: "int", nullable: true, comment: "业务分析角色"),
                    dimension_layer = table.Column<int>(type: "int", nullable: true, comment: "维度层次"),
                    metric_layer = table.Column<int>(type: "int", nullable: true, comment: "度量层次")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_objective_field", x => x.id);
                    table.ForeignKey(
                        name: "FK_business_objective_field_business_objective_business_objective_id",
                        column: x => x.business_objective_id,
                        principalTable: "business_objective",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_business_objective_field_table_field_field_id",
                        column: x => x.field_id,
                        principalTable: "table_field",
                        principalColumn: "field_id");
                },
                comment: "业务目标字段");

            migrationBuilder.CreateIndex(
                name: "IX_business_metric_join_path_business_metric_id",
                table: "business_metric_join_path",
                column: "business_metric_id");

            migrationBuilder.CreateIndex(
                name: "IX_business_metric_join_path_table_id",
                table: "business_metric_join_path",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_business_objective_field_business_objective_id",
                table: "business_objective_field",
                column: "business_objective_id");

            migrationBuilder.CreateIndex(
                name: "IX_business_objective_field_field_id",
                table: "business_objective_field",
                column: "field_id");

            migrationBuilder.CreateIndex(
                name: "IX_business_objective_metric_business_objective_id",
                table: "business_objective_metric",
                column: "business_objective_id");

            migrationBuilder.CreateIndex(
                name: "IX_business_objective_metric_main_table_id",
                table: "business_objective_metric",
                column: "main_table_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_field_table_id",
                table: "table_field",
                column: "table_id");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_metric_join_path");

            migrationBuilder.DropTable(
                name: "business_objective_dependency_table");

            migrationBuilder.DropTable(
                name: "business_objective_field");

            migrationBuilder.DropTable(
                name: "table_relationship");

            migrationBuilder.DropTable(
                name: "business_objective_metric");

            migrationBuilder.DropTable(
                name: "table_field");

            migrationBuilder.DropTable(
                name: "business_objective");

            migrationBuilder.DropTable(
                name: "table_structure");

            migrationBuilder.DropTable(
                name: "database_connection");
        }
    }
}
