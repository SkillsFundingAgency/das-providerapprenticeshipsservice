CREATE TABLE [dbo].[ContractFeedEventRun]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT newsequentialid(),
	[LatestBookmark] UNIQUEIDENTIFIER NOT NULL,
	[UpdatedDate] DateTime NOT NULL
)
