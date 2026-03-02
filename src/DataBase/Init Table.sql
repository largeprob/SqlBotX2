USE CRM2
GO
/****** Object:  Table [dbo].[Bid]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bid](
	[BidId] [int] IDENTITY(1,1) NOT NULL,
	[BidCode] [nvarchar](50) NOT NULL,
	[OpportunityId] [int] NOT NULL,
	[OpportunityCode] [nvarchar](50) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[BidProjectName] [nvarchar](200) NOT NULL,
	[BidProjectCode] [nvarchar](50) NOT NULL,
	[TenderOrganization] [nvarchar](200) NOT NULL,
	[TenderAddress] [nvarchar](500) NULL,
	[BidOpeningDate] [datetime] NOT NULL,
	[BidSubmissionDeadline] [datetime] NOT NULL,
	[BidBondAmount] [decimal](18, 2) NULL,
	[BidBondPaid] [bit] NULL,
	[BidBondPaidTime] [datetime] NULL,
	[PerformanceBondRequired] [bit] NULL,
	[PerformanceBondRate] [decimal](5, 2) NULL,
	[TechnicalScore] [decimal](5, 2) NULL,
	[CommercialScore] [decimal](5, 2) NULL,
	[TotalScore] [decimal](5, 2) NULL,
	[Ranking] [int] NULL,
	[Competitors] [nvarchar](500) NULL,
	[OurAdvantages] [nvarchar](500) NULL,
	[OurDisadvantages] [nvarchar](500) NULL,
	[BidStatus] [int] NOT NULL,
	[ApprovalStatus] [int] NOT NULL,
	[ApprovalFlowId] [int] NULL,
	[ApprovalUserId] [int] NULL,
	[ApprovalUserName] [nvarchar](50) NULL,
	[ApprovalTime] [datetime] NULL,
	[ApprovalComment] [nvarchar](500) NULL,
	[WinNoticeTime] [datetime] NULL,
	[WinNoticeNumber] [nvarchar](50) NULL,
	[BidDocumentPath] [nvarchar](500) NULL,
	[OtherAttachments] [nvarchar](500) NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[BidId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Bid_BidCode] UNIQUE NONCLUSTERED 
(
	[BidCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Contract]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contract](
	[ContractId] [int] IDENTITY(1,1) NOT NULL,
	[ContractCode] [nvarchar](50) NOT NULL,
	[ProjectApprovalId] [int] NOT NULL,
	[ProjectApprovalCode] [nvarchar](50) NOT NULL,
	[OpportunityId] [int] NOT NULL,
	[OpportunityCode] [nvarchar](50) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[ContractName] [nvarchar](200) NOT NULL,
	[ContractType] [int] NOT NULL,
	[ContractAmount] [decimal](18, 2) NOT NULL,
	[TaxRate] [decimal](5, 2) NULL,
	[TaxAmount] [decimal](18, 2) NULL,
	[ContractAmountWithTax] [decimal](18, 2) NULL,
	[Currency] [nvarchar](10) NULL,
	[SignDate] [date] NOT NULL,
	[EffectiveDate] [date] NOT NULL,
	[ExpiryDate] [date] NULL,
	[ContractPeriod] [nvarchar](100) NULL,
	[PaymentMethod] [nvarchar](200) NULL,
	[DeliveryRequirements] [nvarchar](500) NULL,
	[QualityRequirements] [nvarchar](500) NULL,
	[WarrantyPeriod] [nvarchar](100) NULL,
	[ContractTerms] [nvarchar](max) NULL,
	[SpecialAgreements] [nvarchar](max) NULL,
	[ContractStatus] [int] NOT NULL,
	[ApprovalStatus] [int] NOT NULL,
	[ApprovalFlowId] [int] NULL,
	[ApprovalUserId] [int] NULL,
	[ApprovalUserName] [nvarchar](50) NULL,
	[ApprovalTime] [datetime] NULL,
	[ApprovalComment] [nvarchar](500) NULL,
	[ContractFilePath] [nvarchar](500) NULL,
	[ContractScanPath] [nvarchar](500) NULL,
	[IsTemplate] [bit] NULL,
	[ParentContractId] [int] NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ContractId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Contract_ContractCode] UNIQUE NONCLUSTERED 
(
	[ContractCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ContractChange]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContractChange](
	[ContractChangeId] [int] IDENTITY(1,1) NOT NULL,
	[ContractChangeCode] [nvarchar](50) NOT NULL,
	[ContractId] [int] NOT NULL,
	[ContractCode] [nvarchar](50) NOT NULL,
	[ChangeType] [int] NOT NULL,
	[ChangeReason] [nvarchar](max) NOT NULL,
	[ChangeContent] [nvarchar](max) NOT NULL,
	[OriginalAmount] [decimal](18, 2) NOT NULL,
	[ChangeAmount] [decimal](18, 2) NOT NULL,
	[NewAmount] [decimal](18, 2) NOT NULL,
	[OriginalExpiryDate] [date] NULL,
	[NewExpiryDate] [date] NULL,
	[ChangeAgreementPath] [nvarchar](500) NULL,
	[CustomerConfirm] [bit] NULL,
	[CustomerConfirmTime] [datetime] NULL,
	[CustomerConfirmPerson] [nvarchar](50) NULL,
	[CustomerConfirmDocumentPath] [nvarchar](500) NULL,
	[ApprovalStatus] [int] NOT NULL,
	[ApprovalFlowId] [int] NULL,
	[ApprovalUserId] [int] NULL,
	[ApprovalUserName] [nvarchar](50) NULL,
	[ApprovalTime] [datetime] NULL,
	[ApprovalComment] [nvarchar](500) NULL,
	[ChangeStatus] [int] NOT NULL,
	[ImplementTime] [datetime] NULL,
	[ImplementUserId] [int] NULL,
	[ImplementUserName] [nvarchar](50) NULL,
	[ImplementResult] [nvarchar](500) NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ContractChangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ContractChange_ContractChangeCode] UNIQUE NONCLUSTERED 
(
	[ContractChangeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ContractDetail]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContractDetail](
	[ContractDetailId] [int] IDENTITY(1,1) NOT NULL,
	[ContractId] [int] NOT NULL,
	[ContractCode] [nvarchar](50) NOT NULL,
	[LineNumber] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[ProductCode] [nvarchar](50) NOT NULL,
	[ProductName] [nvarchar](200) NOT NULL,
	[ProductSpecification] [nvarchar](max) NULL,
	[Unit] [nvarchar](20) NOT NULL,
	[Quantity] [decimal](18, 4) NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[TaxRate] [decimal](5, 2) NULL,
	[TaxAmount] [decimal](18, 2) NULL,
	[AmountWithTax] [decimal](18, 2) NULL,
	[DeliveryDate] [date] NULL,
	[DeliveryAddress] [nvarchar](500) NULL,
	[TechnicalParameters] [nvarchar](max) NULL,
	[QualityStandard] [nvarchar](500) NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ContractDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ContractHandover]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContractHandover](
	[ContractHandoverId] [int] IDENTITY(1,1) NOT NULL,
	[ContractId] [int] NOT NULL,
	[ContractCode] [nvarchar](50) NOT NULL,
	[HandoverCode] [nvarchar](50) NOT NULL,
	[HandoverType] [int] NOT NULL,
	[HandoverDate] [datetime] NOT NULL,
	[HandoverLocation] [nvarchar](200) NULL,
	[SalesRepresentativeId] [int] NOT NULL,
	[SalesRepresentativeName] [nvarchar](50) NOT NULL,
	[DeliveryTeamLeaderId] [int] NOT NULL,
	[DeliveryTeamLeaderName] [nvarchar](50) NOT NULL,
	[Participants] [nvarchar](max) NULL,
	[KeyContractTerms] [nvarchar](max) NULL,
	[CustomerSpecialRequirements] [nvarchar](max) NULL,
	[TechnicalRequirements] [nvarchar](max) NULL,
	[DeliveryRequirements] [nvarchar](max) NULL,
	[QualityRequirements] [nvarchar](max) NULL,
	[PaymentTerms] [nvarchar](500) NULL,
	[RiskPoints] [nvarchar](max) NULL,
	[RiskMitigationMeasures] [nvarchar](max) NULL,
	[ImportantNotes] [nvarchar](max) NULL,
	[Attachments] [nvarchar](max) NULL,
	[HandoverStatus] [int] NOT NULL,
	[CompletionTime] [datetime] NULL,
	[SalesSignOff] [bit] NULL,
	[SalesSignOffTime] [datetime] NULL,
	[DeliverySignOff] [bit] NULL,
	[DeliverySignOffTime] [datetime] NULL,
	[HandoverEvaluation] [nvarchar](500) NULL,
	[FollowUpActions] [nvarchar](500) NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ContractHandoverId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ContractHandover_HandoverCode] UNIQUE NONCLUSTERED 
(
	[HandoverCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerCode] [nvarchar](50) NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[CustomerType] [int] NOT NULL,
	[CustomerSource] [nvarchar](100) NULL,
	[Industry] [nvarchar](100) NULL,
	[ContactPerson] [nvarchar](50) NULL,
	[ContactPhone] [nvarchar](50) NULL,
	[ContactEmail] [nvarchar](100) NULL,
	[Address] [nvarchar](500) NULL,
	[CustomerLevel] [int] NULL,
	[CreditStatus] [int] NULL,
	[CustomerStatus] [int] NOT NULL,
	[CustomerTransactionType] [int] NOT NULL,
	[IsPublic] [bit] NOT NULL,
	[OwnerUserId] [int] NULL,
	[OwnerUserName] [nvarchar](50) NULL,
	[OwnerOrganizationId] [int] NULL,
	[OwnerOrganizationName] [nvarchar](100) NULL,
	[RecentContactTime] [datetime] NULL,
	[TotalOrderAmount] [decimal](18, 2) NULL,
	[TotalOrderCount] [int] NULL,
	[LastOrderTime] [datetime] NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Customer_CustomerCode] UNIQUE NONCLUSTERED 
(
	[CustomerCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Department]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Department](
	[DepartmentId] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentCode] [nvarchar](50) NOT NULL,
	[DepartmentName] [nvarchar](200) NOT NULL,
	[ParentDepartmentId] [int] NULL,
	[ParentDepartmentCode] [nvarchar](50) NULL,
	[ParentDepartmentName] [nvarchar](200) NULL,
	[OrganizationId] [int] NOT NULL,
	[OrganizationCode] [nvarchar](50) NOT NULL,
	[OrganizationName] [nvarchar](200) NOT NULL,
	[DepartmentType] [int] NOT NULL,
	[DepartmentLevel] [int] NOT NULL,
	[DepartmentPath] [nvarchar](500) NULL,
	[ManagerUserId] [int] NULL,
	[ManagerUserName] [nvarchar](50) NULL,
	[ContactPhone] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL,
	[Status] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Department_DepartmentCode] UNIQUE NONCLUSTERED 
(
	[DepartmentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Opportunity]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Opportunity](
	[OpportunityId] [int] IDENTITY(1,1) NOT NULL,
	[OpportunityCode] [nvarchar](50) NOT NULL,
	[OpportunityName] [nvarchar](200) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[CustomerType] [int] NULL,
	[OpportunityType] [int] NOT NULL,
	[ProjectBackground] [nvarchar](max) NULL,
	[CustomerNeed] [nvarchar](max) NULL,
	[ExpectedAmount] [decimal](18, 2) NOT NULL,
	[ExpectedClosingDate] [date] NULL,
	[WinProbability] [int] NOT NULL,
	[CompetitorInfo] [nvarchar](500) NULL,
	[OpportunityStage] [int] NOT NULL,
	[NeedBid] [bit] NOT NULL,
	[BidProjectCode] [nvarchar](50) NULL,
	[SalesOwnerUserId] [int] NOT NULL,
	[SalesOwnerUserName] [nvarchar](50) NOT NULL,
	[SalesOwnerOrganizationId] [int] NOT NULL,
	[SalesOwnerOrganizationName] [nvarchar](100) NOT NULL,
	[CoSalesUserIds] [nvarchar](500) NULL,
	[CoSalesUserNames] [nvarchar](500) NULL,
	[CurrentTask] [nvarchar](200) NULL,
	[NextStep] [nvarchar](200) NULL,
	[Remark] [nvarchar](500) NULL,
	[OpportunityStatus] [int] NOT NULL,
	[CloseReason] [nvarchar](200) NULL,
	[CloseTime] [datetime] NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[OpportunityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Opportunity_OpportunityCode] UNIQUE NONCLUSTERED 
(
	[OpportunityCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Organization]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Organization](
	[OrganizationId] [int] IDENTITY(1,1) NOT NULL,
	[OrganizationCode] [nvarchar](50) NOT NULL,
	[OrganizationName] [nvarchar](200) NOT NULL,
	[ParentOrganizationId] [int] NULL,
	[ParentOrganizationCode] [nvarchar](50) NULL,
	[ParentOrganizationName] [nvarchar](200) NULL,
	[OrganizationType] [int] NOT NULL,
	[OrganizationLevel] [int] NOT NULL,
	[OrganizationPath] [nvarchar](500) NOT NULL,
	[ContactPerson] [nvarchar](50) NULL,
	[ContactPhone] [nvarchar](50) NULL,
	[Address] [nvarchar](500) NULL,
	[Description] [nvarchar](500) NULL,
	[Status] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
 CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED 
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Organization_OrganizationCode] UNIQUE NONCLUSTERED 
(
	[OrganizationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[ProductCode] [nvarchar](50) NOT NULL,
	[ProductName] [nvarchar](200) NOT NULL,
	[ProductType] [nvarchar](50) NOT NULL,
	[ProductCategory] [nvarchar](50) NULL,
	[ProductSpecification] [nvarchar](max) NULL,
	[Unit] [nvarchar](20) NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[CostPrice] [decimal](18, 2) NOT NULL,
	[MinimumStock] [int] NULL,
	[CurrentStock] [int] NULL,
	[Status] [int] NOT NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Product_ProductCode] UNIQUE NONCLUSTERED 
(
	[ProductCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectApproval]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectApproval](
	[ProjectApprovalId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectApprovalCode] [nvarchar](50) NOT NULL,
	[ReferenceType] [int] NOT NULL,
	[ReferenceId] [int] NOT NULL,
	[ReferenceCode] [nvarchar](50) NOT NULL,
	[OpportunityId] [int] NOT NULL,
	[OpportunityCode] [nvarchar](50) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[ProjectName] [nvarchar](200) NOT NULL,
	[ProjectBackground] [nvarchar](max) NULL,
	[CustomerDemand] [nvarchar](max) NULL,
	[SolutionOverview] [nvarchar](max) NULL,
	[EstimatedAmount] [decimal](18, 2) NOT NULL,
	[EstimatedCost] [decimal](18, 2) NOT NULL,
	[EstimatedProfit] [decimal](18, 2) NOT NULL,
	[ProfitMargin] [decimal](5, 2) NULL,
	[ResourceRequirements] [nvarchar](500) NULL,
	[ImplementationPeriod] [nvarchar](100) NULL,
	[RiskAnalysis] [nvarchar](max) NULL,
	[RiskMitigationMeasures] [nvarchar](max) NULL,
	[SalesDepartmentOpinion] [nvarchar](500) NULL,
	[TechnicalDepartmentOpinion] [nvarchar](500) NULL,
	[FinanceDepartmentOpinion] [nvarchar](500) NULL,
	[ManagementOpinion] [nvarchar](500) NULL,
	[ApprovalStatus] [int] NOT NULL,
	[CurrentApprovalStage] [int] NULL,
	[ApprovalFlowId] [int] NULL,
	[FinalApproverUserId] [int] NULL,
	[FinalApproverUserName] [nvarchar](50) NULL,
	[FinalApprovalTime] [datetime] NULL,
	[ApprovalComment] [nvarchar](500) NULL,
	[IsUrgent] [bit] NULL,
	[UrgentReason] [nvarchar](200) NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProjectApprovalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ProjectApproval_ProjectApprovalCode] UNIQUE NONCLUSTERED 
(
	[ProjectApprovalCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Quotation]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Quotation](
	[QuotationId] [int] IDENTITY(1,1) NOT NULL,
	[QuotationCode] [nvarchar](50) NOT NULL,
	[OpportunityId] [int] NOT NULL,
	[OpportunityCode] [nvarchar](50) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[QuotationDate] [date] NOT NULL,
	[ValidUntilDate] [date] NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[TaxRate] [decimal](5, 2) NULL,
	[TaxAmount] [decimal](18, 2) NULL,
	[TotalAmountWithTax] [decimal](18, 2) NULL,
	[Currency] [nvarchar](10) NULL,
	[PaymentTerms] [nvarchar](500) NULL,
	[DeliveryTerms] [nvarchar](500) NULL,
	[WarrantyTerms] [nvarchar](500) NULL,
	[SpecialTerms] [nvarchar](500) NULL,
	[QuotationStatus] [int] NOT NULL,
	[ApprovalStatus] [int] NOT NULL,
	[ApprovalFlowId] [int] NULL,
	[ApprovalUserId] [int] NULL,
	[ApprovalUserName] [nvarchar](50) NULL,
	[ApprovalTime] [datetime] NULL,
	[ApprovalComment] [nvarchar](500) NULL,
	[CustomerFeedback] [nvarchar](500) NULL,
	[CustomerConfirmTime] [datetime] NULL,
	[NextFollowUpTime] [datetime] NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[QuotationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Quotation_QuotationCode] UNIQUE NONCLUSTERED 
(
	[QuotationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserInfo]    Script Date: 2026/2/6 下午 12:00:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInfo](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserCode] [nvarchar](50) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[LoginName] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](200) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](20) NULL,
	[OrganizationId] [int] NULL,
	[DepartmentId] [int] NULL,
	[DepartmentName] [nvarchar](100) NULL,
	[Position] [nvarchar](50) NULL,
	[UserType] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[LastLoginTime] [datetime] NULL,
	[Remark] [nvarchar](500) NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateOrganizationId] [int] NOT NULL,
	[CreateOrganizationName] [nvarchar](100) NOT NULL,
	[ModifyTime] [datetime] NULL,
	[ModifyUserId] [int] NULL,
	[ModifyUserName] [nvarchar](50) NULL,
	[ModifyOrganizationId] [int] NULL,
	[ModifyOrganizationName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_User_LoginName] UNIQUE NONCLUSTERED 
(
	[LoginName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_User_UserCode] UNIQUE NONCLUSTERED 
(
	[UserCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Bid] ADD  DEFAULT ((0)) FOR [BidBondAmount]
GO
ALTER TABLE [dbo].[Bid] ADD  DEFAULT ((0)) FOR [BidBondPaid]
GO
ALTER TABLE [dbo].[Bid] ADD  DEFAULT ((0)) FOR [PerformanceBondRequired]
GO
ALTER TABLE [dbo].[Bid] ADD  DEFAULT ((0)) FOR [PerformanceBondRate]
GO
ALTER TABLE [dbo].[Bid] ADD  DEFAULT ((1)) FOR [BidStatus]
GO
ALTER TABLE [dbo].[Bid] ADD  DEFAULT ((1)) FOR [ApprovalStatus]
GO
ALTER TABLE [dbo].[Bid] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Contract] ADD  DEFAULT ((1)) FOR [ContractType]
GO
ALTER TABLE [dbo].[Contract] ADD  DEFAULT ((0)) FOR [ContractAmount]
GO
ALTER TABLE [dbo].[Contract] ADD  DEFAULT ((0)) FOR [TaxRate]
GO
ALTER TABLE [dbo].[Contract] ADD  DEFAULT ((0)) FOR [TaxAmount]
GO
ALTER TABLE [dbo].[Contract] ADD  DEFAULT ((0)) FOR [ContractAmountWithTax]
GO
ALTER TABLE [dbo].[Contract] ADD  DEFAULT ('CNY') FOR [Currency]
GO
ALTER TABLE [dbo].[Contract] ADD  DEFAULT ((1)) FOR [ContractStatus]
GO
ALTER TABLE [dbo].[Contract] ADD  DEFAULT ((1)) FOR [ApprovalStatus]
GO
ALTER TABLE [dbo].[Contract] ADD  DEFAULT ((0)) FOR [IsTemplate]
GO
ALTER TABLE [dbo].[Contract] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[ContractChange] ADD  DEFAULT ((1)) FOR [ChangeType]
GO
ALTER TABLE [dbo].[ContractChange] ADD  DEFAULT ((0)) FOR [OriginalAmount]
GO
ALTER TABLE [dbo].[ContractChange] ADD  DEFAULT ((0)) FOR [ChangeAmount]
GO
ALTER TABLE [dbo].[ContractChange] ADD  DEFAULT ((0)) FOR [NewAmount]
GO
ALTER TABLE [dbo].[ContractChange] ADD  DEFAULT ((0)) FOR [CustomerConfirm]
GO
ALTER TABLE [dbo].[ContractChange] ADD  DEFAULT ((1)) FOR [ApprovalStatus]
GO
ALTER TABLE [dbo].[ContractChange] ADD  DEFAULT ((1)) FOR [ChangeStatus]
GO
ALTER TABLE [dbo].[ContractChange] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[ContractDetail] ADD  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[ContractDetail] ADD  DEFAULT ((0)) FOR [UnitPrice]
GO
ALTER TABLE [dbo].[ContractDetail] ADD  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[ContractDetail] ADD  DEFAULT ((0)) FOR [TaxRate]
GO
ALTER TABLE [dbo].[ContractDetail] ADD  DEFAULT ((0)) FOR [TaxAmount]
GO
ALTER TABLE [dbo].[ContractDetail] ADD  DEFAULT ((0)) FOR [AmountWithTax]
GO
ALTER TABLE [dbo].[ContractDetail] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[ContractHandover] ADD  DEFAULT ((1)) FOR [HandoverType]
GO
ALTER TABLE [dbo].[ContractHandover] ADD  DEFAULT ((1)) FOR [HandoverStatus]
GO
ALTER TABLE [dbo].[ContractHandover] ADD  DEFAULT ((0)) FOR [SalesSignOff]
GO
ALTER TABLE [dbo].[ContractHandover] ADD  DEFAULT ((0)) FOR [DeliverySignOff]
GO
ALTER TABLE [dbo].[ContractHandover] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((1)) FOR [CustomerType]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((1)) FOR [CustomerLevel]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((1)) FOR [CreditStatus]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((1)) FOR [CustomerStatus]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((1)) FOR [IsPublic]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((0)) FOR [TotalOrderAmount]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((0)) FOR [TotalOrderCount]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Department] ADD  DEFAULT ((1)) FOR [DepartmentType]
GO
ALTER TABLE [dbo].[Department] ADD  DEFAULT ((1)) FOR [DepartmentLevel]
GO
ALTER TABLE [dbo].[Department] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Department] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Opportunity] ADD  DEFAULT ((1)) FOR [OpportunityType]
GO
ALTER TABLE [dbo].[Opportunity] ADD  DEFAULT ((0)) FOR [ExpectedAmount]
GO
ALTER TABLE [dbo].[Opportunity] ADD  DEFAULT ((30)) FOR [WinProbability]
GO
ALTER TABLE [dbo].[Opportunity] ADD  DEFAULT ((1)) FOR [OpportunityStage]
GO
ALTER TABLE [dbo].[Opportunity] ADD  DEFAULT ((0)) FOR [NeedBid]
GO
ALTER TABLE [dbo].[Opportunity] ADD  DEFAULT ((1)) FOR [OpportunityStatus]
GO
ALTER TABLE [dbo].[Opportunity] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Organization] ADD  DEFAULT ((1)) FOR [OrganizationType]
GO
ALTER TABLE [dbo].[Organization] ADD  DEFAULT ((1)) FOR [OrganizationLevel]
GO
ALTER TABLE [dbo].[Organization] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Organization] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Product] ADD  DEFAULT ((0)) FOR [UnitPrice]
GO
ALTER TABLE [dbo].[Product] ADD  DEFAULT ((0)) FOR [CostPrice]
GO
ALTER TABLE [dbo].[Product] ADD  DEFAULT ((0)) FOR [MinimumStock]
GO
ALTER TABLE [dbo].[Product] ADD  DEFAULT ((0)) FOR [CurrentStock]
GO
ALTER TABLE [dbo].[Product] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Product] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[ProjectApproval] ADD  DEFAULT ((0)) FOR [EstimatedAmount]
GO
ALTER TABLE [dbo].[ProjectApproval] ADD  DEFAULT ((0)) FOR [EstimatedCost]
GO
ALTER TABLE [dbo].[ProjectApproval] ADD  DEFAULT ((0)) FOR [EstimatedProfit]
GO
ALTER TABLE [dbo].[ProjectApproval] ADD  DEFAULT ((0)) FOR [ProfitMargin]
GO
ALTER TABLE [dbo].[ProjectApproval] ADD  DEFAULT ((1)) FOR [ApprovalStatus]
GO
ALTER TABLE [dbo].[ProjectApproval] ADD  DEFAULT ((1)) FOR [CurrentApprovalStage]
GO
ALTER TABLE [dbo].[ProjectApproval] ADD  DEFAULT ((0)) FOR [IsUrgent]
GO
ALTER TABLE [dbo].[ProjectApproval] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Quotation] ADD  DEFAULT ((0)) FOR [TotalAmount]
GO
ALTER TABLE [dbo].[Quotation] ADD  DEFAULT ((0)) FOR [TaxRate]
GO
ALTER TABLE [dbo].[Quotation] ADD  DEFAULT ((0)) FOR [TaxAmount]
GO
ALTER TABLE [dbo].[Quotation] ADD  DEFAULT ((0)) FOR [TotalAmountWithTax]
GO
ALTER TABLE [dbo].[Quotation] ADD  DEFAULT ('CNY') FOR [Currency]
GO
ALTER TABLE [dbo].[Quotation] ADD  DEFAULT ((1)) FOR [QuotationStatus]
GO
ALTER TABLE [dbo].[Quotation] ADD  DEFAULT ((1)) FOR [ApprovalStatus]
GO
ALTER TABLE [dbo].[Quotation] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[UserInfo] ADD  DEFAULT ((1)) FOR [UserType]
GO
ALTER TABLE [dbo].[UserInfo] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[UserInfo] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Bid]  WITH CHECK ADD  CONSTRAINT [FK_Bid_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[Bid] CHECK CONSTRAINT [FK_Bid_Customer]
GO
ALTER TABLE [dbo].[Bid]  WITH CHECK ADD  CONSTRAINT [FK_Bid_Opportunity] FOREIGN KEY([OpportunityId])
REFERENCES [dbo].[Opportunity] ([OpportunityId])
GO
ALTER TABLE [dbo].[Bid] CHECK CONSTRAINT [FK_Bid_Opportunity]
GO
ALTER TABLE [dbo].[Contract]  WITH CHECK ADD  CONSTRAINT [FK_Contract_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[Contract] CHECK CONSTRAINT [FK_Contract_Customer]
GO
ALTER TABLE [dbo].[Contract]  WITH CHECK ADD  CONSTRAINT [FK_Contract_Opportunity] FOREIGN KEY([OpportunityId])
REFERENCES [dbo].[Opportunity] ([OpportunityId])
GO
ALTER TABLE [dbo].[Contract] CHECK CONSTRAINT [FK_Contract_Opportunity]
GO
ALTER TABLE [dbo].[Contract]  WITH CHECK ADD  CONSTRAINT [FK_Contract_ProjectApproval] FOREIGN KEY([ProjectApprovalId])
REFERENCES [dbo].[ProjectApproval] ([ProjectApprovalId])
GO
ALTER TABLE [dbo].[Contract] CHECK CONSTRAINT [FK_Contract_ProjectApproval]
GO
ALTER TABLE [dbo].[ContractChange]  WITH CHECK ADD  CONSTRAINT [FK_ContractChange_Contract] FOREIGN KEY([ContractId])
REFERENCES [dbo].[Contract] ([ContractId])
GO
ALTER TABLE [dbo].[ContractChange] CHECK CONSTRAINT [FK_ContractChange_Contract]
GO
ALTER TABLE [dbo].[ContractDetail]  WITH CHECK ADD  CONSTRAINT [FK_ContractDetail_Contract] FOREIGN KEY([ContractId])
REFERENCES [dbo].[Contract] ([ContractId])
GO
ALTER TABLE [dbo].[ContractDetail] CHECK CONSTRAINT [FK_ContractDetail_Contract]
GO
ALTER TABLE [dbo].[ContractDetail]  WITH CHECK ADD  CONSTRAINT [FK_ContractDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[ContractDetail] CHECK CONSTRAINT [FK_ContractDetail_Product]
GO
ALTER TABLE [dbo].[ContractHandover]  WITH CHECK ADD  CONSTRAINT [FK_ContractHandover_Contract] FOREIGN KEY([ContractId])
REFERENCES [dbo].[Contract] ([ContractId])
GO
ALTER TABLE [dbo].[ContractHandover] CHECK CONSTRAINT [FK_ContractHandover_Contract]
GO
ALTER TABLE [dbo].[ContractHandover]  WITH CHECK ADD  CONSTRAINT [FK_ContractHandover_DeliveryTeamLeader] FOREIGN KEY([DeliveryTeamLeaderId])
REFERENCES [dbo].[UserInfo] ([UserId])
GO
ALTER TABLE [dbo].[ContractHandover] CHECK CONSTRAINT [FK_ContractHandover_DeliveryTeamLeader]
GO
ALTER TABLE [dbo].[ContractHandover]  WITH CHECK ADD  CONSTRAINT [FK_ContractHandover_SalesRepresentative] FOREIGN KEY([SalesRepresentativeId])
REFERENCES [dbo].[UserInfo] ([UserId])
GO
ALTER TABLE [dbo].[ContractHandover] CHECK CONSTRAINT [FK_ContractHandover_SalesRepresentative]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_OwnerUser] FOREIGN KEY([OwnerUserId])
REFERENCES [dbo].[UserInfo] ([UserId])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_OwnerUser]
GO
ALTER TABLE [dbo].[Department]  WITH CHECK ADD  CONSTRAINT [FK_Department_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([OrganizationId])
GO
ALTER TABLE [dbo].[Department] CHECK CONSTRAINT [FK_Department_Organization]
GO
ALTER TABLE [dbo].[Department]  WITH CHECK ADD  CONSTRAINT [FK_Department_ParentDepartment] FOREIGN KEY([ParentDepartmentId])
REFERENCES [dbo].[Department] ([DepartmentId])
GO
ALTER TABLE [dbo].[Department] CHECK CONSTRAINT [FK_Department_ParentDepartment]
GO
ALTER TABLE [dbo].[Opportunity]  WITH CHECK ADD  CONSTRAINT [FK_Opportunity_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[Opportunity] CHECK CONSTRAINT [FK_Opportunity_Customer]
GO
ALTER TABLE [dbo].[Opportunity]  WITH CHECK ADD  CONSTRAINT [FK_Opportunity_SalesOwnerUser] FOREIGN KEY([SalesOwnerUserId])
REFERENCES [dbo].[UserInfo] ([UserId])
GO
ALTER TABLE [dbo].[Opportunity] CHECK CONSTRAINT [FK_Opportunity_SalesOwnerUser]
GO
ALTER TABLE [dbo].[Organization]  WITH CHECK ADD  CONSTRAINT [FK_Organization_ParentOrganization] FOREIGN KEY([ParentOrganizationId])
REFERENCES [dbo].[Organization] ([OrganizationId])
GO
ALTER TABLE [dbo].[Organization] CHECK CONSTRAINT [FK_Organization_ParentOrganization]
GO
ALTER TABLE [dbo].[ProjectApproval]  WITH CHECK ADD  CONSTRAINT [FK_ProjectApproval_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[ProjectApproval] CHECK CONSTRAINT [FK_ProjectApproval_Customer]
GO
ALTER TABLE [dbo].[ProjectApproval]  WITH CHECK ADD  CONSTRAINT [FK_ProjectApproval_Opportunity] FOREIGN KEY([OpportunityId])
REFERENCES [dbo].[Opportunity] ([OpportunityId])
GO
ALTER TABLE [dbo].[ProjectApproval] CHECK CONSTRAINT [FK_ProjectApproval_Opportunity]
GO
ALTER TABLE [dbo].[Quotation]  WITH CHECK ADD  CONSTRAINT [FK_Quotation_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[Quotation] CHECK CONSTRAINT [FK_Quotation_Customer]
GO
ALTER TABLE [dbo].[Quotation]  WITH CHECK ADD  CONSTRAINT [FK_Quotation_Opportunity] FOREIGN KEY([OpportunityId])
REFERENCES [dbo].[Opportunity] ([OpportunityId])
GO
ALTER TABLE [dbo].[Quotation] CHECK CONSTRAINT [FK_Quotation_Opportunity]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标编号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'OpportunityId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'OpportunityCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CustomerId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CustomerName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标项目名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidProjectName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标项目编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidProjectCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标单位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'TenderOrganization'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'TenderAddress'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开标日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidOpeningDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标截止日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidSubmissionDeadline'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标保证金金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidBondAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标保证金是否已支付' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidBondPaid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保证金支付时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidBondPaidTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否需要履约保证金' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'PerformanceBondRequired'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'履约保证金比例' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'PerformanceBondRate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术得分' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'TechnicalScore'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商务得分' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CommercialScore'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'总分' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'TotalScore'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'Ranking'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'竞争对手信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'Competitors'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'我方优势' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'OurAdvantages'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'我方劣势' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'OurDisadvantages'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标状态：1-准备中，2-已提交，3-已开标，4-中标，5-未中标，6-废标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-未提交，2-审批中，3-已通过，4-已驳回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalFlowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ApprovalComment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'中标通知书时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'WinNoticeTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'中标通知书编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'WinNoticeNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投标文件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'BidDocumentPath'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'其他附件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'OtherAttachments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售投标表，记录招投标项目的投标信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Bid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项ID，外键关联ProjectApproval表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ProjectApprovalId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ProjectApprovalCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'OpportunityId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'OpportunityCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CustomerId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CustomerName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同类型：1-销售合同，2-服务合同，3-框架协议，4-补充协议' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'TaxRate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'TaxAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'含税合同金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractAmountWithTax'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'币种，默认CNY（人民币）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'Currency'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'签订日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'SignDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'生效日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'EffectiveDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到期日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ExpiryDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同期限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractPeriod'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'PaymentMethod'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'DeliveryRequirements'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质量要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'QualityRequirements'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质保期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'WarrantyPeriod'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractTerms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'特殊约定' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'SpecialAgreements'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同状态：1-草稿，2-审批中，3-已生效，4-履行中，5-已完成，6-已终止，7-已作废' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-未提交，2-审批中，3-已通过，4-已驳回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalFlowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ApprovalComment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同文件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractFilePath'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同扫描件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ContractScanPath'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否模板合同' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'IsTemplate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'父合同ID（用于补充协议）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ParentContractId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售合同表，记录正式签订的合同信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Contract'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同变更ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ContractChangeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更单号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ContractChangeCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID，外键关联Contract表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ContractId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ContractCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更类型：1-金额变更，2-范围变更，3-时间变更，4-条款变更，5-综合变更' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更原因' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeReason'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeContent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原合同金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'OriginalAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更金额，正数为增加，负数为减少' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新合同金额（原合同金额+变更金额）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'NewAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原到期日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'OriginalExpiryDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新到期日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'NewExpiryDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更协议文件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeAgreementPath'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户是否确认' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CustomerConfirm'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CustomerConfirmTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CustomerConfirmPerson'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认文件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CustomerConfirmDocumentPath'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-草稿，2-审批中，3-已通过，4-已驳回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalFlowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ApprovalComment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更状态：1-待处理，2-处理中，3-已完成，4-已取消' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ChangeStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ImplementTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ImplementUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ImplementUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施结果' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ImplementResult'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同变更表，记录合同的变更申请和审批信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractChange'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同明细ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ContractDetailId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID，外键关联Contract表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ContractId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ContractCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'行号，同一合同内唯一' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'LineNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品ID，外键关联Product表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ProductId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ProductCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ProductName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品规格' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ProductSpecification'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'计量单位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'Unit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'UnitPrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'金额（数量×单价）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'Amount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'TaxRate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'TaxAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'含税金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'AmountWithTax'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'要求交货日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'DeliveryDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交货地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'DeliveryAddress'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术参数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'TechnicalParameters'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质量标准' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'QualityStandard'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同明细表，记录合同的产品明细信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractDetail'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同交底ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ContractHandoverId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同ID，外键关联Contract表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ContractId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ContractCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底单号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底类型：1-首次交底，2-变更交底' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底地点' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售代表ID，外键关联User表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'SalesRepresentativeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售代表姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'SalesRepresentativeName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付团队负责人ID，外键关联User表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'DeliveryTeamLeaderId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付团队负责人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'DeliveryTeamLeaderName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参与人员，可存储多个人员信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'Participants'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关键合同条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'KeyContractTerms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户特殊要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CustomerSpecialRequirements'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'TechnicalRequirements'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'DeliveryRequirements'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'质量要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'QualityRequirements'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'PaymentTerms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险点' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'RiskPoints'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险应对措施' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'RiskMitigationMeasures'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'重要注意事项' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ImportantNotes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'附件路径，JSON格式存储多个附件信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'Attachments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底状态：1-已安排，2-进行中，3-已完成，4-已取消' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'完成时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CompletionTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售签字确认' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'SalesSignOff'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售签字时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'SalesSignOffTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付签字确认' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'DeliverySignOff'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交付签字时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'DeliverySignOffTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交底评价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'HandoverEvaluation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'后续行动' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'FollowUpActions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合同交底表，记录合同签署后的交底信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ContractHandover'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户编码，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户类型：1-企业，2-个人，3-政府，4-其他' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户来源（如：展会、网络搜索、老客户介绍等）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerSource'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属行业' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'Industry'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ContactPerson'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ContactPhone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系邮箱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ContactEmail'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'Address'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户等级：1-A级（重要），2-B级（一般），3-C级（潜在）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerLevel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'信用状态：1-良好，2-一般，3-受限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreditStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户状态：1-公海客户，2-已分配待跟进，3-正式客户，4-流失客户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CustomerStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否公海客户：1-是，0-否' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'IsPublic'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人用户ID，如果为空则表示在公海中' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'OwnerUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'OwnerUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'OwnerOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'OwnerOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近联系时间，用于判断客户是否活跃' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'RecentContactTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'累计订单金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'TotalOrderAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'累计订单数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'TotalOrderCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后下单时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'LastOrderTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户信息表，统一管理公海客户和正式客户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Customer'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门编码，唯一标识部门的代码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级部门ID，自关联外键，指向父级部门' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ParentDepartmentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级部门编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ParentDepartmentCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级部门名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ParentDepartmentName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属组织ID，外键关联到组织表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'OrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属组织编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'OrganizationCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'OrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门类型：1-销售部，2-技术部，3-财务部，4-人事部，5-行政部，6-其他' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门层级，从1开始计数，1为一级部门' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentLevel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门路径，显示部门的完整层级路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'DepartmentPath'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门负责人用户ID，关联用户表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ManagerUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门负责人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ManagerUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门联系电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ContactPhone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门描述，详细说明部门职能和职责' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：1-启用，0-停用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间，默认当前时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键约束，部门ID为主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'CONSTRAINT',@level2name=N'PK_Department'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一约束，确保部门编码唯一' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'CONSTRAINT',@level2name=N'UK_Department_DepartmentCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门表，存储公司部门组织架构信息，支持多级部门管理' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'外键约束，关联组织表，确保所属组织存在' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'CONSTRAINT',@level2name=N'FK_Department_Organization'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'外键约束，部门自关联，确保上级部门存在' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Department', @level2type=N'CONSTRAINT',@level2name=N'FK_Department_ParentDepartment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CustomerId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CustomerName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CustomerType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机类型：1-新客户商机，2-老客户商机' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目背景' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ProjectBackground'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户需求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CustomerNeed'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预计金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ExpectedAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预计成交日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ExpectedClosingDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'赢单概率，取值范围0-100' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'WinProbability'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'竞争对手信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CompetitorInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机阶段：1-初步接触，2-需求分析，3-方案制定，4-报价中，5-谈判中，6-赢单，7-丢单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityStage'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否需要投标：0-否（走报价流程），1-是（走投标流程）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'NeedBid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'招标项目编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'BidProjectCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人用户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'SalesOwnerUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'SalesOwnerUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'SalesOwnerOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售负责人组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'SalesOwnerOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'协同销售人员ID列表，多个ID用逗号分隔' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CoSalesUserIds'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'协同销售人员姓名列表，多个姓名用逗号分隔' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CoSalesUserNames'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前任务' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CurrentTask'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'下一步计划' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'NextStep'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机状态：1-进行中，2-已关闭，3-已取消' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'OpportunityStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关闭原因' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CloseReason'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'关闭时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CloseTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机信息表，记录销售机会的全过程' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Opportunity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织编码，唯一标识组织的代码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级组织ID，自关联外键，指向父级组织' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ParentOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级组织编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ParentOrganizationCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ParentOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织类型：1-公司，2-事业部，3-部门，4-其他' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织层级，从1开始计数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationLevel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织路径，显示组织的完整层级路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'OrganizationPath'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织联系人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ContactPerson'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ContactPhone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'Address'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织描述，详细说明组织职能和职责' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：1-启用，0-停用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间，默认当前时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键约束，组织ID为主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'CONSTRAINT',@level2name=N'PK_Organization'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一约束，确保组织编码唯一' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'CONSTRAINT',@level2name=N'UK_Organization_OrganizationCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织架构表，存储公司组织架构信息，支持多级组织管理' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'外键约束，组织自关联，确保上级组织存在' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Organization', @level2type=N'CONSTRAINT',@level2name=N'FK_Organization_ParentOrganization'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品编码，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品分类' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductCategory'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品规格描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ProductSpecification'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'计量单位（如：个、台、套）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'Unit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标准单价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'UnitPrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'成本价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CostPrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最低库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'MinimumStock'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CurrentStock'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：1-启用，0-停用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品信息表，存储所有可销售的产品数据' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Product'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ProjectApprovalId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'立项编号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ProjectApprovalCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用类型：1-报价，2-投标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ReferenceType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用ID，根据ReferenceType对应QuotationId或BidId' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ReferenceId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引用编号，根据ReferenceType对应QuotationCode或BidCode' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ReferenceCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'OpportunityId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'OpportunityCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CustomerId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CustomerName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ProjectName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目背景' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ProjectBackground'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户需求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CustomerDemand'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'解决方案概述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'SolutionOverview'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预估金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'EstimatedAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预估成本' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'EstimatedCost'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'预估利润' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'EstimatedProfit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'毛利率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ProfitMargin'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'资源需求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ResourceRequirements'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实施周期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ImplementationPeriod'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险分析' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'RiskAnalysis'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'风险应对措施' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'RiskMitigationMeasures'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售部门意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'SalesDepartmentOpinion'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'技术部门意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'TechnicalDepartmentOpinion'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'财务部门意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'FinanceDepartmentOpinion'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'管理层意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ManagementOpinion'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-草稿，2-销售审批中，3-技术审批中，4-财务审批中，5-管理层审批中，6-已通过，7-已驳回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ApprovalStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前审批阶段' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CurrentApprovalStage'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ApprovalFlowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最终审批人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'FinalApproverUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最终审批人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'FinalApproverUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最终审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'FinalApprovalTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ApprovalComment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否加急' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'IsUrgent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'加急原因' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'UrgentReason'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售立项表，记录内部评审和立项信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProjectApproval'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'QuotationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价单号，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'QuotationCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机ID，外键关联Opportunity表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'OpportunityId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商机编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'OpportunityCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户ID，外键关联Customer表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CustomerId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CustomerName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'QuotationDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'有效期至' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ValidUntilDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价总金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'TotalAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'TaxRate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'TaxAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'含税总金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'TotalAmountWithTax'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'币种，默认CNY（人民币）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'Currency'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'PaymentTerms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交货条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'DeliveryTerms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保修条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'WarrantyTerms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'特殊条款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'SpecialTerms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报价状态：1-草稿，2-已提交，3-客户确认中，4-客户已确认，5-客户拒绝，6-已过期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'QuotationStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批状态：1-未提交，2-审批中，3-已通过，4-已驳回' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalFlowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ApprovalComment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户反馈' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CustomerFeedback'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户确认时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CustomerConfirmTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'下次跟进时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'NextFollowUpTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售报价表，记录普通项目的报价信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Quotation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户ID（主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户编码，唯一标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'UserCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登录名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'LoginName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码哈希值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'PasswordHash'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮箱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'Email'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'Phone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'DepartmentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'DepartmentName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'职位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'Position'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户类型：1-销售，2-管理员，3-审核人，4-交付人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'UserType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：1-启用，0-停用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后登录时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'LastLoginTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'CreateOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'CreateOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'ModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'ModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'ModifyUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人所在组织名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo', @level2type=N'COLUMN',@level2name=N'ModifyOrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户信息表，存储系统用户数据' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserInfo'
GO
