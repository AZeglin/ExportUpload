IF EXISTS ( SELECT * FROM sysobjects WHERE type = 'P' AND name = 'CreateExportUploadActivity' )
BEGIN
	DROP PROCEDURE CreateExportUploadActivity
END
GO

CREATE PROCEDURE CreateExportUploadActivity
(
@UserLogin nvarchar(120),
@UserId uniqueidentifier,
@ContractNumber nvarchar(20),
@ActivityType nchar(1),
@ActivityDataType nchar(1),
@SpreadsheetFileName nvarchar(255),
@ChangeId uniqueidentifier = null,
@ModificationNumber nvarchar(20) = null,
@ActivityId int OUTPUT,
@ActivityDetailsId int OUTPUT
)

AS

Declare 	@error int,
		@rowCount int,
		@errorMsg nvarchar(1000),
		@exportUploadStatus nchar(2)


BEGIN TRANSACTION

	select @ActivityId = -1
	select @ActivityDetailsId = -1

	if @ActivityType = 'U'
	BEGIN
		select @exportUploadStatus = 'UI'
	END
	else
	BEGIN
		select @exportUploadStatus = 'EI'
	END

	insert into EU_Activity
	( UserId, CreatedBy, CreationDate, ActivityType, ActivityDataType, ContractNumber, SpreadsheetFileName, ExportUploadStatus )
	values
	( @UserId, @UserLogin, GETDATE(), @ActivityType, @ActivityDataType, @ContractNumber, @SpreadsheetFileName, @exportUploadStatus )


	select @error = @@ERROR, @rowCount = @@ROWCOUNT, @ActivityId = @@IDENTITY
	if @error <> 0 or @rowCount <> 1
	BEGIN
		select @errorMsg = 'Error inserting export/upload activity for contract ' + @ContractNumber
		goto ERROREXIT
	END

	-- medsurg upload
	if @ActivityType = 'U' and @ActivityDataType = 'M'
	BEGIN

		insert into EU_MedSurgActivityDetails
		( ActivityId, ChangeId, ModificationNumber )
		values
		( @ActivityId, @ChangeId, @ModificationNumber )

		select @error = @@ERROR, @rowCount = @@ROWCOUNT, @ActivityDetailsId = @@IDENTITY
		if @error <> 0 or @rowCount <> 1
		BEGIN
			select @errorMsg = 'Error inserting medsurg export/upload activity details for contract ' + @ContractNumber
			goto ERROREXIT
		END
	END
	-- pharm upload
	else if @ActivityType = 'U' and @ActivityDataType = 'P'
	BEGIN

		insert into EU_PharmaceuticalActivityDetails
		( ActivityId, ChangeId, ModificationNumber )
		values
		( @ActivityId, @ChangeId, @ModificationNumber )

		select @error = @@ERROR, @rowCount = @@ROWCOUNT, @ActivityDetailsId = @@IDENTITY
		if @error <> 0 or @rowCount <> 1
		BEGIN
			select @errorMsg = 'Error inserting pharmaceutical export/upload activity details for contract ' + @ContractNumber
			goto ERROREXIT
		END
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


