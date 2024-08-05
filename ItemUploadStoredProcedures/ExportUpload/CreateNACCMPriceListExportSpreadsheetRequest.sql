IF EXISTS ( SELECT * FROM sysobjects WHERE type = 'P' AND name = 'CreateNACCMPriceListExportSpreadsheetRequest' )
BEGIN
	DROP PROCEDURE CreateNACCMPriceListExportSpreadsheetRequest
END
GO

CREATE PROCEDURE CreateNACCMPriceListExportSpreadsheetRequest
(
@CurrentUser uniqueidentifier,
@ContractNumber nvarchar(20),
@ContractId int,
@ScheduleNumber int,
@ExportType nchar(1),  --  'M' or 'P'									              
@RequestingServerName nvarchar(40),
@ActivityId int,  
@StartDate nvarchar(10),
@EndDate nvarchar(10),
@EncodedCriteria char(20),
@ApplicationVersion char(8),
@DestFileType nchar(2) output,
@DestFileName nvarchar(255) output,
@RequestId int output,
@CreationDate datetime output
)

AS

Declare 	@error int,
		@rowCount int,
		@errorMsg nvarchar(1000),		
		@currentdate varchar(20),
		@loginName nvarchar(120)
		
		
BEGIN TRANSACTION


	EXEC  dbo.GetLoginNameFromUserIdLocalProc @CurrentUser, @loginName OUTPUT 
	SELECT @error = @@error		
	IF @error <> 0 or @loginName is null
	BEGIN
		SELECT @errorMsg = 'Error getting login name for UserId ' + convert(nvarchar(120), @CurrentUser )
		GOTO ERROREXIT
	END	


	IF (Len(@StartDate) > 0 and @StartDate is not null and isdate(@StartDate) = 0) OR
	   (LEN(@EndDate) > 0 and @EndDate is not null and isdate(@EndDate) = 0)
	BEGIN
		SELECT @errorMsg = 'Start Date or End Date is not a valid date' 
		SET @DestFileName = null
		SET @RequestId = -1
		GOTO ERROREXIT	
	END

	If (Len(@StartDate) = 0 or @StartDate is  null) 
	Begin
		Set @StartDate = '01/01/1900'
	End
	
	If (Len(@EndDate) = 0 or @EndDate is  null) 
	Begin
		Set @EndDate = '01/01/1900'
	End	
	
	select @CreationDate = getdate()

	SET @currentdate = 	
				substring(Convert(Varchar(10), @CreationDate ,20),6,2) +
				substring(Convert(Varchar(10), @CreationDate ,20),9,2) +
				substring(Convert(Varchar(10), @CreationDate ,20),1,4) + 
											'_' +
				substring(Convert(Varchar(10), @CreationDate ,108),1,2)+
				substring(Convert(Varchar(10), @CreationDate ,108),4,2)+
				substring(Convert(Varchar(10), @CreationDate ,108),7,2)
				

	IF @ExportType = 'M'
	BEGIN
		SET @DestFileName =  @ContractNumber + '_PriceList_' + @currentdate + '.xlsx'
		select @DestFileType = 'XL'
		
	
		-- create an export request which will be picked up by the service
		insert into EU_Requests
		( RequestStatus, RequestingServerName, RequestType, RequestSubType, CreatedBy, CreationDate, ApplicationVersion, ActivityId, ContractNumber, ContractId, ScheduleNumber, SourceFileType, SourceFileExt, SourceFileName, SourceFilePathName, 
					DestFileType, DestFileExt, DestFileName, DestFilePathName, StatusMessage, CompletionLevel, LastModificationDate )
		values
		(
			'ER', @RequestingServerName, 'E', 'M', @CurrentUser, @CreationDate, @ApplicationVersion, @ActivityId, @ContractNumber, @ContractId, @ScheduleNumber, '', '', '', '', 
			@DestFileType, '.xlsx', @DestFileName, '', 'Request Queued', 0, getdate()
		)

		select @error = @@ERROR, @RequestId = SCOPE_IDENTITY()

		if @@error <> 0
		BEGIN
			SELECT @errorMsg = 'Failed to queue request.' 
			SET @DestFileName = null
			SET @RequestId = -1

			update EU_Activity
			set ExportUploadStatus = 'RF'  -- request failed
			where ActivityId = @ActivityId

			GOTO ERROREXIT
		END
		else
		BEGIN
			update EU_Activity
			set ExportUploadStatus = 'ER'
			where ActivityId = @ActivityId
		END	
	END
	ELSE
	BEGIN
		
		SET @DestFileName =  @ContractNumber + '_PriceList_' + @currentdate + '.xlsx'
		select @DestFileType = 'XL'		
			
	    insert into EU_Requests
		( RequestStatus, RequestingServerName, RequestType, RequestSubType, CreatedBy, CreationDate, ApplicationVersion, ActivityId, ContractNumber, ContractId, ScheduleNumber, StartDateCriteria, EndDateCriteria, EncodedCriteria, SourceFileType, SourceFileExt, SourceFileName, SourceFilePathName, 
			DestFileType, DestFileExt, DestFileName, DestFilePathName, StatusMessage, CompletionLevel, LastModificationDate )
		values
		(
			'ER', @RequestingServerName, 'E', 'P', @CurrentUser, getdate(), @ApplicationVersion, @ActivityId, @ContractNumber, @ContractId, @ScheduleNumber, @StartDate, @EndDate, @EncodedCriteria,  '', '', '', '', 
			@DestFileType, '.xlsx', @DestFileName, '', 'Request Queued', 0, getdate()			
		)

		select @error = @@ERROR, @RequestId = SCOPE_IDENTITY()

		if @@error <> 0
		BEGIN
			SELECT @errorMsg = 'Failed to queue request.' 
			SET @DestFileName = null
			SET @RequestId = -1

			update EU_Activity
			set ExportUploadStatus = 'RF'  -- request failed
			where ActivityId = @ActivityId

			GOTO ERROREXIT
		END
		else
		BEGIN
			update EU_Activity
			set ExportUploadStatus = 'ER'
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


