CREATE TABLE [dbo].[ContractFeedEventRun]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT newsequentialid(),
	[EntriesSaved] Int NOT NULL,
	[ExecutionTimeMs] Int NOT NULL,
	[PageNumber] Int NOT NULL,
	[PagesRead] Int Not Null,
	[Updated] DateTime NOT NULL
)
