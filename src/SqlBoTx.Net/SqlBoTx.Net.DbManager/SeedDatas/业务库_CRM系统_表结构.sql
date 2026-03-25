SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- 辅助宏：若表不存在则建表（通过 IF OBJECT_ID 判断）
-- ============================================================


-- ============================================================
-- 1. Organization — 组织架构（根表，无外键依赖）
-- ============================================================
IF OBJECT_ID(N'dbo.Organization', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Organization] (
        [OrganizationId]           INT            IDENTITY(1,1) NOT NULL,
        [OrganizationCode]         NVARCHAR(50)   NOT NULL,
        [OrganizationName]         NVARCHAR(200)  NOT NULL,
        [ParentOrganizationId]     INT            NULL,
        [ParentOrganizationCode]   NVARCHAR(50)   NULL,
        [ParentOrganizationName]   NVARCHAR(200)  NULL,
        -- 1-公司 2-事业部 3-部门 4-其他
        [OrganizationType]         INT            NOT NULL CONSTRAINT [DF_Org_Type]    DEFAULT (1),
        [OrganizationLevel]        INT            NOT NULL CONSTRAINT [DF_Org_Level]   DEFAULT (1),
        [OrganizationPath]         NVARCHAR(500)  NOT NULL,
        [ContactPerson]            NVARCHAR(50)   NULL,
        [ContactPhone]             NVARCHAR(50)   NULL,
        [Address]                  NVARCHAR(500)  NULL,
        [Description]              NVARCHAR(500)  NULL,
        -- 1-启用 0-停用
        [Status]                   INT            NOT NULL CONSTRAINT [DF_Org_Status]  DEFAULT (1),
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_Org_CT]      DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_Organization]              PRIMARY KEY CLUSTERED ([OrganizationId] ASC),
        CONSTRAINT [UK_Organization_Code]         UNIQUE NONCLUSTERED ([OrganizationCode] ASC),
        CONSTRAINT [CK_Organization_Type]         CHECK ([OrganizationType] IN (1,2,3,4)),
        CONSTRAINT [CK_Organization_Status]       CHECK ([Status] IN (0,1))
    )

    ALTER TABLE [dbo].[Organization]
        ADD CONSTRAINT [FK_Organization_Parent]
        FOREIGN KEY ([ParentOrganizationId])
        REFERENCES [dbo].[Organization] ([OrganizationId])

    -- 自关联父节点索引
    CREATE NONCLUSTERED INDEX [IX_Organization_Parent]
        ON [dbo].[Organization] ([ParentOrganizationId])
        WHERE [ParentOrganizationId] IS NOT NULL

    EXEC sp_addextendedproperty N'MS_Description', N'组织架构表，支持多级组织管理',
        N'SCHEMA', N'dbo', N'TABLE', N'Organization';
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织编码，唯一标识组织的代码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationCode'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级组织ID，自关联外键，指向父级组织' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ParentOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级组织编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ParentOrganizationCode'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ParentOrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织类型：1-公司，2-事业部，3-部门，4-其他' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationType'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织层级，从1开始计数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationLevel'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织路径，显示组织的完整层级路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationPath'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织联系人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ContactPerson'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ContactPhone'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'Address'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织描述，详细说明组织职能和职责' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'Description'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：1-启用，0-停用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'Status'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间，默认当前时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'CreateTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'CreateUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'CreateUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ModifyTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键约束，组织ID为主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'CONSTRAINT',@level2name=N'PK_Organization'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'外键约束，组织自关联，确保上级组织存在' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'CONSTRAINT',@level2name=N'FK_Organization_Parent'
END
GO


-- ============================================================
-- 2. Department — 部门表
-- ============================================================
IF OBJECT_ID(N'dbo.Department', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Department] (
        [DepartmentId]             INT            IDENTITY(1,1) NOT NULL,
        [DepartmentCode]           NVARCHAR(50)   NOT NULL,
        [DepartmentName]           NVARCHAR(200)  NOT NULL,
        [ParentDepartmentId]       INT            NULL,
        [ParentDepartmentCode]     NVARCHAR(50)   NULL,
        [ParentDepartmentName]     NVARCHAR(200)  NULL,
        [OrganizationId]           INT            NOT NULL,
        [OrganizationCode]         NVARCHAR(50)   NOT NULL,
        [OrganizationName]         NVARCHAR(200)  NOT NULL,
        [DepartmentLevel]          INT            NOT NULL CONSTRAINT [DF_Dept_Level]  DEFAULT (1),
        [DepartmentPath]           NVARCHAR(500)  NULL,
        [ManagerUserId]            INT            NULL,
        [ManagerUserName]          NVARCHAR(50)   NULL,
        [ContactPhone]             NVARCHAR(50)   NULL,
        [Description]              NVARCHAR(500)  NULL,
        -- 1-启用 0-停用
        [Status]                   INT            NOT NULL CONSTRAINT [DF_Dept_Status] DEFAULT (1),
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_Dept_CT]     DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_Department]                PRIMARY KEY CLUSTERED ([DepartmentId] ASC),
        CONSTRAINT [UK_Department_Code]           UNIQUE NONCLUSTERED ([DepartmentCode] ASC),
        CONSTRAINT [CK_Department_Status]         CHECK ([Status] IN (0,1))
    )

    ALTER TABLE [dbo].[Department]
        ADD CONSTRAINT [FK_Department_Organization]
        FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organization] ([OrganizationId])

    ALTER TABLE [dbo].[Department]
        ADD CONSTRAINT [FK_Department_Parent]
        FOREIGN KEY ([ParentDepartmentId]) REFERENCES [dbo].[Department] ([DepartmentId])

    CREATE NONCLUSTERED INDEX [IX_Department_Organization]
        ON [dbo].[Department] ([OrganizationId])

    CREATE NONCLUSTERED INDEX [IX_Department_Parent]
        ON [dbo].[Department] ([ParentDepartmentId])
        WHERE [ParentDepartmentId] IS NOT NULL

    EXEC sp_addextendedproperty N'MS_Description', N'部门表，支持多级部门管理',
        N'SCHEMA', N'dbo', N'TABLE', N'Department';
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentId'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门编码，唯一标识部门的代码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentCode'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentName'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级部门ID，自关联外键，指向父级部门' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ParentDepartmentId'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级部门编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ParentDepartmentCode'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级部门名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ParentDepartmentName'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属组织ID，外键关联到组织表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'OrganizationId'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属组织编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'OrganizationCode'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'OrganizationName'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门层级，从1开始计数，1为一级部门' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentLevel'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门路径，显示部门的完整层级路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentPath'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门负责人用户ID，关联用户表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ManagerUserId'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门负责人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ManagerUserName'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门联系电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ContactPhone'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门描述，详细说明部门职能和职责' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'Description'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：1-启用，0-停用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'Status'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间，默认当前时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'CreateTime'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'CreateUserId'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'CreateUserName'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ModifyTime'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键约束，部门ID为主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'CONSTRAINT',@level2name=N'PK_Department'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'外键约束，关联组织表，确保所属组织存在' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'CONSTRAINT',@level2name=N'FK_Department_Organization'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'外键约束，部门自关联，确保上级部门存在' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'CONSTRAINT',@level2name=N'FK_Department_Parent'
END
GO


