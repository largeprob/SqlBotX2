-- ============================================================
-- 销售管理系统数据库表结构
-- 采用大驼峰命名规则
-- 包含所有表注释和字段注释
-- ============================================================

-- 1. 用户表 - 系统用户信息
CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,                             -- 用户ID（主键）
    UserCode NVARCHAR(50) NOT NULL,                                   -- 用户编码
    UserName NVARCHAR(100) NOT NULL,                                  -- 用户姓名
    LoginName NVARCHAR(50) NOT NULL,                                  -- 登录名
    PasswordHash NVARCHAR(200) NOT NULL,                              -- 密码哈希
    Email NVARCHAR(100),                                              -- 邮箱
    Phone NVARCHAR(20),                                               -- 电话
    DepartmentId INT,                                                 -- 部门ID
    DepartmentName NVARCHAR(100),                                     -- 部门名称
    Position NVARCHAR(50),                                            -- 职位
    UserType INT NOT NULL DEFAULT 1,                                  -- 用户类型（1-销售，2-管理员，3-审核人，4-交付人员）
    Status INT NOT NULL DEFAULT 1,                                    -- 状态（1-启用，0-停用）
    LastLoginTime DATETIME,                                           -- 最后登录时间
    Remark NVARCHAR(500),                                             -- 备注
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                   -- 创建时间
    CreateUserId INT NOT NULL,                                        -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                             -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                    -- 创建人所在组织名称
    ModifyTime DATETIME,                                              -- 修改时间
    ModifyUserId INT,                                                 -- 修改人ID
    ModifyUserName NVARCHAR(50),                                      -- 修改人姓名
    ModifyOrganizationId INT,                                         -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                              -- 修改人所在组织名称
);
GO

-- 用户表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户信息表，存储系统用户数据', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'UserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户编码，唯一标识', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'UserCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'UserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登录名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'LoginName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码哈希值', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'PasswordHash';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮箱', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'Email';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'电话', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'Phone';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'DepartmentId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'DepartmentName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'职位', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'Position';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户类型：1-销售，2-管理员，3-审核人，4-交付人员', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'UserType';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：1-启用，0-停用', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'Status';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后登录时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'LastLoginTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'User', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- 2. 产品表 - 存储所有可销售的产品信息
CREATE TABLE Product (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,                           -- 产品ID（主键）
    ProductCode NVARCHAR(50) NOT NULL,                                 -- 产品编码
    ProductName NVARCHAR(200) NOT NULL,                                -- 产品名称
    ProductType NVARCHAR(50) NOT NULL,                                 -- 产品类型
    ProductCategory NVARCHAR(50),                                      -- 产品分类
    ProductSpecification NVARCHAR(MAX),                                -- 产品规格
    Unit NVARCHAR(20) NOT NULL,                                        -- 计量单位
    UnitPrice DECIMAL(18,2) NOT NULL DEFAULT 0,                        -- 标准单价
    CostPrice DECIMAL(18,2) NOT NULL DEFAULT 0,                        -- 成本价
    MinimumStock INT DEFAULT 0,                                        -- 最低库存
    CurrentStock INT DEFAULT 0,                                        -- 当前库存
    Status INT NOT NULL DEFAULT 1,                                     -- 状态（1-启用，0-停用）
    Remark NVARCHAR(500),                                              -- 备注
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                    -- 创建时间
    CreateUserId INT NOT NULL,                                         -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                              -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                 -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                     -- 创建人所在组织名称
    ModifyTime DATETIME,                                               -- 修改时间
    ModifyUserId INT,                                                  -- 修改人ID
    ModifyUserName NVARCHAR(50),                                       -- 修改人姓名
    ModifyOrganizationId INT,                                          -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                               -- 修改人所在组织名称
);
GO

-- 产品表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品信息表，存储所有可销售的产品数据', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ProductId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品编码，唯一标识', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ProductCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ProductName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品类型', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ProductType';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品分类', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ProductCategory';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品规格描述', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ProductSpecification';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'计量单位（如：个、台、套）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'Unit';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标准单价', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'UnitPrice';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'成本价', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'CostPrice';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最低库存', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'MinimumStock';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前库存', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'CurrentStock';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：1-启用，0-停用', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'Status';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Product', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- 3. 客户表 - 统一管理公海客户和正式客户
CREATE TABLE Customer (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,                          -- 客户ID（主键）
    CustomerCode NVARCHAR(50) NOT NULL,                                -- 客户编码
    CustomerName NVARCHAR(200) NOT NULL,                               -- 客户名称
    CustomerType INT NOT NULL DEFAULT 1,                               -- 客户类型（1-企业，2-个人，3-政府，4-其他）
    CustomerSource NVARCHAR(100),                                      -- 客户来源
    Industry NVARCHAR(100),                                            -- 所属行业
    ContactPerson NVARCHAR(50),                                        -- 联系人
    ContactPhone NVARCHAR(50),                                         -- 联系电话
    ContactEmail NVARCHAR(100),                                        -- 联系邮箱
    Address NVARCHAR(500),                                             -- 地址
    CustomerLevel INT DEFAULT 1,                                       -- 客户等级（1-A级，2-B级，3-C级）
    CreditStatus INT DEFAULT 1,                                        -- 信用状态（1-良好，2-一般，3-受限）
    CustomerStatus INT NOT NULL DEFAULT 1,                             -- 客户状态（1-公海客户，2-已分配待跟进，3-正式客户，4-流失客户）
    IsPublic BIT NOT NULL DEFAULT 1,                                   -- 是否公海客户（1-是，0-否）
    OwnerUserId INT,                                                   -- 负责人用户ID
    OwnerUserName NVARCHAR(50),                                        -- 负责人姓名
    OwnerOrganizationId INT,                                           -- 负责人组织ID
    OwnerOrganizationName NVARCHAR(100),                               -- 负责人组织名称
    RecentContactTime DATETIME,                                        -- 最近联系时间
    TotalOrderAmount DECIMAL(18,2) DEFAULT 0,                          -- 累计订单金额
    TotalOrderCount INT DEFAULT 0,                                     -- 累计订单数量
    LastOrderTime DATETIME,                                            -- 最后下单时间
    Remark NVARCHAR(500),                                              -- 备注
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                    -- 创建时间
    CreateUserId INT NOT NULL,                                         -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                              -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                 -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                     -- 创建人所在组织名称
    ModifyTime DATETIME,                                               -- 修改时间
    ModifyUserId INT,                                                  -- 修改人ID
    ModifyUserName NVARCHAR(50),                                       -- 修改人姓名
    ModifyOrganizationId INT,                                          -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                               -- 修改人所在组织名称
);
GO

