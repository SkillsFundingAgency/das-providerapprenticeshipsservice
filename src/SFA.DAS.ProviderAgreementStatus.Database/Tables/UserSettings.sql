CREATE TABLE [dbo].[UserSettings]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[UserId] BIGINT NOT NULL,
	[UserRef] varchar(255) NOT NULL,
	[ReceiveNotifications] BIT NOT NULL DEFAULT(1),
	CONSTRAINT [FK_UserAccountSettings_UserId] FOREIGN KEY(UserId) REFERENCES [dbo].[User] ([Id])
)
