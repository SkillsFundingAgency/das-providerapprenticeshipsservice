CREATE TABLE [dbo].[ContractFeedEvent]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY,
	[ProviderId] BigInt NOT NULL,
	[HierarchyType] NVARCHAR(50),
	[FundingTypeCode] NVARCHAR(50),
	[Status] NVARCHAR(50),
	[ParentStatus] NVARCHAR(50),
	[Updated] DateTime,
)