-- 客户表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户信息表，统一管理公海客户和正式客户', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CustomerId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户编码，唯一标识', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CustomerCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CustomerName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户类型：1-企业，2-个人，3-政府，4-其他', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CustomerType';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户来源（如：展会、网络搜索、老客户介绍等）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CustomerSource';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属行业', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'Industry';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系人', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'ContactPerson';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系电话', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'ContactPhone';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系邮箱', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'ContactEmail';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地址', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'Address';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户等级：1-A级（重要），2-B级（一般），3-C级（潜在）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CustomerLevel';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'信用状态：1-良好，2-一般，3-受限', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CreditStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户状态：1-公海客户，2-已分配待跟进，3-正式客户，4-流失客户', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CustomerStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否公海客户：1-是，0-否', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'IsPublic';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人用户ID，如果为空则表示在公海中', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'OwnerUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'OwnerUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'OwnerOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'OwnerOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近联系时间，用于判断客户是否活跃', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'RecentContactTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'累计订单金额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'TotalOrderAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'累计订单数量', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'TotalOrderCount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后下单时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'LastOrderTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Customer', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- 4. 商机登记表 - 销售机会登记
CREATE TABLE Opportunity (
    OpportunityId INT IDENTITY(1,1) PRIMARY KEY,                       -- 商机ID（主键）
    OpportunityCode NVARCHAR(50) NOT NULL,                             -- 商机编号
    OpportunityName NVARCHAR(200) NOT NULL,                            -- 商机名称
    CustomerId INT NOT NULL,                                           -- 客户ID（外键）
    CustomerName NVARCHAR(200) NOT NULL,                               -- 客户名称（冗余）
    CustomerType INT NOT NULL,                                         -- 客户类型（冗余）
    OpportunityType INT NOT NULL DEFAULT 1,                            -- 商机类型（1-新客户商机，2-老客户商机）
    ProjectBackground NVARCHAR(MAX),                                   -- 项目背景
    CustomerNeed NVARCHAR(MAX),                                        -- 客户需求
    ExpectedAmount DECIMAL(18,2) NOT NULL DEFAULT 0,                   -- 预计金额
    ExpectedClosingDate DATE,                                          -- 预计成交日期
    WinProbability INT NOT NULL DEFAULT 30,                            -- 赢单概率（0-100%）
    CompetitorInfo NVARCHAR(500),                                      -- 竞争对手信息
    OpportunityStage INT NOT NULL DEFAULT 1,                           -- 商机阶段（1-初步接触，2-需求分析，3-方案制定，4-报价中，5-谈判中，6-赢单，7-丢单）
    NeedBid BIT NOT NULL DEFAULT 0,                                    -- 是否需要投标（0-否，1-是）
    BidProjectCode NVARCHAR(50),                                       -- 招标项目编号
    SalesOwnerUserId INT NOT NULL,                                     -- 销售负责人用户ID
    SalesOwnerUserName NVARCHAR(50) NOT NULL,                          -- 销售负责人姓名
    SalesOwnerOrganizationId INT NOT NULL,                             -- 销售负责人组织ID
    SalesOwnerOrganizationName NVARCHAR(100) NOT NULL,                 -- 销售负责人组织名称
    CoSalesUserIds NVARCHAR(500),                                      -- 协同销售人员ID列表
    CoSalesUserNames NVARCHAR(500),                                    -- 协同销售人员姓名列表
    CurrentTask NVARCHAR(200),                                         -- 当前任务
    NextStep NVARCHAR(200),                                            -- 下一步计划
    Remark NVARCHAR(500),                                              -- 备注
    OpportunityStatus INT NOT NULL DEFAULT 1,                          -- 商机状态（1-进行中，2-已关闭，3-已取消）
    CloseReason NVARCHAR(200),                                         -- 关闭原因
    CloseTime DATETIME,                                                -- 关闭时间
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                    -- 创建时间
    CreateUserId INT NOT NULL,                                         -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                              -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                 -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                     -- 创建人所在组织名称
    ModifyTime DATETIME,                                               -- 修改时间
    ModifyUserId INT,                                                  -- 修改人ID
    ModifyUserName NVARCHAR(50),                                       -- 修改人姓名
    ModifyOrganizationId INT,                                          -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                               -- 修改人所在组织名称
);
GO

