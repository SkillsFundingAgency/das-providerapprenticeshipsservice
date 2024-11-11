CREATE PROCEDURE [dbo].[SyncIdamsUsers]
    @Ukprn  bigint,
    @users [dbo].[IdamsUsers] READONLY
AS
    -- Delete Users who are NOT in the IDAMS list for Provider
    UPDATE [dbo].[User]
    SET [IsDeleted] = 1
    WHERE Ukprn = @Ukprn AND [Email] NOT IN (SELECT Email FROM @users)

	-- Undelete Users who are in the list for provider 
    UPDATE [dbo].[User]
    SET [IsDeleted] = 0
	FROM @users n 
	INNER JOIN [dbo].[User] u ON n.[Email] = u.[Email]
    WHERE u.Ukprn = @Ukprn