-- ============================================================
-- 3. UserInfo — 用户表
-- ============================================================
IF OBJECT_ID(N'dbo.UserInfo', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[UserInfo] (
        [UserId]                   INT            IDENTITY(1,1) NOT NULL,
        [UserCode]                 NVARCHAR(50)   NOT NULL,
        [UserName]                 NVARCHAR(100)  NOT NULL,
        [LoginName]                NVARCHAR(50)   NOT NULL,
        [PasswordHash]             NVARCHAR(200)  NOT NULL,
        [Email]                    NVARCHAR(100)  NULL,
        [Phone]                    NVARCHAR(20)   NULL,
        [OrganizationId]           INT            NULL,
        [DepartmentId]             INT            NULL,
        [DepartmentName]           NVARCHAR(100)  NULL,
        [Position]                 NVARCHAR(50)   NULL,
        -- 1-销售 2-管理员 3-审核人 4-交付人员
        [UserType]                 INT            NOT NULL CONSTRAINT [DF_User_Type]   DEFAULT (1),
        -- 1-启用 0-停用
        [Status]                   INT            NOT NULL CONSTRAINT [DF_User_Status] DEFAULT (1),
        [LastLoginTime]            DATETIME       NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_User_CT]     DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_UserInfo]                  PRIMARY KEY CLUSTERED ([UserId] ASC),
        CONSTRAINT [UK_UserInfo_Code]             UNIQUE NONCLUSTERED ([UserCode] ASC),
        CONSTRAINT [UK_UserInfo_LoginName]        UNIQUE NONCLUSTERED ([LoginName] ASC),
        CONSTRAINT [CK_UserInfo_Type]             CHECK ([UserType] IN (1,2,3,4)),
        CONSTRAINT [CK_UserInfo_Status]           CHECK ([Status] IN (0,1))
    )

    -- 补全原脚本缺失的 OrganizationId 外键约束
    ALTER TABLE [dbo].[UserInfo]
        ADD CONSTRAINT [FK_UserInfo_Organization]
        FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organization] ([OrganizationId])

    ALTER TABLE [dbo].[UserInfo]
        ADD CONSTRAINT [FK_UserInfo_Department]
        FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Department] ([DepartmentId])

    CREATE NONCLUSTERED INDEX [IX_UserInfo_Organization]
        ON [dbo].[UserInfo] ([OrganizationId])
        WHERE [OrganizationId] IS NOT NULL

    CREATE NONCLUSTERED INDEX [IX_UserInfo_Department]
        ON [dbo].[UserInfo] ([DepartmentId])
        WHERE [DepartmentId] IS NOT NULL

    EXEC sp_addextendedproperty N'MS_Description', N'用户信息表，存储系统用户数据',
        N'SCHEMA', N'dbo', N'TABLE', N'UserInfo';

    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'UserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户编码，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'UserCode'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'UserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登录名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'LoginName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码哈希值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'PasswordHash'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮箱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'Email'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'Phone'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属组织ID，外键关联到组织表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'OrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'DepartmentId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'DepartmentName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'职位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'Position'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户类型：1-销售，2-管理员，3-审核人，4-交付人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'UserType'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：1-启用，0-停用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'Status'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后登录时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'LastLoginTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'Remark'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'CreateTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'CreateUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'CreateUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'ModifyTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'

END
GO


-- ============================================================
-- 4. Product — 产品表
-- ============================================================
IF OBJECT_ID(N'dbo.Product', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Product] (
        [ProductId]                INT            IDENTITY(1,1) NOT NULL,
        [ProductCode]              NVARCHAR(50)   NOT NULL,
        [ProductName]              NVARCHAR(200)  NOT NULL,
        [ProductType]              NVARCHAR(50)   NOT NULL,
        [ProductCategory]          NVARCHAR(50)   NULL,
        [ProductSpecification]     NVARCHAR(MAX)  NULL,
        [Unit]                     NVARCHAR(20)   NOT NULL,
        [UnitPrice]                DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Product_UnitPrice]  DEFAULT (0),
        [CostPrice]                DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Product_CostPrice]  DEFAULT (0),
        [MinimumStock]             INT            NULL        CONSTRAINT [DF_Product_MinStock] DEFAULT (0),
        [CurrentStock]             INT            NULL        CONSTRAINT [DF_Product_CurStock] DEFAULT (0),
        -- 1-启用 0-停用
        [Status]                   INT            NOT NULL CONSTRAINT [DF_Product_Status] DEFAULT (1),
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_Product_CT]     DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_Product]                   PRIMARY KEY CLUSTERED ([ProductId] ASC),
        CONSTRAINT [UK_Product_Code]              UNIQUE NONCLUSTERED ([ProductCode] ASC),
        CONSTRAINT [CK_Product_UnitPrice]         CHECK ([UnitPrice] >= 0),
        CONSTRAINT [CK_Product_CostPrice]         CHECK ([CostPrice] >= 0),
        CONSTRAINT [CK_Product_Status]            CHECK ([Status] IN (0,1))
    )

    -- 按产品类型/分类查询索引
    CREATE NONCLUSTERED INDEX [IX_Product_TypeCategory]
        ON [dbo].[Product] ([ProductType], [ProductCategory])

    EXEC sp_addextendedproperty N'MS_Description', N'产品信息表，存储所有可销售的产品数据',
        N'SCHEMA', N'dbo', N'TABLE', N'Product';
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品编码，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductCode'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductType'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品分类' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductCategory'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品规格描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductSpecification'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'计量单位（如：个、台、套）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'Unit'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标准单价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'UnitPrice'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'成本价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CostPrice'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最低库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'MinimumStock'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CurrentStock'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：1-启用，0-停用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'Status'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'Remark'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CreateTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CreateUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CreateUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ModifyTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
END
GO


-- ============================================================
-- 5. Customer — 客户表
-- ============================================================
IF OBJECT_ID(N'dbo.Customer', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Customer] (
        [CustomerId]               INT            IDENTITY(1,1) NOT NULL,
        [CustomerCode]             NVARCHAR(50)   NOT NULL,
        [CustomerName]             NVARCHAR(200)  NOT NULL,
        -- 1-企业 2-个人 3-政府 4-其他
        [CustomerType]             INT            NOT NULL CONSTRAINT [DF_Cust_Type]        DEFAULT (1),
        [CustomerSource]           NVARCHAR(100)  NULL,
        [Industry]                 NVARCHAR(100)  NULL,
        [ContactPerson]            NVARCHAR(50)   NULL,
        [ContactPhone]             NVARCHAR(50)   NULL,
        [ContactEmail]             NVARCHAR(100)  NULL,
        [Address]                  NVARCHAR(500)  NULL,
        -- 1-A级(重要) 2-B级(一般) 3-C级(潜在)
        [CustomerLevel]            INT            NULL        CONSTRAINT [DF_Cust_Level]        DEFAULT (1),
        -- 1-良好 2-一般 3-受限
        [CreditStatus]             INT            NULL        CONSTRAINT [DF_Cust_Credit]       DEFAULT (1),
        -- 1-公海客户 2-已分配待跟进 3-正式客户 4-流失客户
        [CustomerStatus]           INT            NOT NULL CONSTRAINT [DF_Cust_Status]       DEFAULT (1),
        -- 1-直接客户 2-渠道客户（原脚本字段无注释，补全说明）
        [CustomerTransactionType]  INT            NOT NULL CONSTRAINT [DF_Cust_TxType]       DEFAULT (1),
        -- 1-是(公海) 0-否(私海)
        [IsPublic]                 BIT            NOT NULL CONSTRAINT [DF_Cust_IsPublic]     DEFAULT (1),
        [OwnerUserId]              INT            NULL,
        [OwnerUserName]            NVARCHAR(50)   NULL,
        [OwnerOrganizationId]      INT            NULL,
        [OwnerOrganizationName]    NVARCHAR(100)  NULL,
        [RecentContactTime]        DATETIME       NULL,
        [TotalOrderAmount]         DECIMAL(18,2)  NULL        CONSTRAINT [DF_Cust_OrdAmt]       DEFAULT (0),
        [TotalOrderCount]          INT            NULL        CONSTRAINT [DF_Cust_OrdCnt]       DEFAULT (0),
        [LastOrderTime]            DATETIME       NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_Cust_CT]           DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_Customer]                  PRIMARY KEY CLUSTERED ([CustomerId] ASC),
        CONSTRAINT [UK_Customer_Code]             UNIQUE NONCLUSTERED ([CustomerCode] ASC),
        CONSTRAINT [CK_Customer_Type]             CHECK ([CustomerType] IN (1,2,3,4)),
        CONSTRAINT [CK_Customer_Level]            CHECK ([CustomerLevel] IN (1,2,3)),
        CONSTRAINT [CK_Customer_CreditStatus]     CHECK ([CreditStatus] IN (1,2,3)),
        CONSTRAINT [CK_Customer_Status]           CHECK ([CustomerStatus] IN (1,2,3,4)),
        CONSTRAINT [CK_Customer_TxType]           CHECK ([CustomerTransactionType] IN (1,2))
    )

    ALTER TABLE [dbo].[Customer]
        ADD CONSTRAINT [FK_Customer_OwnerUser]
        FOREIGN KEY ([OwnerUserId]) REFERENCES [dbo].[UserInfo] ([UserId])

    -- 高频查询: 按负责人、状态、公海标记
    CREATE NONCLUSTERED INDEX [IX_Customer_Owner]
        ON [dbo].[Customer] ([OwnerUserId])
        INCLUDE ([CustomerName], [CustomerStatus])
        WHERE [OwnerUserId] IS NOT NULL

    CREATE NONCLUSTERED INDEX [IX_Customer_Status_Public]
        ON [dbo].[Customer] ([CustomerStatus], [IsPublic])
        INCLUDE ([CustomerName], [OwnerUserId])

    CREATE NONCLUSTERED INDEX [IX_Customer_RecentContact]
        ON [dbo].[Customer] ([RecentContactTime] DESC)
        WHERE [RecentContactTime] IS NOT NULL

    EXEC sp_addextendedproperty N'MS_Description', N'客户信息表，统一管理公海客户和正式客户',
        N'SCHEMA', N'dbo', N'TABLE', N'Customer'
    EXEC sp_addextendedproperty N'MS_Description', N'交易类型：1-直接客户，2-渠道客户',
        N'SCHEMA', N'dbo', N'TABLE', N'Customer', N'COLUMN', N'CustomerTransactionType'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户编码，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerCode'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户类型：1-企业，2-个人，3-政府，4-其他' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerType'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户来源（如：展会、网络搜索、老客户介绍等）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerSource'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属行业' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'Industry'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ContactPerson'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ContactPhone'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系邮箱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ContactEmail'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'Address'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户等级：1-A级（重要），2-B级（一般），3-C级（潜在）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerLevel'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'信用状态：1-良好，2-一般，3-受限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreditStatus'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户状态：1-公海客户，2-已分配待跟进，3-正式客户，4-流失客户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerStatus'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否公海客户：1-是，0-否' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'IsPublic'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人用户ID，如果为空则表示在公海中' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'OwnerUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'OwnerUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'OwnerOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'OwnerOrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近联系时间，用于判断客户是否活跃' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'RecentContactTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'累计订单金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'TotalOrderAmount'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'累计订单数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'TotalOrderCount'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后下单时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'LastOrderTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'Remark'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreateTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreateUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreateUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ModifyTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
