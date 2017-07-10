﻿CREATE PROCEDURE [dbo].[UpsertUser]
	@userRef VARCHAR(255),
	@displayName VARCHAR(255),
	@ukprn BIGINT,
	@email VARCHAR(255)
AS
	
	MERGE [dbo].[User] as [Target]
	USING (SELECT @userRef as UserRef) AS [SOURCE]
	ON [Target].UserRef = [Source].UserRef
	WHEN MATCHED THEN UPDATE SET [Target].DisplayName = @displayName, [Target].Ukprn = @ukprn, [Target].Email = @email
	WHEN NOT MATCHED THEN INSERT (UserRef, DisplayName, Ukprn, Email) VALUES (@userRef, @displayName, @ukprn, @email);