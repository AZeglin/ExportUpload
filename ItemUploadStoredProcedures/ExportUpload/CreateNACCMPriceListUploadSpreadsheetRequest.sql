IF EXISTS ( SELECT * FROM sysobjects WHERE type = 'P' AND name = 'CreateNACCMPriceListUploadSpreadsheetRequest' )
BEGIN
	DROP PROCEDURE CreateNACCMPriceListUploadSpreadsheetRequest
END
GO

CREATE PROCEDURE CreateNACCMPriceListUploadSpreadsheetRequest
(
@CurrentUser uniqueidentifier,
@ContractNumber nvarchar(20),
@ContractId int,
@ScheduleNumber int,
@EncodedCriteria char(20),
@SourceFileType nchar(2),
@SourceFileExt nvarchar(10),
@SourceFileName nvarchar(255),
@SourceFilePrefix nvarchar(255),
@SourceFilePathOnly nvarchar(255),
@SourceFilePathAndName nvarchar(255),
@UploadType nchar(1),  --  'M' or 'P'
@RequestingServerName nvarchar(40),
@ActivityId int,
@ActivityDetailsId int,
@ApplicationVersion char(8),
@GuidForCurrentModification uniqueidentifier,
@DestFileType nchar(2) output,
@PotentialErrorFilePathAndName  nvarchar(255) output,                 
@RequestId int output,
@CreationDate datetime output
)

AS

Declare 	@error int,
		@rowCount int,
		@errorMsg nvarchar(1000),
	
		@result int,
		@currentdate varchar(20),
		@loginName nvarchar(120),
		@PotentialErrorFileName nvarchar(255)
		


BEGIN TRANSACTION


	EXEC DrugItem.dbo.GetLoginNameFromUserIdLocalProc @CurrentUser, @loginName OUTPUT 
	SELECT @error = @@error		
	IF @error <> 0 or @loginName is null
	BEGIN
		SELECT @errorMsg = 'Error getting login name for UserId ' + convert(nvarchar(120), @CurrentUser )
		GOTO ERROREXIT
	END	

	
	SET @currentdate = 	
				substring(Convert(Varchar(10), getdate() ,20),6,2) +
				substring(Convert(Varchar(10), getdate() ,20),9,2) +
				substring(Convert(Varchar(10), getdate() ,20),1,4) + 
											'_' +
				substring(Convert(Varchar(10), getdate() ,108),1,2)+
				substring(Convert(Varchar(10), getdate() ,108),4,2)+
				substring(Convert(Varchar(10), getdate() ,108),7,2)
				
	select @PotentialErrorFileName = @SourceFilePrefix + '_Error' + @SourceFileExt
	select @PotentialErrorFilePathAndName = @SourceFilePathOnly + @PotentialErrorFileName

	IF @UploadType = 'M'
	BEGIN
		select @DestFileType = 'XL'
		select @CreationDate = getdate()
	
		-- create an upload request which will be picked up by the service
		insert into EU_Requests
		( RequestStatus, RequestingServerName, RequestType, RequestSubType, CreatedBy, CreationDate, ApplicationVersion, ActivityId, ActivityDetailsId, GuidForCurrentModification, ContractNumber, ContractId, ScheduleNumber, EncodedCriteria, SourceFileType, SourceFileExt, SourceFileName, SourceFilePathName, 
					DestFileType, DestFileExt, DestFileName, DestFilePathName, StatusMessage, CompletionLevel, LastModificationDate )
		values
		(
			'UR', @RequestingServerName, 'U', 'M', @CurrentUser, @CreationDate, @ApplicationVersion, @ActivityId, @ActivityDetailsId, @GuidForCurrentModification, @ContractNumber, @ContractId, @ScheduleNumber, @EncodedCriteria, @SourceFileType, @SourceFileExt, @SourceFileName, @SourceFilePathAndName, 
			@DestFileType, '.xlsx', @PotentialErrorFileName, @PotentialErrorFilePathAndName, 'Request Queued', 0, getdate()
		)

		select @error = @@ERROR, @RequestId = SCOPE_IDENTITY()

		if @@error <> 0
		BEGIN
			SELECT @errorMsg = 'Failed to queue request.' 			
			SET @RequestId = -1

			update EU_Activity
			set ExportUploadStatus = 'RF'  -- request failed
			where ActivityId = @ActivityId

			GOTO ERROREXIT
		END
		else
		BEGIN
			update EU_Activity
			set ExportUploadStatus = 'UR'
			where ActivityId = @ActivityId
		END	
	END
	ELSE
	BEGIN	
		select @DestFileType = 'XL'
			
	    insert into EU_Requests
		( RequestStatus, RequestingServerName, RequestType, RequestSubType, CreatedBy, CreationDate, ApplicationVersion, ActivityId, ActivityDetailsId, GuidForCurrentModification, ContractNumber, ContractId, ScheduleNumber, SourceFileType, SourceFileExt, SourceFileName, SourceFilePathName, 
			DestFileType, DestFileExt, DestFileName, DestFilePathName, StatusMessage, CompletionLevel, LastModificationDate )
		values
		(
			'UR', @RequestingServerName, 'U', 'P', @CurrentUser, getdate(), @ApplicationVersion, @ActivityId, @ActivityDetailsId, @GuidForCurrentModification, @ContractNumber, @ContractId, @ScheduleNumber, @SourceFileType, @SourceFileExt, @SourceFileName, @SourceFilePathAndName, 
			@DestFileType, '.xlsx', @PotentialErrorFileName, @PotentialErrorFilePathAndName, 'Request Queued', 0, getdate()			
		)

		select @error = @@ERROR, @RequestId = SCOPE_IDENTITY()

		if @@error <> 0
		BEGIN
			SELECT @errorMsg = 'Failed to queue request.' 			
			SET @RequestId = -1

			update EU_Activity
			set ExportUploadStatus = 'RF'  -- request failed
			where ActivityId = @ActivityId

			GOTO ERROREXIT
		END
		else
		BEGIN
			update EU_Activity
			set ExportUploadStatus = 'UR'
			where ActivityId = @ActivityId
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