END
GO


-- ============================================================
-- 6. Opportunity — 商机表
-- ============================================================
IF OBJECT_ID(N'dbo.Opportunity', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Opportunity] (
        [OpportunityId]            INT            IDENTITY(1,1) NOT NULL,
        [OpportunityCode]          NVARCHAR(50)   NOT NULL,
        [OpportunityName]          NVARCHAR(200)  NOT NULL,
        [CustomerId]               INT            NOT NULL,
        [CustomerName]             NVARCHAR(200)  NOT NULL,
        [CustomerType]             INT            NULL,
        -- 1-新客户商机 2-老客户商机
        [OpportunityType]          INT            NOT NULL CONSTRAINT [DF_Opp_Type]      DEFAULT (1),
        [ProjectBackground]        NVARCHAR(MAX)  NULL,
        [CustomerNeed]             NVARCHAR(MAX)  NULL,
        [ExpectedAmount]           DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Opp_Amount]    DEFAULT (0),
        [ExpectedClosingDate]      DATE           NULL,
        -- 0-100
        [WinProbability]           INT            NOT NULL CONSTRAINT [DF_Opp_WinProb]   DEFAULT (30),
        [CompetitorInfo]           NVARCHAR(500)  NULL,
        -- 1-初步接触 2-需求分析 3-方案制定 4-报价中 5-谈判中 6-赢单 7-丢单
        [OpportunityStage]         INT            NOT NULL CONSTRAINT [DF_Opp_Stage]     DEFAULT (1),
        -- 0-走报价流程 1-走投标流程
        [NeedBid]                  BIT            NOT NULL CONSTRAINT [DF_Opp_NeedBid]   DEFAULT (0),
        [BidProjectCode]           NVARCHAR(50)   NULL,
        [SalesOwnerUserId]         INT            NOT NULL,
        [SalesOwnerUserName]       NVARCHAR(50)   NOT NULL,
        [SalesOwnerOrganizationId] INT            NOT NULL,
        [SalesOwnerOrganizationName] NVARCHAR(100) NOT NULL,
        [CoSalesUserIds]           NVARCHAR(500)  NULL,
        [CoSalesUserNames]         NVARCHAR(500)  NULL,
        [CurrentTask]              NVARCHAR(200)  NULL,
        [NextStep]                 NVARCHAR(200)  NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        -- 1-进行中 2-已关闭 3-已取消
        [OpportunityStatus]        INT            NOT NULL CONSTRAINT [DF_Opp_Status]    DEFAULT (1),
        [CloseReason]              NVARCHAR(200)  NULL,
        [CloseTime]                DATETIME       NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_Opp_CT]        DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_Opportunity]               PRIMARY KEY CLUSTERED ([OpportunityId] ASC),
        CONSTRAINT [UK_Opportunity_Code]          UNIQUE NONCLUSTERED ([OpportunityCode] ASC),
        CONSTRAINT [CK_Opportunity_Type]          CHECK ([OpportunityType] IN (1,2)),
        CONSTRAINT [CK_Opportunity_Stage]         CHECK ([OpportunityStage] BETWEEN 1 AND 7),
        CONSTRAINT [CK_Opportunity_WinProb]       CHECK ([WinProbability] BETWEEN 0 AND 100),
        CONSTRAINT [CK_Opportunity_Status]        CHECK ([OpportunityStatus] IN (1,2,3))
    )

    ALTER TABLE [dbo].[Opportunity]
        ADD CONSTRAINT [FK_Opportunity_Customer]
        FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId])

    ALTER TABLE [dbo].[Opportunity]
        ADD CONSTRAINT [FK_Opportunity_SalesOwner]
        FOREIGN KEY ([SalesOwnerUserId]) REFERENCES [dbo].[UserInfo] ([UserId])

    CREATE NONCLUSTERED INDEX [IX_Opportunity_Customer]
        ON [dbo].[Opportunity] ([CustomerId])
        INCLUDE ([OpportunityName], [OpportunityStage], [OpportunityStatus])

    CREATE NONCLUSTERED INDEX [IX_Opportunity_SalesOwner]
        ON [dbo].[Opportunity] ([SalesOwnerUserId], [OpportunityStatus])
        INCLUDE ([OpportunityName], [ExpectedAmount])

    CREATE NONCLUSTERED INDEX [IX_Opportunity_Stage_Status]
        ON [dbo].[Opportunity] ([OpportunityStage], [OpportunityStatus])
        INCLUDE ([ExpectedAmount], [ExpectedClosingDate])

    EXEC sp_addextendedproperty N'MS_Description', N'商机信息表，记录销售机会的全过程',
        N'SCHEMA', N'dbo', N'TABLE', N'Opportunity';
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityCode'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CustomerId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CustomerName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CustomerType'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机类型：1-新客户商机，2-老客户商机' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityType'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目背景' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ProjectBackground'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户需求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CustomerNeed'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预计金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ExpectedAmount'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预计成交日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ExpectedClosingDate'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'赢单概率，取值范围0-100' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'WinProbability'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'竞争对手信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CompetitorInfo'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机阶段：1-初步接触，2-需求分析，3-方案制定，4-报价中，5-谈判中，6-赢单，7-丢单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityStage'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否需要投标：0-否（走报价流程），1-是（走投标流程）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'NeedBid'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标项目编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'BidProjectCode'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人用户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'SalesOwnerUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'SalesOwnerUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'SalesOwnerOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'SalesOwnerOrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'协同销售人员ID列表，多个ID用逗号分隔' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CoSalesUserIds'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'协同销售人员姓名列表，多个姓名用逗号分隔' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CoSalesUserNames'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前任务' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CurrentTask'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'下一步计划' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'NextStep'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'Remark'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机状态：1-进行中，2-已关闭，3-已取消' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityStatus'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关闭原因' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CloseReason'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关闭时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CloseTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CreateTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CreateUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CreateUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ModifyTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
END
GO


