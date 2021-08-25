﻿CREATE PROCEDURE [dbo].[UpsertUser]
	@userRef VARCHAR(255),
	@displayName VARCHAR(255),
	@ukprn BIGINT,
	@email VARCHAR(255),
	@isDeleted BIT = 0,
	@serviceClaim nchar(10)
AS
	
	MERGE [dbo].[User] as [Target]
	USING (SELECT @userRef as UserRef) AS [SOURCE]
	ON [Target].UserRef = [Source].UserRef
	WHEN MATCHED THEN UPDATE SET [Target].DisplayName = @displayName, [Target].Ukprn = @ukprn, [Target].Email = @email, [Target].ServiceClaim = @serviceClaim, [Target].IsDeleted = 0, [Target].LastLogin = GETDATE()
	WHEN NOT MATCHED THEN INSERT (UserRef, DisplayName, Ukprn, Email, IsDeleted, LastLogin, ServiceClaim) VALUES (@userRef, @displayName, @ukprn, @email, @isDeleted, GETDATE(), @serviceClaim);