-- 商机表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机信息表，记录销售机会的全过程', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'OpportunityId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号，唯一标识', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'OpportunityCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'OpportunityName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CustomerId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CustomerName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户类型', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CustomerType';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机类型：1-新客户商机，2-老客户商机', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'OpportunityType';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目背景', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'ProjectBackground';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户需求', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CustomerNeed';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预计金额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'ExpectedAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预计成交日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'ExpectedClosingDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'赢单概率，取值范围0-100', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'WinProbability';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'竞争对手信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CompetitorInfo';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机阶段：1-初步接触，2-需求分析，3-方案制定，4-报价中，5-谈判中，6-赢单，7-丢单', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'OpportunityStage';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否需要投标：0-否（走报价流程），1-是（走投标流程）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'NeedBid';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标项目编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'BidProjectCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人用户ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'SalesOwnerUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'SalesOwnerUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'SalesOwnerOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'SalesOwnerOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'协同销售人员ID列表，多个ID用逗号分隔', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CoSalesUserIds';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'协同销售人员姓名列表，多个姓名用逗号分隔', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CoSalesUserNames';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前任务', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CurrentTask';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'下一步计划', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'NextStep';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机状态：1-进行中，2-已关闭，3-已取消', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'OpportunityStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关闭原因', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CloseReason';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关闭时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CloseTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Opportunity', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- 5. 销售报价表 - 普通项目的报价
CREATE TABLE Quotation (
    QuotationId INT IDENTITY(1,1) PRIMARY KEY,                         -- 报价ID（主键）
    QuotationCode NVARCHAR(50) NOT NULL,                               -- 报价单号
    OpportunityId INT NOT NULL,                                        -- 商机ID（外键）
    OpportunityCode NVARCHAR(50) NOT NULL,                             -- 商机编号（冗余）
    CustomerId INT NOT NULL,                                           -- 客户ID（外键）
    CustomerName NVARCHAR(200) NOT NULL,                               -- 客户名称（冗余）
    QuotationDate DATE NOT NULL,                                       -- 报价日期
    ValidUntilDate DATE NOT NULL,                                      -- 有效期至
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,                      -- 报价总金额
    TaxRate DECIMAL(5,2) DEFAULT 0,                                    -- 税率
    TaxAmount DECIMAL(18,2) DEFAULT 0,                                 -- 税额
    TotalAmountWithTax DECIMAL(18,2) DEFAULT 0,                        -- 含税总金额
    Currency NVARCHAR(10) DEFAULT 'CNY',                               -- 币种
    PaymentTerms NVARCHAR(500),                                        -- 付款条款
    DeliveryTerms NVARCHAR(500),                                       -- 交货条款
    WarrantyTerms NVARCHAR(500),                                       -- 保修条款
    SpecialTerms NVARCHAR(500),                                        -- 特殊条款
    QuotationStatus INT NOT NULL DEFAULT 1,                            -- 报价状态（1-草稿，2-已提交，3-客户确认中，4-客户已确认，5-客户拒绝，6-已过期）
    ApprovalStatus INT NOT NULL DEFAULT 1,                             -- 审批状态（1-未提交，2-审批中，3-已通过，4-已驳回）
    ApprovalFlowId INT,                                                -- 审批流程ID
    ApprovalUserId INT,                                                -- 审批人ID
    ApprovalUserName NVARCHAR(50),                                     -- 审批人姓名
    ApprovalTime DATETIME,                                             -- 审批时间
    ApprovalComment NVARCHAR(500),                                     -- 审批意见
    CustomerFeedback NVARCHAR(500),                                    -- 客户反馈
    CustomerConfirmTime DATETIME,                                      -- 客户确认时间
    NextFollowUpTime DATETIME,                                         -- 下次跟进时间
    Remark NVARCHAR(500),                                              -- 备注
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                    -- 创建时间
    CreateUserId INT NOT NULL,                                         -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                              -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                 -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                     -- 创建人所在组织名称
    ModifyTime DATETIME,                                               -- 修改时间
    ModifyUserId INT,                                                  -- 修改人ID
    ModifyUserName NVARCHAR(50),                                       -- 修改人姓名
    ModifyOrganizationId INT,                                          -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                               -- 修改人所在组织名称
);
GO

-- 销售报价表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售报价表，记录普通项目的报价信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'QuotationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价单号，唯一标识', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'QuotationCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'OpportunityId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'OpportunityCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'CustomerId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'CustomerName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'QuotationDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'有效期至', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ValidUntilDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价总金额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'TotalAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税率', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'TaxRate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'TaxAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'含税总金额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'TotalAmountWithTax';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'币种，默认CNY（人民币）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'Currency';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款条款', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'PaymentTerms';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交货条款', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'DeliveryTerms';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保修条款', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'WarrantyTerms';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'特殊条款', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'SpecialTerms';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价状态：1-草稿，2-已提交，3-客户确认中，4-客户已确认，5-客户拒绝，6-已过期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'QuotationStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-未提交，2-审批中，3-已通过，4-已驳回', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ApprovalStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ApprovalFlowId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ApprovalUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ApprovalUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ApprovalTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ApprovalComment';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户反馈', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'CustomerFeedback';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'CustomerConfirmTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'下次跟进时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'NextFollowUpTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Quotation', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- 6. 销售投标表 - 招投标项目的投标管理
CREATE TABLE Bid (
    BidId INT IDENTITY(1,1) PRIMARY KEY,                               -- 投标ID（主键）
    BidCode NVARCHAR(50) NOT NULL,                                     -- 投标编号
    OpportunityId INT NOT NULL,                                        -- 商机ID（外键）
    OpportunityCode NVARCHAR(50) NOT NULL,                             -- 商机编号（冗余）
    CustomerId INT NOT NULL,                                           -- 客户ID（外键）
    CustomerName NVARCHAR(200) NOT NULL,                               -- 客户名称（冗余）
    BidProjectName NVARCHAR(200) NOT NULL,                             -- 招标项目名称
    BidProjectCode NVARCHAR(50) NOT NULL,                              -- 招标项目编号
    TenderOrganization NVARCHAR(200) NOT NULL,                         -- 招标单位
    TenderAddress NVARCHAR(500),                                       -- 招标地址
    BidOpeningDate DATETIME NOT NULL,                                  -- 开标日期
    BidSubmissionDeadline DATETIME NOT NULL,                           -- 投标截止日期
    BidBondAmount DECIMAL(18,2) DEFAULT 0,                             -- 投标保证金金额
    BidBondPaid BIT DEFAULT 0,                                         -- 投标保证金是否已支付
    BidBondPaidTime DATETIME,                                          -- 保证金支付时间
    PerformanceBondRequired BIT DEFAULT 0,                             -- 是否需要履约保证金
    PerformanceBondRate DECIMAL(5,2) DEFAULT 0,                        -- 履约保证金比例
    TechnicalScore DECIMAL(5,2),                                       -- 技术得分
    CommercialScore DECIMAL(5,2),                                      -- 商务得分
    TotalScore DECIMAL(5,2),                                           -- 总分
    Ranking INT,                                                       -- 排名
    Competitors NVARCHAR(500),                                         -- 竞争对手信息
    OurAdvantages NVARCHAR(500),                                       -- 我方优势
    OurDisadvantages NVARCHAR(500),                                    -- 我方劣势
    BidStatus INT NOT NULL DEFAULT 1,                                  -- 投标状态（1-准备中，2-已提交，3-已开标，4-中标，5-未中标，6-废标）
    ApprovalStatus INT NOT NULL DEFAULT 1,                             -- 审批状态（1-未提交，2-审批中，3-已通过，4-已驳回）
    ApprovalFlowId INT,                                                -- 审批流程ID
    ApprovalUserId INT,                                                -- 审批人ID
    ApprovalUserName NVARCHAR(50),                                     -- 审批人姓名
    ApprovalTime DATETIME,                                             -- 审批时间
    ApprovalComment NVARCHAR(500),                                     -- 审批意见
    WinNoticeTime DATETIME,                                            -- 中标通知书时间
    WinNoticeNumber NVARCHAR(50),                                      -- 中标通知书编号
    BidDocumentPath NVARCHAR(500),                                     -- 投标文件路径
    OtherAttachments NVARCHAR(500),                                    -- 其他附件路径
    Remark NVARCHAR(500),                                              -- 备注
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                    -- 创建时间
    CreateUserId INT NOT NULL,                                         -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                              -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                 -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                     -- 创建人所在组织名称
    ModifyTime DATETIME,                                               -- 修改时间
    ModifyUserId INT,                                                  -- 修改人ID
    ModifyUserName NVARCHAR(50),                                       -- 修改人姓名
    ModifyOrganizationId INT,                                          -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                               -- 修改人所在组织名称
);
GO

