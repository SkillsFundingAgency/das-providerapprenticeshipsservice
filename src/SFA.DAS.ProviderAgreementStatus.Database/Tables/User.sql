CREATE TABLE [dbo].[User]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY,
	[UserRef] varchar(255) NOT NULL,
	[DisplayName] varchar(255) NOT NULL,
	[Ukprn] BIGINT NOT NULL,
	[Email] varchar(255) NOT NULL,
	[IsDeleted] BIT NOT NULL DEFAULT 0, 
    [UserType] NCHAR(10) NOT NULL DEFAULT 0, 
    [LastLogin] DATETIME2 NULL
)
GO

CREATE UNIQUE INDEX [IX_User_UserRef] ON [dbo].[User] ([UserRef])
GO