-- ============================================================
-- 7. Quotation — 报价单头表
-- ============================================================
IF OBJECT_ID(N'dbo.Quotation', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Quotation] (
        [QuotationId]              INT            IDENTITY(1,1) NOT NULL,
        [QuotationCode]            NVARCHAR(50)   NOT NULL,
        [OpportunityId]            INT            NOT NULL,
        [OpportunityCode]          NVARCHAR(50)   NOT NULL,
        [CustomerId]               INT            NOT NULL,
        [CustomerName]             NVARCHAR(200)  NOT NULL,
        [QuotationDate]            DATE           NOT NULL,
        [ValidUntilDate]           DATE           NOT NULL,
        [TotalAmount]              DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Quot_Amt]          DEFAULT (0),
        [TaxRate]                  DECIMAL(5,2)   NULL        CONSTRAINT [DF_Quot_TaxRate]    DEFAULT (0),
        [TaxAmount]                DECIMAL(18,2)  NULL        CONSTRAINT [DF_Quot_TaxAmt]     DEFAULT (0),
        [TotalAmountWithTax]       DECIMAL(18,2)  NULL        CONSTRAINT [DF_Quot_AmtTax]     DEFAULT (0),
        [Currency]                 NVARCHAR(10)   NULL        CONSTRAINT [DF_Quot_Currency]   DEFAULT ('CNY'),
        [PaymentTerms]             NVARCHAR(500)  NULL,
        [DeliveryTerms]            NVARCHAR(500)  NULL,
        [WarrantyTerms]            NVARCHAR(500)  NULL,
        [SpecialTerms]             NVARCHAR(500)  NULL,
        -- 1-草稿 2-已提交 3-客户确认中 4-客户已确认 5-客户拒绝 6-已过期
        [QuotationStatus]          INT            NOT NULL CONSTRAINT [DF_Quot_Status]        DEFAULT (1),
        -- 1-未提交 2-审批中 3-已通过 4-已驳回
        [ApprovalStatus]           INT            NOT NULL CONSTRAINT [DF_Quot_ApprSt]        DEFAULT (1),
        [ApprovalFlowId]           INT            NULL,
        [ApprovalUserId]           INT            NULL,
        [ApprovalUserName]         NVARCHAR(50)   NULL,
        [ApprovalTime]             DATETIME       NULL,
        [ApprovalComment]          NVARCHAR(500)  NULL,
        [CustomerFeedback]         NVARCHAR(500)  NULL,
        [CustomerConfirmTime]      DATETIME       NULL,
        [NextFollowUpTime]         DATETIME       NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_Quot_CT]            DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_Quotation]                 PRIMARY KEY CLUSTERED ([QuotationId] ASC),
        CONSTRAINT [UK_Quotation_Code]            UNIQUE NONCLUSTERED ([QuotationCode] ASC),
        CONSTRAINT [CK_Quotation_Status]          CHECK ([QuotationStatus] BETWEEN 1 AND 6),
        CONSTRAINT [CK_Quotation_ApprSt]          CHECK ([ApprovalStatus] IN (1,2,3,4)),
        CONSTRAINT [CK_Quotation_ValidDate]       CHECK ([ValidUntilDate] >= [QuotationDate])
    )

    ALTER TABLE [dbo].[Quotation]
        ADD CONSTRAINT [FK_Quotation_Opportunity]
        FOREIGN KEY ([OpportunityId]) REFERENCES [dbo].[Opportunity] ([OpportunityId])

    ALTER TABLE [dbo].[Quotation]
        ADD CONSTRAINT [FK_Quotation_Customer]
        FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId])

    CREATE NONCLUSTERED INDEX [IX_Quotation_Opportunity]
        ON [dbo].[Quotation] ([OpportunityId])
        INCLUDE ([QuotationStatus], [TotalAmountWithTax])

    CREATE NONCLUSTERED INDEX [IX_Quotation_Customer]
        ON [dbo].[Quotation] ([CustomerId], [QuotationStatus])

    CREATE NONCLUSTERED INDEX [IX_Quotation_FollowUp]
        ON [dbo].[Quotation] ([NextFollowUpTime])
        WHERE [NextFollowUpTime] IS NOT NULL

    EXEC sp_addextendedproperty N'MS_Description', N'销售报价单头表，记录普通项目的报价信息',
        N'SCHEMA', N'dbo', N'TABLE', N'Quotation'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'QuotationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价单号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'QuotationCode'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'OpportunityId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'OpportunityCode'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CustomerId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CustomerName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'QuotationDate'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'有效期至' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ValidUntilDate'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价总金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'TotalAmount'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'TaxRate'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'TaxAmount'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'含税总金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'TotalAmountWithTax'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'币种，默认CNY（人民币）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'Currency'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'PaymentTerms'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交货条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'DeliveryTerms'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保修条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'WarrantyTerms'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'特殊条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'SpecialTerms'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价状态：1-草稿，2-已提交，3-客户确认中，4-客户已确认，5-客户拒绝，6-已过期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'QuotationStatus'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-未提交，2-审批中，3-已通过，4-已驳回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalStatus'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalFlowId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalComment'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户反馈' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CustomerFeedback'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CustomerConfirmTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'下次跟进时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'NextFollowUpTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'Remark'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CreateTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CreateUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CreateUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ModifyTime'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
END
GO


-- ============================================================
-- 8. QuotationDetail — 报价明细表（原脚本缺失，补充）
-- ============================================================
IF OBJECT_ID(N'dbo.QuotationDetail', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[QuotationDetail] (
        [QuotationDetailId]        INT            IDENTITY(1,1) NOT NULL,
        [QuotationId]              INT            NOT NULL,
        [QuotationCode]            NVARCHAR(50)   NOT NULL,
        [LineNumber]               INT            NOT NULL,
        [ProductId]                INT            NOT NULL,
        [ProductCode]              NVARCHAR(50)   NOT NULL,
        [ProductName]              NVARCHAR(200)  NOT NULL,
        [ProductSpecification]     NVARCHAR(MAX)  NULL,
        [Unit]                     NVARCHAR(20)   NOT NULL,
        [Quantity]                 DECIMAL(18,4)  NOT NULL CONSTRAINT [DF_QD_Qty]       DEFAULT (0),
        [UnitPrice]                DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_QD_UnitPrice] DEFAULT (0),
        [Amount]                   DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_QD_Amount]    DEFAULT (0),
        [TaxRate]                  DECIMAL(5,2)   NULL        CONSTRAINT [DF_QD_TaxRate] DEFAULT (0),
        [TaxAmount]                DECIMAL(18,2)  NULL        CONSTRAINT [DF_QD_TaxAmt]  DEFAULT (0),
        [AmountWithTax]            DECIMAL(18,2)  NULL        CONSTRAINT [DF_QD_AmtTax]  DEFAULT (0),
        -- 折扣率，0-100
        [DiscountRate]             DECIMAL(5,2)   NULL,
        [DeliveryDate]             DATE           NULL,
        [TechnicalParameters]      NVARCHAR(MAX)  NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_QD_CT]        DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_QuotationDetail]           PRIMARY KEY CLUSTERED ([QuotationDetailId] ASC),
        CONSTRAINT [UK_QD_QuotationLine]          UNIQUE NONCLUSTERED ([QuotationId], [LineNumber]),
        CONSTRAINT [CK_QD_Quantity]               CHECK ([Quantity] > 0),
        CONSTRAINT [CK_QD_UnitPrice]              CHECK ([UnitPrice] >= 0),
        CONSTRAINT [CK_QD_DiscountRate]           CHECK ([DiscountRate] IS NULL OR ([DiscountRate] BETWEEN 0 AND 100))
    )

    ALTER TABLE [dbo].[QuotationDetail]
        ADD CONSTRAINT [FK_QuotationDetail_Quotation]
        FOREIGN KEY ([QuotationId]) REFERENCES [dbo].[Quotation] ([QuotationId])

    ALTER TABLE [dbo].[QuotationDetail]
        ADD CONSTRAINT [FK_QuotationDetail_Product]
        FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId])

    CREATE NONCLUSTERED INDEX [IX_QuotationDetail_Quotation]
        ON [dbo].[QuotationDetail] ([QuotationId])

    CREATE NONCLUSTERED INDEX [IX_QuotationDetail_Product]
        ON [dbo].[QuotationDetail] ([ProductId])

    EXEC sp_addextendedproperty N'MS_Description', N'报价明细表，记录报价单的产品行项目（原脚本缺失，本版本补充）',
        N'SCHEMA', N'dbo', N'TABLE', N'QuotationDetail'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价明细ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'QuotationDetailId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价ID，外键关联Quotation表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'QuotationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价单号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'QuotationCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'行号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'LineNumber'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品ID，外键关联Product表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'ProductId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'ProductCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'ProductName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品规格' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'ProductSpecification'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'计量单位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'Unit'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'Quantity'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'UnitPrice'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'Amount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'TaxRate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'TaxAmount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'含税金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'AmountWithTax'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'折扣率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'DiscountRate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'要求交货日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'DeliveryDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术参数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'TechnicalParameters'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'Remark'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'CreateTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'CreateUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'CreateUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'ModifyTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'QuotationDetail', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
END
GO