-- 销售投标表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售投标表，记录招投标项目的投标信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标编号，唯一标识', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'OpportunityId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'OpportunityCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'CustomerId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'CustomerName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标项目名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidProjectName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标项目编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidProjectCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标单位', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'TenderOrganization';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标地址', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'TenderAddress';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开标日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidOpeningDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标截止日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidSubmissionDeadline';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标保证金金额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidBondAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标保证金是否已支付', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidBondPaid';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保证金支付时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidBondPaidTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否需要履约保证金', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'PerformanceBondRequired';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'履约保证金比例', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'PerformanceBondRate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术得分', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'TechnicalScore';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商务得分', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'CommercialScore';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'总分', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'TotalScore';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'Ranking';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'竞争对手信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'Competitors';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'我方优势', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'OurAdvantages';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'我方劣势', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'OurDisadvantages';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标状态：1-准备中，2-已提交，3-已开标，4-中标，5-未中标，6-废标', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-未提交，2-审批中，3-已通过，4-已驳回', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ApprovalStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ApprovalFlowId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ApprovalUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ApprovalUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ApprovalTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ApprovalComment';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'中标通知书时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'WinNoticeTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'中标通知书编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'WinNoticeNumber';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标文件路径', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'BidDocumentPath';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'其他附件路径', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'OtherAttachments';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Bid', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- 7. 销售立项表 - 内部评审和立项
CREATE TABLE ProjectApproval (
    ProjectApprovalId INT IDENTITY(1,1) PRIMARY KEY,                   -- 立项ID（主键）
    ProjectApprovalCode NVARCHAR(50) NOT NULL,                         -- 立项编号
    ReferenceType INT NOT NULL,                                        -- 引用类型（1-报价，2-投标）
    ReferenceId INT NOT NULL,                                          -- 引用ID（QuotationId或BidId）
    ReferenceCode NVARCHAR(50) NOT NULL,                               -- 引用编号（QuotationCode或BidCode）
    OpportunityId INT NOT NULL,                                        -- 商机ID（外键）
    OpportunityCode NVARCHAR(50) NOT NULL,                             -- 商机编号（冗余）
    CustomerId INT NOT NULL,                                           -- 客户ID（外键）
    CustomerName NVARCHAR(200) NOT NULL,                               -- 客户名称（冗余）
    ProjectName NVARCHAR(200) NOT NULL,                                -- 项目名称
    ProjectBackground NVARCHAR(MAX),                                   -- 项目背景
    CustomerDemand NVARCHAR(MAX),                                      -- 客户需求
    SolutionOverview NVARCHAR(MAX),                                    -- 解决方案概述
    EstimatedAmount DECIMAL(18,2) NOT NULL DEFAULT 0,                  -- 预估金额
    EstimatedCost DECIMAL(18,2) NOT NULL DEFAULT 0,                    -- 预估成本
    EstimatedProfit DECIMAL(18,2) NOT NULL DEFAULT 0,                  -- 预估利润
    ProfitMargin DECIMAL(5,2) DEFAULT 0,                               -- 毛利率
    ResourceRequirements NVARCHAR(500),                                -- 资源需求
    ImplementationPeriod NVARCHAR(100),                                -- 实施周期
    RiskAnalysis NVARCHAR(MAX),                                        -- 风险分析
    RiskMitigationMeasures NVARCHAR(MAX),                              -- 风险应对措施
    SalesDepartmentOpinion NVARCHAR(500),                              -- 销售部门意见
    TechnicalDepartmentOpinion NVARCHAR(500),                          -- 技术部门意见
    FinanceDepartmentOpinion NVARCHAR(500),                            -- 财务部门意见
    ManagementOpinion NVARCHAR(500),                                   -- 管理层意见
    ApprovalStatus INT NOT NULL DEFAULT 1,                             -- 审批状态（1-草稿，2-销售审批中，3-技术审批中，4-财务审批中，5-管理层审批中，6-已通过，7-已驳回）
    CurrentApprovalStage INT DEFAULT 1,                                -- 当前审批阶段
    ApprovalFlowId INT,                                                -- 审批流程ID
    FinalApproverUserId INT,                                           -- 最终审批人ID
    FinalApproverUserName NVARCHAR(50),                                -- 最终审批人姓名
    FinalApprovalTime DATETIME,                                        -- 最终审批时间
    ApprovalComment NVARCHAR(500),                                     -- 审批意见
    IsUrgent BIT DEFAULT 0,                                            -- 是否加急
    UrgentReason NVARCHAR(200),                                        -- 加急原因
    Remark NVARCHAR(500),                                              -- 备注
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                    -- 创建时间
    CreateUserId INT NOT NULL,                                         -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                              -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                 -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                     -- 创建人所在组织名称
    ModifyTime DATETIME,                                               -- 修改时间
    ModifyUserId INT,                                                  -- 修改人ID
    ModifyUserName NVARCHAR(50),                                       -- 修改人姓名
    ModifyOrganizationId INT,                                          -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                               -- 修改人所在组织名称
);
GO

