IF EXISTS ( SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetItemUploadHistoryForUpload' )
BEGIN
	DROP PROCEDURE GetItemUploadHistoryForUpload
END
GO

CREATE PROCEDURE GetItemUploadHistoryForUpload
(
@UserLogin nvarchar(120),
@UserId uniqueidentifier,
@ContractNumber nvarchar(20),
@StartDate datetime,
@EndDate datetime
)

AS

Declare 	@error int,
		@errorMsg nvarchar(1000)



BEGIN TRANSACTION

	select  ActivityId, UserId, CreatedBy, CreationDate, ActivityType, ActivityDataType, ContractNumber, SpreadsheetFileName, ExportUploadStatus
	from EU_Activity
	where ContractNumber = @ContractNumber
	and CreationDate between @StartDate and DATEADD( dd, 1, @EndDate )  -- want to include today's activity
	and ActivityType = 'U'

	select @error = @@ERROR
	if @error <> 0 
	BEGIN
		select @errorMsg = 'Error retrieving activity for contract ' + @ContractNumber
		goto ERROREXIT
	END


goto OKEXIT

ERROREXIT:

	raiserror( @errorMsg, 16, 1 )
	if @@TRANCOUNT > 1
	BEGIN
		COMMIT TRANSACTION
	END
	Else if @@TRANCOUNT = 1
	BEGIN
		/* only rollback iff this is the highest level */
		ROLLBACK TRANSACTION
	END

	RETURN( -1 )

OKEXIT:

	If @@TRANCOUNT > 0
	BEGIN
		COMMIT TRANSACTION
	END
	RETURN( 0 )