-- ============================================================
-- 9. Bid — 投标表
-- ============================================================
IF OBJECT_ID(N'dbo.Bid', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Bid] (
        [BidId]                    INT            IDENTITY(1,1) NOT NULL,
        [BidCode]                  NVARCHAR(50)   NOT NULL,
        [OpportunityId]            INT            NOT NULL,
        [OpportunityCode]          NVARCHAR(50)   NOT NULL,
        [CustomerId]               INT            NOT NULL,
        [CustomerName]             NVARCHAR(200)  NOT NULL,
        [BidProjectName]           NVARCHAR(200)  NOT NULL,
        [BidProjectCode]           NVARCHAR(50)   NOT NULL,
        [TenderOrganization]       NVARCHAR(200)  NOT NULL,
        [TenderAddress]            NVARCHAR(500)  NULL,
        [BidOpeningDate]           DATETIME       NOT NULL,
        [BidSubmissionDeadline]    DATETIME       NOT NULL,
        [BidBondAmount]            DECIMAL(18,2)  NULL        CONSTRAINT [DF_Bid_BondAmt]      DEFAULT (0),
        [BidBondPaid]              BIT            NULL        CONSTRAINT [DF_Bid_BondPaid]     DEFAULT (0),
        [BidBondPaidTime]          DATETIME       NULL,
        [PerformanceBondRequired]  BIT            NULL        CONSTRAINT [DF_Bid_PerfBond]     DEFAULT (0),
        [PerformanceBondRate]      DECIMAL(5,2)   NULL        CONSTRAINT [DF_Bid_PerfBondRate] DEFAULT (0),
        [TechnicalScore]           DECIMAL(5,2)   NULL,
        [CommercialScore]          DECIMAL(5,2)   NULL,
        [TotalScore]               DECIMAL(5,2)   NULL,
        [Ranking]                  INT            NULL,
        [Competitors]              NVARCHAR(500)  NULL,
        [OurAdvantages]            NVARCHAR(500)  NULL,
        [OurDisadvantages]         NVARCHAR(500)  NULL,
        -- 1-准备中 2-已提交 3-已开标 4-中标 5-未中标 6-废标
        [BidStatus]                INT            NOT NULL CONSTRAINT [DF_Bid_Status]          DEFAULT (1),
        -- 1-未提交 2-审批中 3-已通过 4-已驳回
        [ApprovalStatus]           INT            NOT NULL CONSTRAINT [DF_Bid_ApprSt]          DEFAULT (1),
        [ApprovalFlowId]           INT            NULL,
        [ApprovalUserId]           INT            NULL,
        [ApprovalUserName]         NVARCHAR(50)   NULL,
        [ApprovalTime]             DATETIME       NULL,
        [ApprovalComment]          NVARCHAR(500)  NULL,
        [WinNoticeTime]            DATETIME       NULL,
        [WinNoticeNumber]          NVARCHAR(50)   NULL,
        [BidDocumentPath]          NVARCHAR(500)  NULL,
        [OtherAttachments]         NVARCHAR(500)  NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_Bid_CT]              DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_Bid]                       PRIMARY KEY CLUSTERED ([BidId] ASC),
        CONSTRAINT [UK_Bid_Code]                  UNIQUE NONCLUSTERED ([BidCode] ASC),
        CONSTRAINT [CK_Bid_Status]                CHECK ([BidStatus] BETWEEN 1 AND 6),
        CONSTRAINT [CK_Bid_ApprSt]                CHECK ([ApprovalStatus] IN (1,2,3,4)),
        CONSTRAINT [CK_Bid_Deadline]              CHECK ([BidSubmissionDeadline] <= [BidOpeningDate])
    )

    ALTER TABLE [dbo].[Bid]
        ADD CONSTRAINT [FK_Bid_Opportunity]
        FOREIGN KEY ([OpportunityId]) REFERENCES [dbo].[Opportunity] ([OpportunityId])

    ALTER TABLE [dbo].[Bid]
        ADD CONSTRAINT [FK_Bid_Customer]
        FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId])

    CREATE NONCLUSTERED INDEX [IX_Bid_Opportunity]
        ON [dbo].[Bid] ([OpportunityId])
        INCLUDE ([BidStatus], [BidOpeningDate])

    CREATE NONCLUSTERED INDEX [IX_Bid_Customer]
        ON [dbo].[Bid] ([CustomerId], [BidStatus])

    CREATE NONCLUSTERED INDEX [IX_Bid_OpeningDate]
        ON [dbo].[Bid] ([BidOpeningDate])

    EXEC sp_addextendedproperty N'MS_Description', N'销售投标表，记录招投标项目的投标信息',
        N'SCHEMA', N'dbo', N'TABLE', N'Bid'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标编号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'OpportunityId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'OpportunityCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CustomerId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CustomerName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标项目名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidProjectName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标项目编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidProjectCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标单位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'TenderOrganization'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'TenderAddress'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开标日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidOpeningDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标截止日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidSubmissionDeadline'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标保证金金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidBondAmount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标保证金是否已支付' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidBondPaid'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保证金支付时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidBondPaidTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否需要履约保证金' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'PerformanceBondRequired'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'履约保证金比例' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'PerformanceBondRate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术得分' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'TechnicalScore'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商务得分' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CommercialScore'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'总分' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'TotalScore'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'Ranking'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'竞争对手信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'Competitors'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'我方优势' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'OurAdvantages'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'我方劣势' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'OurDisadvantages'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标状态：1-准备中，2-已提交，3-已开标，4-中标，5-未中标，6-废标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidStatus'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-未提交，2-审批中，3-已通过，4-已驳回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalStatus'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalFlowId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalComment'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'中标通知书时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'WinNoticeTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'中标通知书编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'WinNoticeNumber'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标文件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidDocumentPath'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'其他附件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'OtherAttachments'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'Remark'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CreateTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CreateUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CreateUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ModifyTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
END
GO


-- ============================================================
-- 10. ProjectApproval — 项目立项审批表
-- ============================================================
IF OBJECT_ID(N'dbo.ProjectApproval', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ProjectApproval] (
        [ProjectApprovalId]        INT            IDENTITY(1,1) NOT NULL,
        [ProjectApprovalCode]      NVARCHAR(50)   NOT NULL,
        -- 引用类型：1-报价 2-投标
        [ReferenceType]            INT            NOT NULL,
        [ReferenceId]              INT            NOT NULL,
        [ReferenceCode]            NVARCHAR(50)   NOT NULL,
        [OpportunityId]            INT            NOT NULL,
        [OpportunityCode]          NVARCHAR(50)   NOT NULL,
        [CustomerId]               INT            NOT NULL,
        [CustomerName]             NVARCHAR(200)  NOT NULL,
        [ProjectName]              NVARCHAR(200)  NOT NULL,
        [ProjectBackground]        NVARCHAR(MAX)  NULL,
        [CustomerDemand]           NVARCHAR(MAX)  NULL,
        [SolutionOverview]         NVARCHAR(MAX)  NULL,
        [EstimatedAmount]          DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_PA_EstAmt]    DEFAULT (0),
        [EstimatedCost]            DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_PA_EstCost]   DEFAULT (0),
        [EstimatedProfit]          DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_PA_EstProfit] DEFAULT (0),
        [ProfitMargin]             DECIMAL(5,2)   NULL        CONSTRAINT [DF_PA_Margin]  DEFAULT (0),
        [ResourceRequirements]     NVARCHAR(500)  NULL,
        [ImplementationPeriod]     NVARCHAR(100)  NULL,
        [RiskAnalysis]             NVARCHAR(MAX)  NULL,
        [RiskMitigationMeasures]   NVARCHAR(MAX)  NULL,
        [SalesDepartmentOpinion]   NVARCHAR(500)  NULL,
        [TechnicalDepartmentOpinion] NVARCHAR(500) NULL,
        [FinanceDepartmentOpinion] NVARCHAR(500)  NULL,
        [ManagementOpinion]        NVARCHAR(500)  NULL,
        -- 1-草稿 2-销售审批中 3-技术审批中 4-财务审批中 5-管理层审批中 6-已通过 7-已驳回
        [ApprovalStatus]           INT            NOT NULL CONSTRAINT [DF_PA_ApprSt]    DEFAULT (1),
        [CurrentApprovalStage]     INT            NULL        CONSTRAINT [DF_PA_Stage]   DEFAULT (1),
        [ApprovalFlowId]           INT            NULL,
        [FinalApproverUserId]      INT            NULL,
        [FinalApproverUserName]    NVARCHAR(50)   NULL,
        [FinalApprovalTime]        DATETIME       NULL,
        [ApprovalComment]          NVARCHAR(500)  NULL,
        [IsUrgent]                 BIT            NULL        CONSTRAINT [DF_PA_Urgent]  DEFAULT (0),
        [UrgentReason]             NVARCHAR(200)  NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_PA_CT]         DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_ProjectApproval]           PRIMARY KEY CLUSTERED ([ProjectApprovalId] ASC),
        CONSTRAINT [UK_ProjectApproval_Code]      UNIQUE NONCLUSTERED ([ProjectApprovalCode] ASC),
        CONSTRAINT [CK_PA_ReferenceType]          CHECK ([ReferenceType] IN (1,2)),
        CONSTRAINT [CK_PA_ApprSt]                 CHECK ([ApprovalStatus] BETWEEN 1 AND 7)
    )

    ALTER TABLE [dbo].[ProjectApproval]
        ADD CONSTRAINT [FK_PA_Opportunity]
        FOREIGN KEY ([OpportunityId]) REFERENCES [dbo].[Opportunity] ([OpportunityId])

    ALTER TABLE [dbo].[ProjectApproval]
        ADD CONSTRAINT [FK_PA_Customer]
        FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId])

    CREATE NONCLUSTERED INDEX [IX_PA_Opportunity]
        ON [dbo].[ProjectApproval] ([OpportunityId])
        INCLUDE ([ApprovalStatus], [ProjectName])

    CREATE NONCLUSTERED INDEX [IX_PA_Customer]
        ON [dbo].[ProjectApproval] ([CustomerId], [ApprovalStatus])

    -- 多态引用索引（ReferenceType + ReferenceId）
    CREATE NONCLUSTERED INDEX [IX_PA_Reference]
        ON [dbo].[ProjectApproval] ([ReferenceType], [ReferenceId])

    EXEC sp_addextendedproperty N'MS_Description', N'销售立项表，记录内部评审和立项信息',
        N'SCHEMA', N'dbo', N'TABLE', N'ProjectApproval'
    EXEC sp_addextendedproperty N'MS_Description', N'引用类型：1-报价，2-投标',
        N'SCHEMA', N'dbo', N'TABLE', N'ProjectApproval', N'COLUMN', N'ReferenceType'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ProjectApprovalId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项编号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ProjectApprovalCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用ID，根据ReferenceType对应QuotationId或BidId' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ReferenceId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用编号，根据ReferenceType对应QuotationCode或BidCode' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ReferenceCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'OpportunityId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'OpportunityCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CustomerId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CustomerName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ProjectName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目背景' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ProjectBackground'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户需求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CustomerDemand'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'解决方案概述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'SolutionOverview'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预估金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'EstimatedAmount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预估成本' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'EstimatedCost'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预估利润' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'EstimatedProfit'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'毛利率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ProfitMargin'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'资源需求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ResourceRequirements'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施周期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ImplementationPeriod'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险分析' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'RiskAnalysis'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险应对措施' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'RiskMitigationMeasures'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售部门意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'SalesDepartmentOpinion'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术部门意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'TechnicalDepartmentOpinion'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'财务部门意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'FinanceDepartmentOpinion'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'管理层意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ManagementOpinion'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-草稿，2-销售审批中，3-技术审批中，4-财务审批中，5-管理层审批中，6-已通过，7-已驳回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ApprovalStatus'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前审批阶段' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CurrentApprovalStage'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ApprovalFlowId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最终审批人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'FinalApproverUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最终审批人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'FinalApproverUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最终审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'FinalApprovalTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ApprovalComment'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否加急' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'IsUrgent'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'加急原因' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'UrgentReason'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'Remark'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CreateTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CreateUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CreateUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ModifyTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'