-- 销售立项表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售立项表，记录内部评审和立项信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ProjectApprovalId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项编号，唯一标识', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ProjectApprovalCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用类型：1-报价，2-投标', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ReferenceType';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用ID，根据ReferenceType对应QuotationId或BidId', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ReferenceId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用编号，根据ReferenceType对应QuotationCode或BidCode', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ReferenceCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'OpportunityId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'OpportunityCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'CustomerId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'CustomerName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ProjectName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目背景', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ProjectBackground';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户需求', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'CustomerDemand';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'解决方案概述', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'SolutionOverview';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预估金额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'EstimatedAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预估成本', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'EstimatedCost';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预估利润', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'EstimatedProfit';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'毛利率', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ProfitMargin';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'资源需求', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ResourceRequirements';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施周期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ImplementationPeriod';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险分析', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'RiskAnalysis';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险应对措施', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'RiskMitigationMeasures';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售部门意见', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'SalesDepartmentOpinion';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术部门意见', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'TechnicalDepartmentOpinion';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'财务部门意见', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'FinanceDepartmentOpinion';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'管理层意见', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ManagementOpinion';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-草稿，2-销售审批中，3-技术审批中，4-财务审批中，5-管理层审批中，6-已通过，7-已驳回', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ApprovalStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前审批阶段', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'CurrentApprovalStage';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ApprovalFlowId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最终审批人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'FinalApproverUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最终审批人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'FinalApproverUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最终审批时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'FinalApprovalTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ApprovalComment';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否加急', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'IsUrgent';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'加急原因', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'UrgentReason';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ProjectApproval', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- 8. 销售合同表 - 正式合同
CREATE TABLE Contract (
    ContractId INT IDENTITY(1,1) PRIMARY KEY,                          -- 合同ID（主键）
    ContractCode NVARCHAR(50) NOT NULL,                                -- 合同编号
    ProjectApprovalId INT NOT NULL,                                    -- 立项ID（外键）
    ProjectApprovalCode NVARCHAR(50) NOT NULL,                         -- 立项编号（冗余）
    OpportunityId INT NOT NULL,                                        -- 商机ID（外键）
    OpportunityCode NVARCHAR(50) NOT NULL,                             -- 商机编号（冗余）
    CustomerId INT NOT NULL,                                           -- 客户ID（外键）
    CustomerName NVARCHAR(200) NOT NULL,                               -- 客户名称（冗余）
    ContractName NVARCHAR(200) NOT NULL,                               -- 合同名称
    ContractType INT NOT NULL DEFAULT 1,                               -- 合同类型（1-销售合同，2-服务合同，3-框架协议，4-补充协议）
    ContractAmount DECIMAL(18,2) NOT NULL DEFAULT 0,                   -- 合同金额
    TaxRate DECIMAL(5,2) DEFAULT 0,                                    -- 税率
    TaxAmount DECIMAL(18,2) DEFAULT 0,                                 -- 税额
    ContractAmountWithTax DECIMAL(18,2) DEFAULT 0,                     -- 含税合同金额
    Currency NVARCHAR(10) DEFAULT 'CNY',                               -- 币种
    SignDate DATE NOT NULL,                                            -- 签订日期
    EffectiveDate DATE NOT NULL,                                       -- 生效日期
    ExpiryDate DATE,                                                   -- 到期日期
    ContractPeriod NVARCHAR(100),                                      -- 合同期限
    PaymentMethod NVARCHAR(200),                                       -- 付款方式
    DeliveryRequirements NVARCHAR(500),                                -- 交付要求
    QualityRequirements NVARCHAR(500),                                 -- 质量要求
    WarrantyPeriod NVARCHAR(100),                                      -- 质保期
    ContractTerms NVARCHAR(MAX),                                       -- 合同条款
    SpecialAgreements NVARCHAR(MAX),                                   -- 特殊约定
    ContractStatus INT NOT NULL DEFAULT 1,                             -- 合同状态（1-草稿，2-审批中，3-已生效，4-履行中，5-已完成，6-已终止，7-已作废）
    ApprovalStatus INT NOT NULL DEFAULT 1,                             -- 审批状态（1-未提交，2-审批中，3-已通过，4-已驳回）
    ApprovalFlowId INT,                                                -- 审批流程ID
    ApprovalUserId INT,                                                -- 审批人ID
    ApprovalUserName NVARCHAR(50),                                     -- 审批人姓名
    ApprovalTime DATETIME,                                             -- 审批时间
    ApprovalComment NVARCHAR(500),                                     -- 审批意见
    ContractFilePath NVARCHAR(500),                                    -- 合同文件路径
    ContractScanPath NVARCHAR(500),                                    -- 合同扫描件路径
    IsTemplate BIT DEFAULT 0,                                          -- 是否模板合同
    ParentContractId INT,                                              -- 父合同ID（用于补充协议）
    Remark NVARCHAR(500),                                              -- 备注
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                    -- 创建时间
    CreateUserId INT NOT NULL,                                         -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                              -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                 -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                     -- 创建人所在组织名称
    ModifyTime DATETIME,                                               -- 修改时间
    ModifyUserId INT,                                                  -- 修改人ID
    ModifyUserName NVARCHAR(50),                                       -- 修改人姓名
    ModifyOrganizationId INT,                                          -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                               -- 修改人所在组织名称
);
GO

