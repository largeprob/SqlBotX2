 

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
        N'SCHEMA', N'dbo', N'TABLE', N'Organization'
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
        -- 1-销售部 2-技术部 3-财务部 4-人事部 5-行政部 6-其他
        [DepartmentType]           INT            NOT NULL CONSTRAINT [DF_Dept_Type]   DEFAULT (1),
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
        CONSTRAINT [CK_Department_Type]           CHECK ([DepartmentType] IN (1,2,3,4,5,6)),
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
        N'SCHEMA', N'dbo', N'TABLE', N'Department'
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
        N'SCHEMA', N'dbo', N'TABLE', N'UserInfo'
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
        N'SCHEMA', N'dbo', N'TABLE', N'Product'
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
        N'SCHEMA', N'dbo', N'TABLE', N'Opportunity'
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
END
GO


-- ============================================================
-- 完成提示
-- ============================================================
PRINT '数据库初始化完成。共创建 14 张表'
GO