END
GO


-- ============================================================
-- 11. Contract — 合同主表
-- ============================================================
IF OBJECT_ID(N'dbo.Contract', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Contract] (
        [ContractId]               INT            IDENTITY(1,1) NOT NULL,
        [ContractCode]             NVARCHAR(50)   NOT NULL,
        [ProjectApprovalId]        INT            NOT NULL,
        [ProjectApprovalCode]      NVARCHAR(50)   NOT NULL,
        [OpportunityId]            INT            NOT NULL,
        [OpportunityCode]          NVARCHAR(50)   NOT NULL,
        [CustomerId]               INT            NOT NULL,
        [CustomerName]             NVARCHAR(200)  NOT NULL,
        [ContractName]             NVARCHAR(200)  NOT NULL,
        -- 1-销售合同 2-服务合同 3-框架协议 4-补充协议
        [ContractType]             INT            NOT NULL CONSTRAINT [DF_Cont_Type]       DEFAULT (1),
        [ContractAmount]           DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Cont_Amt]        DEFAULT (0),
        [TaxRate]                  DECIMAL(5,2)   NULL        CONSTRAINT [DF_Cont_TaxRate]  DEFAULT (0),
        [TaxAmount]                DECIMAL(18,2)  NULL        CONSTRAINT [DF_Cont_TaxAmt]   DEFAULT (0),
        [ContractAmountWithTax]    DECIMAL(18,2)  NULL        CONSTRAINT [DF_Cont_AmtTax]   DEFAULT (0),
        [Currency]                 NVARCHAR(10)   NULL        CONSTRAINT [DF_Cont_Currency] DEFAULT ('CNY'),
        [SignDate]                 DATE           NOT NULL,
        [EffectiveDate]            DATE           NOT NULL,
        [ExpiryDate]               DATE           NULL,
        [ContractPeriod]           NVARCHAR(100)  NULL,
        [PaymentMethod]            NVARCHAR(200)  NULL,
        [DeliveryRequirements]     NVARCHAR(500)  NULL,
        [QualityRequirements]      NVARCHAR(500)  NULL,
        [WarrantyPeriod]           NVARCHAR(100)  NULL,
        [ContractTerms]            NVARCHAR(MAX)  NULL,
        [SpecialAgreements]        NVARCHAR(MAX)  NULL,
        -- 1-草稿 2-审批中 3-已生效 4-履行中 5-已完成 6-已终止 7-已作废
        [ContractStatus]           INT            NOT NULL CONSTRAINT [DF_Cont_Status]      DEFAULT (1),
        -- 1-未提交 2-审批中 3-已通过 4-已驳回
        [ApprovalStatus]           INT            NOT NULL CONSTRAINT [DF_Cont_ApprSt]      DEFAULT (1),
        [ApprovalFlowId]           INT            NULL,
        [ApprovalUserId]           INT            NULL,
        [ApprovalUserName]         NVARCHAR(50)   NULL,
        [ApprovalTime]             DATETIME       NULL,
        [ApprovalComment]          NVARCHAR(500)  NULL,
        [ContractFilePath]         NVARCHAR(500)  NULL,
        [ContractScanPath]         NVARCHAR(500)  NULL,
        [IsTemplate]               BIT            NULL        CONSTRAINT [DF_Cont_IsTmpl]   DEFAULT (0),
        -- 补充协议指向父合同
        [ParentContractId]         INT            NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_Cont_CT]           DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_Contract]                  PRIMARY KEY CLUSTERED ([ContractId] ASC),
        CONSTRAINT [UK_Contract_Code]             UNIQUE NONCLUSTERED ([ContractCode] ASC),
        CONSTRAINT [CK_Contract_Type]             CHECK ([ContractType] IN (1,2,3,4)),
        CONSTRAINT [CK_Contract_Status]           CHECK ([ContractStatus] BETWEEN 1 AND 7),
        CONSTRAINT [CK_Contract_ApprSt]           CHECK ([ApprovalStatus] IN (1,2,3,4)),
        CONSTRAINT [CK_Contract_Amount]           CHECK ([ContractAmount] >= 0),
        CONSTRAINT [CK_Contract_EffDate]          CHECK ([EffectiveDate] >= [SignDate])
    )

    ALTER TABLE [dbo].[Contract]
        ADD CONSTRAINT [FK_Contract_ProjectApproval]
        FOREIGN KEY ([ProjectApprovalId]) REFERENCES [dbo].[ProjectApproval] ([ProjectApprovalId])

    ALTER TABLE [dbo].[Contract]
        ADD CONSTRAINT [FK_Contract_Opportunity]
        FOREIGN KEY ([OpportunityId]) REFERENCES [dbo].[Opportunity] ([OpportunityId])

    ALTER TABLE [dbo].[Contract]
        ADD CONSTRAINT [FK_Contract_Customer]
        FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId])

    -- 补充协议自关联
    ALTER TABLE [dbo].[Contract]
        ADD CONSTRAINT [FK_Contract_Parent]
        FOREIGN KEY ([ParentContractId]) REFERENCES [dbo].[Contract] ([ContractId])

    CREATE NONCLUSTERED INDEX [IX_Contract_ProjectApproval]
        ON [dbo].[Contract] ([ProjectApprovalId])

    CREATE NONCLUSTERED INDEX [IX_Contract_Opportunity]
        ON [dbo].[Contract] ([OpportunityId])
        INCLUDE ([ContractStatus], [ContractAmountWithTax])

    CREATE NONCLUSTERED INDEX [IX_Contract_Customer]
        ON [dbo].[Contract] ([CustomerId], [ContractStatus])
        INCLUDE ([ContractName], [ContractAmount])

    CREATE NONCLUSTERED INDEX [IX_Contract_Status]
        ON [dbo].[Contract] ([ContractStatus], [ApprovalStatus])

    CREATE NONCLUSTERED INDEX [IX_Contract_Expiry]
        ON [dbo].[Contract] ([ExpiryDate])
        WHERE [ExpiryDate] IS NOT NULL

    CREATE NONCLUSTERED INDEX [IX_Contract_Parent]
        ON [dbo].[Contract] ([ParentContractId])
        WHERE [ParentContractId] IS NOT NULL

    EXEC sp_addextendedproperty N'MS_Description', N'销售合同主表，记录正式签订的合同信息',
        N'SCHEMA', N'dbo', N'TABLE', N'Contract'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项ID，外键关联ProjectApproval表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ProjectApprovalId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ProjectApprovalCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'OpportunityId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'OpportunityCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CustomerId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CustomerName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同类型：1-销售合同，2-服务合同，3-框架协议，4-补充协议' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractType'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractAmount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'TaxRate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'TaxAmount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'含税合同金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractAmountWithTax'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'币种，默认CNY（人民币）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'Currency'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'签订日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'SignDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'生效日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'EffectiveDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到期日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ExpiryDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同期限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractPeriod'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'PaymentMethod'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'DeliveryRequirements'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质量要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'QualityRequirements'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质保期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'WarrantyPeriod'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractTerms'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'特殊约定' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'SpecialAgreements'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同状态：1-草稿，2-审批中，3-已生效，4-履行中，5-已完成，6-已终止，7-已作废' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractStatus'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-未提交，2-审批中，3-已通过，4-已驳回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalStatus'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalFlowId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalComment'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同文件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractFilePath'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同扫描件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractScanPath'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否模板合同' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'IsTemplate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'父合同ID（用于补充协议）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ParentContractId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'Remark'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CreateTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CreateUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CreateUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ModifyTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