-- 销售合同表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售合同表，记录正式签订的合同信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号，唯一标识', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项ID，外键关联ProjectApproval表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ProjectApprovalId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ProjectApprovalCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'OpportunityId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'OpportunityCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'CustomerId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'CustomerName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同类型：1-销售合同，2-服务合同，3-框架协议，4-补充协议', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractType';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同金额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税率', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'TaxRate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'TaxAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'含税合同金额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractAmountWithTax';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'币种，默认CNY（人民币）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'Currency';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'签订日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'SignDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'生效日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'EffectiveDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到期日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ExpiryDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同期限', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractPeriod';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款方式', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'PaymentMethod';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付要求', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'DeliveryRequirements';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质量要求', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'QualityRequirements';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质保期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'WarrantyPeriod';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同条款', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractTerms';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'特殊约定', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'SpecialAgreements';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同状态：1-草稿，2-审批中，3-已生效，4-履行中，5-已完成，6-已终止，7-已作废', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-未提交，2-审批中，3-已通过，4-已驳回', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ApprovalStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ApprovalFlowId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ApprovalUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ApprovalUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ApprovalTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ApprovalComment';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同文件路径', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractFilePath';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同扫描件路径', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ContractScanPath';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否模板合同', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'IsTemplate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'父合同ID（用于补充协议）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ParentContractId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Contract', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- 9. 销售合同明细表 - 合同的产品明细
CREATE TABLE ContractDetail (
    ContractDetailId INT IDENTITY(1,1) PRIMARY KEY,                    -- 合同明细ID（主键）
    ContractId INT NOT NULL,                                           -- 合同ID（外键）
    ContractCode NVARCHAR(50) NOT NULL,                                -- 合同编号（冗余）
    LineNumber INT NOT NULL,                                           -- 行号
    ProductId INT NOT NULL,                                            -- 产品ID（外键）
    ProductCode NVARCHAR(50) NOT NULL,                                 -- 产品编码（冗余）
    ProductName NVARCHAR(200) NOT NULL,                                -- 产品名称（冗余）
    ProductSpecification NVARCHAR(MAX),                                -- 产品规格（冗余）
    Unit NVARCHAR(20) NOT NULL,                                        -- 计量单位
    Quantity DECIMAL(18,4) NOT NULL DEFAULT 0,                         -- 数量
    UnitPrice DECIMAL(18,2) NOT NULL DEFAULT 0,                        -- 单价
    Amount DECIMAL(18,2) NOT NULL DEFAULT 0,                           -- 金额
    TaxRate DECIMAL(5,2) DEFAULT 0,                                    -- 税率
    TaxAmount DECIMAL(18,2) DEFAULT 0,                                 -- 税额
    AmountWithTax DECIMAL(18,2) DEFAULT 0,                             -- 含税金额
    DeliveryDate DATE,                                                 -- 要求交货日期
    DeliveryAddress NVARCHAR(500),                                     -- 交货地址
    TechnicalParameters NVARCHAR(MAX),                                 -- 技术参数
    QualityStandard NVARCHAR(500),                                     -- 质量标准
    Remark NVARCHAR(500),                                              -- 备注
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                    -- 创建时间
    CreateUserId INT NOT NULL,                                         -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                              -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                 -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                     -- 创建人所在组织名称
    ModifyTime DATETIME,                                               -- 修改时间
    ModifyUserId INT,                                                  -- 修改人ID
    ModifyUserName NVARCHAR(50),                                       -- 修改人姓名
    ModifyOrganizationId INT,                                          -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                               -- 修改人所在组织名称
);
GO

-- 合同明细表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同明细表，记录合同的产品明细信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同明细ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ContractDetailId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID，外键关联Contract表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ContractId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ContractCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'行号，同一合同内唯一', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'LineNumber';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品ID，外键关联Product表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ProductId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品编码', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ProductCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ProductName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品规格', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ProductSpecification';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'计量单位', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'Unit';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'Quantity';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单价', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'UnitPrice';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'金额（数量×单价）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'Amount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税率', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'TaxRate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'TaxAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'含税金额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'AmountWithTax';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'要求交货日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'DeliveryDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交货地址', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'DeliveryAddress';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术参数', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'TechnicalParameters';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质量标准', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'QualityStandard';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractDetail', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- 10. 销售合同变更表 - 合同变更申请
CREATE TABLE ContractChange (
    ContractChangeId INT IDENTITY(1,1) PRIMARY KEY,                    -- 合同变更ID（主键）
    ContractChangeCode NVARCHAR(50) NOT NULL,                          -- 变更单号
    ContractId INT NOT NULL,                                           -- 合同ID（外键）
    ContractCode NVARCHAR(50) NOT NULL,                                -- 合同编号（冗余）
    ChangeType INT NOT NULL DEFAULT 1,                                 -- 变更类型（1-金额变更，2-范围变更，3-时间变更，4-条款变更，5-综合变更）
    ChangeReason NVARCHAR(MAX) NOT NULL,                               -- 变更原因
    ChangeContent NVARCHAR(MAX) NOT NULL,                              -- 变更内容
    OriginalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,                   -- 原合同金额
    ChangeAmount DECIMAL(18,2) NOT NULL DEFAULT 0,                     -- 变更金额（正数为增加，负数为减少）
    NewAmount DECIMAL(18,2) NOT NULL DEFAULT 0,                        -- 新合同金额
    OriginalExpiryDate DATE,                                           -- 原到期日期
    NewExpiryDate DATE,                                                -- 新到期日期
    ChangeAgreementPath NVARCHAR(500),                                 -- 变更协议文件路径
    CustomerConfirm BIT DEFAULT 0,                                     -- 客户是否确认
    CustomerConfirmTime DATETIME,                                      -- 客户确认时间
    CustomerConfirmPerson NVARCHAR(50),                                -- 客户确认人
    CustomerConfirmDocumentPath NVARCHAR(500),                         -- 客户确认文件路径
    ApprovalStatus INT NOT NULL DEFAULT 1,                             -- 审批状态（1-草稿，2-审批中，3-已通过，4-已驳回）
    ApprovalFlowId INT,                                                -- 审批流程ID
    ApprovalUserId INT,                                                -- 审批人ID
    ApprovalUserName NVARCHAR(50),                                     -- 审批人姓名
    ApprovalTime DATETIME,                                             -- 审批时间
    ApprovalComment NVARCHAR(500),                                     -- 审批意见
    ChangeStatus INT NOT NULL DEFAULT 1,                               -- 变更状态（1-待处理，2-处理中，3-已完成，4-已取消）
    ImplementTime DATETIME,                                            -- 实施时间
    ImplementUserId INT,                                               -- 实施人ID
    ImplementUserName NVARCHAR(50),                                    -- 实施人姓名
    ImplementResult NVARCHAR(500),                                     -- 实施结果
    Remark NVARCHAR(500),                                              -- 备注
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                    -- 创建时间
    CreateUserId INT NOT NULL,                                         -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                              -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                 -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                     -- 创建人所在组织名称
    ModifyTime DATETIME,                                               -- 修改时间
    ModifyUserId INT,                                                  -- 修改人ID
    ModifyUserName NVARCHAR(50),                                       -- 修改人姓名
    ModifyOrganizationId INT,                                          -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                               -- 修改人所在组织名称
);
GO

