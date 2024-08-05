IF EXISTS ( SELECT * FROM sysobjects WHERE type = 'P' AND name = 'UpdateExportUploadActivityInfo' )
BEGIN
	DROP PROCEDURE UpdateExportUploadActivityInfo
END
GO

CREATE PROCEDURE UpdateExportUploadActivityInfo
(
@ActivityId int,
@SpreadsheetFileName nvarchar(255)
)

AS

Declare 	@error int,
		@rowCount int,
		@errorMsg nvarchar(1000)

BEGIN TRANSACTION

	update EU_Activity
	set SpreadsheetFileName = @SpreadsheetFileName
	where ActivityId = @ActivityId

	select @error = @@ERROR, @rowCount = @@ROWCOUNT
	if @error <> 0 or @rowCount <> 1
	BEGIN
		select @errorMsg = 'Error updating export/upload activity info for activity id =  ' + convert( nvarchar(20), @ActivityId )
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


