CREATE TABLE [dbo].[ContractFeedEvent]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY,
	[ProviderId] BigInt NOT NULL,
	[HierarchyType] NVARCHAR(50) NOT NULL,
	[FundingTypeCode] NVARCHAR(50) NOT NULL,
	[Status] NVARCHAR(50) NOT NULL,
	[ParentStatus] NVARCHAR(50) NOT NULL,
	[UpdatedInFeed] DateTime NOT NULL,
    [CreatedDate] DATETIME NOT NULL
)