-- 合同变更表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同变更表，记录合同的变更申请和审批信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同变更ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ContractChangeId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更单号，唯一标识', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ContractChangeCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID，外键关联Contract表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ContractId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ContractCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更类型：1-金额变更，2-范围变更，3-时间变更，4-条款变更，5-综合变更', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ChangeType';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更原因', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ChangeReason';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更内容', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ChangeContent';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原合同金额', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'OriginalAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更金额，正数为增加，负数为减少', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ChangeAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新合同金额（原合同金额+变更金额）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'NewAmount';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原到期日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'OriginalExpiryDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新到期日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'NewExpiryDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更协议文件路径', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ChangeAgreementPath';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户是否确认', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'CustomerConfirm';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'CustomerConfirmTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认人', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'CustomerConfirmPerson';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认文件路径', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'CustomerConfirmDocumentPath';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-草稿，2-审批中，3-已通过，4-已驳回', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ApprovalStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ApprovalFlowId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ApprovalUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ApprovalUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ApprovalTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ApprovalComment';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更状态：1-待处理，2-处理中，3-已完成，4-已取消', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ChangeStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ImplementTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ImplementUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ImplementUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施结果', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ImplementResult';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractChange', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- 11. 合同交底表 - 合同签署后的交底信息
CREATE TABLE ContractHandover (
    ContractHandoverId INT IDENTITY(1,1) PRIMARY KEY,                  -- 合同交底ID（主键）
    ContractId INT NOT NULL,                                           -- 合同ID（外键）
    ContractCode NVARCHAR(50) NOT NULL,                                -- 合同编号（冗余）
    HandoverCode NVARCHAR(50) NOT NULL,                                -- 交底单号
    HandoverType INT NOT NULL DEFAULT 1,                               -- 交底类型（1-首次交底，2-变更交底）
    HandoverDate DATETIME NOT NULL,                                    -- 交底日期
    HandoverLocation NVARCHAR(200),                                    -- 交底地点
    SalesRepresentativeId INT NOT NULL,                                -- 销售代表ID
    SalesRepresentativeName NVARCHAR(50) NOT NULL,                     -- 销售代表姓名
    DeliveryTeamLeaderId INT NOT NULL,                                 -- 交付团队负责人ID
    DeliveryTeamLeaderName NVARCHAR(50) NOT NULL,                      -- 交付团队负责人姓名
    Participants NVARCHAR(MAX),                                        -- 参与人员
    KeyContractTerms NVARCHAR(MAX),                                    -- 关键合同条款
    CustomerSpecialRequirements NVARCHAR(MAX),                         -- 客户特殊要求
    TechnicalRequirements NVARCHAR(MAX),                               -- 技术要求
    DeliveryRequirements NVARCHAR(MAX),                                -- 交付要求
    QualityRequirements NVARCHAR(MAX),                                 -- 质量要求
    PaymentTerms NVARCHAR(500),                                        -- 付款条款
    RiskPoints NVARCHAR(MAX),                                          -- 风险点
    RiskMitigationMeasures NVARCHAR(MAX),                              -- 风险应对措施
    ImportantNotes NVARCHAR(MAX),                                      -- 重要注意事项
    Attachments NVARCHAR(MAX),                                         -- 附件路径（JSON格式存储多个附件）
    HandoverStatus INT NOT NULL DEFAULT 1,                             -- 交底状态（1-已安排，2-进行中，3-已完成，4-已取消）
    CompletionTime DATETIME,                                           -- 完成时间
    SalesSignOff BIT DEFAULT 0,                                        -- 销售签字确认
    SalesSignOffTime DATETIME,                                         -- 销售签字时间
    DeliverySignOff BIT DEFAULT 0,                                     -- 交付签字确认
    DeliverySignOffTime DATETIME,                                      -- 交付签字时间
    HandoverEvaluation NVARCHAR(500),                                  -- 交底评价
    FollowUpActions NVARCHAR(500),                                     -- 后续行动
    Remark NVARCHAR(500),                                              -- 备注
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),                    -- 创建时间
    CreateUserId INT NOT NULL,                                         -- 创建人ID
    CreateUserName NVARCHAR(50) NOT NULL,                              -- 创建人姓名
    CreateOrganizationId INT NOT NULL,                                 -- 创建人所在组织ID
    CreateOrganizationName NVARCHAR(100) NOT NULL,                     -- 创建人所在组织名称
    ModifyTime DATETIME,                                               -- 修改时间
    ModifyUserId INT,                                                  -- 修改人ID
    ModifyUserName NVARCHAR(50),                                       -- 修改人姓名
    ModifyOrganizationId INT,                                          -- 修改人所在组织ID
    ModifyOrganizationName NVARCHAR(100)                               -- 修改人所在组织名称
);
GO

-- 合同交底表注释
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同交底表，记录合同签署后的交底信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同交底ID（主键）', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'ContractHandoverId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID，外键关联Contract表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'ContractId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'ContractCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底单号，唯一标识', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'HandoverCode';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底类型：1-首次交底，2-变更交底', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'HandoverType';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底日期', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'HandoverDate';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底地点', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'HandoverLocation';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售代表ID，外键关联User表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'SalesRepresentativeId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售代表姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'SalesRepresentativeName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付团队负责人ID，外键关联User表', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'DeliveryTeamLeaderId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付团队负责人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'DeliveryTeamLeaderName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参与人员，可存储多个人员信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'Participants';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关键合同条款', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'KeyContractTerms';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户特殊要求', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'CustomerSpecialRequirements';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术要求', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'TechnicalRequirements';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付要求', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'DeliveryRequirements';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质量要求', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'QualityRequirements';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款条款', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'PaymentTerms';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险点', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'RiskPoints';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险应对措施', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'RiskMitigationMeasures';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'重要注意事项', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'ImportantNotes';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'附件路径，JSON格式存储多个附件信息', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'Attachments';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底状态：1-已安排，2-进行中，3-已完成，4-已取消', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'HandoverStatus';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'完成时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'CompletionTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售签字确认', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'SalesSignOff';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售签字时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'SalesSignOffTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付签字确认', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'DeliverySignOff';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付签字时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'DeliverySignOffTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底评价', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'HandoverEvaluation';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'后续行动', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'FollowUpActions';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'Remark';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'CreateTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'CreateUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'CreateUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'CreateOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'CreateOrganizationName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'ModifyTime';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'ModifyUserId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'ModifyUserName';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationId';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称', 
    @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ContractHandover', @level2type=N'COLUMN', @level2name=N'ModifyOrganizationName';