END
GO


-- ============================================================
-- 12. ContractDetail — 合同明细表
-- ============================================================
IF OBJECT_ID(N'dbo.ContractDetail', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ContractDetail] (
        [ContractDetailId]         INT            IDENTITY(1,1) NOT NULL,
        [ContractId]               INT            NOT NULL,
        [ContractCode]             NVARCHAR(50)   NOT NULL,
        [LineNumber]               INT            NOT NULL,
        [ProductId]                INT            NOT NULL,
        [ProductCode]              NVARCHAR(50)   NOT NULL,
        [ProductName]              NVARCHAR(200)  NOT NULL,
        [ProductSpecification]     NVARCHAR(MAX)  NULL,
        [Unit]                     NVARCHAR(20)   NOT NULL,
        [Quantity]                 DECIMAL(18,4)  NOT NULL CONSTRAINT [DF_CD_Qty]       DEFAULT (0),
        [UnitPrice]                DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_CD_UnitPrice] DEFAULT (0),
        [Amount]                   DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_CD_Amount]    DEFAULT (0),
        [TaxRate]                  DECIMAL(5,2)   NULL        CONSTRAINT [DF_CD_TaxRate] DEFAULT (0),
        [TaxAmount]                DECIMAL(18,2)  NULL        CONSTRAINT [DF_CD_TaxAmt]  DEFAULT (0),
        [AmountWithTax]            DECIMAL(18,2)  NULL        CONSTRAINT [DF_CD_AmtTax]  DEFAULT (0),
        [DeliveryDate]             DATE           NULL,
        [DeliveryAddress]          NVARCHAR(500)  NULL,
        [TechnicalParameters]      NVARCHAR(MAX)  NULL,
        [QualityStandard]          NVARCHAR(500)  NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_CD_CT]        DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_ContractDetail]            PRIMARY KEY CLUSTERED ([ContractDetailId] ASC),
        CONSTRAINT [UK_CD_ContractLine]           UNIQUE NONCLUSTERED ([ContractId], [LineNumber]),
        CONSTRAINT [CK_CD_Quantity]               CHECK ([Quantity] > 0),
        CONSTRAINT [CK_CD_UnitPrice]              CHECK ([UnitPrice] >= 0)
    )

    ALTER TABLE [dbo].[ContractDetail]
        ADD CONSTRAINT [FK_ContractDetail_Contract]
        FOREIGN KEY ([ContractId]) REFERENCES [dbo].[Contract] ([ContractId])

    ALTER TABLE [dbo].[ContractDetail]
        ADD CONSTRAINT [FK_ContractDetail_Product]
        FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId])

    CREATE NONCLUSTERED INDEX [IX_ContractDetail_Contract]
        ON [dbo].[ContractDetail] ([ContractId])

    CREATE NONCLUSTERED INDEX [IX_ContractDetail_Product]
        ON [dbo].[ContractDetail] ([ProductId])

    EXEC sp_addextendedproperty N'MS_Description', N'合同明细表，记录合同的产品行项目',
        N'SCHEMA', N'dbo', N'TABLE', N'ContractDetail'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同明细ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ContractDetailId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID，外键关联Contract表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ContractId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ContractCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'行号，同一合同内唯一' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'LineNumber'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品ID，外键关联Product表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ProductId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ProductCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ProductName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品规格' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ProductSpecification'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'计量单位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'Unit'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'Quantity'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'UnitPrice'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'金额（数量×单价）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'Amount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'TaxRate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'TaxAmount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'含税金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'AmountWithTax'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'要求交货日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'DeliveryDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交货地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'DeliveryAddress'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术参数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'TechnicalParameters'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质量标准' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'QualityStandard'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'Remark'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'CreateTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'CreateUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'CreateUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ModifyTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
END
GO


-- ============================================================
-- 13. ContractChange — 合同变更表
-- ============================================================
IF OBJECT_ID(N'dbo.ContractChange', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ContractChange] (
        [ContractChangeId]         INT            IDENTITY(1,1) NOT NULL,
        [ContractChangeCode]       NVARCHAR(50)   NOT NULL,
        [ContractId]               INT            NOT NULL,
        [ContractCode]             NVARCHAR(50)   NOT NULL,
        -- 1-金额变更 2-范围变更 3-时间变更 4-条款变更 5-综合变更
        [ChangeType]               INT            NOT NULL CONSTRAINT [DF_CC_ChangeType]  DEFAULT (1),
        [ChangeReason]             NVARCHAR(MAX)  NOT NULL,
        [ChangeContent]            NVARCHAR(MAX)  NOT NULL,
        [OriginalAmount]           DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_CC_OrigAmt]     DEFAULT (0),
        -- 正数为增加，负数为减少
        [ChangeAmount]             DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_CC_ChgAmt]      DEFAULT (0),
        [NewAmount]                DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_CC_NewAmt]      DEFAULT (0),
        [OriginalExpiryDate]       DATE           NULL,
        [NewExpiryDate]            DATE           NULL,
        [ChangeAgreementPath]      NVARCHAR(500)  NULL,
        [CustomerConfirm]          BIT            NULL        CONSTRAINT [DF_CC_CustConf]  DEFAULT (0),
        [CustomerConfirmTime]      DATETIME       NULL,
        [CustomerConfirmPerson]    NVARCHAR(50)   NULL,
        [CustomerConfirmDocumentPath] NVARCHAR(500) NULL,
        -- 1-草稿 2-审批中 3-已通过 4-已驳回
        [ApprovalStatus]           INT            NOT NULL CONSTRAINT [DF_CC_ApprSt]      DEFAULT (1),
        [ApprovalFlowId]           INT            NULL,
        [ApprovalUserId]           INT            NULL,
        [ApprovalUserName]         NVARCHAR(50)   NULL,
        [ApprovalTime]             DATETIME       NULL,
        [ApprovalComment]          NVARCHAR(500)  NULL,
        -- 1-待处理 2-处理中 3-已完成 4-已取消
        [ChangeStatus]             INT            NOT NULL CONSTRAINT [DF_CC_Status]      DEFAULT (1),
        [ImplementTime]            DATETIME       NULL,
        [ImplementUserId]          INT            NULL,
        [ImplementUserName]        NVARCHAR(50)   NULL,
        [ImplementResult]          NVARCHAR(500)  NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_CC_CT]           DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_ContractChange]            PRIMARY KEY CLUSTERED ([ContractChangeId] ASC),
        CONSTRAINT [UK_ContractChange_Code]       UNIQUE NONCLUSTERED ([ContractChangeCode] ASC),
        CONSTRAINT [CK_CC_ChangeType]             CHECK ([ChangeType] IN (1,2,3,4,5)),
        CONSTRAINT [CK_CC_ApprSt]                 CHECK ([ApprovalStatus] IN (1,2,3,4)),
        CONSTRAINT [CK_CC_Status]                 CHECK ([ChangeStatus] IN (1,2,3,4)),
        -- 新金额必须等于原金额加变更金额（允许 1 分误差）
        CONSTRAINT [CK_CC_AmountConsistency]      CHECK (ABS([NewAmount] - ([OriginalAmount] + [ChangeAmount])) < 0.01)
    )

    ALTER TABLE [dbo].[ContractChange]
        ADD CONSTRAINT [FK_ContractChange_Contract]
        FOREIGN KEY ([ContractId]) REFERENCES [dbo].[Contract] ([ContractId])

    CREATE NONCLUSTERED INDEX [IX_ContractChange_Contract]
        ON [dbo].[ContractChange] ([ContractId])
        INCLUDE ([ChangeStatus], [ApprovalStatus])

    CREATE NONCLUSTERED INDEX [IX_ContractChange_Status]
        ON [dbo].[ContractChange] ([ChangeStatus], [ApprovalStatus])

    EXEC sp_addextendedproperty N'MS_Description', N'合同变更表，记录合同的变更申请和审批信息',
        N'SCHEMA', N'dbo', N'TABLE', N'ContractChange'
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同变更ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ContractChangeId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更单号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ContractChangeCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID，外键关联Contract表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ContractId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ContractCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更类型：1-金额变更，2-范围变更，3-时间变更，4-条款变更，5-综合变更' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeType'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更原因' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeReason'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeContent'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原合同金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'OriginalAmount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更金额，正数为增加，负数为减少' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeAmount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新合同金额（原合同金额+变更金额）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'NewAmount'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原到期日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'OriginalExpiryDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新到期日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'NewExpiryDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更协议文件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeAgreementPath'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户是否确认' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CustomerConfirm'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CustomerConfirmTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CustomerConfirmPerson'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认文件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CustomerConfirmDocumentPath'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-草稿，2-审批中，3-已通过，4-已驳回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalStatus'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalFlowId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalComment'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更状态：1-待处理，2-处理中，3-已完成，4-已取消' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeStatus'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ImplementTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ImplementUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ImplementUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施结果' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ImplementResult'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'Remark'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CreateTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CreateUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CreateUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ModifyTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'

END
GO


-- ============================================================
-- 14. ContractHandover — 合同交底表
-- ============================================================
IF OBJECT_ID(N'dbo.ContractHandover', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ContractHandover] (
        [ContractHandoverId]       INT            IDENTITY(1,1) NOT NULL,
        [ContractId]               INT            NOT NULL,
        [ContractCode]             NVARCHAR(50)   NOT NULL,
        [HandoverCode]             NVARCHAR(50)   NOT NULL,
        -- 1-首次交底 2-变更交底
        [HandoverType]             INT            NOT NULL CONSTRAINT [DF_CH_Type]          DEFAULT (1),
        [HandoverDate]             DATETIME       NOT NULL,
        [HandoverLocation]         NVARCHAR(200)  NULL,
        [SalesRepresentativeId]    INT            NOT NULL,
        [SalesRepresentativeName]  NVARCHAR(50)   NOT NULL,
        [DeliveryTeamLeaderId]     INT            NOT NULL,
        [DeliveryTeamLeaderName]   NVARCHAR(50)   NOT NULL,
        [Participants]             NVARCHAR(MAX)  NULL,
        [KeyContractTerms]         NVARCHAR(MAX)  NULL,
        [CustomerSpecialRequirements] NVARCHAR(MAX) NULL,
        [TechnicalRequirements]    NVARCHAR(MAX)  NULL,
        [DeliveryRequirements]     NVARCHAR(MAX)  NULL,
        [QualityRequirements]      NVARCHAR(MAX)  NULL,
        [PaymentTerms]             NVARCHAR(500)  NULL,
        [RiskPoints]               NVARCHAR(MAX)  NULL,
        [RiskMitigationMeasures]   NVARCHAR(MAX)  NULL,
        [ImportantNotes]           NVARCHAR(MAX)  NULL,
        -- JSON 格式存储多个附件信息
        [Attachments]              NVARCHAR(MAX)  NULL,
        -- 1-已安排 2-进行中 3-已完成 4-已取消
        [HandoverStatus]           INT            NOT NULL CONSTRAINT [DF_CH_Status]        DEFAULT (1),
        [CompletionTime]           DATETIME       NULL,
        [SalesSignOff]             BIT            NULL        CONSTRAINT [DF_CH_SalesSign]   DEFAULT (0),
        [SalesSignOffTime]         DATETIME       NULL,
        [DeliverySignOff]          BIT            NULL        CONSTRAINT [DF_CH_DlvSign]     DEFAULT (0),
        [DeliverySignOffTime]      DATETIME       NULL,
        [HandoverEvaluation]       NVARCHAR(500)  NULL,
        [FollowUpActions]          NVARCHAR(500)  NULL,
        [Remark]                   NVARCHAR(500)  NULL,
        [CreateTime]               DATETIME       NOT NULL CONSTRAINT [DF_CH_CT]             DEFAULT (GETDATE()),
        [CreateUserId]             INT            NOT NULL,
        [CreateUserName]           NVARCHAR(50)   NOT NULL,
        [CreateOrganizationId]     INT            NOT NULL,
        [CreateOrganizationName]   NVARCHAR(100)  NOT NULL,
        [ModifyTime]               DATETIME       NULL,
        [ModifyUserId]             INT            NULL,
        [ModifyUserName]           NVARCHAR(50)   NULL,
        [ModifyOrganizationId]     INT            NULL,
        [ModifyOrganizationName]   NVARCHAR(100)  NULL,
        CONSTRAINT [PK_ContractHandover]          PRIMARY KEY CLUSTERED ([ContractHandoverId] ASC),
        CONSTRAINT [UK_ContractHandover_Code]     UNIQUE NONCLUSTERED ([HandoverCode] ASC),
        CONSTRAINT [CK_CH_Type]                   CHECK ([HandoverType] IN (1,2)),
        CONSTRAINT [CK_CH_Status]                 CHECK ([HandoverStatus] IN (1,2,3,4))
    )

    ALTER TABLE [dbo].[ContractHandover]
        ADD CONSTRAINT [FK_CH_Contract]
        FOREIGN KEY ([ContractId]) REFERENCES [dbo].[Contract] ([ContractId])

    ALTER TABLE [dbo].[ContractHandover]
        ADD CONSTRAINT [FK_CH_SalesRep]
        FOREIGN KEY ([SalesRepresentativeId]) REFERENCES [dbo].[UserInfo] ([UserId])

    ALTER TABLE [dbo].[ContractHandover]
        ADD CONSTRAINT [FK_CH_DeliveryLeader]
        FOREIGN KEY ([DeliveryTeamLeaderId]) REFERENCES [dbo].[UserInfo] ([UserId])

    CREATE NONCLUSTERED INDEX [IX_CH_Contract]
        ON [dbo].[ContractHandover] ([ContractId])
        INCLUDE ([HandoverStatus])

    CREATE NONCLUSTERED INDEX [IX_CH_SalesRep]
        ON [dbo].[ContractHandover] ([SalesRepresentativeId])

    CREATE NONCLUSTERED INDEX [IX_CH_DeliveryLeader]
        ON [dbo].[ContractHandover] ([DeliveryTeamLeaderId])

    EXEC sp_addextendedproperty N'MS_Description', N'合同交底表，记录合同签署后销售向交付团队的交底信息',
        N'SCHEMA', N'dbo', N'TABLE', N'ContractHandover'

        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同交底ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ContractHandoverId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID，外键关联Contract表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ContractId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ContractCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底单号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverCode'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底类型：1-首次交底，2-变更交底' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverType'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverDate'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底地点' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverLocation'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售代表ID，外键关联User表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'SalesRepresentativeId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售代表姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'SalesRepresentativeName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付团队负责人ID，外键关联User表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'DeliveryTeamLeaderId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付团队负责人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'DeliveryTeamLeaderName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参与人员，可存储多个人员信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'Participants'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关键合同条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'KeyContractTerms'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户特殊要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CustomerSpecialRequirements'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'TechnicalRequirements'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'DeliveryRequirements'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质量要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'QualityRequirements'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'PaymentTerms'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险点' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'RiskPoints'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险应对措施' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'RiskMitigationMeasures'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'重要注意事项' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ImportantNotes'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'附件路径，JSON格式存储多个附件信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'Attachments'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底状态：1-已安排，2-进行中，3-已完成，4-已取消' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverStatus'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'完成时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CompletionTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售签字确认' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'SalesSignOff'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售签字时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'SalesSignOffTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付签字确认' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'DeliverySignOff'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付签字时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'DeliverySignOffTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底评价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverEvaluation'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'后续行动' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'FollowUpActions'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'Remark'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CreateTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CreateUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CreateUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ModifyTime'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
END
GO


-- ============================================================
-- 完成提示
-- ============================================================
PRINT '数据库初始化完成。共创建 14 张表'
GO