GO

-- ============================================================
-- 添加外键约束
-- ============================================================

-- 客户表外键约束
ALTER TABLE Customer ADD CONSTRAINT FK_Customer_OwnerUser FOREIGN KEY (OwnerUserId) REFERENCES [User](UserId);

-- 商机表外键约束
ALTER TABLE Opportunity ADD CONSTRAINT FK_Opportunity_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId);
ALTER TABLE Opportunity ADD CONSTRAINT FK_Opportunity_SalesOwnerUser FOREIGN KEY (SalesOwnerUserId) REFERENCES [User](UserId);

-- 报价表外键约束
ALTER TABLE Quotation ADD CONSTRAINT FK_Quotation_Opportunity FOREIGN KEY (OpportunityId) REFERENCES Opportunity(OpportunityId);
ALTER TABLE Quotation ADD CONSTRAINT FK_Quotation_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId);

-- 投标表外键约束
ALTER TABLE Bid ADD CONSTRAINT FK_Bid_Opportunity FOREIGN KEY (OpportunityId) REFERENCES Opportunity(OpportunityId);
ALTER TABLE Bid ADD CONSTRAINT FK_Bid_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId);

-- 立项表外键约束
ALTER TABLE ProjectApproval ADD CONSTRAINT FK_ProjectApproval_Opportunity FOREIGN KEY (OpportunityId) REFERENCES Opportunity(OpportunityId);
ALTER TABLE ProjectApproval ADD CONSTRAINT FK_ProjectApproval_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId);

-- 合同表外键约束
ALTER TABLE Contract ADD CONSTRAINT FK_Contract_ProjectApproval FOREIGN KEY (ProjectApprovalId) REFERENCES ProjectApproval(ProjectApprovalId);
ALTER TABLE Contract ADD CONSTRAINT FK_Contract_Opportunity FOREIGN KEY (OpportunityId) REFERENCES Opportunity(OpportunityId);
ALTER TABLE Contract ADD CONSTRAINT FK_Contract_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId);

-- 合同明细表外键约束
ALTER TABLE ContractDetail ADD CONSTRAINT FK_ContractDetail_Contract FOREIGN KEY (ContractId) REFERENCES Contract(ContractId);
ALTER TABLE ContractDetail ADD CONSTRAINT FK_ContractDetail_Product FOREIGN KEY (ProductId) REFERENCES Product(ProductId);

-- 合同变更表外键约束
ALTER TABLE ContractChange ADD CONSTRAINT FK_ContractChange_Contract FOREIGN KEY (ContractId) REFERENCES Contract(ContractId);

-- 合同交底表外键约束
ALTER TABLE ContractHandover ADD CONSTRAINT FK_ContractHandover_Contract FOREIGN KEY (ContractId) REFERENCES Contract(ContractId);
ALTER TABLE ContractHandover ADD CONSTRAINT FK_ContractHandover_SalesRepresentative FOREIGN KEY (SalesRepresentativeId) REFERENCES [User](UserId);
ALTER TABLE ContractHandover ADD CONSTRAINT FK_ContractHandover_DeliveryTeamLeader FOREIGN KEY (DeliveryTeamLeaderId) REFERENCES [User](UserId);
GO

-- ============================================================
-- 添加唯一约束
-- ============================================================

ALTER TABLE [User] ADD CONSTRAINT UK_User_UserCode UNIQUE (UserCode);
ALTER TABLE [User] ADD CONSTRAINT UK_User_LoginName UNIQUE (LoginName);
ALTER TABLE Product ADD CONSTRAINT UK_Product_ProductCode UNIQUE (ProductCode);
ALTER TABLE Customer ADD CONSTRAINT UK_Customer_CustomerCode UNIQUE (CustomerCode);
ALTER TABLE Opportunity ADD CONSTRAINT UK_Opportunity_OpportunityCode UNIQUE (OpportunityCode);
ALTER TABLE Quotation ADD CONSTRAINT UK_Quotation_QuotationCode UNIQUE (QuotationCode);
ALTER TABLE Bid ADD CONSTRAINT UK_Bid_BidCode UNIQUE (BidCode);
ALTER TABLE ProjectApproval ADD CONSTRAINT UK_ProjectApproval_ProjectApprovalCode UNIQUE (ProjectApprovalCode);
ALTER TABLE Contract ADD CONSTRAINT UK_Contract_ContractCode UNIQUE (ContractCode);
ALTER TABLE ContractChange ADD CONSTRAINT UK_ContractChange_ContractChangeCode UNIQUE (ContractChangeCode);
ALTER TABLE ContractHandover ADD CONSTRAINT UK_ContractHandover_HandoverCode UNIQUE (HandoverCode);
GO

-- ============================================================
-- 表关系说明
-- ============================================================
/*
核心表关系链：
1. 客户管理链：Customer ←→ Opportunity
2. 商机跟进链：Opportunity → Quotation/Bid → ProjectApproval → Contract
3. 合同执行链：Contract → ContractDetail → ContractChange → ContractHandover
4. 产品关联链：Product → ContractDetail
5. 用户关联链：User → Customer/Opportunity/ContractHandover等

关键业务规则：
1. 新客户流程：Customer(IsPublic=1) → Opportunity → Quotation/Bid → ProjectApproval → Contract → Customer(IsPublic=0, CustomerStatus=3)
2. 老客户流程：Customer(IsPublic=0, CustomerStatus=3) → Opportunity → Quotation/Bid → ProjectApproval → Contract
3. 合同变更触发新的交底流程：Contract → ContractChange → ContractHandover(HandoverType=2)
*/